using UnityEngine;
using System.Collections;
using MoreMountains.CorgiEngine;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Jump Extended")]
    public class CharacterJumpExtended : CharacterJump
    {
        [Header("Hard Landing Settings")]
        [Tooltip("How long (in seconds) the player will be frozen when HardLanded is called.")]
        public float hardLandingDuration = 1.0f;

        public bool isHardLanding = false;
        protected Coroutine _hardLandingCoroutine;

        // Cached reference to our companion extended dash script
        protected CharacterDashExtended _characterDashExtended;

        protected override void Initialization()
        {
            base.Initialization();
            _characterDashExtended = _character?.FindAbility<CharacterDashExtended>();
        }

        /// <summary>
        /// Overriding Corgi's condition check to allow jumping while in the Dashing state.
        /// </summary>
        protected override bool EvaluateJumpConditions()
        {
            // If hard landing or frozen, absolutely do not jump
            if (isHardLanding || _condition.CurrentState == CharacterStates.CharacterConditions.Frozen)
            {
                return false;
            }

            // Force permission if the player is actively dashing
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing)
            {
                return true;
            }

            // Otherwise, fall back to default Corgi jump requirements (grounded, jump counts, etc.)
            return base.EvaluateJumpConditions();
        }

        /// <summary>
        /// Intercepts the jump start sequence to cleanly interrupt an active dash.
        /// </summary>
        public override void JumpStart()
        {
            if (_movement.CurrentState == CharacterStates.MovementStates.Dashing && _characterDashExtended != null)
            {
                // Tells the dash script to stop its internal coroutines while preserving horizontal force
                _characterDashExtended.CancelDashForJump();
            }

            base.JumpStart();
        }

        public override void ProcessAbility()
        {
            if (isHardLanding)
            {
                _controller.SetHorizontalForce(0f);
            }

            base.ProcessAbility();

            if (!AbilityAuthorized) { return; }

            bool isJumpingState = (_movement.CurrentState == CharacterStates.MovementStates.Jumping)
                               || (_movement.CurrentState == CharacterStates.MovementStates.DoubleJumping);

            if (isJumpingState)
            {
                if (_controller.Speed.y < 0f && !_controller.State.IsGrounded)
                {
                    _movement.ChangeState(CharacterStates.MovementStates.Falling);
                }
            }
        }

        public virtual void HardLanded()
        {
            if (_hardLandingCoroutine != null)
            {
                StopCoroutine(_hardLandingCoroutine);
            }
            _hardLandingCoroutine = StartCoroutine(HardLandingSequence());
        }

        protected virtual IEnumerator HardLandingSequence()
        {
            isHardLanding = true;
            _controller.SetHorizontalForce(0f);

            if (_character != null && _character.LinkedInputManager != null)
            {
                _character.LinkedInputManager.InputDetectionActive = false;
            }

            _movement.ChangeState(CharacterStates.MovementStates.Idle);
            yield return new WaitForSeconds(hardLandingDuration);

            if (_character != null && _character.LinkedInputManager != null)
            {
                _character.LinkedInputManager.InputDetectionActive = true;
            }

            isHardLanding = false;
            _hardLandingCoroutine = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (isHardLanding && _character != null && _character.LinkedInputManager != null)
            {
                _character.LinkedInputManager.InputDetectionActive = true;
            }
            isHardLanding = false;
        }
    }
}