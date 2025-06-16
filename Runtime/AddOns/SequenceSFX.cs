using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace StepSequencer
{
    public class SequenceSFX : SequencerAddonBase
    {
        [SerializeField] [Required] protected AudioSource audioSource;
        [ToggleGroup("Sequence event SFX")]
        [SerializeField] private AudioClip sequenceStartedAudioClip;
        [ToggleGroup("Sequence event SFX")]
        [SerializeField] private AudioClip sequenceCompletedAudioClip;
        [ToggleGroup("Step event SFX")]
        [SerializeField] private AudioClip stepStartedAudioClip;
        [ToggleGroup("Step event SFX")]
        [SerializeField] private AudioClip stepCompletedAudioClip;
        [ToggleGroup("Step event SFX")]
        [SerializeField] private AudioClip stepUndoneAudioClip;

        protected override void SequencerOnStarted()
        {
            if(sequenceStartedAudioClip != null)
                audioSource.PlayOneShot(sequenceStartedAudioClip);
        }
        
        protected override void SequencerOnCompleted()
        {
            if(stepCompletedAudioClip != null)
                audioSource.PlayOneShot(stepCompletedAudioClip);
        }
        
        protected override void SequencerOnStepStarted(object sender, StepEventArgs args)
        {
            if(stepStartedAudioClip != null)
                audioSource.PlayOneShot(stepStartedAudioClip);
        }

        protected override void SequencerOnStepCompleted(object sender, StepEventArgs args)
        {
            if(stepCompletedAudioClip != null)
                audioSource.PlayOneShot(stepCompletedAudioClip);
        }

        protected override void SequencerOnStepUndone(object sender, StepEventArgs args)
        {
            if(stepUndoneAudioClip != null)
                audioSource.PlayOneShot(stepUndoneAudioClip);
        }
    }
}
