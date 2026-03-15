# Production Phases
Status: Active
Owner: Team / AI
Last updated: 2026-03-15
Source of truth for: phased project delivery from current repo state to publishable release
Depends on: `.ai/00-index.md`, `.ai/01-product-vision.md`, `.ai/02-game-design-pillars.md`, `.ai/03-tech-stack.md`, `.ai/04-architecture.md`, `.ai/05-project-structure.md`, `.ai/06-scene-flow.md`, `.ai/07-gameplay-systems.md`, `.ai/08-ui-ux-style-guide.md`, `.ai/09-audio-vfx-guide.md`, `.ai/10-data-content-model.md`, `.ai/11-testing-quality-bar.md`, `.ai/12-adrs.md`, `.ai/14-agent-rules.md`, `.ai/15-roadmap-backlog.md`
Do not duplicate with: gameplay systems, architecture, tech stack, UI style guide, audio/VFX guide, or backlog idea lists

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file turns the existing source-of-truth docs into an execution order from the current repository state to a publishable mobile release.

This file owns:

- phase order
- delivery gates
- anti-scope timing rules
- definitions of phase completion
- production-readiness standards for this project
- execution guidance for future AI agents and human contributors

This file does not own:

- product identity
- gameplay rules
- architecture boundaries
- tech stack approval
- UI style ownership
- audio/VFX event ownership
- backlog idea ownership

Use this file as the project execution layer.

- Read it after the core source-of-truth docs.
- Use it to decide what should be built now, what must wait, and what quality bar must be met before moving on.
- If this file conflicts with a higher-authority doc, the higher-authority doc wins.
- If real project direction changes, update the owning source-of-truth doc first, then update this file.

Future AI agents should use this file to sequence work, not to reinterpret the game.

- build phases in order
- keep scope narrow until exit gates are met
- treat this file as the delivery map, not as permission to bypass architecture or design rules
- report when the current repo state does not satisfy the current phase gate

## Non-negotiables

- Gameplay remains one-tap only.
- Portrait mobile remains the release target.
- Instant retry remains central to the product.
- Fairness and readability beat content quantity.
- Config-driven tuning is mandatory.
- Build size, startup discipline, memory use, and frame pacing matter.
- No feature sprawl is allowed before the core loop is proven.
- No unapproved packages, tech paths, or parallel systems are allowed.
- Polish must support readability, not clutter.
- UI must stay lightweight during gameplay.
- Difficulty may become severe, but never intentionally unreadable or unfair.
- Save data must remain small, versioned, and reliable.
- Production scenes remain `Bootstrap`, `MainMenu`, and `Gameplay` unless an ADR changes that decision.

## Starting point

The roadmap begins from a repository that is still close to a Unity template rather than the documented production baseline.

Current known repo reality:

- only `SampleScene` exists
- build scene order is not aligned with `Bootstrap`, `MainMenu`, `Gameplay`
- the required `Assets/_Game/...` structure does not yet exist
- the package manifest still includes extra packages that are outside the approved baseline
- player/project settings still look template-like rather than project-owned

Immediate alignment gaps between repo and docs:

- scene ownership does not match `06-scene-flow.md`
- folder ownership does not match `05-project-structure.md`
- package policy is not yet aligned with `03-tech-stack.md`
- project naming and release posture are not yet aligned with the documented mobile-first baseline
- there is no established execution path yet from template state to release

Foundation cleanup comes first because the project should not prove gameplay on top of the wrong structure.

- template leftovers create false progress
- scene, package, and folder drift create expensive rework later
- early gameplay work without config and structure invites hardcoded shortcuts
- later agents should inherit a documented production baseline, not a half-template repository

## Delivery philosophy

- Foundation before features.
- The smallest full playable loop comes before expansion.
- Feel comes before content growth.
- Tests and validation come before broadening scope.
- Optimize late, but budget early.
- Production-ready means stable, attractive, performant, readable, and maintainable.
- Each phase should unlock the next with the minimum clean system set.
- If a phase exit gate is missed, do not hide the miss by starting later-phase work.
- If a requested addition belongs to a later phase, defer it unless an approved roadmap change promotes it.

## Phase map

| Phase | Name | Purpose | Key outcome | Exit gate |
| --- | --- | --- | --- | --- |
| 0 | Foundation audit and repo alignment | Compare current repo state to project truth | Explicit alignment plan with no ambiguity about baseline gaps | Alignment tasks are identified and ordered |
| 1 | Unity baseline and project structure | Convert the template into the documented project baseline | Correct scenes, folders, assemblies, and approved package baseline | Repo structure matches project docs |
| 2 | Data/config backbone | Create the static data model that future systems read | Config assets, catalogs, stable IDs, validators | Core systems can read config instead of scene constants |
| 3 | Core playable vertical slice | Prove the one-tap loop with minimum runtime systems | Flip, line motion, hazards, score, death, retry | The loop is playable and understandable |
| 4 | Core UI flow and retry UX | Establish the player-facing app flow | Menu, HUD, result panel, retry-first flow | Menu-to-retry experience is clear and fast |
| 5 | First feel pass: audio, VFX, feedback | Make the loop emotionally representative | Flip, death, score, near-miss, and baseline music feedback | Core actions feel satisfying without clutter |
| 6 | Save/profile/settings persistence | Make progress and settings reliable | Versioned save profile and essential settings persistence | Best score and settings survive relaunch safely |
| 7 | Difficulty, fairness, and tuning pass | Turn the slice into a fair repeatable game | Readable pacing, tuned spacing, fairness confidence | Core loop repeatedly passes fairness and replay tests |
| 8 | Scope-complete content pass | Reach intended launch feature scope without drift | Launch-critical hazards, theme, UI states, and content completeness | Launch scope is complete and feature-frozen |
| 9 | Production polish and consistency pass | Make the product feel intentionally authored | Cohesive visuals, copy, motion, theme, and feedback consistency | No obvious placeholder or cohesion breaks remain |
| 10 | Testing, QA, and regression hardening | Reduce breakage risk before release | Automated coverage, device QA, blocker triage | No release-blocking bugs remain open |
| 11 | Performance, memory, and build-size optimization | Meet mobile budgets without damaging feel | Stable frame pacing, measured budgets, trimmed footprint | Performance and size are not launch blockers |
| 12 | Release candidate and publishing readiness | Approve the ship candidate | Correct builds, settings, checklist, and publishing readiness | Candidate build is approved for submission |
| 13 | Post-launch stabilization boundaries | Protect the shipped product from drift | Safe post-launch change boundaries | Post-launch work stays narrow and identity-safe |

