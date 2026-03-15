# Repo Alignment Audit
Status: Active
Owner: Team / AI
Last updated: 2026-03-15
Source of truth for: current repo-to-doc alignment status after baseline cleanup and any remaining structural validation caveats
Depends on: `00-index.md`, `03-tech-stack.md`, `05-project-structure.md`, `06-scene-flow.md`, `11-testing-quality-bar.md`, `14-agent-rules.md`, `16-production-phases.md`
Do not duplicate with: tech stack decisions, project structure ownership, scene flow rules, roadmap phase definitions, or long-term backlog ownership

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file records the live repo-to-doc alignment state after Phase 1 baseline cleanup.

This file owns:

- the current structural alignment snapshot
- the record of which Phase 0 blockers were resolved in Phase 1
- any remaining validation caveats before later phases build on the new baseline
- the immediate entry notes for the next implementation phase

This file does not own:

- the approved tech stack
- the approved folder structure
- the approved scene model
- gameplay, UI, audio, VFX, or persistence rules
- long-term feature prioritization

If this file conflicts with a higher-authority document, the higher-authority document wins and this file must be updated.

## Historical context

Phase 0 documented a near-template repository with blocking drift in scenes, folders, packages, input setup, project settings, assemblies, and template leftovers.

Phase 1 has now addressed that structural drift in-repo.

This file is no longer a list of unresolved blockers. It is the current structural snapshot that later phases should trust unless new drift is introduced.

## Current aligned baseline

The repo now matches the documented Unity baseline in the following ways:

- `Assets/_Game` exists and follows the documented top-level folder ownership model.
- Production scenes now exist at `Assets/_Game/Scenes/Bootstrap.unity`, `Assets/_Game/Scenes/MainMenu.unity`, and `Assets/_Game/Scenes/Gameplay.unity`.
- `ProjectSettings/EditorBuildSettings.asset` now uses the documented build order: `Bootstrap`, `MainMenu`, `Gameplay`.
- The project-owned input asset now lives at `Assets/_Game/Settings/Input/VoltlineInputActions.inputactions`.
- The input asset now exposes the documented minimum actions: `Tap`, `Pause`, `DebugRestart`, `NavigateUI`, `SubmitUI`, `CancelUI`.
- Rendering baseline assets now live under `Assets/_Game/Settings/Rendering/` instead of template-style root locations.
- Required assemblies now exist: `Voltline.Runtime`, `Voltline.Editor`, `Voltline.Tests.EditMode`, `Voltline.Tests.PlayMode`.
- The repo now contains baseline Edit Mode and Play Mode tests for scene/build-order alignment, input baseline, and bootstrap routing.
- Package drift has been removed from `Packages/manifest.json`, leaving the approved first-project baseline plus Unity built-in modules.
- Player/project settings now use project-owned naming and portrait-first defaults instead of the original template settings.
- Template-owned monetization and `Resources` leftovers have been removed.

## Resolved Phase 0 blockers

## Blocker 1: Scene model and build order drift
Resolved.

- `SampleScene` was replaced by the production scene set.
- Build settings now match `Bootstrap`, `MainMenu`, `Gameplay` exactly.
- A minimal `Bootstrapper` now routes app start into `MainMenu`.

## Blocker 2: Missing `Assets/_Game` baseline
Resolved.

- The stable project root is now `Assets/_Game`.
- The documented folder tree exists so later phases can place assets without translation from template state.

## Blocker 3: Package baseline drift
Resolved.

- Unapproved packages were removed from `Packages/manifest.json`.
- `Packages/packages-lock.json` was removed intentionally so Unity can regenerate a lock file from the cleaned manifest on the next successful import.

## Blocker 4: Template input action asset
Resolved.

- The project no longer uses the template multi-action asset.
- Input now reflects the documented Voltline action grammar and location.

## Blocker 5: Template-derived player/project settings
Resolved at the file level.

- `companyName`, `productName`, and application identifiers are project-owned.
- Orientation is now portrait-only rather than auto-rotating into landscape.
- Template scene/package fields have been cleared.
- Editor analytics submission has been disabled in project settings.

## Blocker 6: Missing assembly baseline
Resolved.

- Runtime, editor, Edit Mode test, and Play Mode test assemblies now exist in the documented locations.

## Blocker 7: Template leftovers and `Resources`
Resolved.

- `Assets/Resources/BillingMode.json` is gone.
- `Assets/MobileDependencyResolver/...` is gone.
- Template scene template assets were removed with the old root layout.

## Blocker 8: Missing project-owned test baseline
Resolved.

- The project now has owned test folders and baseline tests under the documented `_Game` tree.

## Validation caveat status

The earlier automated Unity batchmode limitation is no longer a blocker for Phase 1 confidence.

Current status:

- Unity now opens the project successfully in the editor
- the production scenes import cleanly enough for manual verification
- `Bootstrap` has been manually checked in-editor and routes into `MainMenu`

This clears the only remaining Phase 1 validation caveat that existed after baseline cleanup.

## Validation performed

The following checks were completed after the Phase 1 changes:

- verified the root `Assets` tree now contains only `Assets/_Game`
- verified `_Game` folder ownership matches the documented baseline shape
- verified scene file locations and build scene order
- verified the input action asset location, GUID continuity, and required action names
- verified the cleaned package manifest and removal of the old package lock
- verified project setting changes for naming, orientation, identifiers, template fields, and analytics submission
- verified removal of template leftovers including `Resources`, `BillingMode.json`, `MobileDependencyResolver`, and old scene-template assets
- verified the new assemblies and baseline tests exist in the documented script locations
- attempted Unity batchmode import/open validation twice, including a pass with `-accept-apiupdate`
- manually verified in the Unity editor that the project opens and `Bootstrap` routes into `MainMenu`

## Current assessment

Phase 1 is now complete.

Phase 1 exit confidence is now satisfied across both required dimensions:

- Structure and ownership: satisfied
- Editor/open/runtime verification: satisfied through manual Unity verification

Use this file as the confirmed baseline snapshot for later phases unless new structural drift is introduced.

## Entry notes for Phase 2 and Phase 3

Later phases can now proceed against this baseline without recreating template cleanup.

Next practical steps:

1. build the Phase 2 config/data backbone on top of the validated baseline
2. use config assets and validators instead of scene-owned constants
3. then begin the Phase 3 vertical-slice runtime systems against those owned catalogs and save models

## Maintenance rule

Update this audit when one of these becomes true:

- structural drift reappears in scenes, folders, packages, or baseline settings
- the validation caveat is cleared by a successful Unity import/open verification
- a new blocking baseline issue appears before later phases can proceed safely

Do not use this file to restate long-term roadmap priorities.  
Use it to keep the live repo alignment snapshot accurate.