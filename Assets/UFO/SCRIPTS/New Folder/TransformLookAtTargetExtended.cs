using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;

namespace MyGame.PlayMaker.Actions
{
    [PublicAPI]
    [Serializable]
    [ActionCategory(Category.GameplayOrientationTransform)]
    [ConvertibleGroup("LookAt")]
    [ActionDescription("Rotate a Transform so a chosen local axis faces another Transform. " +
                       "Supports Rotation Constraint (None/X/Y/Z) with a dynamic Y-offset, Axis Direction, SmoothTime, and MaxSpeed.")]
    [HelpURL("actions/transform-actions/look-at-actions/")]
    public sealed class TransformLookAtTargetExtended : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.UpdateEveryFrame;

        [ActionTarget]
        [OwnerDefaultValue, Tooltip("The Transform to rotate.")]
        [FormerlySerializedAs("Transform")]
        [SerializeField] private TransformVar _transform;

        [Tooltip("The target Transform to face.")]
        [FormerlySerializedAs("Target")]
        [SerializeField] private TransformVar _target;

        [ActionHeader("Rotation")]
        [Tooltip("Rotation constraint. Select None for unrestricted 3D rotation, or choose an axis to rotate around.")]
        [SerializeField] private RotationConstraintVar _rotationConstraint;

        [Tooltip("Additional Y-axis rotation offset in degrees (applies when Rotation Constraint is set to Y).")]
        [SerializeField, OptionalField] private FloatVar _yOffset;

        [Tooltip("Which local axis should face the target (X / Y / Z / NegativeX / NegativeY / NegativeZ).")]
        [SerializeField] private AxisDirectionVar _facingAxis;

        [HideIf(nameof(HideWorldUp))]
        [DefaultValue("~Vector3Up")]
        [Tooltip("World up vector (used when Rotation Constraint = None).")]
        [SerializeField] private Vector3Var _worldUp;

        [ActionHeader("Motion")]
        [VarSlider(0.0f, 1.0f)]
        [Tooltip("Smooth Time in seconds (roughly time to halve the remaining angle). 0 = no smoothing.")]
        [SerializeField] private FloatVar _smoothTime;

        [VarSlider(0, 1080)]
        [Tooltip("Maximum turn speed in degrees per second. 0 = uncapped.")]
        [FormerlySerializedAs("MaxSpeed")]
        [SerializeField] private FloatVar _maxSpeed;

        private Quaternion _desired;

        public override bool CanStart() =>
            CheckParameters(_transform, _target, _rotationConstraint, _facingAxis);

        public override bool CanExecute() =>
            CheckParameters(_transform, _rotationConstraint, _facingAxis);

        public override void Execute()
        {
            var t = _transform.Value;
            var tf = _target.Value;
            if (t == null)
            {
                return;
            }

            if (tf == null)
            {
                Finish();
                return;
            }

            var up = _worldUp.IsNone ? Vector3.up : _worldUp.Value;

            // Compute desired target rotation internally
            _desired = ComputeTargetRotation(t, tf.position, _rotationConstraint.Value, _facingAxis.Value, up);

            // Apply dynamic Y-axis offset when constrained to Y
            if (_rotationConstraint.Value == RotationConstraint.Y && _yOffset.IsAssigned && Mathf.Abs(_yOffset.Value) > Mathf.Epsilon)
            {
                _desired *= Quaternion.Euler(0f, _yOffset.Value, 0f);
            }

            t.rotation = SmoothLookAtHelper.Update(t.rotation, _desired, _smoothTime.Value, _maxSpeed.Value);
        }

        private static Quaternion ComputeTargetRotation(
            Transform transform,
            Vector3 targetPosition,
            RotationConstraint constraint,
            AxisDirection facingAxis,
            Vector3 worldUp)
        {
            Vector3 dir = targetPosition - transform.position;

            switch (constraint)
            {
                case RotationConstraint.X:
                    dir.x = 0f;
                    break;
                case RotationConstraint.Y:
                    dir.y = 0f;
                    break;
                case RotationConstraint.Z:
                    dir.z = 0f;
                    break;
            }

            if (dir.sqrMagnitude < 0.0001f)
            {
                return transform.rotation;
            }

            dir.Normalize();

            Quaternion rawRotation = Quaternion.LookRotation(dir, worldUp);
            Quaternion axisOffset = GetAxisOffset(facingAxis);

            return rawRotation * axisOffset;
        }

        private static Quaternion GetAxisOffset(AxisDirection axis)
        {
            return axis switch
            {
                AxisDirection.X => Quaternion.Euler(0f, 90f, 0f),
                AxisDirection.NegativeX => Quaternion.Euler(0f, -90f, 0f),
                AxisDirection.Y => Quaternion.Euler(-90f, 0f, 0f),
                AxisDirection.NegativeY => Quaternion.Euler(90f, 0f, 0f),
                AxisDirection.Z => Quaternion.identity,
                AxisDirection.NegativeZ => Quaternion.Euler(0f, 180f, 0f),
                _ => Quaternion.identity
            };
        }

        [UsedImplicitly]
        private bool HideWorldUp =>
            !_rotationConstraint.IsVariable && _rotationConstraint.Value != RotationConstraint.None;

        public override string GetSummary()
        {
            var s = "Rotate {_transform} {_facingAxis}";

            if (_rotationConstraint.IsNotDefault(RotationConstraint.None))
                s += " around {_rotationConstraint}";

            s += " to look at {_target}";

            if (_yOffset.IsAssigned)
                s += " (+{_yOffset}° Y)";

            if (_smoothTime.IsNotDefault())
                s += " in {_smoothTime}s";

            if (_maxSpeed.IsNotDefault())
                s += " max {_maxSpeed}°/s";

            if ((_rotationConstraint.IsVariable || _rotationConstraint.Value == RotationConstraint.None) &&
                _worldUp.IsNotDefault(Vector3.up))
                s += " up: {_worldUp}";

            return s;
        }
    }
}