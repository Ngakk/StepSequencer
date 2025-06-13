using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    public class Step : SerializedMonoBehaviour, IStep
    {
        #region Variables
        protected StepEvaluationMode m_evaluationMode;
        public event StepEventHandler Started;
        public event StepEventHandler Completed;
        public event StepEventHandler Undone;
        #endregion

        public virtual bool IsCompleted { get; protected set; }
        
        #region Monobehaviors

        protected virtual void OnEnable()
        {
            if(m_evaluationMode == StepEvaluationMode.Forward)
                Invoke(nameof(DelayerStart), 0f); //Wait for all other OnEnables to finish
        }
        
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

        private void DelayerStart()
        {
            Started?.Invoke(this , new StepEventArgs(this));
        }
        
        public virtual void SetEvaluationMode(StepEvaluationMode stepEvaluationMode)
        {
            m_evaluationMode = stepEvaluationMode;
        }

        protected void Complete()
        {
            IsCompleted = true;
            Completed?.Invoke(this, new StepEventArgs(this));
        }

        protected void Undo()
        {
            IsCompleted = false;
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
        event StepEventHandler Started; //Only called when on forward evaluation
        event StepEventHandler Completed;
        event StepEventHandler Undone;

        void SetEvaluationMode(StepEvaluationMode mode);
        GameObject gameObject { get; }
        bool IsCompleted { get; }
    }
}
