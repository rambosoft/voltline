# Project Structure
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: repository layout, folder ownership, file naming, namespace rules, assembly boundaries, asset placement rules
Depends on: 03-tech-stack.md, 04-architecture.md
Do not duplicate with: ad-hoc folder conventions in implementation notes

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file defines how the Unity repository is laid out.  
It prevents “where should this live?” drift and keeps the project rename-safe.

## Root content rule

The stable gameplay root folder is:

`Assets/_Game`

Do not rename this folder because the public game title may still change.

## High-level folder tree

```text
Assets/
  _Game/
    Art/
      Fonts/
      Materials/
      Shaders/
      Sprites/
        Characters/
        Hazards/
        UI/
        VFX/
      Themes/
    Audio/
      Music/
      SFX/
        Gameplay/
        UI/
      Mixers/
    Config/
      Audio/
      Balance/
      Gameplay/
      Themes/
      UI/
    Prefabs/
      Characters/
      Gameplay/
        Hazards/
        Track/
      UI/
        HUD/
        Menus/
        Overlays/
      VFX/
      Shared/
    Scenes/
      Bootstrap.unity
      MainMenu.unity
      Gameplay.unity
    Scripts/
      Runtime/
        Core/
        Gameplay/
        Input/
        UI/
        Audio/
        VFX/
        Data/
        Save/
        Utilities/
      Editor/
      Tests/
        EditMode/
        PlayMode/
    Settings/
      Input/
      Rendering/
      Quality/
    UI/
      Atlases/
      Icons/
      Layouts/
    Gizmos/
Packages/
ProjectSettings/
```

## Folder ownership rules

### `Assets/_Game/Art`
Owns raw and authored art assets.

Rules:

- keep source art grouped by role
- theme-specific variations go under `Themes/`
- gameplay readability beats folder cleverness

### `Assets/_Game/Audio`
Owns music, SFX, and mixer assets.

Rules:

- separate Gameplay SFX from UI SFX
- mixer assets live under `Mixers/`
- avoid duplicate exported clips with vague names like `sound_final_03`

### `Assets/_Game/Config`
Owns ScriptableObject configuration assets.

This is the project’s balancing and static-content heart.

Examples:

- `GameBalanceConfig`
- `DifficultyCurveConfig`
- `ObstacleCatalog`
- `AudioCueCatalog`
- `VfxCatalog`
- `ThemeCatalog`

### `Assets/_Game/Prefabs`
Owns reusable prefab assets.

Rules:

- group by domain, not by “Misc”
- world hazards go in `Gameplay/Hazards`
- UI prefabs go in `UI`
- effects go in `VFX`
- use prefab variants intentionally, not casually

### `Assets/_Game/Scenes`
Owns all production scenes included in builds.

Initial production scene set:

- `Bootstrap.unity`
- `MainMenu.unity`
- `Gameplay.unity`

### `Assets/_Game/Scripts`
Owns code.

Structure:

- `Runtime/` for game/runtime code
- `Editor/` for editor-only tooling
- `Tests/EditMode/` for pure logic/editor-time tests
- `Tests/PlayMode/` for scene/runtime interaction tests

### `Assets/_Game/Settings`
Owns authored project-local setup assets such as:

- input action assets
- URP renderer assets
- quality presets
- platform-specific data assets if needed

## Namespace rules

Primary namespace root:

`Voltline`

Suggested namespace layout:

- `Voltline.Core`
- `Voltline.Gameplay`
- `Voltline.Input`
- `Voltline.UI`
- `Voltline.Audio`
- `Voltline.Vfx`
- `Voltline.Data`
- `Voltline.Save`
- `Voltline.Editor`
- `Voltline.Tests.EditMode`
- `Voltline.Tests.PlayMode`

Rules:

- namespaces mirror folder/domain responsibility
- do not invent alternate top-level namespace roots
- keep namespace depth practical

## Assembly definition plan

Required initial assemblies:

- `Voltline.Runtime`
- `Voltline.Editor`
- `Voltline.Tests.EditMode`
- `Voltline.Tests.PlayMode`

### Dependency graph

- `Voltline.Runtime` -> no dependency on test assemblies
- `Voltline.Editor` -> may depend on `Voltline.Runtime`
- `Voltline.Tests.EditMode` -> may depend on `Voltline.Runtime`
- `Voltline.Tests.PlayMode` -> may depend on `Voltline.Runtime`

