using System;
using System.Collections.Generic;
using UnityEngine;

namespace StepSequencer
{
    [RequireComponent(typeof(Sequencer))]
    public class SubSequencerStep : Step, IStepExcluder
    {
        [SerializeField] Sequencer sequencer;

        public override bool CanUndo => false;

        private void OnValidate()
        {
            if(sequencer == null)
                sequencer = GetComponent<Sequencer>();
        }


        protected override void OnEnable()
        {
            base.OnEnable();

            if (m_evaluationMode is StepEvaluationMode.Forward)
            {
                sequencer.Completed += OnSequenceComplete;
                sequencer.StartSequence();
            }
        }

        void OnSequenceComplete()
        {
            sequencer.Completed -= OnSequenceComplete;
            Complete();
        }
        
        public IEnumerable<IStep> GetExclusions()
        {
            return sequencer.Steps;
        }
    }
}
