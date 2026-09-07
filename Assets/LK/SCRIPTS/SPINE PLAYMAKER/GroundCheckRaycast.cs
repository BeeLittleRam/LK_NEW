using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

namespace MyGame.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Physics2D)]
    [ActionDescription("Performs a downward 2D raycast from an origin transform to check for ground.")]
    public sealed class GroundCheckRaycast : BaseTrueFalseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.FixedUpdateEveryFrame;

        [ActionTarget]
        [Tooltip("The origin transform from which the ground check raycast is cast.")]
        [SerializeField] private TransformVar _origin;

        [Tooltip("The raycast distance.")]
        [SerializeField, DefaultValue(0.1f)] private FloatVar _distance;

        [Tooltip("Layer mask filter for the raycast.")]
        [SerializeField] private LayerMaskVar _layerMask;

        [Tooltip("Draw a debug line in the Scene view to visualize the raycast.")]
        [SerializeField, DefaultValue(false)] private BoolVar _debug;

        [Tooltip("Optionally store the hit point.")]
        [SerializeField, OptionalField, WriteOnly] private Vector3Ref _storeHitPoint;

        [Tooltip("Optionally store the hit normal.")]
        [SerializeField, OptionalField, WriteOnly] private Vector3Ref _storeHitNormal;

        protected override string TrueSummary => "{_origin} ground check hit within {_distance}";
        protected override string FalseSummary => "{_origin} ground check missed within {_distance}";

        public override bool CanExecute() =>
            CheckParameters(_origin, _distance);

        protected override bool Test()
        {
            if (_origin.Value == null) return false;

            Vector2 originPos = _origin.Value.position;
            RaycastHit2D hit = Physics2D.Raycast(
                originPos,
                Vector2.down,
                _distance.Value,
                _layerMask.Value
            );

            bool grounded = hit.collider != null;

            if (_debug.Value)
            {
                Color lineColor = grounded ? Color.green : Color.red;
                Debug.DrawRay(originPos, Vector2.down * _distance.Value, lineColor);
            }

            if (grounded)
            {
                if (_storeHitPoint.IsAssigned)
                {
                    _storeHitPoint.Value = hit.point;
                }

                if (_storeHitNormal.IsAssigned)
                {
                    _storeHitNormal.Value = hit.normal;
                }
            }

            return grounded;
        }
    }
}