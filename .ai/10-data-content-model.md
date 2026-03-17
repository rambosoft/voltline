# Data and Content Model
Status: Active
Owner: Team
Last updated: 2026-03-17
Source of truth for: what is code vs config vs content, ScriptableObject catalogs, IDs, persistence model, balancing workflow
Depends on: 03-tech-stack.md, 04-architecture.md, 07-gameplay-systems.md
Do not duplicate with: hardcoded scene values, throwaway prototype constants

> Working title: **Voltline**  
> Current public-facing release title: **Voltline**  
> Primary production theme direction: **Live Wire City**

## Purpose

This file defines how the project represents tunable data, authored content, and persistent player data.

It prevents the project from turning into:

- hardcoded values everywhere
- duplicated inspector constants
- unversioned save drift
- asset lookup chaos

## Data categories

### 1. Code
Owns behavior, orchestration, and rules execution.

Examples:
- player flip logic
- score event handling
- scene flow
- save read/write orchestration

### 2. Config
Owns tunable values and structured rules that designers/agents should adjust without rewriting behavior code.

Examples:
- base speed
- flip duration
- near-miss distance
- milestone thresholds
- theme colors
- hazard spacing ranges

### 3. Content
Owns authored assets the player experiences.

Examples:
- sprites
- sounds
- prefabs
- materials
- music
- theme assets

### 4. Save data
Owns player progress and preferences.

Examples:
- best score
- unlocked themes
- selected theme
- volume settings
- first-launch flags

## Source-of-truth rule

Behavior lives in code.  
Tuning and static definitions live in config assets.  
Player history lives in save data.

Do not blur these categories casually.

## Config asset model

Use ScriptableObjects for project-owned static config and catalogs.

### Required config asset families

#### `GameBalanceConfig`
Owns global gameplay tuning values.

Suggested fields:
- base speed
- speed ramp parameters
- flip duration
- side offset
- near-miss threshold
- safe-start window
- milestone thresholds

#### `GameplayPresentationConfig`
Owns frozen gameplay-critical presentation assumptions for the current baseline.

Suggested fields:
- track camera size
- line width
- line curve amplitudes/wavelengths
- player readable visual scale
- player line clearance
- player collision half extents

#### `BackgroundPresentationConfig`
Owns gameplay background layer rules and the allowed speed-feel budget.

Suggested fields:
- max runtime layer count
- draw-call budget
- lane quiet-zone width
- maximum allowed layer alpha
- layer definitions
- per-layer quiet-zone enforcement flag for side-anchored structure layers vs centered atmosphere layers
- per-layer velocity-response multiplier so skyline, utility, haze, and streak passes keep distinct parallax feel without hardcoded runtime constants

#### `PlayerVisualConfig`
Owns the player's readable visual footprint and semantic state-driven art/effect behavior.

Suggested fields:
- visual prefab or fallback sprite
- visible bounds scale
- local offset
- base rotation offset
- sorting order
- menu preview size/offset
- state definition list for `Idle`, `Flip`, `NearMiss`, `Score`, `Milestone`, and `Death`
- optional sprite/material override per state
- per-state duration, scale multiplier, rotation speed, and pulse settings

#### `DifficultyCurveConfig`
Owns pacing and escalation rules.

Suggested fields:
- score bands
- speed multipliers
- spawn spacing ranges
- hazard unlock thresholds
- telegraph minimums
- mix complexity rules

#### `ObstacleConfig`
Owns one obstacle family or authored obstacle definition.

Suggested fields:
- obstacle ID
- family type
- allowed score range
- spawn rules
- telegraph requirement
- collider/layout references
- weight or frequency hints

#### `ObstacleCatalog`
Owns the set of available obstacle configs.

#### `HazardPresentationCatalog`
Owns approved hazard-family presentation bounds used to preserve fairness before a future art refresh.

Suggested fields:
- family ID / enum binding
- readable visual bounds
- telegraph bounds
- collision bounds
- minimum readable gap padding

