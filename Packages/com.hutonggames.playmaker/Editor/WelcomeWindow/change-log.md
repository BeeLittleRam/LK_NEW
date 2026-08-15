# :feather: Changelog

A running log of recent changes.  
See the package CHANGELOG for a full list of changes.

## [2.0.0 beta 80] - 2026-8-7

### Added

- Added Copy/Paste support in quaternion action fields.
- Added alt-drag middle or right mouse button to zoom the graph view (useful for wacom support).
- Added Recursive option to [ForEachChild](action:ForEachChild) action.
- Added optional output variable to [FloatRemap](action:FloatRemap) action.
- Added [TweenTransformLocalRotation](action:TweenTransformLocalRotation) action.

### Fixed

- Fixed debug values for input variables.
- Fixed NullReferenceException when selecting a list variable in actions like List Remove.
- Fixed the occasional NullReferenceException when deleting states in prefabs.
- Fixed AnimationCurve and Gradient values not always saving when creating variables in the new variable dialog from an action field.
- Fixed auto-selection of new Result variable in [GameObjectGetComponentsInChildren](action:GameObjectGetComponentsInChildren) action.
- Fixed [CheckGameObject](action:CheckGameObject) and [LoopSkip__CheckGameObject](action:LoopSkip__CheckGameObject) to allow checking against null.
- Fixed Comment Node checkboxes not updating the Markdown source.
- Fixed [TransformSetLocalRotation](action:TransformSetLocalRotation) so you can enter a constant value.
- Hide WorldUp in [TransformLookAtDirection](action:TransformLookAtDirection) when not used.

## [2.0.0 beta 79] - 2026-8-4

### Added

- Added Runtime Errors for actions that report runtime errors or throw exceptions. These errors are shown on actions and in the error finders.
- Added [LoadGameFromString](action:LoadGameFromString) and [SaveGameToString](action:SaveGameToString) actions.
- Added [EncryptString](action:EncryptString) and [DecryptString](action:DecryptString) actions.
- Added [UnityWebRequest](addon:unity-web-request) addon.
- Added [AssetBundle](addon:asset-bundle) addon.
- Added [Tools > Custom Action Wizard](menu:PlayMaker/Tools/Custom Action Wizard...). This is a 1.0 version that makes the outline of an action; you still need to implement the Execute method.
- Added Smooth Time and Max Speed options to [TransformAlignToDirection](action:TransformAlignToDirection) and [TransformAlignToDirection2D](action:TransformAlignToDirection2D) actions.
- Added Space setting and debug gizmos to [AimCanShootAtPoint](action:AimCanShootAtPoint) action.
- Added API to set [custom action category icons](docs:api/custom-actions/action-category-icons/).

## [2.0.0 beta 78] - 2026-8-1

### Changed

- Added [PlayerInput](category:InputSystem/PlayerInput) and [InputAction](category:InputSystem/InputAction) actions to base install (instead of an Addon).
- Optimized textures in [Space Shooter](addon:space-shooter) sample to reduce install package size.

### Added

- Added a note to InputAction actions about enabling the InputAction.
- Added [PlayerInputSimpleLook](action:PlayerInputSimpleLook) and [InputActionSimpleLook](action:InputActionSimpleLook) actions.
- Ctrl+click Edit buttons in FSM Component inspectors to open the FSM in a new window.

### Fixed

