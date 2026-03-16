# Presentation Refresh Readiness Roadmap
Status: Active
Owner: Team / AI
Last updated: 2026-03-16
Source of truth for: phased readiness and gating before presentation refresh implementation
Depends on: `@.ai/00-index.md`, `@.ai/03-tech-stack.md`, `@.ai/04-architecture.md`, `@.ai/07-gameplay-systems.md`, `@.ai/08-ui-ux-style-guide.md`, `@.ai/09-audio-vfx-guide.md`, `@.ai/10-data-content-model.md`, `@.ai/11-testing-quality-bar.md`, `@.ai/14-agent-rules.md`, `@.ai/15-roadmap-backlog.md`, `@.ai/16-production-phases.md`, `@.ai/18-visual-refresh-and-theme-system-guide.md`
Do not duplicate with: production phases, gameplay systems, architecture, UI style guide, audio/VFX guide, or visual refresh system guide

## 1. Purpose
This file is the strict readiness and gating roadmap that must be satisfied before presentation refresh implementation begins. It owns rollout order, blocked work, readiness gates, and approval criteria for updating player art, obstacle art, backgrounds, themes, VFX, and audio presentation. It does not own gameplay rules, core architecture, UI behavior standards, audio/VFX semantics, or the future implementation details already covered by other source-of-truth docs.

## 2. Non-negotiables
- Gameplay remains one-tap only.
- Fairness, collision honesty, and fast retry outrank decoration.
- Readability beats spectacle in every scene and on every theme.
- Presentation changes must not hide safe-vs-danger meaning.
- Mobile performance, small build size, and restart speed remain protected.
- Theme changes must not create surprise failures or state confusion.
- VFX and audio exist to reinforce clarity and satisfaction, not to add clutter.
- Refresh work must not introduce feature creep, parallel systems, or scene-local hacks.

## 3. Why a readiness roadmap is needed
The current repo is playable and broadly validated, and the frozen presentation baseline now lives in explicit config assets. Player and hazard visuals are decoupled from gameplay collision and spacing ownership, the repo has a dedicated background presentation owner, and the theme model is presentation-ready with milestone-gated runtime sequencing. VFX and audio refresh pipelines are now also defined structurally through theme-owned profile assets and service-level semantic routing. That still does not make the whole refresh safe by default. Any rollout still needs explicit approval, staged delivery, and repeated validation so fairness, readability, restart speed, and performance do not regress.

## 4. Relationship to other docs
- `16-production-phases.md` owns the broader production sequence; this file narrows only the pre-refresh readiness path.
- `18-visual-refresh-and-theme-system-guide.md` explains how a future refresh should be built once the repo is ready; this file explains what must be true before that work may begin.
- `07-gameplay-systems.md` still owns gameplay truth.
- `08-ui-ux-style-guide.md` still owns menu/HUD behavior and presentation hierarchy.
- `09-audio-vfx-guide.md` still owns semantic feedback policy.
- `10-data-content-model.md` still owns config, catalog, and persistence structures.
- `11-testing-quality-bar.md` still owns shipping-quality validation rules.

## 5. Readiness philosophy
Voltline should not refresh presentation by replacing visuals first and repairing fairness later. Readiness comes from separating what the player sees from what the gameplay uses, then making those relationships explicit, testable, and configurable. Any refresh work that cannot explain how it preserves silhouette clarity, spacing safety, collision honesty, theme meaning, and mobile performance is blocked.