#### `ThemeConfig`
Owns one theme's visual role definitions and presentation-package links.

Suggested fields:
- theme ID
- display name
- background colors
- line colors
- player accent colors
- danger colors
- milestone colors
- optional player visual override
- optional obstacle visual override
- optional background presentation override
- required `WorldProgressionConfig`
- required `ThemeVfxProfile`
- required `ThemeAudioProfile`
- runtime transition allowance flag
- preferred transition duration

#### `ThemeCatalog`
Owns all theme entries plus the shared branding, copy, and UI token configs used by current first-release surfaces.

#### `WorldProgressionConfig`
Owns one theme''s score-band district states and milestone reaction library.

Suggested fields:
- district state definitions with score ranges and stage IDs
- milestone reaction definitions with score thresholds and copy IDs
- district-restoration counting rules for result copy
- stage colors or presentation modifiers that feed line, hazard, HUD, and background responses

#### `BrandingPresentationConfig`
Owns placeholder-safe public branding presentation for the current release posture.

Suggested fields:
- public title
- optional subtitle
- optional logo sprite
- optional-entry visibility flags for splash, share, daily, and Grid Status surfaces
- optional splash duration for the menu logo moment
- first-release theme-selection visibility gate

#### `ProductionCopyConfig`
Owns public-facing production copy for menu, pause, result, settings, and milestone text.

Suggested fields:
- menu hint text
- play/settings/retry/home/close labels
- pause/settings labels
- result title and message libraries
- milestone message library
- Grid Status labels, score-band formatting, and online/locked copy
- share-surface labels and clipboard-friendly summary/flavor formats
- first-run tutorial title/body/action labels
- theme-status labels and locked-theme requirement format

#### `ThemeVfxProfile`
Owns theme-aware VFX cue overrides and VFX safety budgets.

Suggested fields:
- semantic cue override entries
- max active transient effects
- max burst count per effect
- minimum replay cooldown
- death camera shake allow/disallow flag

#### `ThemeAudioProfile`
Owns theme-aware audio cue overrides and audio safety budgets.

Suggested fields:
- semantic cue override entries
- max concurrent gameplay voices
- max concurrent UI voices
- minimum UI-click interval
- music volume multiplier

#### `PresentationRolloutPlanConfig`
Owns the stop/go rollout gate for staged presentation refresh work.

Suggested fields:
- required global approval checks
- ordered rollout slice list
- per-slice collision/readability/performance requirements
- per-slice stop/go blocking rules

#### `AudioCueCatalog`
Owns semantic cue mapping.

Suggested fields:
- cue ID
- mixer group route
- clip list or variation list
- volume/pitch range
- concurrency rules

#### `VfxCatalog`
Owns semantic effect mapping.

Suggested fields:
- VFX ID
- spawn mode (`Prefab` or lightweight procedural)
- prefab reference when prefab-backed
- scale rules
- lifetime category
- optional pooling hint

#### `UIThemeConfig`
Owns UI-specific role tokens for panels, buttons, text, shadows, outlines, and optional TMP font assignments used by menu, HUD, pause, result, and settings surfaces.

## ID rules

Use stable IDs for config/cue/theme references.

Examples:

- `theme.live-wire-city`
- `district.failing-grid`
- `obstacle.grounded.blocker.basic`
- `obstacle.active-electric.short-gate`
- `audio.flip.default`
- `vfx.death.default`

Rules:

- IDs are lowercase dot-separated strings
- IDs are stable once shipped if they touch save data or telemetry
- display names can change without changing IDs

## Save-data model

Use a versioned player profile save.

Suggested schema:

```json
{
  "version": 1,
  "bestScore": 0,
  "selectedThemeId": "theme.live-wire-city",
  "unlockedThemeIds": ["theme.live-wire-city"],
  "musicVolume": 1.0,
  "sfxVolume": 1.0,
  "vibrationEnabled": true,
  "hasSeenFirstLaunchHint": false,
  "dailyChallenge": {
    "lastPlayedDate": null,
    "lastSubmittedSeedId": null,
    "bestDailyScore": 0
  }
}
```