## Detailed phases

## Phase 0: Foundation audit and repo alignment

- Objective: document the exact gap between the current repository and the documented Voltline baseline.
- Why this phase exists: the repo is still near-template, so the team needs a clean alignment target before structural work begins.
- Scope: audit scenes, build order, packages, folders, project settings, input setup location, assembly absence, naming drift, and template leftovers.
- Out-of-scope / anti-scope: gameplay implementation, content production, tuning, polish, and speculative feature work.
- Dependencies: `.ai/00-index.md`, `.ai/03-tech-stack.md`, `.ai/05-project-structure.md`, `.ai/06-scene-flow.md`, `.ai/14-agent-rules.md`.
- Key systems involved: project baseline, scene model, package baseline, project structure, build configuration.
- Likely files or file areas affected: `Packages/manifest.json`, `ProjectSettings/`, `Assets/Scenes/`, future `Assets/_Game/`, `.ai/16-production-phases.md`, and only other docs if contradictions are discovered.
- Risks: treating template defaults as intentional decisions, missing a package or settings drift item, or allowing feature work to begin before the baseline is agreed.
- Validation steps: compare the live repo against the approved stack, folder tree, scene list, and authority docs; record every gap explicitly; confirm there is no unresolved contradiction in higher-authority docs.
- Definition of done: there is a complete, prioritized alignment list from current repo state to the documented baseline.
- Exit criteria: the team can begin repo cleanup without guessing what must change first.
- Documentation updates required: update `.ai/12-adrs.md` only if a real cross-cutting contradiction or baseline decision change is found; update this file if the starting-state assessment changes materially.
- What future phases unlock: Phase 1 can begin safely because the baseline target is explicit.

## Phase 1: Unity baseline and project structure

- Objective: convert the repository from template state into the documented Voltline Unity baseline.
- Why this phase exists: every later system depends on correct scenes, folders, assemblies, packages, and mobile-first project setup.
- Scope: create `Assets/_Game/...`, create `Bootstrap`, `MainMenu`, and `Gameplay`, set build order correctly, add required `.asmdef` files, align package baseline to approved tech, and replace template-like naming/settings with project-owned values.
- Out-of-scope / anti-scope: gameplay systems, final content, extra packages, live-service integrations, monetization SDKs, alternate scene models, and placeholder-friendly shortcuts that bypass documented structure.
- Dependencies: Phase 0 and the authority docs for stack, structure, and scene flow.
- Key systems involved: Unity project baseline, scene flow, package policy, assembly policy, input asset placement, rendering baseline.
- Likely files or file areas affected: `Assets/_Game/`, `Assets/_Game/Scenes/`, `Assets/_Game/Scripts/`, `Assets/_Game/Settings/`, `Packages/manifest.json`, `ProjectSettings/EditorBuildSettings.asset`, `ProjectSettings/ProjectSettings.asset`.
- Risks: breaking URP or Input System references during cleanup, accidentally keeping unapproved packages, or recreating template structure under a new name.
- Validation steps: open the project without baseline reference errors, verify exact scene names, verify build order, confirm the package list matches approved requirements, verify the project is still portrait-mobile oriented, and confirm sample/template dependencies are removed.
- Definition of done: the repository layout matches the documented Unity baseline closely enough that future work can follow the docs directly instead of translating from template state.
- Exit criteria: the project compiles, opens, and routes through the documented scene baseline with the documented folder and assembly structure in place.
- Documentation updates required: update `.ai/03-tech-stack.md`, `.ai/05-project-structure.md`, `.ai/06-scene-flow.md`, or `.ai/12-adrs.md` only if an approved baseline deviation is introduced; otherwise no source-of-truth change is needed because this phase is an alignment phase.
- What future phases unlock: Phase 2 and Phase 3 can start without structural drift.

## Phase 2: Data/config backbone

