using UnityEngine;

namespace StepSequencer
{
    public class SequencerLogger : SequencerAddonBase
    {
        [SerializeField] private bool logSequenceEvents = true;
        [SerializeField] private bool logStepEvents = false;

        protected override void SequencerOnStarted()
        {
            if(!logSequenceEvents) return;
            Debug.Log($"Sequencer {gameObject.name} Started");
        }

        protected override void SequencerOnCompleted()
        {
            if(!logSequenceEvents) return;
            Debug.Log($"Sequencer {gameObject.name} Completed");
        }

        protected override void SequencerOnStepStarted(object sender, StepEventArgs args)
        {
            if(!logStepEvents) return;
            Debug.Log($"Sequencer {gameObject.name} Step {args.Step.gameObject.name} Started");
        }

        protected override void SequencerOnStepUndone(object sender, StepEventArgs args)
        {
            if(!logStepEvents) return;
            Debug.Log($"Sequencer {gameObject.name} Step {args.Step.gameObject.name} Undone");
        }

        protected override void SequencerOnStepCompleted(object sender, StepEventArgs args)
        {
            if(!logStepEvents) return;
            Debug.Log($"Sequencer {gameObject.name} Step {args.Step.gameObject.name} Completed");
        }
    }
}
