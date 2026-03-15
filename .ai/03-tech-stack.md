# Tech Stack
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: approved engine, rendering path, input, UI, audio, data, packages, assembly strategy, performance targets, banned tech choices
Depends on: 00-index.md, 12-adrs.md
Do not duplicate with: 04-architecture.md, implementation notes

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Stack decision

The game is built in **Unity 6.x LTS** with **C#**.

This stack is chosen because the project needs all of the following in a single production path:

- 2D content and sprite workflows
- mobile export to iOS and Android
- touch input
- fast iteration in editor
- particle-based polish
- audio mixing
- clean portrait UI
- a maintainable small-team architecture

## Approved core stack

- **Engine:** Unity 6.x LTS
- **Language:** C#
- **Rendering:** Universal Render Pipeline (URP)
- **2D rendering mode:** URP with 2D Renderer
- **Input:** Input System package
- **UI:** Canvas-based UI with TextMeshPro
- **Audio:** AudioSource + AudioMixer
- **Particles / juice:** built-in Particle System
- **Config/content:** ScriptableObject assets
- **Code organization:** assembly definitions (`.asmdef`)
- **Testing:** Unity Test Framework (Edit Mode + Play Mode)
- **Persistence:** local save data in app storage with a versioned schema
- **Target orientation:** portrait

## Why this stack

This project is a small, feedback-heavy arcade game, not a general web app.  
The approved stack keeps rendering, touch input, build targets, UI, effects, and audio inside one engine and one editor workflow.

That is more important than theoretical stack novelty.

## Unity version policy

Use the current Unity 6 LTS line approved by the team.  
At the time this documentation set was prepared, the target line is **Unity 6.3 LTS**.

Policy:

- do not jump Unity versions casually
- upgrade only after validating project stability, package compatibility, and build pipeline health
- log major engine upgrades in `12-adrs.md`
- if an upgrade changes project structure, update `05-project-structure.md` and `11-testing-quality-bar.md`

## Package policy

All package dependencies must be declared through Unity’s package management system and resolved from `Packages/manifest.json`.

### Required packages

1. **URP**
   - reason: scalable cross-platform rendering and 2D lighting support

2. **Input System**
   - reason: touch-friendly, modern input workflows and action-based bindings

3. **TextMeshPro** (project-standard UI text solution)
   - reason: crisp mobile UI typography

4. **Unity Test Framework**
   - reason: Edit Mode and Play Mode tests, including mobile-target validation workflows

### Optional packages, allowed only when justified

- **Addressables**
  - allowed later if asset scale, patching, or memory management complexity justifies it
  - not required for the initial release

- **2D Animation / Sprite packages**
  - allowed if the chosen character or pipeline needs them
  - not mandatory for the first playable

### Package rules

- do not add a package without updating this file
- do not keep experimental packages in production manifests without explicit approval
- prefer fewer stable packages over many convenience packages

## Rendering policy

Use **URP** with a **2D Renderer**.

Reasons:

- strong fit for 2D lighting workflows
- mobile-friendly path
- supports the premium neon look without requiring a heavy VFX stack
- consistent rendering setup across gameplay and UI overlays

### Rendering goals

- dark soft backgrounds
- glowing line
- readable hazard silhouettes
- restrained light usage
- small, controlled bloom/glow
- effects that support readability rather than drown it

### Rendering constraints

- no HDRP
- no custom render-pipeline experimentation for first release
- no effect that materially reduces gameplay clarity on target devices

## Input policy

Use **Input System**, not the legacy Input Manager.

### Required action map

The project should define an action asset that includes at minimum:

- `Tap`
- `Pause`
- `DebugRestart` (development only)
- `NavigateUI`
- `SubmitUI`
- `CancelUI`

### Input rules

- gameplay is touch-first
- tap can be bound to mouse click in editor for testing
- gameplay logic should consume abstract actions, not raw device checks
- avoid sprinkling input polling across many classes

## UI policy

Use **Canvas-based UI**.

### Why

The game’s UI is simple, screen-based, and mobile-first:

- main menu
- HUD
- death panel
- settings overlay
- future daily challenge and cosmetic screens

