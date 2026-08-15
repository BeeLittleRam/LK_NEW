using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Physics)]
    [ActionDescription(
        "Moves a fast projectile by updating its Transform position each physics step and " +
        "sweeping (raycast or spherecast) to avoid tunneling. " +
        "Supports Local or World direction space.")]
    [HelpURL("actions/physics-actions/gameplay/transform-fast-projectile/")]
    public sealed class TransformFastProjectile : BaseAction
    {
        public override UpdateMode DefaultUpdateMode => UpdateMode.FixedUpdateEveryFrame;

        // ─────────────────────────────────────────────────────────────────────────────
        //  Space mode selector
        // ─────────────────────────────────────────────────────────────────────────────
        public enum SpaceMode
        {
            World,
            Local
        }
        
        // ─────────────────────────────────────────────────────────────────────────────
        //  Projectile & Motion
        // ─────────────────────────────────────────────────────────────────────────────
        [Header("Projectile")]

        [Tooltip("The Transform to move as a fast projectile.")]
        [SerializeField] private TransformVar _transform;

        [Header("Motion")]

        [Tooltip("Units per second.")]
        [SerializeField, DefaultValue(100f)]
        private FloatVar _speed;
        
        [Tooltip("Space mode for the direction vector.")]
        [SerializeField]
        private SpaceMode _spaceMode = SpaceMode.World;
        
        [Tooltip(
            "Direction of travel. " +
            "If zero, uses the Transform's forward. " +
            "Space is determined by the Space Mode above.")]
        [SerializeField]
        private Vector3Var _direction;

        // ─────────────────────────────────────────────────────────────────────────────
        //  Collision Sweep
        // ─────────────────────────────────────────────────────────────────────────────
        [Header("Collision Sweep")]

        [Tooltip("0 = Raycast, > 0 = SphereCast with this radius.")]
        [SerializeField, DefaultValue(0.1f)]
        private FloatVar _collisionRadius;

        [Tooltip("Layers the projectile can hit.")]
        [SerializeField] private LayerMask _layerMask = ~0;

        [Tooltip("How triggers behave in the sweep.")]
        [SerializeField] private QueryTriggerInteraction _triggerInteraction =
            QueryTriggerInteraction.Ignore;

        // ─────────────────────────────────────────────────────────────────────────────
        //  Hit Output
        // ─────────────────────────────────────────────────────────────────────────────
        [Header("Hit Output")]

        [Tooltip("Event sent when the projectile hits something.")]
        [SerializeField] private EventRef _hitEvent;

        [Tooltip("True if a hit occurred this step.")]
        [SerializeField, WriteOnly, OptionalField] 
        private BoolRef _hit;

        [Tooltip("Object that was hit. Uses the RaycastHit transform, which is the hit Rigidbody GameObject when available, otherwise the collider GameObject.")]
        [SerializeField, WriteOnly, OptionalField] 
        private GameObjectRef _hitObject;
        
        [OptionalField, WriteOnly]
        [Tooltip("Store complete hit information from the raycast. " +
                 "Use RaycastHit actions or properties to get more info.")]
        [SerializeField]
        private RaycastHitRef _hitInfo;

        // ─────────────────────────────────────────────────────────────────────────────
        //  Debug
        // ─────────────────────────────────────────────────────────────────────────────
        [Header("Debug")]

        [Tooltip("Draw the sweep segment in the Scene view.")]
        [SerializeField] private BoolVar _drawDebug;

        [SerializeField] private ColorVar _debugColor;

        // internal
        private Vector3 _previousPosition;
        private bool _initialized;

        // ─────────────────────────────────────────────────────────────────────────────
        //  Lifecycle
        // ─────────────────────────────────────────────────────────────────────────────
        public override bool CanExecute() => CheckParameters(_transform, _speed);

        public override void OnStart()
        {
            var t = _transform.Value;
            if (t != null)
            {
                _previousPosition = t.position;
                _initialized = true;
            }
            else _initialized = false;

            if (!_hit.IsNone) _hit.Value = false;
            if (!_hitObject.IsNone) _hitObject.Value = null;
        }

        public override void OnStop()
        {
            _initialized = false;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        //  EXECUTE
        // ─────────────────────────────────────────────────────────────────────────────
        public override void Execute()
        {
            var t = _transform.Value;
            if (t == null)
            {
                Finish();
                return;
            }

            if (!_initialized)
            {
                _previousPosition = t.position;
                _initialized = true;
            }

            var dt = Time.deltaTime; // FixedUpdateEveryFrame uses fixedDeltaTime
            var speed = Mathf.Max(0f, _speed.Value);
            var distance = speed * dt;

            if (distance <= 0f)
            {
                if (!_hit.IsNone) _hit.Value = false;
                return;
            }

            // ─────────────────────────────────────────────
            // Direction (Local or World)
            // ─────────────────────────────────────────────
            var dir = _direction.Value;

            if (dir.sqrMagnitude < 0.0001f)
            {
                // fallback to Transform forward (world)
                dir = t.forward;
            }
            else
            {
                dir = _spaceMode == SpaceMode.Local
                    ? t.TransformDirection(dir)
                    : dir; // already world space
            }

            dir.Normalize();
            if (dir.sqrMagnitude < 0.0001f)
            {
                if (!_hit.IsNone) _hit.Value = false;
                return;
            }

            // ─────────────────────────────────────────────
            // Sweep test
            // ─────────────────────────────────────────────
            var origin = _previousPosition;
            var radius = Mathf.Max(0f, _collisionRadius.Value);

            if (_drawDebug.Value)
            {
                Debug.DrawLine(origin, origin + dir * distance, _debugColor.Value);
            }

            bool hasHit;
            RaycastHit hitInfo;

            if (radius > 0f)
            {
                hasHit = Physics.SphereCast(
                    origin, radius, dir,
                    out hitInfo, distance,
                    _layerMask, _triggerInteraction);
            }
            else
            {
                hasHit = Physics.Raycast(
                    origin, dir,
                    out hitInfo, distance,
                    _layerMask, _triggerInteraction);
            }

            // ─────────────────────────────────────────────
            // Hit
            // ─────────────────────────────────────────────
            if (hasHit)
            {
                t.position = hitInfo.point;
                _previousPosition = hitInfo.point;

                StoreHit(hitInfo, true);

                if (!_hitEvent.IsNone)
                    SendEvent(_hitEvent);

                Finish();
            }
            else
            {
                // No hit → advance transform
                var newPos = origin + dir * distance;
                t.position = newPos;
                _previousPosition = newPos;

                StoreHit(default, false);
            }
        }

        // ─────────────────────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────────────────────
        private void StoreHit(RaycastHit hitInfo, bool didHit)
        {
            if (!_hit.IsNone) _hit.Value = didHit;

            if (!didHit)
            {
                if (!_hitObject.IsNone) _hitObject.Value = null;
                return;
            }

            if (!_hitObject.IsNone)
                _hitObject.Value = hitInfo.transform ? hitInfo.transform.gameObject : null;

            if (!_hitInfo.IsNone)
                _hitInfo.Value = hitInfo;
        }

        public override string GetSummary() =>
            "{_transform} fast projectile {_speed}m/s ({_spaceMode}) {_hitEvent}";
    }
}