- Fixed [OnBecameInvisible](docs:guides/system-events/) system event.
- Fixed list view focus when switching tabs so delete, ctrl+c, ctrl+v, and ctrl+x work as expected.
- Fixed [GameObject Tag actions re-entry bug](https://hutonggames.com/playmakerforum/index.php?topic=26897.0).
- Fixed Scene GUI errors in [TransformClampRotation](action:TransformClampRotation) and [TransformMoveTowardsPosition](action:TransformMoveTowardsPosition) actions when using variables.
- Fixed [TransformCheckIsVisible](action:TransformCheckIsVisible) missing description.
- Fixed FSM log errors not added before Error Pause breaks.
- Fixed console errors when hovering variable selectors in the Action Browser preview.
- Fixed auto-selection of new FSMs when multiple FSM Editors are open.

## [2.0.0 beta 77] - 2026-7-26

### Improved

- Improved performance when running many FSMs.

### Added

- Added PlayerInputGetMoveVector2/Vector3 actions in [PlayerInput Addon](addon:input-system.player-input).
- Added InputActionGetMoveVector2/Vector3 actions in [InputAction Addon](addon:input-system.input-action).
- Added CHANGELOG.md to install package.

### Fixed

- Fixed [Global Variables occasionally losing their type](https://hutonggames.com/playmakerforum/index.php?topic=26887.0).
- Fixed [variable selection errors after deleting global variables](https://hutonggames.com/playmakerforum/index.php?topic=26887.0).
- Fixed an infinite loop when a DataRecord had a field using the same DataDefinition as the record itself.
- Fixed [CharacterControllerMoveOnGround](action:CharacterControllerMoveOnGround) [downward velocity bug](https://hutonggames.com/playmakerforum/index.php?topic=26886.0).
- Fixed [NextFrameEvent](action:NextFrameEvent) update mode (it requires EveryFrame).
- Fixed [SetObjectProperty](action:SetObjectProperty) not selecting the GameObject Active (Bool) property.
- Fixed property setter dropdowns showing read-only instance properties.
- Fixed [namespace conflict](https://hutonggames.com/playmakerforum/index.php?topic=26883.msg114590) with StateMachine.
- Fixed [LoadSceneByIndex needing a Result variable](https://hutonggames.com/playmakerforum/index.php?topic=26888.0).
- Fixed optional fields being required in some actions.
- Fixed Auto Process Prefabs setting when updating samples to URP.

## [2.0.0 beta 76] - 2026-7-10

### Fixed

- Fixed Bool Is True summary.
- Fixed alignment of Enum Flags fields.

## [2.0.0 beta 75] - 2026-7-7

## Added

- Added [WaitForFrames](action:WaitForFrames) action.
- Added DidHit output to Raycast actions.

### Fixed

- Fixed null ref error when removing DataDefinition from a DataComponent.
- Fixed new state name when dragging some actions (e.g., Loop State or Tween actions) into the graph view.

## [2.0.0 beta 74] - 2026-7-4

### Changed

- Show relative path in search result action category headers.
- Allow IntegerRef and FloatRef write conversions in actions.

### Added

- Added [TransformMoveAwayFromPosition](action:TransformMoveAwayFromPosition) action.
- Added [FloatSpring](action:FloatSpring), [Vector2Spring](action:Vector2Spring), [Vector3Spring](action:Vector3Spring) for spring like oscillations.
- Added [TransformSquashAndStretch](action:TransformSquashAndStretch) and [TransformSquashAndStretch2D](action:TransformSquashAndStretch2D) actions. [FloatSpring](action:FloatSpring) is a good input to control these if you want bouncy reactions.
- Added [RandomGetGaussian](action:RandomGetGaussian), [RandomGetGaussianVector2](action:RandomGetGaussianVector2), [RandomGetGaussianVector3](action:RandomGetGaussianVector3) and actions.
- Added [PostProcessStackV2 Addon](addon:post-processing-stack-v2) and [URP Volumes Addons](addon:universal-render-pipeline-volumes) for post-processing control.
- Added a Hide button to [Addons Browser](menu:PlayMaker/Addons) to hide tags you're not interested in.
- Added experimental [Sketch Addon](addon:sketch) and [Sketch Samples](addon:samples.sketch) for [Processing](https://processing.org/) style sketching.
- Added experimental [Image Processing Addon](addon:image-processing) for performant image processing.
- Added dotted lines to show [GotoState](action:GotoState) connections in the Graph View.
- Added a Restore On Exit option to Flicker actions.

### Fixed

- Fixed undo/redo when swapping an action with the convert menu.
- Fixed editor performance when selecting a state that rapidly enters/exits while playing, such as a tight loop between two states.
- Fixed editor performance when many visible variable values change rapidly while playing.
- Fixed dropping actions into manually resized parent states so the new state is in the parent state.
- Fixed expanding parent state sometimes overlapping other states.
- Fixed rendering artifacts when pasting text into a Markdown edit field.
- Fixed URP shader errors in samples when URP was not enabled.

## [2.0.0 beta 73] - 2026-6-23

### Changed

- Removed private beta information from the Welcome Window in preparation for public release.
- Tweaked category list view headers:
  - The main header and sticky header are now separate.
  - The main header always shows the selected category and has clickable breadcrumbs. This makes it clearer which category you're searching inside.
  - The sticky header shows the last category that scrolled offscreen.

### Improved

- Better summaries for [DataTableAddRow](action:DataTableAddRow) and [DataTableSetRowValues](action:DataTableSetRowValues) actions.

### Fixed

- Fixed shortened string values disappearing in action summaries.

## [2.0.0 beta 72] - 2026-6-19

### Changed

- Improved [Global Variable Text Binding](https://hutonggames.com/playmaker/docs/guides/ui-widgets/helpers/global-variable-text-binding/) UI.

### Added

- Added an Orange and Blue color to the node color picker.

### Fixed

- Fixed errors in Unity 6.5 (use the new 6.5 unitypackage)
- Fixed missing Sent By section in Transition inspector.

## [2.0.0 beta 71] - 2026-6-16

### Added

- Added raycast outputs to [InputMousePickEvent](action:InputMousePickEvent) action.
- Added [NavMeshPathDebugDraw](action:NavMeshDebugDraw) action to debug NavMeshPaths.
- Added [NavMeshPathGetNextCorner](action:NavMeshPathGetNextCorner) action to more easily use a NavMeshPath without a NavMeshAgent.
- Added documentation for [Global Variable Text Binding](https://hutonggames.com/playmaker/docs/guides/ui-widgets/helpers/global-variable-text-binding/).
- Added a few more controls to  [UIToolkit Samples](addon:samples.ui-toolkit).
- Added screenshots for all samples in [Addons](menu:PlayMaker/Addons).

### Fixed

- Fixed null exception errors [NavMeshCalculatePath](action:NavMeshCalculatePath) actions.
- Fixed disabling [Debug FSM](docs:components/debug-fsm/) while playing.
- Fixed missing UI Tag in [UGUI Samples](addon:ui).
- Fixed NavMeshPath variable icon.

## [2.0.0 beta 70] - 2026-6-13

### Changed

- Removed some internal PlayMaker methods from [CallComponentMethod](action:CallComponentMethod) method selectors.
- Tweaked formatting of method return type in CallMethod actions.
- Improved the Save System Settings inspector.

### Added

- Added [TouchEvent](action:TouchEvent) and [TouchObjectEvent](action:TouchObjectEvent) actions for the new Input System.
- Added [InputTouchEvent](action:InputTouchEvent) and [InputTouchObjectEvent](action:InputTouchObjectEvent) actions for the legacy input system.
- Added [GetFsmState](action:GetFsmState), [GotoPreviousState](action:GotoPreviousState), and [GetPreviousStateName](action:GetPreviousStateName).
- Added [ListGetLastItem](action:ListGetLastItem), [ListGetPreviousItem](action:ListGetPreviousItem), and [ListSwapItems](action:ListSwapItems).
- Added some sound effects to [Space Shooter](addon:space-shooter) sample.

### Fixed

- Fixed Null reference exception in CallMethod selector.

## [2.0.0 beta 69] - 2026-6-10

### Changed

- Moved WebCam and Location Services to addons so you don't need to set permissions if you don't use them.
- Addons now use version info to measure Needs Update. If local files differ from the package you still get a refresh option.
- Improved action browser search by replacing the old search index with a simpler in-memory search system.

### Added

- Added Check Usages and Remove buttons in the Addons window.

## [2.0.0 beta 68] - 2026-6-2

### Changed

- More work on [SpaceShooter Sample](addon:space-shooter). It's changed enough that you should delete the folder before re-importing it.
    - Re-worked movement tutorial to advance on player progress.
    - Added collect beacons flight training.
    - Added shoot sentry turrets combat training.
    - Added SOS mission and pirate ambush.
- Changed graph view Broadcast Icon to show events sent to other GameObjects (not just Broadcast Events).

### Added

- Added pasting of copied regions as new FSMs.
- Added Rect variable properties: center, position, and size.
- Added option to use first string field in the DataDefinition as the DataTable key.
- Added [UIToolkit Addon](addon:ui-toolkit) and [UIToolkit Samples](addon:samples.ui-toolkit). Not 100% complete, but should cover common UI needs.
- Added EnhancedTouch actions to Input System [Touch Addon](addon:input-system.touch) actions.
- Added TargetCone generator and validator blocks for [FindValidRandomPosition] action.
- Added TrueEvent and FalseEvent to [EvaluateBooleanExpression](action:EvaluateBooleanExpression).
- Added [TransformOnChildCountChanged](action:TransformOnChildCountChanged) action.
- Added [FlickerCanvasGroup](action:FlickerCanvasGroup) and [FlickerGraphic](action:FlickerGraphic) actions.
- Added Use Variable Tokens option to more actions. E.g. [GameObjectSetName](action:GameObjectSetName).
- Added [DebugVariable](action:DebugVariable) action to debug a specific variable while in a state.
- Added Debug Panel Scale setting in [PlayMaker Settings](menu:PlayMaker/Help/Settings).

### Fixed

- Fixed Exclude Self in [BroadcastEvent](action:BroadcastEvent). It could sometimes interrupt ForEach loops.
- Fixed <, >, and & rendering in action summaries, e.g., in ExpressionEvaluator actions.
- Fixed re-importing Playmaker after previously removing it from the project.
- Fixed GlobalObjectId warnings after deleting assets.
- Fixed sample billboard components in URP.

## [2.0.0 beta 67] - 2026-5-24

### Interactables

- [Interactable system](docs:guides/interactables/) has changed a bit in this release. This might require setup changes in your project.
- Added [Interactor component](docs:guides/interactables/interactor-component/). Use [InteractorUpdate](action:InteractorUpdate) instead of GameObjectUpdateInteractables (now obsolete).
- Streamlined [Interactable component](docs:guides/interactables/interactable-component/). Added **Inside Trigger** and **Measurement Space** options.
- Added docking setup to [Interactable component](docs:guides/interactables/interactable-component/). Used by [GameObjectDockWithInteractable](action:GameObjectDockWithInteractable) action.
- Added [PlayMaker > Browsers > Interactables](menu:PlayMaker/Browsers/Interactables) to browse all interactable objects in the scene.
- Reworked [First Person Samples](addon:first-person) to use the new Interactor component and actions.

### Changed

- Improved action summary tooltips. Shows parameter name and description.
- Allow selection of compatible variable properties (not just exact types).
- Meter components now set the current value in OnEnable.
- Show regions in the minimap.

### Added

- Added [PlayMaker > Debug > Find Obsolete Actions](menu:PlayMaker/Debug/Find Obsolete Actions) to make it easier to find deprecated actions.
- Added **RowNotFound** policy to DataTable actions like [DataTableGetRowValues](action:DataTableGetRowValues).
- Added constraints to [CharacterControllerClimb](action:CharacterControllerClimb) action.
- Added a tilted ladder to ClimbLadder scene in [First Person Samples](addon:first-person).
- Added line docking to ClimbWall scene in [First Person Samples](addon:first-person).
- Added [RigidbodyPickUp](action:RigidbodyPickUp), [RigidbodyDrop](action:RigidbodyDrop), and [RigidbodyThrow](action:RigidbodyThrow) actions. Updated Pickup and Throw scenes in [First Person Samples](addon:first-person).
- Added [Rigidbody2DPickUp](action:Rigidbody2DPickUp), [Rigidbody2DDrop](action:Rigidbody2DDrop), and [Rigidbody2DThrow](action:Rigidbody2DThrow) actions.
- Added [RandomGetPointOffscreen](action:RandomGetPointOffscreen) action and offscreen blocks for [FindValidRandomPosition](action:FindValidRandomPosition) and [FindValidRandomPosition2D](action:FindValidRandomPosition2D) actions.
- Added Enemy scenes to [Top-Down Shooter Samples](addon:top-down-shooter).

### Fixed

- Fixed actions that should be allowed to set null values. E.g., [ImageSetSprite](action:ImageSetSprite).
- Fixed [CharacterControllerClimb](action:CharacterControllerClimb) action so LookDirection respect input direction. Forward moves in look direction, back moves in opposite direction.
- Fixed [CharacterControllerCheckIsGrounded (SphereCast)](action:CharacterControllerCheckIsGrounded__SphereCast) action false positives.
- Fixed Stopwatch meter in [Shooting Gallery Sample](addon:shooting-gallery).

## [2.0.0 beta 66] - 2026-5-8

### Changed

- Improved transition and logging performance. This is actually fairly invasive, so please report any issues you encounter.
- Restored icon, name, and description editing in FSM Inspector.
- Changed [CharacterControllerJump](action:CharacterControllerJump) to output a jump velocity that you then use in "In Air" actions like [CharacterControllerMoveInAir](action:CharacterControllerMoveInAir). This fixes hitches in the hand-off between Jump and other movement actions.
- Tweaked Jump scenes in [First Person Samples](addon:first-person) to better handle falling and ramps.
- Moved Require Raycast Hit from actions to the Interactable component – it's part of the setup of an interactable object.
- Converted ButtonDoor buttons in [First Person Samples](addon:first-person) to use Interactables.
- Only dim PlayMaker hierarchy icon if all FSMs on the GameObject are disabled.

### Added

- Added root canvas menu to make it easier to add a second FSM and select FSMs on a GameObject.
- Added [CharacterControllerMoveOnGround](action:CharacterControllerMoveOnGround) action that applies downward velocity to stick to ramps etc.
- Added [FloatSelectValue](action:FloatSelectValue), [IntegerSelectValue](action:IntegerSelectValue), etc. to select a value based on a Bool. They're conceptually similar to ConvertBoolToFloat etc. but with that emphasis on the target type.
- Added DebugInfo to [PhysicsCheckCollider](action:PhysicsCheckCollider) action.
- Added PressurePlateDoor scene to [First Person Samples](addon:first-person).
- Added logging tools to PlayMaker/Tools. If you have many FSMs updating often, you can disable logging on them to improve performance. The trade-off is the FSM Log and Debug Flow will not work on those FSMs. NOTE: Logging is always off in builds.
- Added a separate Enable Variable History setting in the Debug settings. Recording variable history can be expensive. The trade-off is that you won't see old variable values as you step Prev/Next states using Debug Flow.

### Fixed

- Fixed infinite loop checking in [Loop State](action:LoopState) action with Forever setting.
- Fixed [ListFind](action:ListFind) error with GameObjectList and Name tests.
- Fixed random null ref error when editing prefabs.
- Fixed Global variables losing type information.

