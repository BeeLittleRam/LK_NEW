using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Dash Extended")]
    public class CharacterDashExtended : CharacterDash
    {
        [Header("Input")]
        public bool ReadInput = true;

        [Header("Backdash")]
        public bool EnableBackdash = true;
        public float BackdashDistance = 2f;
        public float BackdashForce = 30f;

        [Header("Runtime")]
        [SerializeField]
        protected bool _isBackdashing;
        public bool IsBackdashing => _isBackdashing;

        protected CharacterExtended _characterExtended;
        protected float _savedDistance;
        protected float _savedForce;

        // Local tracking for when a dash is canceled by a jump
        protected float _cancelCooldownTimestamp = 0f;

        protected override void Initialization()
        {
            base.Initialization();
            _characterExtended = _character as CharacterExtended;

            if (_characterExtended == null)
            {
                Debug.LogError($"CharacterDashExtended on {gameObject.name} requires a CharacterExtended component instead of a regular Character component!", this);
            }
        }

        protected override void HandleInput()
        {
            if (!ReadInput) { return; }
            base.HandleInput();
        }

        /// <summary>
        /// Overriding Corgi's standard conditions to make sure our custom jump-cancel cooldown is respected.
        /// </summary>
        public override bool DashConditions()
        {
            if (Time.time < _cancelCooldownTimestamp)
            {
                return false;
            }
            return base.DashConditions();
        }

        public virtual void StartBackdash()
        {
            if (!EnableBackdash || !DashAuthorized() || !DashConditions()) { return; }
            InitiateBackdash();
        }

        protected virtual void InitiateBackdash()
        {
            if (_characterExtended != null)
            {
             //   _characterExtended.SetFlipLock(true);
            }

            _isBackdashing = true;
            _savedDistance = DashDistance;
            _savedForce = DashForce;

            DashDistance = BackdashDistance;
            DashForce = BackdashForce;

            InitiateDash();
        }

        protected override void ComputeDashDirection()
        {
            if (_isBackdashing)
            {
                _dashDirection = _character.IsFacingRight ? Vector2.left : Vector2.right;
                return;
            }

            base.ComputeDashDirection();
        }

        /// <summary>
        /// Called exclusively by our Extended Jump script to safely shut down the dash mechanics
        /// while intentionally letting horizontal inertia/velocity flow uninterrupted into the jump physics.
        /// </summary>
        public virtual void CancelDashForJump()
        {
            StopDash(); // let Corgi clean itself properly

            _cancelCooldownTimestamp = Time.time + DashCooldown;

            if (_isBackdashing)
            {
                DashDistance = _savedDistance;
                DashForce = _savedForce;
                _isBackdashing = false;
            }

            // DO NOT touch movement state here
        }

        public override void StopDash()
        {
            base.StopDash();

            if (_isBackdashing)
            {
                DashDistance = _savedDistance;
                DashForce = _savedForce;
                _isBackdashing = false;
              //  StartCoroutine(UnlockFlipAfterDelay());
            }
        }

        protected virtual IEnumerator UnlockFlipAfterDelay()
        {
            yield return null;
            yield return new WaitForSeconds(0.05f);

            if (_characterExtended != null)
            {
                _characterExtended.SetFlipLock(false);
            }
        }
    }
}