## 6. Readiness overview
Current repo state relevant to readiness:
- `PlayerController.cs` owns hit logic and line anchoring while `PlayerVisualView.cs` renders the player from `PlayerVisualConfig`.
- `HazardManager.cs` owns hazard collision/spacing while `HazardVisualView.cs` renders family visuals from `ObstacleVisualCatalog`.
- `GameplayPresentationConfig` owns the frozen player/track presentation baseline for the live repo.
- `HazardPresentationCatalog` owns the frozen hazard-family readability, collision, and spacing baseline for the live repo.
- `BackgroundPresentationController.cs` owns a small budgeted gameplay background layer stack through `BackgroundPresentationConfig`, while `TrackManager.cs` stays focused on line/path ownership.
- `ThemeConfig.cs` is now presentation-ready, and `ThemePresentationController.cs` plus `ThemeSequenceConfig.cs` own milestone-gated runtime theme application rules.
- `VfxService.cs` and `AudioService.cs` are centralized, theme-aware, and now route variation through `ThemeVfxProfile` and `ThemeAudioProfile`.
- `PresentationRefreshApprovalAudit.cs` and `PresentationRolloutPlanConfig` now provide the hard stop/go approval gate for staged refresh work.
- `Assets/_Game/Art` and most authored audio content folders are structurally present but still sparse.

Readiness gates:
- Gate 0: baseline presentation dependencies are audited and frozen.
- Gate 1: gameplay-critical visual assumptions are documented explicitly.
- Gate 2: hardcoded presentation values are moved into config-ready ownership.
- Gate 3: player visuals are decoupled from collision.
- Gate 4: obstacle visuals are decoupled from collision and spacing.
- Gate 5: background architecture and performance budget exist.
- Gate 6: the theme system is presentation-ready.
- Gate 7: VFX and audio refresh pipelines are defined.
- Gate 8: the refresh approval gate passes.

## 7. Detailed phased roadmap

### Phase 1. Audit current presentation dependencies
- Objective: produce a repo-grounded inventory of all gameplay-critical presentation dependencies before refresh work begins.
- Why it exists: the project currently mixes presentation and gameplay assumptions in runtime code, so safe refresh requires an explicit dependency map first.
- In scope: code-path inventory for player, hazards, track/background, theme usage, VFX, audio, preview UI, save/settings dependencies, and authored-asset readiness.
- Out of scope: replacing assets, retuning gameplay, or introducing new visual systems.
- Dependencies: `00`, `04`, `07`, `08`, `09`, `10`, `11`, `18`.
- Affected systems: gameplay runtime, menu preview, settings, save, audio, VFX, scenes, config assets.
- Likely files/areas touched: documentation only, with reference to `PlayerController.cs`, `HazardManager.cs`, `TrackManager.cs`, `ThemeConfig.cs`, `ThemeCatalog.cs`, `AudioService.cs`, `VfxService.cs`, `Gameplay.unity`, `MainMenu.unity`, `Assets/_Game/Art`, `Assets/_Game/Audio`.
- Performance concerns: none directly, but the audit must identify expensive presentation paths and any runtime-generated allocation risks.
- Fairness/readability concerns: the audit must identify every place where visible shape, spacing, or theme color affects gameplay understanding.
- Validation requirements: the audit matches actual repo structure and names; no guessed systems; known blockers are listed explicitly.
- Definition of done: all current presentation owners, dependencies, and blockers are documented in one consistent view.
- Exit criteria: contributors can answer "what breaks if we swap this asset?" without guessing.
- What it unlocks: assumption freeze and config-readiness work.
- What stays blocked: all asset swaps and theme refresh implementation.

### Phase 2. Freeze and document gameplay-critical visual assumptions
- Objective: capture the current baseline truth for the visual assumptions that fairness depends on.
- Why it exists: refresh work must preserve or deliberately retune these assumptions rather than accidentally changing them.
- In scope: player side offset, line clearance, player readable size, player collision extents, hazard readable core, hazard collision extents, minimum spacing, near-miss thresholds, danger color semantics, milestone/theme readability constraints.
- Out of scope: changing values or replacing visuals.
- Dependencies: Phase 1.
- Affected systems: `GameplayPresentationConfig`, `HazardPresentationCatalog`, `HazardManager.cs`, `PlayerController.cs`, `TrackManager.cs`, tests, the presentation audit, and manual QA notes.
- Likely files/areas touched: documentation, readiness audit output, test expectations, and config asset baselines.
- Performance concerns: none directly.
- Fairness/readability concerns: this phase defines the fairness baseline future changes must prove against.
- Validation requirements: frozen assumptions are traceable to live repo behavior and current tests/manual validation.
- Definition of done: a stable baseline exists for what the current player sees and what the current gameplay means.
- Exit criteria: every future refresh task can say whether it preserves or intentionally changes each baseline assumption.
- What it unlocks: config migration and decoupling work.
- What stays blocked: direct player/hazard art swaps.