- Objective: establish the data-driven backbone so gameplay systems read owned config instead of scattered inspector values.
- Why this phase exists: the project docs require config-driven tuning, stable IDs, and catalog-based lookup before the gameplay surface area grows.
- Scope: create the initial ScriptableObject families, define stable IDs, create the first theme and baseline catalogs, add config validation, and establish a minimal save-profile schema model for future persistence work.
- Out-of-scope / anti-scope: shipping many themes, overbuilt authoring tools, runtime mutation of ScriptableObjects, and content scale that the first release does not need.
- Dependencies: Phase 1 and the approved data/content model.
- Key systems involved: `GameBalanceConfig`, `DifficultyCurveConfig`, `ObstacleConfig`, `ObstacleCatalog`, `ThemeConfig`, `ThemeCatalog`, `AudioCueCatalog`, `VfxCatalog`, and save schema model definitions.
- Likely files or file areas affected: `Assets/_Game/Config/`, `Assets/_Game/Scripts/Runtime/Data/`, `Assets/_Game/Scripts/Runtime/Save/`, validators, and catalog-related tests.
- Risks: unstable IDs, overdesigned data models, or hardcoded thresholds appearing before the config path exists.
- Validation steps: confirm validators catch missing references and invalid values, confirm IDs are unique and stable, confirm milestone lists and difficulty ranges are sane, and confirm runtime systems can read config without scene-value sprawl.
- Definition of done: the minimum required static data model exists and is usable by future runtime systems.
- Exit criteria: core gameplay values no longer need to be invented per scene or prefab.
- Documentation updates required: update `.ai/10-data-content-model.md` if any config families, ID strategy, or save schema assumptions change; update `.ai/11-testing-quality-bar.md` only if validator/test expectations change materially.
- What future phases unlock: Phase 3, Phase 5, Phase 6, and Phase 7 can all build on stable config and catalogs.

## Phase 3: Core playable vertical slice

- Objective: prove the core one-tap loop with the smallest complete set of runtime systems.
- Why this phase exists: the project must validate fun, fairness, and restart speed before it invests in broader content or polish.
- Scope: implement the run state flow, line progression, player side flip, at least two approved hazard families, score progression, death handling, and a fast retry path inside `Gameplay`.
- Out-of-scope / anti-scope: extra modes, deep progression, broad content expansion, theme variety, complex pause/settings flows, and effects-heavy polish that hides whether the game is actually fun.
- Dependencies: Phase 1 and Phase 2.
- Key systems involved: `GameManager`, `TrackManager`, `PlayerController`, `HazardManager`, `ScoreSystem`, `DifficultyDirector`, input routing, and minimal debug hooks for testing.
- Likely files or file areas affected: `Assets/_Game/Scripts/Runtime/Core/`, `Assets/_Game/Scripts/Runtime/Gameplay/`, `Assets/_Game/Prefabs/Gameplay/`, `Assets/_Game/Scenes/Gameplay.unity`, and gameplay config assets.
- Risks: an unclear line/spatial grammar, unreadable hazard setups, retry that is too slow, or a vertical slice that grows into unofficial final scope.
- Validation steps: manual smoke testing for tap-to-flip reliability, death readability, and retry speed; automated tests for state transitions, score behavior, and config-backed rules where practical.
- Definition of done: a new player can start, understand, play, fail, and retry inside the documented loop.
- Exit criteria: the team can repeatedly play the slice and discuss feel/fairness rather than missing baseline systems.
- Documentation updates required: update `.ai/04-architecture.md` if runtime ownership changes, `.ai/07-gameplay-systems.md` if actual behavior or timing rules change, `.ai/10-data-content-model.md` for new IDs or config structures, and `.ai/11-testing-quality-bar.md` for added test scope.
- What future phases unlock: Phase 4, Phase 5, and Phase 7 become meaningful because the core loop exists.
## Phase 4: Core UI flow and retry UX

- Objective: establish the player-facing flow around the core loop with retry as the dominant post-failure action.
- Why this phase exists: Voltline is not just a runtime loop; it is a menu-to-run-to-retry product, and that flow is central to retention.
- Scope: implement `Bootstrap` handoff, `MainMenu`, gameplay HUD, result panel, safe-area handling, and the minimal pause/restart/home flow needed by the documented scene model.
- Out-of-scope / anti-scope: deep mode selection, daily challenge UI, cosmetics browsing, verbose onboarding, or UI chrome that competes with gameplay readability.
- Dependencies: Phase 1, Phase 3, and the existing scene/UI docs.
- Key systems involved: `Bootstrapper`, `UIStateCoordinator`, menu views, HUD views, result panel views, pause overlay, and theme-driven UI presentation.
- Likely files or file areas affected: `Assets/_Game/Scenes/Bootstrap.unity`, `Assets/_Game/Scenes/MainMenu.unity`, `Assets/_Game/Scenes/Gameplay.unity`, `Assets/_Game/Prefabs/UI/`, `Assets/_Game/Scripts/Runtime/UI/`, safe-area helpers, and UI config/theme assets.
- Risks: cluttered screens, a weak Retry hierarchy, safe-area mistakes, or UI behavior that takes ownership away from gameplay systems.
- Validation steps: verify app boot flow, Play flow, Home flow, Pause flow, Retry prominence, screenshot clarity, and phone safe-area behavior on representative aspect ratios.
- Definition of done: the core player journey is understandable without reading a paragraph, and retry is visually and interactionally primary after death.
- Exit criteria: menu-to-gameplay-to-results-to-retry/home flow is stable, fast, and readable.
- Documentation updates required: update `.ai/06-scene-flow.md` if scene transitions or ownership shift, `.ai/08-ui-ux-style-guide.md` if canonical screen behavior changes, and `.ai/11-testing-quality-bar.md` if UI flow testing expectations expand.
- What future phases unlock: Phase 5 and Phase 6 can build on a stable user-facing shell.

