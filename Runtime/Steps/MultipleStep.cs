using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    public class MultipleStep : Step
    {
        //The steps to encapsulate
        [SerializeField] private Type type;
        [SerializeField] IStep[] steps;
        
        [ShowInInspector]
        public int EvaluatedStepsCount => evaluatedSteps?.Count ?? 0;
        
        public event StepEventHandler Completed;
        public event StepEventHandler Undone;

        private List<IStep> evaluatedSteps; //Always holds the completed steps
        
        #region Monobehaviour
        protected override void OnEnable()
        {
            base.OnEnable();
            evaluatedSteps = new List<IStep>();
            foreach (var step in steps)
            {
                step.SetEvaluationMode(step.IsCompleted ? StepEvaluationMode.Backward : StepEvaluationMode.Forward);
                step.gameObject.SetActive(true);
                step.Completed += OnCompleted;
                step.Undone += OnUndone;

                if (step.IsCompleted)
                    evaluatedSteps.Add(step);
                
            }    
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
            e.Step.SetEvaluationMode(StepEvaluationMode.Backward);
            e.Step.gameObject.SetActive(true);
            
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
    }
}
