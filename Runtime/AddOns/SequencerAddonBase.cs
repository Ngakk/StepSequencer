using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    [RequireComponent(typeof(Sequencer))]
    public class SequencerAddonBase : MonoBehaviour
    {
        [SerializeField] private Sequencer sequencer;
        
        [OnInspectorInit]
        private void AssignReferences()
        {
            if (sequencer == null)
                sequencer = GetComponent<Sequencer>();
        }
        
        protected virtual void OnEnable()
        {
            SubscribeToSequencer();
        }

        protected virtual void OnDisable()
        {
            UnsubscribeFromSequencer();
        }

        protected virtual void SubscribeToSequencer()
        {
            sequencer.Started += SequencerOnStarted;   
            sequencer.Completed += SequencerOnCompleted;
            sequencer.StepStarted += SequencerOnStepStarted;
            sequencer.StepCompleted += SequencerOnStepCompleted;
            sequencer.StepUndone += SequencerOnStepUndone;
        }
        
        protected virtual void UnsubscribeFromSequencer()
        {
            sequencer.Started -= SequencerOnStarted;   
            sequencer.Completed -= SequencerOnCompleted;
            sequencer.StepStarted -= SequencerOnStepStarted;
            sequencer.StepCompleted -= SequencerOnStepCompleted;
            sequencer.StepUndone -= SequencerOnStepUndone;
        }

        protected virtual void SequencerOnStepUndone(object sender, StepEventArgs args)
        {
            
        }

        protected virtual  void SequencerOnStepCompleted(object sender, StepEventArgs args)
        {
            
        }

        protected virtual  void SequencerOnStepStarted(object sender, StepEventArgs args)
        {
            
        }

        protected virtual  void SequencerOnCompleted()
        {
            
        }

        protected virtual  void SequencerOnStarted()
        {
            
        }
    }
}