## Phase 5: First feel pass: audio, VFX, feedback

- Objective: make the core loop emotionally representative of the final game without sacrificing clarity.
- Why this phase exists: Voltline succeeds on feel as much as rules, so core feedback must become satisfying early enough to guide honest playtesting.
- Scope: implement baseline flip feedback, death feedback, score response, near-miss feedback, main loop music, menu click polish, and restrained camera feedback where approved.
- Out-of-scope / anti-scope: VFX clutter, theme-specific effect libraries, effect systems that require heavy new tech, long musical systems, and noise that hides hazard readability.
- Dependencies: Phase 2, Phase 3, and Phase 4.
- Key systems involved: `AudioService`, `VfxService`, cue/effect catalogs, mixer groups, HUD feedback bindings, and approved camera polish.
- Likely files or file areas affected: `Assets/_Game/Audio/`, `Assets/_Game/Prefabs/VFX/`, `Assets/_Game/Config/Audio/`, `Assets/_Game/Config/Gameplay/`, `Assets/_Game/Scripts/Runtime/Audio/`, `Assets/_Game/Scripts/Runtime/VFX/`.
- Risks: effect spam, repeated-audio fatigue, unclear mixer routing, or feedback that masks fail reasons.
- Validation steps: repeated-run feel testing, overlap/concurrency testing, hazard readability checks under stress, and confirmation that each core event feels distinct and short.
- Definition of done: flip feels addictive, death feels fair and conclusive, score and near-miss feedback support play rather than compete with it, and the mix remains clean across rapid retries.
- Exit criteria: the build is emotionally representative enough for serious playtesting without being misleadingly over-polished in the wrong areas.
- Documentation updates required: update `.ai/09-audio-vfx-guide.md` for canonical event mapping changes, `.ai/10-data-content-model.md` for cue/effect IDs or catalog changes, and `.ai/11-testing-quality-bar.md` if feedback validation scope changes.
- What future phases unlock: Phase 6 persistence work and Phase 7 tuning can now be judged on a more honest player feel baseline.

## Phase 6: Save/profile/settings persistence

- Objective: persist the essential player-facing state safely and simply.
- Why this phase exists: the game becomes a real product only when best score, settings, and basic player preferences survive relaunch without corruption or inconsistency.
- Scope: implement the versioned player profile, persist best score and essential settings, connect a minimal settings overlay to persisted values, and define safe write timing.
- Out-of-scope / anti-scope: cloud saves, accounts, backend identity, bloated settings trees, and save logic embedded inside low-level gameplay or effect scripts.
- Dependencies: Phase 2, Phase 4, and Phase 5.
- Key systems involved: `SaveService`, settings UI, audio settings, theme selection state, and app bootstrap save load flow.
- Likely files or file areas affected: `Assets/_Game/Scripts/Runtime/Save/`, `Assets/_Game/Scripts/Runtime/Core/`, `Assets/_Game/Scripts/Runtime/UI/`, settings overlay prefabs, and save-related tests.
- Risks: schema drift, corrupted save writes, too-frequent persistence, or settings ownership becoming scattered between UI and services.
- Validation steps: cold start tests, relaunch tests, upgrade/default-save tests, best-score persistence tests, settings persistence tests, and failure handling tests for missing or invalid save data.
- Definition of done: best score and essential settings survive relaunch safely through a versioned profile model.
- Exit criteria: persistence is reliable enough for wider playtesting and future release builds.
- Documentation updates required: update `.ai/10-data-content-model.md` when save schema or persisted fields change, `.ai/08-ui-ux-style-guide.md` if settings behavior becomes canonical, and `.ai/11-testing-quality-bar.md` for persistence test coverage expectations.
- What future phases unlock: Phase 7 tuning and Phase 10 QA can rely on stable best-score and settings behavior.

## Phase 7: Difficulty, fairness, and tuning pass

- Objective: tune the game into a fair, readable, replayable loop rather than a merely functional prototype.
- Why this phase exists: the documented identity depends on harsh-but-fair challenge, readable danger, and near-instant retry, all of which require deliberate tuning rather than default values.
- Scope: tune speed, spacing, telegraph windows, intro safety, hazard unlock order, difficulty bands, near-miss thresholds, milestone timing, and score pacing; refine only within approved mechanic boundaries.
- Out-of-scope / anti-scope: new verbs, filler mechanics, difficulty-by-noise, or content expansion used to avoid solving fairness problems.
- Dependencies: Phase 3 through Phase 6.
- Key systems involved: `DifficultyDirector`, `HazardManager`, `TrackManager`, `PlayerController`, `ScoreSystem`, and gameplay config assets.
- Likely files or file areas affected: `Assets/_Game/Config/Balance/`, `Assets/_Game/Config/Gameplay/`, hazard prefabs, gameplay tests, and fairness/regression test assets.
- Risks: impossible-feeling sequences, unreadable ramps, tuning by intuition without enough play evidence, or masking fairness issues behind effects.
- Validation steps: use the fairness checklist, run repeated short-session playtests, confirm early teaching windows, confirm no impossible intro patterns, and expand automated coverage around difficulty selection and progression logic.
- Definition of done: the core loop consistently produces understandable deaths, motivating near-misses, and a score curve that feels beatable.
- Exit criteria: the team repeatedly judges the core loop as fun, fair, and worthy of more content polish.
- Documentation updates required: update `.ai/07-gameplay-systems.md` when actual rules or tuning expectations change materially, `.ai/10-data-content-model.md` if difficulty config structure changes, and `.ai/11-testing-quality-bar.md` if fairness validation gates evolve.
- What future phases unlock: Phase 8 can complete launch scope without building on unstable gameplay foundations.
## Phase 8: Scope-complete content pass

