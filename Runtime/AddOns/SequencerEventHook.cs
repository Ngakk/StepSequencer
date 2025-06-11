using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace StepSequencer
{
    public class SequencerEventHook : SequencerAddonBase
    {
        [FoldoutGroup("Sequencer events")] [SerializeField]
        private UnityEvent sequenceStarted;
        [FoldoutGroup("Sequencer events")] [SerializeField]
        private UnityEvent sequenceCompleted;
        [FoldoutGroup("Step events")] [SerializeField]
        private UnityEvent stepStarted;
        [FoldoutGroup("Step events")] [SerializeField]
        private UnityEvent stepCompleted;
        [FoldoutGroup("Step events")] [SerializeField]
        private UnityEvent stepUndone;

        protected override void SequencerOnStepStarted(object sender, StepEventArgs args)
        {
            stepStarted?.Invoke();   
        }
        
        protected override void SequencerOnStepUndone(object sender, StepEventArgs args)
        {
            stepUndone?.Invoke();
        }

        protected override void SequencerOnStepCompleted(object sender, StepEventArgs args)
        {
            stepCompleted?.Invoke();
        }

        protected override void SequencerOnCompleted()
        {
            sequenceCompleted?.Invoke();
        }

        protected override void SequencerOnStarted()
        {
            sequenceStarted?.Invoke();
        }
    }
}
