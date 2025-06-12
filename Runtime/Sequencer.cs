using System.Collections.Generic;
using Sirenix.OdinInspector;
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
        [Button("Start")]
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
            if (PreviousStep != null)
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