- Objective: complete the intended launch scope without turning the game into a larger product.
- Why this phase exists: the game needs enough approved content and presentation completeness to ship, but not a backlog-driven explosion of systems.
- Scope: complete the approved initial hazard family set only as far as readability and fairness support it, finalize the first launch theme, complete required menu/HUD/result/settings content, and remove launch-blocking placeholders.
- Out-of-scope / anti-scope: daily challenge, social features, leaderboards, deep unlock economies, extra game modes, feature branches created from wish-list items, and cosmetic quantity for its own sake.
- Dependencies: Phase 7.
- Key systems involved: hazard content pipeline, theme data, UI flow, audio/VFX catalogs, and gameplay presentation assets.
- Likely files or file areas affected: `Assets/_Game/Art/`, `Assets/_Game/Prefabs/Gameplay/`, `Assets/_Game/Prefabs/UI/`, `Assets/_Game/Config/Gameplay/`, `Assets/_Game/Config/Themes/`, `Assets/_Game/Audio/`, `Assets/_Game/Scenes/`.
- Risks: feature creep, inconsistent hazard quality, unfinished placeholder assets surviving into later phases, or adding content before the existing content is actually good enough.
- Validation steps: review launch-scope checklist, verify that every launch-critical feature is present, verify that placeholders are identified or removed, and confirm no blocked or rejected idea has slipped into scope.
- Definition of done: the intended launch feature set exists and is complete enough to feature-freeze.
- Exit criteria: no missing launch-critical system or content piece remains open.
- Documentation updates required: update `.ai/07-gameplay-systems.md`, `.ai/08-ui-ux-style-guide.md`, `.ai/09-audio-vfx-guide.md`, `.ai/10-data-content-model.md`, or `.ai/15-roadmap-backlog.md` when launch-scope decisions or content ownership change.
- What future phases unlock: Phase 9 can polish a feature-complete game instead of a moving target.

## Phase 9: Production polish and consistency pass

- Objective: make the complete launch scope feel cohesive, premium, and intentionally authored.
- Why this phase exists: the game should look and feel like a finished product, not a stack of individually working features.
- Scope: unify motion timing, color usage, typography treatment, visual hierarchy, copy tone, feedback balance, menu demo quality, theme consistency, and result flow presentation; remove obvious placeholder naming and presentation.
- Out-of-scope / anti-scope: new mechanics, new modes, large content additions, and polish work that slows retry or reduces readability.
- Dependencies: Phase 8.
- Key systems involved: UI presentation, theme application, audio balance, VFX balance, menu presentation, result messaging, and art/audio asset consistency.
- Likely files or file areas affected: UI prefabs, theme configs, audio mix assets, VFX prefabs, text/copy assets, menu and gameplay scenes, and polish-related config values.
- Risks: polishing the wrong problem, introducing UI clutter, over-animating transitions, or making the game look louder while becoming less readable.
- Validation steps: long-session playtests, screenshot review, screen-to-screen consistency review, copy review, and fatigue checks across repeated failures and retries.
- Definition of done: there are no obvious presentation mismatches, placeholder feel problems, or consistency breaks in the launch-critical flow.
- Exit criteria: the build feels cohesive enough that remaining work is primarily stability, QA, and performance hardening.
- Documentation updates required: update `.ai/08-ui-ux-style-guide.md` and `.ai/09-audio-vfx-guide.md` for any canonical presentation rule changes; update `.ai/15-roadmap-backlog.md` if polish work moves roadmap priorities.
- What future phases unlock: Phase 10 and Phase 11 can harden and optimize a stable product.

## Phase 10: Testing, QA, and regression hardening

- Objective: reduce pre-release risk by expanding verification, catching regressions, and clearing release blockers.
- Why this phase exists: a polished arcade game fails immediately if taps drop, retries stall, saves break, or safe-area/performance issues remain unstable.
- Scope: add and expand Edit Mode and Play Mode tests, execute the smoke checklist and regression checklist, run manual device QA, triage blocker bugs, and eliminate normal-flow console error noise.
- Out-of-scope / anti-scope: speculative refactors without release impact, late feature additions, or large redesigns disguised as bug fixing.
- Dependencies: Phase 8 and Phase 9, with persistence work complete from Phase 6.
- Key systems involved: all gameplay, UI, persistence, scene flow, and test harnesses.
- Likely files or file areas affected: `Assets/_Game/Scripts/Tests/EditMode/`, `Assets/_Game/Scripts/Tests/PlayMode/`, debug/test helpers, runtime fixes across core systems, and QA tracking notes if maintained.
- Risks: test gaps around critical flows, device-only bugs discovered too late, or churn from late fixes causing new regressions.
- Validation steps: run the documented smoke checklist, regression checklist, persistence checks, safe-area checks, and representative device tests; confirm no release-blocking bug class remains unresolved.
- Definition of done: core flows are covered by automated tests where appropriate, manual QA has exercised feel and device risks, and release blockers are fixed or formally cut from scope.
- Exit criteria: the game is stable enough that performance and size work can proceed without a flood of correctness bugs.
- Documentation updates required: update `.ai/11-testing-quality-bar.md` if testing scope or quality gates evolve; update `.ai/15-roadmap-backlog.md` when bugs force scoped cuts or deferrals.
- What future phases unlock: Phase 11 and Phase 12 can proceed with lower release risk.

