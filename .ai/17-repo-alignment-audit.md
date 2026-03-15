# Repo Alignment Audit
Status: Active
Owner: Team / AI
Last updated: 2026-03-15
Source of truth for: current repo-to-doc alignment status and ordered baseline remediation before Phase 1
Depends on: `00-index.md`, `03-tech-stack.md`, `05-project-structure.md`, `06-scene-flow.md`, `11-testing-quality-bar.md`, `14-agent-rules.md`, `16-production-phases.md`
Do not duplicate with: tech stack decisions, project structure ownership, scene flow rules, roadmap phase definitions, or long-term backlog ownership

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file records the exact gap between the current repository and the documented Voltline baseline at the completion of Phase 0.

This file owns:

- the current repo-to-doc alignment snapshot
- the concrete gap list blocking Phase 1
- the ordered baseline remediation checklist for the next implementation phase
- the evidence used to justify the Phase 0 completion claim

This file does not own:

- the approved tech stack
- the approved folder structure
- the approved scene model
- gameplay, UI, audio, VFX, or persistence rules
- long-term feature prioritization

If this file conflicts with a higher-authority document, the higher-authority document wins and this file must be updated.

## Phase 0 contract

### Objective

Document the exact gap between the current repository and the documented Voltline baseline.

### Scope

- audit scenes
- audit build order
- audit packages
- audit folders
- audit project settings
- audit input setup location and shape
- audit assembly absence
- audit template leftovers and naming drift

### Anti-scope

- gameplay implementation
- scene creation or cleanup
- package cleanup
- settings cleanup
- content production
- runtime architecture expansion
- speculative Phase 1+ work

### Dependencies

- `00-index.md`
- `03-tech-stack.md`
- `05-project-structure.md`
- `06-scene-flow.md`
- `14-agent-rules.md`
- `16-production-phases.md`

### Definition of done

- a complete prioritized alignment list exists from current repo state to the documented baseline

### Exit criteria

- the team can begin repo cleanup without guessing what must change first

## Audit method

The audit was completed by inspecting the current live repository state against the approved docs, with emphasis on:

- `Packages/manifest.json`
- `ProjectSettings/EditorBuildSettings.asset`
- `ProjectSettings/ProjectSettings.asset`
- `ProjectSettings/EditorSettings.asset`
- `ProjectSettings/VersionControlSettings.asset`
- `Assets/InputSystem_Actions.inputactions`
- `Assets/Scenes/SampleScene.unity`
- `Assets/Resources/BillingMode.json`
- `Assets/MobileDependencyResolver/...`
- current `Assets/`, `Packages/`, and `ProjectSettings/` file layout

## Current compliant baseline items

The current repo is not aligned overall, but these items already match or support the documented baseline:

- Unity version is already on the approved `Unity 6.3 LTS` line.
- URP assets and a 2D renderer asset are present.
- `Input System` is installed and configured as the active input handler.
- `Unity Test Framework` is already present in the manifest.
- `Visible Meta Files` is already enabled.
- `Force Text` serialization is already enabled.

These reduce Phase 1 risk, but they do not remove the need for baseline alignment work.

## Alignment gaps

## Gap 1: Scene model and build order are still template-derived

- Current evidence: `ProjectSettings/EditorBuildSettings.asset` includes only `Assets/Scenes/SampleScene.unity`.
- Current evidence: the documented production scenes `Bootstrap`, `MainMenu`, and `Gameplay` do not exist.
- Why this blocks Phase 1: scene ownership, retry flow, and build verification all depend on the documented three-scene model.
- Required Phase 1 action: create the approved production scenes, place them in the documented location, and align build order to `Bootstrap`, `MainMenu`, `Gameplay`.
- Priority: blocking.

## Gap 2: Project structure does not match `Assets/_Game`

- Current evidence: the repo still uses template root assets under `Assets/Scenes`, `Assets/Settings`, and root-level project assets.
- Current evidence: `Assets/_Game` and its documented subfolders do not exist.
- Current evidence: there are no runtime, editor, or test folders under the documented project root.
- Why this blocks Phase 1: every later system depends on stable asset placement, naming, and assembly boundaries.
- Required Phase 1 action: establish `Assets/_Game` and move or recreate production-owned content inside the documented tree.
- Priority: blocking.

## Gap 3: Package baseline includes undocumented and pre-core drift

- Current evidence: `Packages/manifest.json` includes packages outside the approved baseline, including Ads, Analytics, Purchasing, Multiplayer Center, AI Navigation, Visual Scripting, and XR legacy input helpers.
- Current evidence: template/editor convenience packages and extra 2D packages have not been reviewed against first-playable scope.
- Why this blocks Phase 1: the docs require a deliberate package baseline and explicitly reject ad SDK integration before core feel is proven.
- Required Phase 1 action: classify packages into keep, remove, or justify; remove unsupported pre-core packages and record any justified exceptions.
- Priority: blocking.

## Gap 4: Input action asset is still the template action map

- Current evidence: the project input asset still contains template actions such as `Move`, `Look`, `Attack`, `Interact`, `Jump`, and `Sprint`.
- Current evidence: the required minimum actions `Tap`, `Pause`, `DebugRestart`, `NavigateUI`, `SubmitUI`, and `CancelUI` are not yet the project-owned baseline.
- Current evidence: the asset lives at `Assets/InputSystem_Actions.inputactions`, not the documented project settings location.
- Why this blocks Phase 1: the one-tap game verb and future input routing depend on a clean project-owned action asset.
- Required Phase 1 action: replace the template action map with the documented Voltline action baseline and relocate it into the project-owned settings area.
- Priority: blocking.

