# :star: What's New

For a full list of changes, see the ChangeLog.

## [2.0.0 beta 81] - 2026-8-11

### CHANGED

- Zoom with right-click drag instead of Alt middle/right click drag.

### IMPROVED

- Improved performance when editing graphs with many regions.

### ADDED

- Added InputSystem binding actions.
- Added [PlayerInputButtonEvents](action:PlayerInputButtonEvents) action.
- Added FSM Input/Output support for InputSystem variables.
- Added [Export/Import CSV](docs:guides/data-tables/data-table-csv/) support for [DataTableComponents](docs:components/data-table/).
- Added Ctrl/Cmd + Alt Click on variable toggles to make a new variable and keep the current value.

### FIXED

- Fixed occasional console errors while editing templates.
- Fixed RunTemplate update timing, especially for physics actions in templates.
- Fixed empty Addons list if there are any empty json files (or read errors).
- Fixed [editor crashes on Linux](https://hutonggames.com/playmakerforum/index.php?topic=26922.0).
