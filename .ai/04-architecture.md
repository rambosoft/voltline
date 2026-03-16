# Architecture
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: runtime system boundaries, ownership, dependencies, events, allowed patterns, banned patterns
Depends on: 03-tech-stack.md, 07-gameplay-systems.md, 10-data-content-model.md
Do not duplicate with: feature-specific implementation notes

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Architectural style

The project uses a **small, modular, event-driven gameplay architecture** built around explicit ownership.

The architecture should feel like this:

- a small number of clearly named systems
- gameplay rules owned by runtime systems, not UI
- configuration pulled from config assets, not scattered inspector state
- dependencies flowing in one direction
- small MonoBehaviours with one clear purpose each
- shared services used sparingly and explicitly

This is not a framework showcase.  
It is a focused arcade architecture optimized for clarity and iteration speed.

## Top-level runtime model

### Application layer

Responsible for app bootstrap, persistent services, save/load, settings, and scene entry.

### Menu layer

Responsible for title screen, preview/demo, navigation to gameplay, settings, and future daily challenge/cosmetic entry points.

### Gameplay layer

Responsible for the run itself: line motion, player side-flip, obstacle generation, scoring, failure, feedback events, and restart.

### Presentation layer

Responsible for UI views, particles, screen shake, camera polish, and audio playback in response to game events.

## Core runtime systems

### `Bootstrapper`
Owns app startup and service registration.

Responsibilities:

- initialize save/profile data
- load project-level config
- initialize services
- load initial scene
- validate critical references in development builds

### `GameManager`
Owns the run state machine.

Responsibilities:

- enter run states
- coordinate start / active / death / restart
- publish run lifecycle events
- stop gameplay on death
- coordinate result presentation timing

### `TrackManager`
Owns line/path progression and challenge sequencing.

Responsibilities:

- manage the current path
- provide line position and local orientation data
- maintain safe spawn windows
- advance the world at current speed
- own only line/path presentation, not broader background spectacle

### `BackgroundPresentationController`
Owns gameplay background layers and speed-feel ambience.

Responsibilities:

- build the approved background layer stack from config
- enforce lane quiet-zone and draw-call budget rules
- animate subtle background motion without touching gameplay truth
- apply theme-safe background transitions

### `ThemePresentationController`
Owns gameplay-scene theme application and milestone-gated theme sequencing.

Responsibilities:

- apply the selected theme at run start
- apply only approved runtime theme transitions
- keep theme changes out of gameplay rule ownership
- coordinate theme updates across background, line, visuals, UI, and VFX

### `PlayerController`
Owns player-side logic.

Responsibilities:

- consume `Tap`
- flip from one side of the line to the other
- query path normals/tangents
- evaluate local collision / fail conditions as needed
- publish player feedback events (flip, near miss, death context)

### `PlayerVisualView`
Owns player visual-state playback.

Responsibilities:

- render the currently active player look from `PlayerVisualConfig`
- normalize authored sprite size back to the approved readable gameplay footprint
- apply lightweight state-specific presentation effects such as idle rotation or transient pulse
- keep visual-state timing out of gameplay collision ownership
- fall back safely when optional state-specific art is still missing

### `HazardManager`
Owns hazard lifecycle.

Responsibilities:

- spawn hazard clusters from config
- activate telegraphs where needed
- despawn off-screen content
- expose hazard information for scoring / near-miss evaluation

### `ScoreSystem`
Owns visible score and milestone logic.

Responsibilities:

- increment score when challenge beats are cleared
- track run score, best score updates, and milestone thresholds
- publish milestone events
- keep score rules independent of UI text rendering

### `DifficultyDirector`
Owns difficulty ramp selection.

Responsibilities:

- read `DifficultyCurveConfig`
- choose obstacle density/spacing windows
- determine speed ramp stage
- unlock hazard patterns progressively
- protect fairness constraints during escalation

### `AudioService`
Owns playback requests at the service layer.

Responsibilities:

- play named cues routed through mixer groups
- handle global volume/state
- avoid duplicate noisy overlap
- keep gameplay code free from clip reference sprawl

### `VfxService`
Owns reusable spawned feedback effects.

Responsibilities:

- spawn predefined effect prefabs
- centralize effect naming/lookup
- enforce lightweight effect policy
- simplify future pooling if needed

### `UIStateCoordinator`
Owns high-level UI state transitions.

Responsibilities:

- show/hide menu states
- bind gameplay HUD
- present death panel
- coordinate retry/home/settings entry points
- preserve UI hierarchy consistency

### `SaveService`
Owns persistent player data.

Responsibilities:

- load/write player profile
- persist best score and settings
- version save schema
- expose safe mutation methods to higher-level systems