## Gap 5: Player and project settings still expose template/mobile-baseline drift

- Current evidence: `companyName` remains `DefaultCompany`.
- Current evidence: the application identifier remains template-derived.
- Current evidence: `templatePackageId` and `templateDefaultScene` still point to the template origin and `SampleScene`.
- Current evidence: orientation settings still allow landscape autorotation, which conflicts with the portrait-mobile release target.
- Why this blocks Phase 1: release posture, build verification, and future mobile QA depend on project-owned settings rather than template defaults.
- Required Phase 1 action: align naming, identifiers, orientation, and template-derived player settings to the documented portrait-mobile baseline.
- Priority: blocking.
## Gap 6: Assembly baseline is missing

- Current evidence: the required `.asmdef` files `Voltline.Runtime`, `Voltline.Editor`, `Voltline.Tests.EditMode`, and `Voltline.Tests.PlayMode` do not exist.
- Current evidence: there is no documented runtime/editor/test code layout yet in the repo.
- Why this blocks Phase 1: the approved maintainability model depends on explicit assemblies rather than a catch-all default assembly.
- Required Phase 1 action: establish the documented assemblies and place code into the documented runtime, editor, and test boundaries.
- Priority: blocking.

## Gap 7: Template leftovers include `Resources` and monetization-related residue

- Current evidence: `Assets/Resources/BillingMode.json` exists.
- Current evidence: `Assets/MobileDependencyResolver/...` exists.
- Current evidence: these assets are consistent with Purchasing/Google dependency resolver tooling rather than the documented first-release scope.
- Why this blocks Phase 1: the docs forbid `Resources` as a general strategy and reject monetization or ad-driven infrastructure before the core loop is proven.
- Required Phase 1 action: remove or explicitly justify these leftovers; if any exception remains, document it through the approved authority path.
- Priority: blocking.

## Gap 8: Testing baseline is structurally absent even though the package exists

- Current evidence: the Unity Test Framework package is installed, but there are no project-owned test folders or test assemblies in the documented location.
- Current evidence: no project-specific Edit Mode or Play Mode tests exist yet.
- Why this matters now: Phase 0 does not require gameplay tests, but Phase 1 should establish the structural test baseline so later phases do not grow without coverage paths.
- Required Phase 1 action: create the documented test folder and assembly structure even if only a minimal baseline test set lands at first.
- Priority: high, but only after the structural baseline is in place.

## Ordered Phase 1 entry checklist

The next implementation phase should address alignment in this order:

1. Lock the approved package baseline and remove or justify unsupported package drift.
2. Remove monetization and `Resources` leftovers that do not belong to the approved first-playable baseline.
3. Create the `Assets/_Game` root and the documented folder tree.
4. Create `Bootstrap`, `MainMenu`, and `Gameplay` in the documented scene location and align build order.
5. Replace the template input action asset with the documented project-owned action baseline and move it into the documented settings location.
6. Align player/project settings to the portrait-mobile baseline and remove template-derived identifiers and references.
7. Add the required runtime, editor, and test assemblies.
8. Re-run the repo alignment audit and confirm no template-owned production blockers remain.

## Risks if Phase 1 starts without using this audit

- scene work may be created in the wrong location and need migration
- input may be built on the wrong action grammar
- unsupported packages may silently shape architecture decisions
- template leftovers may leak monetization or `Resources` patterns into core production
- later contributors may build against the wrong repo baseline and create avoidable rework

## Phase 0 completion assessment

Phase 0 is complete if judged by the Phase 0 contract in `16-production-phases.md`.

Reasons:

- the current repo-to-doc gap is now explicit
- the alignment blockers are prioritized
- the Phase 1 entry checklist is ordered and concrete
- no unresolved contradiction was found in the governing docs

## Validation performed

- verified current Unity editor version from `ProjectSettings/ProjectVersion.txt`
- verified package baseline from `Packages/manifest.json`
- verified build scene state from `ProjectSettings/EditorBuildSettings.asset`
- verified player/project settings drift from `ProjectSettings/ProjectSettings.asset`
- verified text/meta settings from `ProjectSettings/EditorSettings.asset` and `ProjectSettings/VersionControlSettings.asset`
- verified input asset drift from `Assets/InputSystem_Actions.inputactions`
- verified monetization and `Resources` leftovers from `Assets/Resources/BillingMode.json` and `Assets/MobileDependencyResolver/...`
- verified top-level asset/file layout from the live repo inventory

Not performed in Phase 0:

- gameplay compile or runtime verification, because this phase introduced no runtime code or scene changes
- Unity-editor play validation, because this phase is an audit and planning phase rather than an implementation phase
- automated test changes, because no gameplay, persistence, or config logic changed in this phase

## Maintenance rule

Update this audit when one of these becomes true:

- the repo baseline changes enough that the current gap list is no longer accurate
- Phase 1 resolves or reorders the blocking alignment work
- a previously documented blocker is removed or replaced by a different blocker

Do not use this file to restate long-term roadmap priorities.  
Use it to keep the repo alignment snapshot accurate until the baseline cleanup is complete.
