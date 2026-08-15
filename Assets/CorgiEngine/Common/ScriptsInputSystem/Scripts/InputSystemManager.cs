using System;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace MoreMountains.CorgiEngine
{
    /// <summary>
    /// This is a replacement InputManager if you prefer using Unity's InputSystem over the legacy one.
    /// Note that it's not the default solution in the engine at the moment, because older versions of Unity don't support it, 
    /// and most people still prefer not using it
    /// You can see an example of how to set it up in the MinimalLevel_InputSystem demo scene
    /// </summary>
    public class InputSystemManager : InputManager
    {
        /// a set of input actions to use to read input on
        [NonSerialized]
        public CorgiEngineInputActions InputActions;

        [Header("Optional Input Asset Override")]
        [Tooltip("If assigned, this asset is used instead of the built-in CorgiEngineInputActions generated asset.")]
        public InputActionAsset CustomInputActionsAsset;

        [Tooltip("Action map name to read controls from when using either default or custom assets.")]
        public string PlayerControlsActionMapName = "PlayerControls";

        protected bool _inputActionsEnabled = true;
        protected bool _initialized = false;
        protected InputActionAsset _activeInputActionsAsset;
        protected InputActionMap _playerControlsMap;
        protected InputAction _primaryMovementAction;
        protected InputAction _secondaryMovementAction;
        
        protected override void Start()
        {
            if (!_initialized)
            {
                Initialization();
            }            
        }

        /// <summary>
        /// On init we register to all our actions
        /// </summary>
        protected override void Initialization()
        {
            base.Initialization();

            _inputActionsEnabled = true;

            if (CustomInputActionsAsset != null)
            {
                _activeInputActionsAsset = CustomInputActionsAsset;
                InputActions = null;
            }
            else
            {
                InputActions = new CorgiEngineInputActions();
                _activeInputActionsAsset = InputActions.asset;
            }

            _playerControlsMap = _activeInputActionsAsset.FindActionMap(PlayerControlsActionMapName, false);
            if (_playerControlsMap == null)
            {
                Debug.LogWarning($"{nameof(InputSystemManager)} could not find action map '{PlayerControlsActionMapName}' on asset '{_activeInputActionsAsset.name}'.");
                _initialized = true;
                return;
            }

            _primaryMovementAction = FindPlayerAction("PrimaryMovement");
            if (_primaryMovementAction != null)
            {
                _primaryMovementAction.performed += context => _primaryMovement = context.ReadValue<Vector2>();
            }

            _secondaryMovementAction = FindPlayerAction("SecondaryMovement");
            if (_secondaryMovementAction != null)
            {
                _secondaryMovementAction.performed += context => _secondaryMovement = context.ReadValue<Vector2>();
            }

            BindButtonAction("Jump", JumpButton);
            BindButtonAction("Run", RunButton);
            BindButtonAction("Dash", DashButton);
            BindButtonAction("Shoot", ShootButton);
            BindButtonAction("SecondaryShoot", SecondaryShootButton);
            BindButtonAction("Interact", InteractButton);
            BindButtonAction("Reload", ReloadButton);
            BindButtonAction("Pause", PauseButton);
            BindButtonAction("SwitchWeapon", SwitchWeaponButton);
            BindButtonAction("SwitchCharacter", SwitchCharacterButton);
            BindButtonAction("TimeControl", TimeControlButton);
            BindButtonAction("Roll", RollButton);
            BindButtonAction("Swim", SwimButton);
            BindButtonAction("Glide", GlideButton);
            BindButtonAction("Jetpack", JetpackButton);
            BindButtonAction("Fly", FlyButton);
            BindButtonAction("Grab", GrabButton);
            BindButtonAction("Throw", ThrowButton);
            BindButtonAction("Push", PushButton);

            _initialized = true;
        }

        protected virtual InputAction FindPlayerAction(string actionName)
        {
            var action = _playerControlsMap.FindAction(actionName, false);
            if (action == null)
            {
                Debug.LogWarning($"{nameof(InputSystemManager)} could not find action '{actionName}' in map '{PlayerControlsActionMapName}'.");
            }
            return action;
        }

        protected virtual void BindButtonAction(string actionName, MMInput.IMButton imButton)
        {
            var action = FindPlayerAction(actionName);
            if (action == null)
            {
                return;
            }

            action.performed += context => { BindButton(context, imButton); };
            action.canceled += context => { BindButton(context, imButton); };
        }

        /// <summary>
        /// Changes the state of our button based on the input value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="imButton"></param>
        protected virtual void BindButton(InputAction.CallbackContext context, MMInput.IMButton imButton)
        {
            var control = context.control;

            if (control is ButtonControl button)
            {
                if (button.wasPressedThisFrame)
                {
                    imButton.TriggerButtonDown();
                }
                if ( button.wasReleasedThisFrame || (imButton.State.CurrentState == MMInput.ButtonStates.ButtonPressed && !button.isPressed) )
                {
                    imButton.TriggerButtonUp();
                }
            }
        }

        protected override void Update()
        {
            if (IsMobile && _inputActionsEnabled)
            {
                _inputActionsEnabled = false;
                _activeInputActionsAsset?.Disable();
            }

            if (!IsMobile && (InputDetectionActive != _inputActionsEnabled))
            {
                if (InputDetectionActive)
                {
                    _inputActionsEnabled = true;
                    _activeInputActionsAsset?.Enable();
                    ForceRefresh();
                }
                else
                {
                    _inputActionsEnabled = false;
                    _activeInputActionsAsset?.Disable();
                }
            }
        }

        protected virtual void ForceRefresh()
        {
            _primaryMovement = (_primaryMovementAction != null) ? _primaryMovementAction.ReadValue<Vector2>() : Vector2.zero;
            _secondaryMovement = (_secondaryMovementAction != null) ? _secondaryMovementAction.ReadValue<Vector2>() : Vector2.zero;
        }

        /// <summary>
        /// On enable we enable our input actions
        /// </summary>
        protected virtual void OnEnable()
        {
            if (!_initialized)
            {
                Initialization();
            }
            _activeInputActionsAsset?.Enable();
        }

        /// <summary>
        /// On disable we disable our input actions
        /// </summary>
        protected virtual void OnDisable()
        {
            _activeInputActionsAsset?.Disable();
        }
    }
}