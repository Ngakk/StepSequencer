using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace StepSequencer
{
    public class StepUIButton : Step
    {
        [Required] [SerializeField] private Button completeButton;
        [SerializeField] private Button undoButton;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (m_evaluationMode == StepEvaluationMode.Forward)
                completeButton.onClick.AddListener(Complete);
            else if(m_evaluationMode == StepEvaluationMode.Backward && undoButton != null)
                undoButton.onClick.AddListener(Undo);
            
        }

        void OnDisable()
        {
            completeButton.onClick.RemoveListener(Complete);
            if(undoButton != null)
                undoButton.onClick.RemoveListener(Undo);
        }
    }
}