Do not create circular assembly references.

## Naming conventions

### Classes

Use clear role-based suffixes where helpful:

- `Controller`
- `Manager`
- `Service`
- `Config`
- `Catalog`
- `View`
- `Presenter`
- `State`
- `Factory`
- `Spawner`

Examples:

- `PlayerController`
- `TrackManager`
- `AudioService`
- `GameBalanceConfig`
- `DeathPanelView`

### Interfaces

Prefix with `I`, only when the interface has a real purpose.

Examples:

- `ISaveService`
- `IAudioService`

Do not create interfaces just for fashion.

### Assets

Use stable descriptive names.

Examples:

- `CFG_GameBalance_Default`
- `CFG_DifficultyCurve_Main`
- `THM_NeonNight`
- `AUD_Mixer_Main`
- `PFB_Player_GlowBot`
- `PFB_VFX_FlipSpark`

### Scenes

Use exact scene names from `06-scene-flow.md`.  
Do not create alternate duplicates like `Gameplay_New`, `Gameplay2`, `MenuFinal`.

## File naming rules

- use PascalCase for C# file names and class names
- use clear asset prefixes where it helps team scan speed
- no vague names like `Manager2`, `TempUI`, `NewBehaviourScript`
- delete dead temp files quickly

Suggested prefixes:

- `CFG_` = config
- `CAT_` = catalog
- `PFB_` = prefab
- `MAT_` = material
- `SPR_` = sprite
- `VFX_` = effect prefab/material
- `AUD_` = audio asset/mixer
- `SCN_` = optional scene prefix for exported references only

## MonoBehaviour responsibility rule

Default rule: one MonoBehaviour = one clear runtime responsibility.

Good examples:

- one component reads input
- one component handles score text binding
- one component owns obstacle rotation animation
- one component handles safe area padding

Bad examples:

- one component does input + scoring + save writes + VFX + pause

## Inspector dependency rule

Do not hide important runtime dependencies in random scene references when a typed config asset or explicit installer is cleaner.

Prefer:

- config asset references
- explicit serialized references with obvious names
- narrow setup components

Avoid:

- mystery scene objects dragged into many fields
- duplicated constants set per prefab with no owning config

## Prefab conventions

### Gameplay prefabs

Should be built to survive across themes and difficulty changes.

Rules:

- logic-light if possible
- config-driven setup
- no unique scene-only assumptions
- clean root object names
- child hierarchy readable at a glance

### UI prefabs

Must reflect screen ownership.

Examples:

- `PFB_HUD_Root`
- `PFB_ResultPanel`
- `PFB_SettingsOverlay`

### VFX prefabs

Keep one prefab per semantic effect.

Examples:

- `PFB_VFX_FlipSpark`
- `PFB_VFX_NearMiss`
- `PFB_VFX_DeathBurst`
- `PFB_VFX_MilestoneBurst`

## Asset placement rules

- Do not place production assets outside `Assets/_Game` unless engine/package workflow requires it.
- Third-party assets must be isolated clearly if ever introduced.
- Generated files should live in known generated/output folders, not mixed into authored content.
- Editor-only assets should not pollute runtime folders.

## Script folder breakdown

Suggested runtime script breakdown:

```text
Assets/_Game/Scripts/Runtime/
  Core/
    Bootstrapper.cs
    GameManager.cs
    SceneFlowService.cs
  Gameplay/
    PlayerController.cs
    TrackManager.cs
    HazardManager.cs
    DifficultyDirector.cs
    ScoreSystem.cs
  Input/
    InputReader.cs
    InputActionRouter.cs
  UI/
    HudView.cs
    MainMenuView.cs
    ResultPanelView.cs
    SettingsOverlayView.cs
  Audio/
    AudioService.cs
  VFX/
    VfxService.cs
  Data/
    ConfigLoaders/
    Catalogs/
  Save/
    SaveService.cs
  Utilities/
```

## Non-negotiables

- `Assets/_Game` stays the stable project root.
- Scene names remain exact and documented.
- Config assets live in `Config/`, not scattered across scene folders.
- Runtime code uses explicit assemblies.
- New top-level folders require this document to be updated.
- Naming must optimize clarity, not personal preference.
