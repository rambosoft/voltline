# Scene Flow
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: approved scenes, build order, transitions, persistent objects, menu/gameplay/results ownership
Depends on: 04-architecture.md, 05-project-structure.md
Do not duplicate with: UI flow notes inside scene-specific implementation comments

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Scene model

The project uses **three production scenes**:

1. `Bootstrap`
2. `MainMenu`
3. `Gameplay`

This is intentionally small.

## Why this model

The game needs:

- a clean app start
- a clear title/menu experience
- isolated gameplay state
- a fast retry path
- low mental overhead for contributors

Three scenes provide clean boundaries without overengineering.

## Scene list and purpose

### `Bootstrap`
Purpose:

- app initialization only
- load/save initialization
- service setup
- environment validation in dev builds
- route to `MainMenu`

Rules:

- no heavy gameplay content
- no long-lived user-facing screen time
- should hand off quickly

### `MainMenu`
Purpose:

- title screen
- attract/demo animation
- play button
- settings entry
- future daily challenge entry
- future cosmetics entry

Rules:

- should explain the game visually, not verbally
- can include a lightweight ambient/demo presentation
- should remain responsive and uncluttered

### `Gameplay`
Purpose:

- actual run
- HUD
- pause overlay
- result/death panel
- retry loop
- optional settings overlay access

Rules:

- result panel is in-scene, not a separate results scene
- retry should not round-trip through `MainMenu`
- scene restart or run reset path must be optimized for speed

## Build scene order

Build order:

1. `Bootstrap`
2. `MainMenu`
3. `Gameplay`

Do not change build order casually.  
If changed, update this file and any build scripts.

## Transition rules

### App start
`Bootstrap` -> `MainMenu`

### Start run
`MainMenu` -> `Gameplay`

### Retry after death
Stay in `Gameplay`, reset run state quickly

Preferred behavior:
- reset systems in-place when architecturally safe  
or
- reload `Gameplay` only if the measured retry speed still meets the quality bar

Decision rule:
- choose the simplest implementation that preserves immediate-feeling retry

### Return home
`Gameplay` -> `MainMenu`

### Open settings
Preferred as overlay/panel within current scene context.

## Persistent objects

Approved persistent app-level objects may be created during `Bootstrap` and preserved across scenes only if they are truly app-level.

Examples:

- save/profile service
- audio mixer controller
- scene flow coordinator

Do not make gameplay-specific systems persistent across scenes unless there is a clear measured need.

## Scene ownership boundaries

### `Bootstrap` owns
- application startup
- first-time system initialization
- early validation

### `MainMenu` owns
- title/logo presentation
- menu navigation
- home-screen demo
- future non-run front-door entry points

### `Gameplay` owns
- live run state
- score HUD
- pause state
- death/results state
- retry loop

## UI hierarchy model per scene

### `MainMenu`
Contains:

- Title / branding
- animated demo region
- Play button
- Best score summary
- Daily entry (when enabled)
- Skins/Themes entry (when enabled)
- Settings entry

### `Gameplay`
Contains:

- world/camera
- HUD canvas
- pause overlay
- result panel
- optional settings overlay
- debug overlay in non-release builds only

## Result flow policy

Results are intentionally lightweight.

After death:

1. show immediate fail feedback
2. freeze or settle relevant motion cleanly
3. show score / best / short emotional line
4. place large Retry button at center priority
5. keep Home and Share smaller than Retry

Do not create a dedicated “results scene” for first release.

## Pause policy

Pause exists as an overlay, not a new scene.

Pause should:

- stop gameplay progression safely
- not break input mappings
- preserve the current visible run state
- allow resume, restart, settings, home

## Daily challenge policy

If daily challenge is added later, it still routes through the same gameplay scene.  
It changes seed/config, not scene architecture.

## Scene creation rules

Do not create new production scenes unless one of these is true:

- scene ownership becomes genuinely unclear
- memory/performance measurements justify a split
- onboarding / menu complexity becomes impossible to manage cleanly in current scenes
- the change is recorded in `12-adrs.md`

## Loading rules

- keep loading screens minimal or absent if transitions are fast enough
- avoid multi-scene complexity until needed
- additive scene setups are allowed only with explicit architecture documentation

## Non-negotiables

- Production scene count remains intentionally small.
- Retry does not route back through the main menu.
- Result UI remains part of gameplay flow, not a separate content silo.
- Persistent objects are limited to true app-level services.
- Any new scene requires documentation and decision logging.