## Phase 11: Performance, memory, and build-size optimization

- Objective: meet mobile performance and footprint expectations without damaging feel, readability, or maintainability.
- Why this phase exists: a small polished mobile game must load quickly, run smoothly, and ship without unnecessary package or asset bloat.
- Scope: measure startup flow, frame pacing, memory use, overdraw, audio footprint, texture footprint, package footprint, and build size; optimize hot paths, trim unused packages/assets, and add pooling only where profiling shows a real need.
- Out-of-scope / anti-scope: speculative architecture rewrites, tech novelty experiments, and optimization work with no measured problem behind it.
- Dependencies: Phase 10 on a stable near-release build.
- Key systems involved: rendering, UI, VFX, audio, asset import settings, package baseline, build pipeline, and any hot gameplay paths creating avoidable allocations.
- Likely files or file areas affected: runtime hotspots, asset import settings, audio assets, texture assets, particle prefabs, `Packages/manifest.json`, `ProjectSettings/`, and build-related configs.
- Risks: over-optimizing too early, hiding quality problems behind lowered fidelity, or carrying unnecessary template and package weight into release builds.
- Validation steps: profile on representative mobile hardware, compare build sizes across iterations, confirm stable frame pacing during common gameplay, confirm fast restart after optimization, and verify asset/package trimming did not break content.
- Definition of done: performance, memory, and build-size concerns are measured, addressed, and no longer considered release blockers.
- Exit criteria: the release candidate can target stable 60 FPS on intended hardware tiers with acceptable startup and a justified build footprint.
- Documentation updates required: update `.ai/03-tech-stack.md` if package or baseline tech decisions change, `.ai/11-testing-quality-bar.md` if budgets or verification expectations change, and `.ai/12-adrs.md` if a cross-cutting optimization decision changes architecture or loading strategy.
- What future phases unlock: Phase 12 can finalize the release candidate with confidence.

## Phase 12: Release candidate and publishing readiness

- Objective: produce the final candidate build and verify that it is ready for submission and launch.
- Why this phase exists: the project needs an explicit release gate, not an informal assumption that a late beta is close enough.
- Scope: verify build settings, scene list, package alignment, release-safe input and debug configuration, persistence behavior on clean installs and relaunches, required assets/copy, and publishing readiness tasks appropriate to mobile launch.
- Out-of-scope / anti-scope: new features, broad tuning rewrites, major refactors, and store-facing commitments that the product itself does not support.
- Dependencies: Phase 10 and Phase 11.
- Key systems involved: build pipeline, scene flow, player settings, persistence, UI, audio, and release verification.
- Likely files or file areas affected: `ProjectSettings/`, scene list, build configuration assets, release notes/checklists, publishing metadata assets if stored in-repo, and any final blocker fixes.
- Risks: hidden debug hooks, incorrect scene/build settings, platform-specific regressions, or last-minute undocumented changes.
- Validation steps: execute the final release checklist, run clean-install and relaunch tests, confirm no critical blockers remain, confirm docs are current, and secure release approval from the team.
- Definition of done: there is an approved release candidate build that satisfies scope, quality, performance, and documentation expectations.
- Exit criteria: the team considers the candidate build publishable without known critical blockers.
- Documentation updates required: update `.ai/15-roadmap-backlog.md` for deferred items, `.ai/12-adrs.md` only if a cross-cutting release decision changes project policy, and this file if release gates or post-launch boundaries shift.
- What future phases unlock: Phase 13 begins after launch or release-candidate sign-off when post-launch handling becomes the focus.

## Phase 13: Post-launch stabilization boundaries

- Objective: protect the launched game from identity drift while still allowing responsible post-launch support.
- Why this phase exists: the period after launch is when short-term pressure can easily create mechanic creep, UI clutter, and monetization drift.
- Scope: bug fixes, narrow balance tuning, small polish improvements, measured readability/performance fixes, and narrow content additions that fit the shipped identity.
- Out-of-scope / anti-scope: a second gameplay verb, scene sprawl, backend dependency added to core play, ad-spam patterns, feature systems that turn the game into a different product, and post-launch work that weakens fairness or retry speed.
- Dependencies: a shipped or approved release baseline.
- Key systems involved: only the systems directly touched by validated post-launch issues or narrowly approved additions.
- Likely files or file areas affected: targeted runtime systems, configs, content assets, release notes, and the relevant source-of-truth docs for any approved change.
- Risks: reacting to feedback by changing the product identity, shipping too many systems under the label of support, or letting launch bugs justify undocumented architecture drift.
- Validation steps: confirm each post-launch change strengthens stability, clarity, fairness, or presentation without crossing non-negotiable boundaries; rerun regression checks after every patch.
- Definition of done: launch issues are handled without opening uncontrolled roadmap expansion.
- Exit criteria: post-launch work stays disciplined enough that future roadmap decisions can be made intentionally rather than reactively.
- Documentation updates required: update the owning source-of-truth docs for any approved post-launch behavior change, update `.ai/12-adrs.md` for cross-cutting decision changes, update `.ai/15-roadmap-backlog.md` for promoted or deferred work, and update this file if post-launch boundaries change.
- What future phases unlock: only explicitly approved next-scope planning, not automatic feature expansion.
## Deliverables by phase

