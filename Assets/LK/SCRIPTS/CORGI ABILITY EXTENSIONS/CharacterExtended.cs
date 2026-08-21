using UnityEngine;
using System.Collections;
using MoreMountains.Tools;

namespace MoreMountains.CorgiEngine
{
    [AddComponentMenu("Corgi Engine/Character/Core/Character Extended")]
    public class CharacterExtended : Character
    {
        [Header("Delayed Turning")]
        [Tooltip("Delay before turning to the opposite direction.")]
        public float TurnDelay = 0.2f;

        public bool IsTurning { get; protected set; }
        public bool FlipLocked { get; set; }
        public Vector2 LockedFacingDirection { get; set; }

        protected Coroutine _flipCoroutine;
        protected bool _pendingFlip;

        public override void Flip(bool IgnoreFlipOnDirectionChange = false)
        {
            // 1. Strict Lock Check: Abort instantly if an ability locked flipping
            if (FlipLocked)
            {
                return;
            }

            if (TurnDelay <= 0f)
            {
                base.Flip(IgnoreFlipOnDirectionChange);
                return;
            }

            // If we are already waiting out a turn delay for this direction, ignore duplicate requests
            if (_pendingFlip)
            {
                return;
            }

            if (_flipCoroutine != null)
            {
                StopCoroutine(_flipCoroutine);
            }

            _flipCoroutine = StartCoroutine(DelayedFlip(IgnoreFlipOnDirectionChange));
        }

        protected virtual IEnumerator DelayedFlip(bool IgnoreFlipOnDirectionChange)
        {
            _pendingFlip = true;
            IsTurning = true;

            // Cache the orientation state before waiting
            bool initialFacingRight = IsFacingRight;

            yield return new WaitForSeconds(TurnDelay);

            // 2. Mid-Delay Lock Check: If an ability locked the flip WHILE we were waiting, abort!
            if (FlipLocked)
            {
                _pendingFlip = false;
                IsTurning = false;
                _flipCoroutine = null;
                yield break;
            }

            // --- INPUT BUFFER VALIDATION ---
            // Read Corgi's input manager to verify if the key/joystick is still actively held down
            float horizontalInput = LinkedInputManager.PrimaryMovement.x;
            bool inputIsRight = horizontalInput > 0.1f;
            bool inputIsLeft = horizontalInput < -0.1f;

            bool structuralValidationPassed = false;

            if (initialFacingRight && inputIsLeft)
            {
                // Was facing right, and player is STILL holding Left after the window
                structuralValidationPassed = true;
            }
            else if (!initialFacingRight && inputIsRight)
            {
                // Was facing left, and player is STILL holding Right after the window
                structuralValidationPassed = true;
            }

            // Only perform the true Corgi flip if they held the input past the buffer duration
            if (structuralValidationPassed)
            {
                base.Flip(IgnoreFlipOnDirectionChange);
            }

            // Clean up state tracking
            _pendingFlip = false;
            IsTurning = false;
            _flipCoroutine = null;
        }

        public virtual void CancelTurn()
        {
            if (_flipCoroutine != null)
            {
                StopCoroutine(_flipCoroutine);
            }

            _flipCoroutine = null;
            _pendingFlip = false;
            IsTurning = false;
        }

        public virtual void SetFlipLock(bool value)
        {
            FlipLocked = value;
            FlipModelOnDirectionChange = !value;

            // If an ability locks our direction, we should clear out any active turn buffers immediately
            if (value)
            {
                CancelTurn();
            }
        }
    }
}