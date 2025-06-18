using Sirenix.OdinInspector;
using UnityEngine;

namespace StepSequencer
{
    public class StepSFX : StepAddonBase
    {
        [SerializeField] [Required] protected AudioSource audioSource;
        [SerializeField] protected AudioClip startedAudioClip;
        [SerializeField] protected AudioClip completedAudioClip;
        [SerializeField] protected AudioClip undoneAudioClip;
        
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
