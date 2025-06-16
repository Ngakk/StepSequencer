using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    public class StepSFX : StepAddonBase
    {
        [SerializeField] [Required] AudioSource audioSource;
        [SerializeField] private AudioClip startedAudioClip;
        [SerializeField] private AudioClip completedAudioClip;
        [SerializeField] private AudioClip undoneAudioClip;
        
        protected override void StepOnStarted(object sender, StepEventArgs args)
        {
            if(startedAudioClip != null)
                audioSource.PlayOneShot(startedAudioClip);
        }

        protected override void StepOnCompleted(object sender, StepEventArgs args)
        {
            if(completedAudioClip != null)
                audioSource.PlayOneShot(completedAudioClip);
        }

        protected override void StepOnUndone(object sender, StepEventArgs args)
        {
            if(undoneAudioClip != null)
                audioSource.PlayOneShot(undoneAudioClip);
        }
    }
}
