using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace StepSequencer
{
    /// <summary>
    /// Main class responsible for managing the sequence of steps.
    /// </summary>
    [System.Serializable]
    
    public class Sequencer : SerializedMonoBehaviour
    {
        [Tooltip("The steps to take, ordered")]
        [SerializeField] private IStep[] steps;
        
        private Stack<IStep> stepStack; //Next steps
        private Stack<IStep> undoStack; //Stores current previous step and all other done steps

        /// <summary>
        /// Class to handle events thrown by the Sequencer.
        /// </summary>
        public delegate void SequencerEventHandler();
        
        public event SequencerEventHandler Started;
        public event SequencerEventHandler Completed;
        public event StepEventHandler StepStarted;
        public event StepEventHandler StepCompleted;
        public event StepEventHandler StepUndone;
        
        /// <summary>
        /// Current status of the sequencer.
        /// </summary>
        public Status CurrentStatus { get; private set; } = Status.Idle;
        /// <summary>
        /// The step that is currently active in the sequence.
        /// </summary>
        private IStep CurrentStep => stepStack.Count > 0 ? stepStack.Peek() : null;
        /// <summary>
        /// The step that was previously active before the current step.
        /// </summary>
        private IStep PreviousStep => undoStack.Count > 0 ? undoStack.Peek() : null;

        /// <summary>
        /// Begins the entire sequence of steps.
        /// </summary>
        [ShowIf("@UnityEngine.Application.isPlaying")][Button("Start")]
        public void StartSequence()
        {
            stepStack = new Stack<IStep>(steps.Length);
            undoStack = new Stack<IStep>(steps.Length);

            for (int i = steps.Length - 1; i >= 0; i--)
            {
                stepStack.Push(steps[i] as IStep);
                stepStack.Peek().SetEvaluationMode(StepEvaluationMode.Forward);
            }
            
            CurrentStatus = Status.Running;
            
            Started?.Invoke();
            
            InitializeSteps();
        }

        /// <summary>
        /// Advances the sequencer to the next step.
        /// </summary>
        private void MoveToNextStep()
        {
            CleanupSteps();
            
            //Move next
            undoStack.Push(stepStack.Pop());

            //Check if there are still steps
            if (stepStack.Count == 0)
            {
                Finish();
                return;
            }
            
            InitializeSteps();

            //Immediately skip if it's already completed
            if (CurrentStep.IsCompleted)
            {
                OnStepCompleted(this, new StepEventArgs(CurrentStep));
            }
        }
        
        /// <summary>
        /// Handles the event when a step is undone.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing step details.</param>
        private void OnStepUndone(object sender, StepEventArgs e)
        {
            StepUndone?.Invoke(sender, e);
            MoveToPreviousStep();
        }
        
        /// <summary>
        /// Handles the event when a step is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">Event arguments containing step details.</param>
        private void OnStepCompleted(object sender, StepEventArgs e)
        {
            StepCompleted?.Invoke(sender, e);
            MoveToNextStep();
        }
        
        /// <summary>
        /// Moves the sequencer to the previous step in the sequence.
        /// </summary>
        private void MoveToPreviousStep()
        {
            CleanupSteps();

            if (undoStack.Count == 0)
            {
                Debug.LogWarning("Undo stack is empty but still trying to undo, probably a coding error.");
                return;
            }
            
            //Move previous
            stepStack.Push(undoStack.Pop());
            
            InitializeSteps();
        }

        /// <summary>
        /// Sets up and initializes all steps in the sequence.
        /// </summary>
        private void InitializeSteps()
        {
            //Make current step into a previous step
            if (PreviousStep is { CanUndo: true })
            {
                PreviousStep.SetEvaluationMode(StepEvaluationMode.Backward);
                PreviousStep.gameObject.SetActive(true);
                PreviousStep.Undone += OnStepUndone;
            }

            if (CurrentStep != null)
            {
                //Prepare next step and start
                CurrentStep.SetEvaluationMode(StepEvaluationMode.Forward);
                StepStarted?.Invoke(CurrentStep.gameObject, new StepEventArgs(CurrentStep));
                CurrentStep.gameObject.SetActive(true);
                CurrentStep.Completed += OnStepCompleted;
            }
        }
        
        /// <summary>
        /// Cleans up any resources or references related to the sequence steps.
        /// </summary>
        private void CleanupSteps()
        {
            //Hide prev step and archive
            if (PreviousStep != null)
            {
                PreviousStep.gameObject.SetActive(false);
                PreviousStep.SetEvaluationMode(StepEvaluationMode.None);
                PreviousStep.Undone -= OnStepUndone;
            }

            //End current step
            if (CurrentStep != null)
            {
                CurrentStep.gameObject.SetActive(false);
                CurrentStep.SetEvaluationMode(StepEvaluationMode.None);
                CurrentStep.Completed -= OnStepCompleted;
            }
        }

        private void Finish()
        {
            CurrentStatus = Status.Completed;
                
            CleanupSteps();
                
            Completed?.Invoke();
        }

#if UNITY_EDITOR
        [Button]
        public void RunStepSetup()
        {
            var childSteps = GetComponentsInChildren<IStep>(true).ToList();
            
            int undoID = UnityEditor.Undo.GetCurrentGroup();
            UnityEditor.Undo.SetCurrentGroupName("Batch modifying steps");
            UnityEditor.Undo.RecordObject(this, "Enumerate steps");
            
            //Checking that MultiStep's child steps are not in the list
            List<IStep> stepsToRemove = new List<IStep>();
            foreach(var s in childSteps)
            {
                if (s is MultipleStep multipleStep)
                {
                    stepsToRemove.AddRange(multipleStep.Steps);
                }
            }

            foreach (var str in stepsToRemove)
            {
                childSteps.Remove(str);
            }

            steps = childSteps.ToArray();
            
            EnumerateChildren(transform, "", undoID);
            
            UnityEditor.Undo.CollapseUndoOperations(undoID);
        }

        private void EnumerateChildren(Transform parent, string prefix, int undoID)
        {
            string pattern = @"^(\d+(?:\.\d+)*)(?:\s+)(.+)";
            int count = 0;

            if(prefix == null) prefix = string.Empty;
            
            if (prefix != string.Empty)
                prefix += ".";
            
            foreach (Transform child in parent)
            {
                string title = child.gameObject.name.Trim();
                var match = Regex.Match(title, pattern);
                if (match.Success)
                {
                    title = match.Groups[2].Value;
                }

                UnityEditor.Undo.RecordObject(child.gameObject, "Step setup");
                if (child.gameObject.GetComponent<IStep>() != null)
                {
                    child.gameObject.name = $"{prefix}{count} {title}";
                    child.gameObject.SetActive(false);
                }
                
                EnumerateChildren(child, prefix + count, undoID);
                count++;
            }
        }
#endif
        
        /// <summary>
        /// Represents the status of the Sequencer.
        /// </summary>
        public enum Status
        {
            Idle,
            Running,
            Completed,
        }
    }
}
