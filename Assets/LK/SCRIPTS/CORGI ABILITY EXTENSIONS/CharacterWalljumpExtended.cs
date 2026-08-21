using System;
using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// An extension of CharacterWalljump that allows modifying horizontal movement speed during a wall jump.
    /// </summary>
    [MMHiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Corgi Engine/Character/Abilities/Character Walljump Extended")]
    public class CharacterWalljumpExtended : CharacterWalljump
    {
        [Header("Extended Walljump Speed")]
        /// Whether or not to modify the horizontal movement speed modifier when wall jumping
        [Tooltip("Whether or not to modify the horizontal movement speed modifier when wall jumping")]
        public bool ModifyMovementSpeed = true;
        /// The movement speed multiplier applied immediately upon wall jumping (e.g., 1.5f for 150% speed, 0.5f for 50% speed)
        [Tooltip("The movement speed multiplier applied immediately upon wall jumping (e.g., 1.5f for 150% speed, 0.5f for 50% speed)")]
        [MMCondition("ModifyMovementSpeed", true)]
        public float WallJumpMovementSpeedMultiplier = 1.5f;

        protected bool _speedModified = false;

        /// <summary>
        /// Overrides the base WalljumpRequest to inject our speed modifier logic
        /// </summary>
        protected override void WalljumpRequest()
        {
            // Cache the state before jumping to see if it succeeds
            bool willWallJump = EvaluateWallJumpConditions() && AbilityAuthorized && _condition.CurrentState == CharacterStates.CharacterConditions.Normal;

            // Run the original base Corgi Engine wall jump logic
            base.WalljumpRequest();

            // If the wall jump successfully triggered, apply the speed modifier
            if (willWallJump && ModifyMovementSpeed && _characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeedMultiplier = WallJumpMovementSpeedMultiplier;
                _speedModified = true;
            }
        }

        /// <summary>
        /// Every frame, we check if we should reset the speed back to normal
        /// </summary>
        public override void ProcessAbility()
        {
            base.ProcessAbility();

            // If we modified the speed, reset it once the character touches the ground 
            // or enters a state that isn't WallJumping/Jumping/Falling anymore
            if (_speedModified)
            {
                if (_controller.State.IsGrounded ||
                    (_movement.CurrentState != CharacterStates.MovementStates.WallJumping &&
                     _movement.CurrentState != CharacterStates.MovementStates.Jumping &&
                     _movement.CurrentState != CharacterStates.MovementStates.Falling))
                {
                    ResetWallJumpSpeed();
                }
            }
        }

        /// <summary>
        /// Resets the movement speed multiplier back to normal (1f)
        /// </summary>
        protected virtual void ResetWallJumpSpeed()
        {
            if (_characterHorizontalMovement != null)
            {
                _characterHorizontalMovement.MovementSpeedMultiplier = 1f;
            }
            _speedModified = false;
        }

        /// <summary>
        /// Safely clear modifiers if the ability gets reset or disabled
        /// </summary>
        public override void ResetAbility()
        {
            base.ResetAbility();
            if (_speedModified)
            {
                ResetWallJumpSpeed();
            }
        }
    }
}