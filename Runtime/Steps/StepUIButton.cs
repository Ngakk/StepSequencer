using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace StepSequencer
{
    public class StepUIButton : Step
    {
        [Required] [SerializeField] private Button completeButton;
        [SerializeField] private Button undoButton;

        public override bool CanUndo => undoButton != null && undoButton.interactable;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            if(completeButton != null)
                completeButton.gameObject.SetActive(true);
            if(undoButton != null)
                undoButton.gameObject.SetActive(true);
            
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
            
            if(completeButton != null)
                completeButton.gameObject.SetActive(false);
            if(undoButton != null)
                undoButton.gameObject.SetActive(false);
        }
    }
}