Canvas UI is the approved approach for these screens.

### UI rules

- gameplay HUD remains lightweight
- TextMeshPro is the default text solution
- use anchors, safe area handling, and responsive scaling
- gameplay UI may read state, but should not own gameplay rules

### Disallowed for gameplay UI

- UI Toolkit as the primary gameplay UI solution for first release
- custom HTML/web-view style UI shell inside the app
- overly animated UI that competes with gameplay

## Audio policy

Use Unity’s built-in audio workflow:

- `AudioSource` for playback
- `AudioMixer` for routing and balance
- grouped buses for Music / Gameplay SFX / UI SFX

### Reasons

The project has modest but important audio needs:

- responsive flip sound
- clear death sound
- small set of polished one-shots
- one looping music track
- volume control and quick tuning without per-clip chaos

## VFX policy

Use the built-in **Particle System** plus lightweight sprite/material effects.

Approved effect types:

- flip sparks
- milestone bursts
- near-miss flashes
- death fragments
- line pulses
- small electrical arcs

Disallowed for first release:

- overbuilt VFX Graph dependency
- effects that require complex GPU-only authoring just to achieve small arcade feedback
- visually noisy persistent emitters that obscure danger

## Data/config policy

Use **ScriptableObject** assets for static and tunable project data such as:

- game balance values
- difficulty curves
- theme color sets
- obstacle definitions
- audio cue catalogs
- VFX catalogs

Do not use ScriptableObjects as the runtime save-game source.

## Save/persistence policy

Persist player-facing data locally with a **versioned save model**.

Initial persisted data should include:

- best score
- audio settings
- unlocked themes
- selected theme
- tutorial/first-launch flags
- daily challenge history if implemented

## Assembly policy

Use `.asmdef` files to organize project code into explicit assemblies.

Initial assembly plan:

- `Voltline.Runtime`
- `Voltline.Editor`
- `Voltline.Tests.EditMode`
- `Voltline.Tests.PlayMode`

Rules:

- runtime code should not depend on test assemblies
- editor code should be isolated from runtime
- explicit references only
- avoid a giant catch-all assembly for everything

## Scene policy

Approved scene model for initial production:

- `Bootstrap`
- `MainMenu`
- `Gameplay`

Result UI lives inside `Gameplay` as a panel, not a separate scene.

## Forbidden or disallowed technologies for core gameplay

- React / Next.js for the gameplay runtime
- legacy Input Manager as the project standard
- UI Toolkit as the primary gameplay UI system
- DOTS/ECS-first architecture for the initial release
- heavy backend dependency for the first playable
- VFX Graph as a baseline requirement
- ad SDK integration before the core feel is proven
- `Resources` folder as a general content-loading strategy

## `Resources` folder policy

Default rule: **do not use `Resources`**.

Exception rule:

- only allowed with a documented reason
- must be called out in `12-adrs.md`
- must not become the silent default asset access pattern

## Addressables policy

Addressables are **not required** for the first release.

Allow later only if at least one becomes true:

- large theme/content library
- memory pressure from dynamic loading
- remote content delivery is explicitly required
- build organization materially benefits from address-based loading

## Performance targets

Target feel and performance for the initial release:

- **60 FPS** on target mid-range mobile devices
- retry-to-control responsiveness that feels immediate
- cold start to menu should remain short and uncluttered
- gameplay frame pacing should remain stable during particles and UI updates
- memory usage should remain conservative for a small arcade title

Suggested working budgets:

- keep active particle counts low
- keep overdraw modest
- keep full-screen post effects restrained
- keep per-frame allocations near zero in gameplay

## Build policy

- mobile portrait only for release builds
- debug/dev tools compiled or enabled safely for non-release builds
- platform-specific settings must be documented when added
- build scene list must match `06-scene-flow.md`

## Non-negotiables

- Unity remains the gameplay engine.
- URP + 2D Renderer remains the rendering baseline.
- Input System remains the input standard.
- Canvas + TextMeshPro remains the gameplay UI standard.
- ScriptableObjects own static config, not scene inspector sprawl.
- Assembly definitions are required for maintainable growth.
- Do not add new packages or tech paths silently.
