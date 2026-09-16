# :star: What's New

For a full list of changes, see the ChangeLog.

## [2.0.0 beta 83] - 2026-8-25

### ADDED

- Added network syncing of variables. Use a PlayMaker Network Variable Sync component on the same GameObject as the FSM you want to sync.
- Added more networking actions. Update the [Networking](addon:networking) addon and then the [Netcode for GameObjects](addon:netcode-for-gameobjects) addon.
- Added more [networking samples scenes](addon:samples.networking).
- Added optional name field to [GameObjectFindWithTag](action:GameObjectFindWithTag) action.
- Added [ListAddItems](action:ListAddItems) action so you can add multiple variables to a list.
- Added *Add Global Transition* to Parent State context menu.
- Added image drag and drop onto the graph view to create a new Image node.
- Added *Select Icon* to Image nodes to make it easier to select built-in icons.
- Added Size control to Image nodes to size the node using the native image size.
- Added an installer warning for Unity versions 6.3.0 - 6.3.4 that had compatibility issues with third party tools.
- Added some colors to the node colors palette.

### FIXED

- Fixed [String Split](action:StringSplit) errors when selecting Options: Everything.
- Fixed math functions in [ExpressionEvaluatorEvaluate](action:ExpressionEvaluatorEvaluate) actions.
- Fixed an issue where nested event transitions could leave a parallel region with no active state.
- Fixed duplicate OnMouseXXX system events when only the new Input System is enabled.
- Fixed Animate Variable actions sometimes entering an infinite transition loop when short-duration animations completed immediately on the first frame after entering Play Mode.
- Fixed InputShim compile errors with Input System package versions earlier than 1.14.0.
- Fixed a null reference error when checking unused events in prefab FSMs.
- Fixed [SpaceShooter](addon:samples.space-shooter) InputSystem requirement when importing.