### Phase 3. Move hardcoded presentation values into config
- Objective: move gameplay-critical presentation tuning out of static helpers and into explicit config ownership.
- Why it exists: the repo is not refresh-ready while fairness-critical sizing, offsets, and spacing live in hardcoded helpers.
- In scope: player presentation tuning, collision tuning, hazard family visual sizing, hazard collision sizing, spacing multipliers, line presentation constants, theme-ready visual defaults, background budget constants where appropriate.
- Out of scope: new art, dynamic transitions, or final theme presentation packages.
- Dependencies: Phases 1 and 2.
- Affected systems: runtime data/config model, gameplay installer, tests, validation tooling.
- Likely files/areas touched: `GameplayPresentationConfig`, `HazardPresentationCatalog`, `GameplaySceneInstaller.cs`, config assets under `Assets/_Game/Config/Gameplay/`, validation tooling, and tests under `Assets/_Game/Scripts/Tests`.
- Performance concerns: config reads must not add per-frame allocation or scene-start complexity.
- Fairness/readability concerns: migration must preserve live values exactly before any refresh retuning happens.
- Validation requirements: existing gameplay looks and behaves the same after the migration; tests cover the moved values.
- Definition of done: no fairness-critical presentation constant is trapped in static helpers without config ownership.
- Exit criteria: a refresh task can change presentation data without editing gameplay logic.
- What it unlocks: safe player and hazard visual decoupling.
- What stays blocked: broad asset swaps, dynamic theme transitions, and background spectacle.

### Phase 4. Decouple player visuals from collision
- Objective: separate the player's visual representation from the player's gameplay hit shape and line relationship.
- Why it exists: player art cannot change safely while the visible body, anchor, and collision assumptions are entangled.
- In scope: player visual config, pivot/anchor rules, visual scale rules, optional visual root separation, collision root or collision data path, theme override hooks.
- Out of scope: final player art production, new movement rules, or gameplay balance changes.
- Dependencies: Phase 3.
- Affected systems: `PlayerController.cs`, gameplay installer, theme application, menu preview, tests.
- Likely files/areas touched: `PlayerController.cs`, preview UI components, player-related config assets, optional player prefab path under `Assets/_Game/Prefabs/Characters/`.
- Performance concerns: visual replacement must not add heavy per-run instantiation overhead.
- Fairness/readability concerns: player anchor must stay stable on the line; visible art must not imply a larger hit area than gameplay uses.
- Validation requirements: front hit, side graze, flip timing, line overlap avoidance, theme preview consistency.
- Definition of done: player art can change independently of collision and side-offset truth.
- Exit criteria: swapping player visuals no longer silently changes fairness.
- What it unlocks: player asset refresh work.
- What stays blocked: obstacle art swaps and any theme-wide visual refresh beyond player-safe prototypes.

### Phase 5. Decouple obstacle visuals from collision and spacing
- Objective: separate obstacle look from obstacle hit shape, telegraph core, and spacing rules.
- Why it exists: hazards must be able to evolve visually without altering collision honesty or spacing fairness.
- In scope: obstacle visual profiles, separate collision extents, separate readable-danger extents, spacing calculation inputs, family-specific telegraph ownership, themed appearance hooks.
- Out of scope: adding new hazard families or changing gameplay rules for family behavior.
- Dependencies: Phases 2, 3, and 4.
- Affected systems: `HazardManager.cs`, obstacle configs/catalogs, difficulty tuning, tests.
- Likely files/areas touched: `HazardManager.cs`, `HazardPresentationCatalog.cs`, obstacle config assets, future obstacle presentation config assets, tests.
- Performance concerns: avoid runtime visual assembly that adds excessive allocations or per-hazard object counts.
- Fairness/readability concerns: visible dangerous core must match or slightly exceed collision honesty expectations; spacing must remain readable at speed; no family may become ambiguous under theme variation.
- Validation requirements: per-family collision checks, spacing checks, near-miss checks, screenshot readability review, repeated run manual QA.
- Definition of done: obstacle art can change without changing hit logic or spacing safety by accident.
- Exit criteria: hazard presentation is controlled by data and explicit rules rather than implicit helper coupling.
- What it unlocks: safe obstacle asset refresh work.
- What stays blocked: high-variance themed hazard appearances and dynamic theme switching.

