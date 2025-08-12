using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    [RequireComponent(typeof(IStep))]
    public abstract class StepAddonBase : SerializedMonoBehaviour
    {
        [SerializeField] protected IStep step;
        
        [OnInspectorInit]
        private void AssignReferences()
        {
            step = GetComponent<IStep>();
        }

        protected virtual void OnEnable()
        {
            SubscribeToStep();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromStep();
        }

        protected virtual void SubscribeToStep()
        {
            step.Started += StepOnStarted;
            step.Completed += StepOnCompleted;
            step.Undone += StepOnUndone;
        }

        protected virtual void UnsubscribeFromStep()
        {
            step.Started -= StepOnStarted;
            step.Completed -= StepOnCompleted;
            step.Undone -= StepOnUndone;
        }
        
        protected virtual void StepOnStarted(object sender, StepEventArgs args)
        {
            
        }
        
        protected virtual void StepOnCompleted(object sender, StepEventArgs args)
        {
            
        }
        
        protected virtual void StepOnUndone(object sender, StepEventArgs args)
        {
            
        }
    }
}
