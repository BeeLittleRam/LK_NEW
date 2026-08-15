using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable, PublicAPI]
    [ActionCategory(Category.Blink)]
    [ActionDescription("Turns a Light on and off in a blink pattern.")]
    public class BlinkLight : BaseBlinkAction
    {
        [OwnerDefaultValue]
        [Tooltip("The Light to turn on/off.")]
        public LightVar Light;

        public override bool CanExecute() => CheckParameters(Light);

        protected override string Target => nameof(Light);
        
        protected override void Apply(bool on) => Light.Value.enabled = on;
    }
}