### Phase 6. Establish background architecture and performance budget
- Objective: define a dedicated owner and budget for background presentation and speed-feel effects.
- Why it exists: background spectacle added too early would compete with gameplay readability unless it has its own owner and limits.
- In scope: background ownership model, layer plan, allowable effect types, theme hooks, memory budget, particle budget, draw-call expectations, safe-area considerations.
- Out of scope: shipping final background art or motion FX content.
- Dependencies: Phases 1 through 5.
- Affected systems: gameplay scene ownership, theme application, possibly menu preview, performance validation.
- Likely files/areas touched: `BackgroundPresentationController.cs`, `BackgroundPresentationConfig.cs`, gameplay scene wiring, theme config/data, `Assets/_Game/Config/Gameplay/`, and future background assets.
- Performance concerns: background work must preserve frame pacing, restart speed, and small build size; avoid heavy full-screen effects in normal play.
- Fairness/readability concerns: no layer may look like a hazard, obscure the line, or reduce danger contrast.
- Validation requirements: device profiling, screenshot readability, repeated-retry smoke pass, safe-area review.
- Definition of done: there is a documented and testable background architecture with a hard performance budget.
- Exit criteria: background visuals can evolve without improvising architecture during art production.
- What it unlocks: background refresh implementation and controlled motion FX prototypes.
- What stays blocked: dense spectacle and unbudgeted background refresh.

### Phase 7. Expand theme system to a presentation-ready model
- Objective: evolve the current theme model into a presentation package model without overengineering it.
- Why it exists: a broad refresh cannot stay clean if themes only own flat colors.
- In scope: theme-level presentation roles, override references, preview readiness, save compatibility, menu/settings ownership, default fallback rules.
- Out of scope: dynamic in-run transitions and broad per-theme audio score replacement.
- Dependencies: Phases 3 through 6.
- Affected systems: `ThemeConfig.cs`, `ThemeCatalog.cs`, `SaveService.cs`, settings UI, preview UI, gameplay installer, VFX/audio integration points.
- Likely files/areas touched: theme config assets under `Assets/_Game/Config/Themes/`, `ThemeConfig.cs`, `ThemeCatalog.cs`, settings and preview scripts, save schema only if needed.
- Performance concerns: theme packages must remain lightweight and avoid redundant asset duplication.
- Fairness/readability concerns: theme overrides must preserve safe-vs-danger semantics and line readability.
- Validation requirements: theme selection persistence, preview consistency, fallback safety when an override is missing.
- Definition of done: themes can describe more than colors without hardcoded `switch(themeId)` branching.
- Exit criteria: the repo can support authored presentation packages cleanly.
- What it unlocks: safe static theme visual refresh work.
- What stays blocked: dynamic theme switching during runs.

### Phase 8. Establish dynamic theme transition rules
- Objective: define the strict rules and data model for if and when themes may change during a run.
- Why it exists: mid-run theme changes can easily break readability, surprise the player, or clash with fast retry.
- In scope: allowed triggers, forbidden moments, transition duration rules, transition ownership, configurable thresholds, fallback behavior.
- Out of scope: building complex cinematic transitions or theme-specific gameplay changes.
- Dependencies: Phase 7 and stable background/theme architecture.
- Affected systems: gameplay scene presentation ownership, score/milestone hooks, theme application path, VFX/audio hooks.
- Likely files/areas touched: `ThemeSequenceConfig.cs`, `ThemePresentationController.cs`, gameplay installer, milestone hooks, tests.
- Performance concerns: transitions must not allocate heavily or stall retry flow.
- Fairness/readability concerns: no transition may occur during a teaching moment, a death event, or a decision-critical hazard window; danger semantics remain stable through the transition.
- Validation requirements: threshold tests, timing tests, manual readability checks, retry-state reset checks.
- Definition of done: theme switching rules are explicit, limited, and testable.
- Exit criteria: theme switching can be implemented without guesswork and without unsafe runtime visual swaps.
- What it unlocks: controlled dynamic theme prototypes and milestone-based theme refresh behavior.
- What stays blocked: broad theme spectacle and aggressive audio/visual transition layering.

