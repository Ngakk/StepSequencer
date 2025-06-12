using UnityEngine;
using UnityEngine.UI;

namespace StepSequencer
{
    public class StepUIToggle : Step
    {
        [SerializeField] private Toggle toggle;
        [SerializeField] private bool checkForOn = true;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDisable()
        {
            toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        void OnValueChanged(bool value)
        {
            if (m_evaluationMode == StepEvaluationMode.Forward && checkForOn)
            {
                Complete();
            }
            else if (m_evaluationMode == StepEvaluationMode.Backward && !checkForOn)
            {
                Undo();
            }
        }
    }
}
