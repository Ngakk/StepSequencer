using UnityEngine;

namespace StepSequencer
{
    public class Step : MonoBehaviour
    {
        #region Variables
        
        protected EvaluationMode evaluationMode;
        
        public delegate void StepEventHandler();

        public event StepEventHandler Completed;
        public event StepEventHandler Undone;
        #endregion
        
        #region Methods

        public void SetEvaluationMode(EvaluationMode evaluationMode)
        {
            this.evaluationMode = evaluationMode;
        }
        
        protected void OnCompleted()
        {
            Completed?.Invoke();
        }

        protected void OnUndone()
        {
            Undone?.Invoke();
        }
        #endregion
        
        public enum EvaluationMode
        {
            None,
            Forward,
            Backward,
        }
    }
}
