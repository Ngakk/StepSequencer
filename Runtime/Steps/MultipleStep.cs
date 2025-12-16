using System.Collections.Generic;
using UnityEngine;

namespace StepSequencer
{
    public class MultipleStep : Step, IStepExcluder
    {
        //The steps to encapsulate
        [SerializeField] private Type type;
        [SerializeField] IStep[] steps;
        [SerializeField] private bool canUndo = true;

        private List<IStep> evaluatedSteps; //Always holds the completed steps
        
        public IEnumerable<IStep> Steps => steps;
        public override bool CanUndo => canUndo;
        
        #region Monobehaviour
        protected override void OnEnable()
        {
            base.OnEnable();
            evaluatedSteps = new List<IStep>();
            foreach (var step in steps)
            {
                step.Completed += OnCompleted;
                step.Undone += OnUndone;
                
                step.SetEvaluationMode(step.IsCompleted ? StepEvaluationMode.Backward : StepEvaluationMode.Forward);
                step.gameObject.SetActive(true);
                
                if(step.IsCompleted)
                    evaluatedSteps.Add(step);
            }
            
            if(type is Type.All && evaluatedSteps.Count == steps.Length || type is Type.Any && evaluatedSteps.Count > 0)
                IsCompleted = true;
        }
        
        private void OnDisable()
        {
            foreach (var step in steps)
            {
                step.gameObject.SetActive(false);
                step.Completed -= OnCompleted;
                step.Undone -= OnUndone;
                step.SetEvaluationMode(StepEvaluationMode.None);
            }    
        }
        
        #endregion

        public override void SetEvaluationMode(StepEvaluationMode stepEvaluationMode)
        {
            base.SetEvaluationMode(stepEvaluationMode);
            
            foreach (var step in steps) //Just forward the evaluation mode to all steps
            {
                step.SetEvaluationMode(stepEvaluationMode);
            }
        }

        public override void Reset()
        {
            base.Reset();
            
            foreach (var step in steps) //Just forward the evaluation mode to all steps
            {
                step.Reset();
            }
        }

        private void OnUndone(object sender, StepEventArgs e)
        {
            //Need to start checking for step completion again, and reset the step by disabling/enabling
            e.Step.gameObject.SetActive(false);
            e.Step.SetEvaluationMode(StepEvaluationMode.Forward);
            e.Step.gameObject.SetActive(true);
            
            evaluatedSteps.Remove(e.Step);
            
            if (m_evaluationMode != StepEvaluationMode.Backward) return; //Can't undo if not backward evaluation
            
            if(type is Type.All || (type is Type.Any && evaluatedSteps.Count == 0))
                Undo();
        }

        private void OnCompleted(object sender, StepEventArgs e)
        {
            //Need to start checking for step undo, and reset the step by disabling/enabling
            e.Step.gameObject.SetActive(false);
            if (e.Step.CanUndo)
            {
                e.Step.SetEvaluationMode(StepEvaluationMode.Backward);
                e.Step.gameObject.SetActive(true);
            }

            if (!evaluatedSteps.Contains(e.Step))
                evaluatedSteps.Add(e.Step);
            
            if (m_evaluationMode != StepEvaluationMode.Forward) return; //can't complete if not forward evaluation
            
            if(type is Type.Any || (type is Type.All && evaluatedSteps.Count == steps.Length))
                Complete();
        }
        
        private enum Type
        {
            All,
            Any
        }

        public IEnumerable<IStep> GetExclusions()
        {
            return steps;
        }
    }
}