### Phase 9. Establish VFX refresh pipeline
- Objective: define how themed or refreshed VFX will be authored, cataloged, overridden, budgeted, and validated.
- Why it exists: the current VFX service is centralized, and the project now needs clear rules before adding richer authored effects.
- In scope: semantic event map confirmation, prefab-vs-procedural policy, theme override policy, effect budget, pooling/reuse expectations, catalog ownership.
- Out of scope: building every final VFX asset.
- Dependencies: Phases 5 through 8 and `09-audio-vfx-guide.md`.
- Affected systems: `VfxService.cs`, `VfxCatalog.cs`, gameplay feedback coordinator, theme data.
- Likely files/areas touched: `VfxService.cs`, `ThemeVfxProfile.cs`, VFX config assets, `Assets/_Game/Prefabs/VFX/`, `Assets/_Game/Art/VFX/`, tests and validation tooling.
- Performance concerns: avoid effect spam, keep object counts and overdraw low, protect restart speed.
- Fairness/readability concerns: effects must never hide the line, player, or active danger core.
- Validation requirements: effect budget checks, event coverage checks, manual clutter review, repeated-run fatigue review.
- Definition of done: there is a stable themed-VFX pipeline that does not rely on ad hoc per-effect logic.
- Exit criteria: VFX refresh implementation can proceed within clear limits.
- What it unlocks: themed/refreshed VFX production.
- What stays blocked: high-density VFX spectacle and unbudgeted signature effects.

### Phase 10. Establish audio refresh pipeline
- Objective: define how refreshed or theme-aware audio content is authored, routed, varied, and validated.
- Why it exists: the current audio stack is semantically sound, and the project now has the profile path needed for broader content refresh without parallel systems.
- In scope: semantic cue ownership, mixer routing expectations, theme-aware variation rules, reuse-vs-replace rules, music variation rules, content budget, fallback behavior.
- Out of scope: composing a full soundtrack or adding a new audio architecture.
- Dependencies: Phase 7 and `09-audio-vfx-guide.md`.
- Affected systems: `AudioService.cs`, `AudioCueCatalog.cs`, `ThemeAudioProfile.cs`, mixer asset/routing, settings persistence, feedback coordinator.
- Likely files/areas touched: `AudioService.cs`, audio catalogs, mixer assets, authored audio folders, tests.
- Performance concerns: memory footprint, decode/load strategy, overlapping cue spam, clean relaunch behavior.
- Fairness/readability concerns: feedback remains sharp, low-latency, and uncluttered; themed audio must not blur cue identity.
- Validation requirements: mixer routing checks, category balance checks, repeated-retry fatigue review, persistence checks, device listening pass.
- Definition of done: the project has a disciplined audio refresh pipeline with clear variation rules.
- Exit criteria: SFX/music refresh can proceed without parallel systems or cue confusion.
- What it unlocks: authored audio replacement and light theme-aware variation.
- What stays blocked: large soundtrack branching and noisy layered mixes.

### Phase 11. Define refresh approval gate
- Objective: define the exact conditions required before any broad presentation refresh rollout is approved.
- Why it exists: readiness work only matters if there is a hard gate that can stop unsafe rollout.
- In scope: required config readiness, required tests, required manual reviews, required device checks, required performance thresholds, rollback expectations.
- Out of scope: performing the rollout itself.
- Dependencies: Phases 1 through 10.
- Affected systems: validation tooling, tests, QA checklist, docs.
- Likely files/areas touched: `.ai/11-testing-quality-bar.md`, release audit rules, approval audit rules, tests, future refresh specs.
- Performance concerns: approval must include device frame pacing, build-size impact, startup and retry timing.
- Fairness/readability concerns: approval must explicitly include collision honesty, spacing readability, theme clarity, and no-clutter confirmation.
- Validation requirements: all required checks are enumerated, reproducible, and owned.
- Definition of done: there is a clear yes/no gate for refresh implementation approval.
- Exit criteria: teams know exactly what evidence is needed before rolling out refreshed presentation.
- What it unlocks: controlled presentation rollout planning.
- What stays blocked: broad implementation without evidence.

