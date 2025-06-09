using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    public class Step : SerializedMonoBehaviour, IStep
    {
        #region Variables
        protected StepEvaluationMode m_evaluationMode;
        public event StepEventHandler Completed;
        public event StepEventHandler Undone;
        #endregion
        
        #region Monobehaviors

        void Update()
        {
            switch (m_evaluationMode)
            {
                case StepEvaluationMode.Forward:
                    CompleteCheckUpdate();
                    break;
                case StepEvaluationMode.Backward:
                    UndoneCheckUpdate();
                    break;
                case StepEvaluationMode.None:
                    return;
            }
        }
        #endregion
        
        #region Methods
        public virtual void SetEvaluationMode(StepEvaluationMode stepEvaluationMode)
        {
            m_evaluationMode = stepEvaluationMode;
        }

        protected void Complete()
        {
            Completed?.Invoke(this, new StepEventArgs(this));
        }

        protected void Undo()
        {
            Undone?.Invoke(this , new StepEventArgs(this));
        }

        protected virtual void CompleteCheckUpdate() { }
        
        protected virtual void UndoneCheckUpdate() { }
        #endregion
    }
    public delegate void StepEventHandler(object sender, StepEventArgs args);

    public class StepEventArgs : EventArgs
    {
        public IStep Step { get; private set; }

        public StepEventArgs(IStep step)
        {
            Step = step;
        }
    }

    public enum StepEvaluationMode
    {
        None,
        Forward,
        Backward,
    }
    
    public interface IStep
    {
        event StepEventHandler Completed;
        event StepEventHandler Undone;

        void SetEvaluationMode(StepEvaluationMode mode);
        GameObject gameObject { get; }
    }
}