- Phase 0: explicit repo-to-doc alignment checklist and ordered implementation baseline.
- Phase 1: `Assets/_Game` structure, required scenes, required `.asmdef` files, approved package baseline, corrected build order, and project-owned baseline settings.
- Phase 2: config asset families, stable IDs, first theme data, audio/VFX catalogs, validators, and a defined save-profile model.
- Phase 3: playable loop with one-tap flip, moving line, approved hazards, score, death, and retry.
- Phase 4: `Bootstrap`, `MainMenu`, `Gameplay` flow, HUD, result panel, pause/home/retry flow, and safe-area handling.
- Phase 5: flip feedback, death feedback, score feedback, near-miss feedback, menu click polish, and baseline music/mixer routing.
- Phase 6: versioned save profile, best-score persistence, settings persistence, and reliable relaunch behavior.
- Phase 7: tuned difficulty curve, fairness-reviewed hazards, pacing refinements, and updated gameplay tests.
- Phase 8: launch-scope-complete content set, completed required theme/UI states, and removal of blocking placeholders.
- Phase 9: cohesive UI, copy, motion, audio balance, theme consistency, and product-wide polish pass.
- Phase 10: automated tests, manual QA evidence, blocker triage completion, and regression-hardening fixes.
- Phase 11: measured performance budgets, memory/build-size trimming, startup improvements, and optimization fixes backed by profiling.
- Phase 12: approved release candidate, verified build settings, verified package alignment, complete release checklist, and submission-ready product state.
- Phase 13: controlled post-launch support plan with explicit boundaries around allowed fixes and disallowed scope drift.

## Quality gates

- Repo/doc alignment gate: do not begin substantial feature work until the template-vs-doc gap is identified and the baseline target is explicit.
- Architecture compliance gate: do not add major runtime systems, scenes, or tech paths that are not already owned by the architecture, scene-flow, and tech-stack docs.
- Gameplay feel gate: do not expand launch scope until the team can repeatedly play the core loop and report that it feels immediate, understandable, and retryable.
- Fairness/readability gate: do not add hazard quantity or complexity until current hazards pass readability and fairness checks at actual run speed.
- UI clarity gate: do not broaden menus or overlays until Play, HUD, Result, and Retry hierarchy are clear on small portrait screens and safe areas.
- Audio/VFX consistency gate: do not pile on extra effects until flip, death, score, and near-miss feedback are distinct, restrained, and readable.
- Persistence reliability gate: do not distribute broader playtest builds until best score and settings survive relaunch safely and predictably.
- Testing coverage gate: do not treat a phase as complete without automated coverage for logic/persistence areas and manual coverage for feel/readability areas.
- Performance gate: do not approve release candidates until the game runs with stable frame pacing on representative devices and restart speed remains immediate-feeling.
- Build-size gate: do not ship with unused heavy packages, leftover template/sample content, or unjustified asset footprint growth; build size must be measured and intentionally accepted.
- Release-readiness gate: do not submit a build until scope is complete, critical blockers are closed, docs are current, and the release checklist is fully satisfied.

## Production standards

Production-ready for Voltline means all of the following are true:

- Stability: the core app flow boots, starts, plays, fails, retries, pauses, returns home, and relaunches without soft locks or normal-flow error spam.
- Consistency: scene naming, folder ownership, config ownership, UI behavior, cue naming, and save behavior match the documented project rules.
- Maintainability: systems remain small, responsibilities remain explicit, config owns tuning, and the project has not accumulated parallel managers or undocumented shortcuts.
- Player-facing polish: the game looks intentional, not template-like or placeholder-heavy, and the core loop feels responsive every run.
- Visual cohesion: menu, gameplay, results, and settings all belong to the same product language, with disciplined color roles and readable composition.
- Audio cohesion: flip, death, score, milestone, near-miss, and UI audio are routed and balanced intentionally rather than layered ad hoc.
- Loading/startup expectations: startup to menu remains short and uncluttered, transitions stay light, and retry avoids heavy detours or loading friction.
- Bug tolerance expectations: release-blocking bugs listed in `.ai/11-testing-quality-bar.md` are not acceptable in a production-ready build.
- Data integrity expectations: best score, settings, theme selection, and other persisted fields survive normal usage without corruption or silent resets.
- Test expectations: automated tests cover logic/persistence risk, manual validation covers feel/readability/device behavior, and known risks are explicitly stated.

## Performance and build-size guidance

- Startup discipline: keep app start minimal, route quickly through `Bootstrap`, and avoid unnecessary loading screens, warm-up flows, or extra initialization work that is not required for menu entry.
- Frame-rate expectations: target stable 60 FPS on representative mid-range mobile hardware, and preserve stable frame pacing during common gameplay, UI updates, and approved VFX.
- Memory discipline: keep runtime memory conservative for a small arcade title, avoid large unnecessary asset loads, and track high-memory content before it becomes release risk.
- Effect budget discipline: keep particles short-lived and low-count, avoid persistent hazard-adjacent clutter, and prefer effect precision over effect density.
- UI/rendering discipline: keep overdraw modest, keep HUD lightweight, keep full-screen effects restrained, and do not add rendering complexity that undermines gameplay clarity.
- Asset compression awareness: use mobile-appropriate texture and audio import settings, and trim or recompress assets that contribute heavily without adding visible value.
- Audio footprint awareness: keep loops and one-shots small and purposeful, avoid redundant variations that players will not perceive, and preserve mix quality without bloating build size.
- Package discipline: remove packages that are outside the approved baseline unless an updated source-of-truth doc and ADR justify them; do not ship unused SDK weight.
- Build-size awareness: measure build size regularly in late production, remove template leftovers and unused assets, and treat unexplained size growth as a production issue rather than a release surprise.
- Optimization principle: profile first, optimize the real bottlenecks, and avoid architecture churn unless measured evidence shows the current approach cannot hit the quality bar.