### Phase 12. Define controlled rollout and validation
- Objective: require slice-by-slice rollout instead of a single large presentation swap.
- Why it exists: presentation refresh affects fairness, performance, and polish across many systems, so controlled rollout reduces regression risk.
- In scope: rollout order, fallback strategy, comparison checkpoints, rollback expectations, validation cadence across slices.
- Out of scope: post-launch live-ops or feature expansion.
- Dependencies: Phase 11.
- Affected systems: implementation planning, QA, docs, release process.
- Likely files/areas touched: `PresentationRolloutPlanConfig.cs`, refresh approval audit tooling, backlog sequencing, test plans, release checklists.
- Performance concerns: every rollout slice must be measured independently.
- Fairness/readability concerns: every rollout slice must re-prove collision honesty and visual clarity before the next one starts.
- Validation requirements: slice signoff after player refresh, hazard refresh, background refresh, theme expansion, VFX refresh, and audio refresh.
- Definition of done: there is a documented rollout order with stop/go gates between slices.
- Exit criteria: refresh work can proceed incrementally without bundling unrelated risk together.
- What it unlocks: safe staged implementation.
- What stays blocked: all-at-once presentation overhauls.

## 8. Now / Next / Later / Blocked
### Now
- Run refresh work only through the approval gate and the ordered rollout slices in `PresentationRolloutPlanConfig`.
- Start with the player refresh slice, then obstacle refresh, then background/static-theme refresh only after signoff.

### Next
- Replace procedural fallback content with authored assets slice-by-slice through the VFX and audio pipelines.
- Re-run approval evidence after each slice before advancing to the next one.

### Later
- Expand authored theme packages and runtime transition polish only if the earlier slices remain fair, readable, and performant.

### Blocked
- Any all-at-once presentation overhaul.
- Any refresh slice that skips automated checks, manual readability review, device checks, or stop/go approval.
- Any VFX/audio replacement path that bypasses `ThemeVfxProfile`, `ThemeAudioProfile`, the semantic catalogs, or the approval audit.

## 9. Asset-swap gating rules
- Player art swaps are allowed only through `PlayerVisualConfig` and `PlayerVisualView`; they remain blocked from touching collision or line anchoring code directly.
- Obstacle art swaps are allowed only through `ObstacleVisualCatalog` and `HazardVisualView`; they remain blocked from touching collision or spacing code directly.
- Background and theme refresh work are now structurally allowed only through `BackgroundPresentationConfig`, `ThemeConfig`, `ThemeCatalog`, and `ThemeSequenceConfig`.
- VFX and audio replacement is allowed only through the approved `ThemeVfxProfile` / `ThemeAudioProfile` plus semantic catalog/service pipeline, and only after the presentation refresh approval audit is clean for the current slice.
- Temporary scene-local overrides are not allowed as a shortcut around these gates.

## 10. Performance protection rules
- Preserve instant-feeling retry and avoid heavy scene-start construction.
- Prefer small, reusable assets and shared materials over duplicated theme content.
- Keep background motion subtle and low-overdraw.
- Avoid full-screen distortion, dense particle stacks, and unnecessary active objects in gameplay.
- Profile on representative Android hardware before approving any richer presentation layer.
- Treat build-size growth as a tracked cost, not an afterthought.

## 11. Fairness and readability protection rules
- Visible danger core must explain the hit result.
- Collision may be simplified, but it must never be more punishing than the visual implies.
- The line, player, safe side, and danger side must remain readable at a glance.
- Theme changes must preserve semantic color roles or replace them with equally readable alternatives.
- Background and VFX may support motion and feel, but never compete with the decision lane.
- Near-miss clarity, side-graze honesty, and spacing fairness must be revalidated after every presentation-affecting slice.

