using UnityEngine;

namespace StepSequencer
{
    public class Step : MonoBehaviour, IStep
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
        public void SetEvaluationMode(StepEvaluationMode stepEvaluationMode)
        {
            m_evaluationMode = stepEvaluationMode;
        }

        protected void OnCompleted()
        {
            Completed?.Invoke();
        }

        protected void OnUndone()
        {
            Undone?.Invoke();
        }

        protected virtual void CompleteCheckUpdate() { }
        
        protected virtual void UndoneCheckUpdate() { }
        #endregion
    }
    public delegate void StepEventHandler();

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
