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
        
        public event StepEventHandler Completed;
        public event StepEventHandler Undone;

        private List<IStep> evaluatedSteps;
        
        #region Monobehaviour
        private void OnEnable()
        {
            evaluatedSteps = new List<IStep>();
            foreach (var step in steps)
            {
                step.SetEvaluationMode(m_evaluationMode);
                step.gameObject.SetActive(true);
                step.Completed += OnCompleted;
                step.Undone += OnUndone;
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
            
            if (m_evaluationMode == StepEvaluationMode.Forward)
            {
                evaluatedSteps.Remove(e.Step);
            }
            else if (m_evaluationMode == StepEvaluationMode.Backward)
            {
                switch (type)
                {
                    case Type.All: //Any step that is undone will break the 'all' requirement
                        Undo();
                        break;
                    case Type.Any: //Need all steps to be undone to break the 'any' requirement
                        if (!evaluatedSteps.Contains(e.Step))
                            evaluatedSteps.Add(e.Step);
                        if (evaluatedSteps.Count == steps.Length)
                            Undo();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            
            
        }

        private void OnCompleted(object sender, StepEventArgs e)
        {
            //Need to start checking for step undo, and reset the step by disabling/enabling
            e.Step.gameObject.SetActive(false);
            e.Step.SetEvaluationMode(StepEvaluationMode.Backward);
            e.Step.gameObject.SetActive(true);
            
            switch (type)
            {
                case Type.All: //Check for all steps to be completed
                    if (!evaluatedSteps.Contains(e.Step))
                        evaluatedSteps.Add(e.Step);
                    if (evaluatedSteps.Count == steps.Length)
                        Complete();
                    break;
                case Type.Any:
                    Complete();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private enum Type
        {
            All,
            Any
        }
    }
}
