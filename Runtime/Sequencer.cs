using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace StepSequencer
{
    [System.Serializable]
    public class Sequencer : SerializedMonoBehaviour
    {
        //The steps to take, ordered
        [SerializeField] IStep[] steps;
        
        private Stack<IStep> stepStack; //Next steps
        private Stack<IStep> undoStack; //Stores current previous step and all other done steps

        public delegate void SequencerEventHandler();
        
        public event SequencerEventHandler Started;
        public event SequencerEventHandler Completed;
        
        public Status CurrentStatus { get; private set; } = Status.Idle;
        private IStep CurrentStep => stepStack.Count > 0 ? stepStack.Peek() : null;
        private IStep PreviousStep => undoStack.Count > 0 ? undoStack.Peek() : null;

        [ContextMenu("Start")]
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
            
            Debug.Log("[seq] Starting sequence");
            Started?.Invoke();
            
            InitializeSteps();
        }

        private void MoveToNextStep()
        {
            CleanupSteps();

            //Check if there are still steps
            if (stepStack.Count == 0)
            {
                CurrentStatus = Status.Completed;
                Debug.Log("[seq] Completed sequence");
                Completed?.Invoke();
                return;
            }
            
            //Move next
            undoStack.Push(stepStack.Pop());

            InitializeSteps();
        }

        private void OnStepUndone(object sender, StepEventArgs e)
        {
            Debug.Log($"[seq] Step {e.Step.gameObject.name} was <color=red>undone</color>");
            MoveToPreviousStep();
        }
        
        private void OnStepCompleted(object sender, StepEventArgs e)
        {
            Debug.Log($"[seq] Step {e.Step.gameObject.name} was <color=green>completed</color>");
            MoveToNextStep();
        }
        
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

        private void InitializeSteps()
        {
            //Make current step into a previous step
            if (PreviousStep != null)
            {
                PreviousStep.gameObject.SetActive(true);
                PreviousStep.SetEvaluationMode(StepEvaluationMode.Backward);
                PreviousStep.Undone += OnStepUndone;
            }

            if (CurrentStep != null)
            {
                //Prepare next step and start
                CurrentStep.gameObject.SetActive(true);
                CurrentStep.SetEvaluationMode(StepEvaluationMode.Forward);
                CurrentStep.Completed += OnStepCompleted;
            }
        }
        
        /// <summary>
        /// Unsubscribes and resets settings of current and previous step
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

        public enum Status
        {
            Idle,
            Running,
            Completed,
        }
    }
}