## Dependency rules

Preferred direction:

- Config/data assets -> read by runtime systems
- Runtime systems -> publish state/events
- UI/views -> subscribe to state/events
- Audio/VFX -> react to state/events or service requests
- Save system -> called by orchestration systems, not by low-level effect code

### Example dependency chain

`InputReader` -> `PlayerController` -> `GameManager` / `ScoreSystem` events -> `HUDView`, `AudioService`, `VfxService`

### Forbidden dependency patterns

- UI view directly changing game rules
- hazard code reading UI state to decide behavior
- VFX owning gameplay timing
- save code inside individual obstacle scripts
- circular references between managers
- giant singleton classes with mixed responsibilities

## Ownership rules

### Run state ownership

`GameManager` is the source of truth for whether the run is:

- preparing
- active
- dying
- dead / results
- restarting

### Score ownership

`ScoreSystem` is the source of truth for visible score and milestone events.  
HUD only displays it.

### Difficulty ownership

`DifficultyDirector` is the source of truth for difficulty stage and ramp parameters.  
Obstacle prefabs should not each invent their own difficulty logic.

### Theme ownership

Theme selection is data-driven via config and save state.  
Individual views should read theme state, not define theme state.

### Audio/VFX ownership

Audio/VFX trigger mapping is defined centrally.  
Individual gameplay scripts can request a named event, but should not hold unique hard-wired clip bundles unless justified.

## Recommended runtime pattern set

### Allowed patterns

- composition over inheritance
- data-driven config through ScriptableObjects
- state machine for run lifecycle
- prefab-based content
- explicit service interfaces where helpful
- event-driven updates for UI/audio/VFX
- object pooling where profiling shows benefit

### Avoid unless necessary

- generic service locators with no boundaries
- “manager” classes that also serve as data stores, UI binders, and effect spawners
- abstract factory layers that exist only for theory
- deep inheritance trees for hazards

## Singleton policy

Allowed only for a very small number of app-level services, and only if their lifetime and ownership are obvious.

Possible persistent services:

- `SaveService`
- `AudioService`
- `VfxService` (or effect registry coordinator)
- `SceneFlowService` if one is introduced

Rules:

- prefer scene references or dependency wiring over hidden static access
- static global mutation points are discouraged
- if a service becomes too broad, split it

## Scene ownership policy

- `Bootstrap` owns startup only
- `MainMenu` owns title/menu presentation and navigation to play
- `Gameplay` owns the active run and result panel

Settings can exist as a reusable overlay prefab/panel shared by menu and gameplay.

## Prefab ownership policy

Prefabs are grouped by role:

- player
- hazards
- HUD fragments
- menus
- VFX
- shared world props

Rules:

- prefabs should not silently depend on unrelated scene-only objects
- authoring-time references should be obvious
- gameplay prefabs should be reusable across seeds and themes
- config-driven setup is preferred over many one-off prefab variants

## Event model

The project uses small explicit gameplay events, for example:

- `RunStarted`
- `RunEnded`
- `PlayerFlipped`
- `ScoreChanged`
- `MilestoneReached`
- `NearMissTriggered`
- `ThemeChanged`
- `RetryRequested`

These do not need to be implemented through one specific library.  
The important rule is that event flow remains readable and local.

## Proposed core state machine

### Application states
- Boot
- Menu
- Gameplay
- Quit/Suspend

### Gameplay run states
- Ready
- Starting
- Active
- Dying
- Results
- Restarting

### UI states
- MainMenu
- SettingsOverlay
- HUD
- PauseOverlay
- ResultPanel

## Example system boundary map

- `GameManager`: “What state is the run in?”
- `TrackManager`: “Where is the line and what comes next?”
- `PlayerController`: “Which side is the player on and how does it flip?”
- `HazardManager`: “Which hazards are live and what do they mean?”
- `ScoreSystem`: “What is the score?”
- `DifficultyDirector`: “How intense should the current run be?”
- `UI`: “How is current state shown?”
- `Audio/VFX`: “How does current state feel?”
- `SaveService`: “What persists between sessions?”

## Banned patterns

- giant god classes
- circular references
- magic strings scattered through code for scene names or cue names
- UI directly mutating gameplay state
- per-prefab hardcoded balance values that bypass config
- hidden dependencies only wired through inspector with no documentation
- branching architecture for multiple future game modes before the first mode is excellent

## Non-negotiables

- The run state has one clear owner.
- UI reads gameplay state; UI does not own gameplay rules.
- Difficulty is directed centrally, not invented per obstacle script.
- Config lives in assets, not random scene objects.
- System responsibilities stay small enough to explain in one sentence.
- New runtime systems require explicit ownership justification.