## UI/UX execution standards

- Clean menu hierarchy: `MainMenu` should explain the game visually, lead with one dominant Play action, and avoid equal-weight button clutter.
- Clear HUD priorities: gameplay HUD should emphasize score first, keep pause reachable, and avoid covering the main danger area.
- Dominant retry action: the result panel must make Retry the primary visual and input target over Home or Share.
- Safe area handling: score, pause, and core buttons must remain readable and tappable on tall phones, small phones, and notch-heavy devices.
- Touch target quality: all player-facing buttons must be comfortably tappable on mobile without precision frustration.
- Animation discipline: UI transitions should be short, smooth, and supportive; they should not delay re-entry or compete with gameplay motion.
- Contrast/readability: danger, player, line, score, and primary actions must remain readable at a glance in motion and in screenshots.
- Screenshot-worthiness: menu and gameplay layouts should produce still frames that show score, line, player, and danger clearly without overcrowding.
- Screen consistency: menu, gameplay, result, and settings screens should share spacing logic, typography logic, color role logic, and interaction tone.
- Copy discipline: text stays short, concrete, and secondary to visual understanding; no screen should rely on paragraphs to teach the core mechanic.

## Audio/VFX execution standards

- Satisfying flip: the flip cue and effect must make the primary action feel immediate, precise, and repeatable across many retries.
- Readable near-miss: near-miss feedback should feel special and skilled, but must never confuse whether the player was actually safe.
- Satisfying score response: score feedback should confirm success cleanly without becoming louder or more important than the main flip feel.
- Memorable but concise death feedback: death must feel final, fair, and motivating with a short, readable burst rather than a long interruption.
- Milestone feedback: milestone celebration should be noticeable and rewarding, but never slow the next decision or clutter the next obstacle.
- Overlap discipline: repeated one-shots should be gated or varied enough to stay clear; duplicate noise on the same frame should be controlled.
- Mixer discipline: Music, Gameplay SFX, and UI SFX must route intentionally and stay balanced around gameplay readability.
- No noisy clutter: persistent electrical arcs, glow, particles, and pulses must support hazard recognition rather than covering it.
- Readability and fairness first: every feedback choice should help players understand what happened, what was dangerous, and why the run ended.
- Thematic discipline: audio and VFX should support the premium neon arcade tone without tipping into harsh, exhausting, or juvenile noise.

## Release checklist

- Intended launch scope is complete.
- No critical or release-blocking bugs remain open.
- Build settings match portrait-mobile launch targets.
- Package list is aligned with the approved baseline and contains no unjustified extras.
- Production scenes are exactly `Bootstrap`, `MainMenu`, and `Gameplay` in the correct build order.
- Persistence has been verified for best score, settings, and other shipped save fields.
- Main menu, gameplay HUD, result panel, pause flow, and settings flow are complete and safe-area compliant.
- Audio routing, core SFX, music, and required VFX are complete and balanced.
- Performance is acceptable on representative devices and restart speed remains immediate-feeling.
- Build size is measured, reviewed, and accepted with no obvious avoidable bloat.
- Automated tests and manual QA required by the quality bar have been completed successfully.
- Source-of-truth docs are updated to match the shipped behavior and no major undocumented drift remains.
- The release candidate has been explicitly approved as publishable by the team.

## Post-launch boundaries

Allowed after launch:

- bug fixes
- fairness and readability tuning
- narrow performance or stability fixes
- small polish improvements
- narrow content additions that fit the shipped identity

Not allowed after launch without explicit product-direction approval:

- mechanic creep
- a second gameplay verb
- UI clutter that weakens readability
- over-monetized patterns that damage trust or fast re-entry
- systems that reduce fairness or increase randomness
- features that turn the product into a different game
- backend or package expansion added to core play without documented justification

## Agent execution guidance

- Follow phases in order.
- Do not skip quality gates because a later phase sounds more interesting.
- Do not expand scope before core quality is validated.
- Use the smallest implementation that satisfies the current phase objective cleanly.
- Update the correct source-of-truth docs whenever implementation changes their owned topics.
- Create or update an ADR when a cross-cutting technical or product decision changes.
- Prefer small validated increments over broad speculative work.
- If the current repo state does not satisfy the phase gate, report the miss explicitly instead of pretending the phase is complete.
- If a requested task belongs to a later phase, either defer it or document the roadmap change before proceeding.
- Do not treat backlog ideas as launch scope unless they are explicitly promoted.
- Keep this file synchronized with actual phase boundaries and release standards when those change.

## Maintenance protocol for this roadmap

Update this file when:

- phase boundaries change
- release scope changes
- quality gates change
- production standards change
- post-launch scope changes
- the starting-state assumptions are no longer accurate enough to guide the next major work unit

Do not update this file for routine implementation progress alone.  
Update it when the delivery map itself changes.
