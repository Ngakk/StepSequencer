using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace StepSequencer
{
    public class StepEventHook : StepAddonBase
    {
        [FoldoutGroup("Step events")]
        [SerializeField] private UnityEvent<IStep> stepStarted;
        [FoldoutGroup("Step events")]
        [SerializeField] private UnityEvent<IStep> stepCompleted;
        [FoldoutGroup("Step events")]
        [SerializeField] private UnityEvent<IStep> stepUndone;

        protected override void StepOnStarted(object sender, StepEventArgs args) => stepStarted?.Invoke(args.Step);
        protected override void StepOnCompleted(object sender, StepEventArgs args) => stepCompleted?.Invoke(args.Step);
        protected override void StepOnUndone(object sender, StepEventArgs args) => stepUndone?.Invoke(args.Step);
    }
}