This is a model reference, not a mandate to store JSON in this exact formatting.

## Save strategy

Preferred first-release strategy:

- serialize a small player profile to app-local persistent storage
- include a schema version
- migrate carefully if fields change later

Reasons:

- lightweight
- offline-friendly
- adequate for a small arcade title
- safer than spreading state across many independent keys

## Persistence rules

- save on meaningful changes, not every frame
- update best score safely
- avoid save writes from low-level effect scripts
- preserve schema migration ability
- keep save model small and human-comprehensible

## Balancing workflow

Balance changes should be made by editing config assets, not rewriting code where possible.

Recommended workflow:

1. tune config asset values
2. test in editor/dev build
3. validate against quality bar
4. document notable tuning changes if they affect player expectations

## Asset registry policy

All semantic lookup systems should resolve through known catalogs/config.

Examples:
- audio cue requests -> `AudioCueCatalog`
- VFX requests -> `VfxCatalog`
- obstacle spawn choices -> `ObstacleCatalog`
- theme selection -> `ThemeCatalog`

Do not create many parallel lookup systems.

## What should not be hardcoded

Avoid hardcoding these in random runtime scripts:

- milestone thresholds
- color roles
- default volumes
- difficulty band breakpoints
- obstacle family weights
- theme unlock order
- near-miss thresholds
- score pop behavior constants

## Runtime mutation rules

ScriptableObject assets are authoring-time config and content definitions.  
Do not use them as the mutable player-state database at runtime.

Allowed:
- reading config
- editor authoring
- dev tooling validation

Disallowed:
- writing player progression back into shipped config assets
- treating SO assets as save files

## Theme data model

Current first-release shipping posture:

- `theme.live-wire-city` is the surfaced base theme
- `ThemeCatalog` surfaces one active production theme while preserving later extensibility for more themes
- the former `Themes` menu slot can be promoted to a `Grid` entry while only one production theme remains surfaced
- `WorldProgressionConfig` owns the five district stages `Failing Grid`, `Local Power Restored`, `Grid Stabilization`, `Surge City`, and `Overclock City`
- milestone reactions for `10`, `20`, `30`, `40`, and `50` are config-owned through `WorldProgressionConfig` and `ProductionCopyConfig`
- `ThemeSequenceConfig` remains in the repo only as a dormant compatibility asset and is not the shipping runtime progression model
- staged presentation rollout is still governed by `PresentationRolloutPlanConfig`

## Difficulty data model

Difficulty should be data-driven through score bands or progression curves.

Suggested score bands:
- band 0: scores 0-9
- band 1: scores 10-19
- band 2: scores 20-29
- band 3: scores 30-39
- band 4: 40+

Actual values are tunable, but the model should exist in config.

## Daily challenge data model

If daily challenge is added:

- daily seed ID
- date key
- best daily score
- attempt metadata only if justified

Do not build a large live-service schema for first release.

## Validation rules

Add lightweight validation for config integrity where useful.

Examples:
- milestone list ascending
- theme IDs unique
- obstacle IDs unique
- no negative timing values
- no missing prefab references in prefab-backed VFX entries
- no missing clip mappings in Audio cue catalog where authored clips are expected
- every `ThemeConfig` references a `ThemeVfxProfile` and `ThemeAudioProfile`
- `PresentationRolloutPlanConfig` keeps the approved ordered rollout slice list and stop/go flags

## Non-negotiables

- Static config lives in ScriptableObjects.
- Player progress lives in versioned save data.
- Stable IDs are required for referenced content.
- Balance values should not be scattered across scene objects.
- Runtime systems should read catalogs, not invent parallel registries.
- Do not mutate content assets as a save mechanism.