## 12. Future-proof implementation rules
- Prefer config-driven presentation over hardcoded theme branches.
- Keep visual-only data separate from gameplay-critical collision/spacing data.
- Use semantic services and catalogs already present in the repo; do not build parallel effect or audio lookup systems.
- Preserve scene ownership from `04-architecture.md` and `06-scene-flow.md`.
- Keep the refresh additive to the current architecture where possible, not a rewrite.
- Add new config families only when their ownership is explicit and stable.

## 13. Required data/config readiness
Before refresh implementation, the repo should have explicit ownership for:
- player visual configuration
- player collision tuning
- obstacle visual configuration by family or obstacle type
- obstacle collision tuning and spacing inputs
- background presentation configuration and performance budget rules
- theme presentation package data
- theme transition sequencing data
- theme-aware VFX override mapping
- theme-aware audio variation mapping
- presentation rollout gate data

Recommended asset families for refresh implementation:
- `PlayerVisualConfig`
- `ObstacleVisualCatalog`
- `CollisionTuningConfig`
- `BackgroundPresentationConfig`
- `ThemeSequenceConfig`
- `ThemeVfxProfile`
- `ThemeAudioProfile`
- `PresentationRolloutPlanConfig`

## 14. Required validation before rollout
The refresh approval gate must require:
- passing Edit Mode and Play Mode coverage for any new config/model changes
- manual gameplay checks for collision honesty, side grazes, near misses, and spacing readability
- screenshot readability review for menu and gameplay
- device performance checks on representative Android hardware
- restart-speed and relaunch checks
- save/settings/theme persistence checks where relevant
- build-size review after any significant asset addition
- no normal-flow console noise in menu, gameplay, pause, result, settings, and theme selection flows

## 15. Controlled rollout guidance
Use staged rollout, not one giant presentation change:
1. Config readiness and decoupling only.
2. Player refresh slice.
3. Obstacle refresh slice.
4. Background architecture and motion slice.
5. Static theme package slice.
6. Dynamic theme transition slice.
7. VFX refresh slice.
8. Audio refresh slice.

Each slice must end with a stop/go review. If fairness, readability, performance, restart speed, or maintainability regress, the next slice is blocked until the issue is resolved.

## 16. Documentation update requirements
When implementation later happens, update only the owning docs affected by the actual change:
- `04-architecture.md` if new runtime presentation owners are introduced.
- `05-project-structure.md` if new folders, prefabs, asset families, or naming rules are introduced.
- `06-scene-flow.md` if theme/background initialization ownership changes scene behavior.
- `07-gameplay-systems.md` if collision, spacing, near-miss, or transition timing becomes canonical gameplay truth.
- `08-ui-ux-style-guide.md` if theme preview, settings selection, or UI presentation hierarchy changes materially.
- `09-audio-vfx-guide.md` if semantic feedback rules or effect/audio policy changes.
- `10-data-content-model.md` if new config families, IDs, catalogs, or save fields are introduced.
- `11-testing-quality-bar.md` if new validation gates become mandatory.
- `12-adrs.md` if a cross-cutting architecture or ownership decision changes.
- `15-roadmap-backlog.md` if refresh readiness work moves between Now / Next / Later.
- `18-visual-refresh-and-theme-system-guide.md` if the implementation strategy itself changes materially.

## 17. Recommendations
- All readiness phases are now structurally complete, and future presentation work must move through the approval gate plus the ordered rollout slices in `PresentationRolloutPlanConfig`.
- Start actual refresh implementation with the player slice, then obstacle slice, then background/static-theme slice, re-running approval evidence after each stop/go point.
- Keep collision, spacing, line anchoring, and transition rules off-limits to ad hoc asset swaps; they must continue to flow through config and controllers.
- Keep this roadmap narrower than `16` and more restrictive than `18`.
- Use it as the stop/go gate for any future visual refresh proposal.
