using UnityEngine;

namespace StepSequencer
{
    public class StepLogger : StepAddonBase
    {
        protected override void StepOnStarted(object sender, StepEventArgs args)
        {
            Debug.Log($"Step OnStarted: {args.Step.gameObject.name}");
        }

        protected override void StepOnCompleted(object sender, StepEventArgs args)
        {
            Debug.Log($"Step OnCompleted: {args.Step.gameObject.name}");
        }
        
        protected override void StepOnUndone(object sender, StepEventArgs args)
        {
            Debug.Log($"Step OnUndone: {args.Step.gameObject.name}");
        }
    }
}
