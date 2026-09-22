using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace MyGame.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Transform)]
    [ActionDescription("Copies a specific local rotation axis (X, Y, or Z) from a source transform to a target transform with an option to invert.")]
    public sealed class MatchRotationAxis : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;

        [ActionTarget]
        [Tooltip("Target Transform whose local rotation axis will be updated.")]
        [SerializeField] private TransformVar _target;

        [Tooltip("Source Transform to copy the local rotation axis from.")]
        [SerializeField] private TransformVar _source;

        [Tooltip("Which local rotation axis to match.")]
        [SerializeField] private RotationAxis _axis;

        [Tooltip("Invert the angle value from the source object (useful for FBX bone rotation offsets).")]
        [SerializeField] private BoolVar _invert;

        public override bool CanExecute() =>
            CheckParameters(_target, _source);

        public override void Execute()
        {
            var targetTransform = _target.Value;
            var sourceTransform = _source.Value;

            if (targetTransform == null || sourceTransform == null)
            {
                return;
            }

            Vector3 targetEuler = targetTransform.localEulerAngles;
            Vector3 sourceEuler = sourceTransform.localEulerAngles;

            float axisValue = _axis switch
            {
                RotationAxis.X => sourceEuler.x,
                RotationAxis.Y => sourceEuler.y,
                RotationAxis.Z => sourceEuler.z,
                _ => 0f
            };

            if (_invert.Value)
            {
                axisValue = -axisValue;
            }

            switch (_axis)
            {
                case RotationAxis.X:
                    targetEuler.x = axisValue;
                    break;
                case RotationAxis.Y:
                    targetEuler.y = axisValue;
                    break;
                case RotationAxis.Z:
                    targetEuler.z = axisValue;
                    break;
            }

            targetTransform.localEulerAngles = targetEuler;
        }

        public override string GetSummary() =>
            "Match {_target} local rotation {_axis} axis to {_source} {_invert:option}";
    }
}