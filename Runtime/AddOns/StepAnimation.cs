using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace StepSequencer
{
    public class StepAnimation : StepAddonBase
    {
        [Required] [SerializeField] private Animator animator;
        [SerializeField] private Type type;
        
        [Required] [SerializeField] [ValueDropdown("GetAnimatorProperties")]
        private string animPropName;
        
        [SerializeField] private Steps steps;

        [ShowIf("@this.type == Type.Bool")] [SerializeField]
        private bool boolValue;
        
        [ShowIf("@this.type == Type.Float")][SerializeField]
        private float floatValue;
        
        [ShowIf("@this.type == Type.Integer")][SerializeField]
        private int intValue;

        private void OnValidate()
        {
            if (animator != null)
            {
                if (animator.parameters.Count(x => x.name == animPropName && TypeEquals(type, x.type)) == 0)
                {
                    animPropName = string.Empty;
                }
            }
        }

        protected override void StepOnStarted(object sender, StepEventArgs args)
        {
            if ((steps & Steps.Start) > 0)
                PlayAnim();
        }

        protected override void StepOnCompleted(object sender, StepEventArgs args)
        {
            if((steps & Steps.End) > 0)
                PlayAnim();
        }

        protected override void StepOnUndone(object sender, StepEventArgs args)
        {
            Debug.Log($"{gameObject.name}: StepOnUndone");
            if ((steps & Steps.Undo) > 0)
                PlayAnim();
        }

        private void PlayAnim()
        {
            switch (type)
            {
                case Type.Trigger:
                    animator.SetTrigger(animPropName);
                    break;
                case Type.Bool:
                    animator.SetBool(animPropName, boolValue);
                    break;
                case Type.Float:
                    animator.SetFloat(animPropName, floatValue);
                    break;
                case Type.Integer:
                    animator.SetInteger(animPropName, intValue);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
        
        private IEnumerable<string> GetAnimatorProperties()
        {
            if (animator == null)
                return new string[0];

            return animator.parameters.Where(x => TypeEquals(type, x.type)).Select(x => x.name);
        }
        
        public enum Type : byte
        {
            Trigger,
            Bool,
            Float,
            Integer
        }

        [Flags]
        private enum Steps : byte
        {
            None = 0,
            Start = 1,
            End = 2,
            Undo = 4,
            All = 7
        }

        private bool TypeEquals(Type a, AnimatorControllerParameterType b)
        {
            return (a == Type.Bool && b == AnimatorControllerParameterType.Bool)
                   || (a == Type.Float && b == AnimatorControllerParameterType.Float)
                   || (a == Type.Trigger && b == AnimatorControllerParameterType.Trigger)
                   || (a == Type.Integer && b == AnimatorControllerParameterType.Int);
        }
    }
}
