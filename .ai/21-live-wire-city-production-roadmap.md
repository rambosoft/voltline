# Live Wire City Production Roadmap
Status: Active
Owner: Team / AI
Last updated: 2026-03-17
Source of truth for: phased transition from current repo state to production-ready Live Wire City presentation quality, including prerequisite preparation before theme rollout
Depends on: `.ai/20-live-wire-city-production-style-guide.md`, `.ai/19-presentation-refresh-readiness-roadmap.md`, `.ai/00-index.md`, `.ai/01-product-vision.md`, `.ai/02-game-design-pillars.md`, `.ai/03-tech-stack.md`, `.ai/04-architecture.md`, `.ai/05-project-structure.md`, `.ai/06-scene-flow.md`, `.ai/07-gameplay-systems.md`, `.ai/08-ui-ux-style-guide.md`, `.ai/09-audio-vfx-guide.md`, `.ai/10-data-content-model.md`, `.ai/11-testing-quality-bar.md`, `.ai/12-adrs.md`, `.ai/14-agent-rules.md`, `.ai/15-roadmap-backlog.md`, `.ai/16-production-phases.md`
Do not duplicate with: gameplay systems, architecture, tech stack, backlog planning, generic implementation phases, or pre-existing preparation notes

## 1. Purpose

This file is the execution roadmap for moving the current repo from its readiness-complete presentation baseline into the final **Live Wire City** production state defined by `.ai/20-live-wire-city-production-style-guide.md`.

This file owns:

- the migration path from current repo reality to the final Live Wire City target
- the prerequisite work still needed before broad theme rollout begins
- the ordered implementation phases for gameplay presentation, UI, audio, VFX, copy, and branding-safe product surfaces
- the quality gates that keep the rollout aligned, readable, fair, maintainable, and performant

This file does not own:

- gameplay rules in their final source-of-truth form
- stable runtime architecture rules
- stack decisions
- generic release sequencing
- backlog idea ownership

How to use this file:

- Treat `.ai/20-live-wire-city-production-style-guide.md` as the final target-state presentation decision source.
- Treat `.ai/19-presentation-refresh-readiness-roadmap.md` as the completed technical readiness foundation.
- Treat older docs and current repo reality as the starting baseline that this roadmap intentionally migrates away from where needed.
- If `20` is silent on a product-facing choice, use the decisions locked in this roadmap.
- When implementation crosses a current-doc baseline and lands a new Live Wire City target-state fact, update the owning source-of-truth docs in the same work unit.

## 2. Production target summary

The target state is the shipped **Voltline** experience under the **Live Wire City** direction from `20`, not a partial reskin of the current Neon Night / Candy Pop baseline.

Target-state summary:

- one progressive Live Wire City theme is the first-release production direction
- the game remains a premium, one-tap, portrait arcade run with instant retry and fair readability
- the player is a living electric transfer core riding a dangerous city power conduit
- the line is a city-scale energized conduit, not a generic abstract track
- hazards use the world language, types, and descriptions from `20`
- the city visibly wakes up as score increases through world-state progression and milestone response
- the menu, HUD, pause, result, settings, audio, and VFX all feel like one shipped product
- public branding remains `Voltline`, with placeholder-safe logo and subtitle handling until final brand assets exist
- the front door stays minimal, flat, clean, and modern while clearly expressing electricity, line, and city identity
- result and milestone copy stay short, retry-friendly, and city/power-grid themed

First-release target posture:

- ship one progressive Live Wire City theme first
- keep future support for multiple distinct themes structurally possible, but later
- keep splash/logo moment, separate world progression screen, share screen, and expanded tutorial treatment optional unless intentionally promoted
- promoted Phase 7 extras must stay additive overlays inside `MainMenu` or `Gameplay`, not new scenes

## 3. Preparation-roadmap assessment

`.ai/19-presentation-refresh-readiness-roadmap.md` already solved the technical readiness layer well.

What `19` already covers strongly:

- player and obstacle visual decoupling from gameplay truth
- background presentation ownership and budgets
- theme-aware audio and VFX pipelines
- staged rollout gating and approval audits
- config-driven presentation ownership across gameplay-facing systems

What `19` partially covers:

- theme progression infrastructure
- background progression support
- presentation gating for staged rollout
- the technical side of future art/audio/VFX replacement

What `19` does not fully cover for the final Live Wire City rollout:

- final product-direction decisions from `20`
- branding-safe `Voltline` title/logo/subtitle handling
- production copy ownership for title, milestone, result, and other user-facing text
- menu/HUD/result/settings styling standards tied specifically to Live Wire City
- migration from whole-theme swapping toward one evolving Live Wire City world
- product-facing UI tokens for typography, panels, icons, and branded surfaces
- the final hazard model and presentation language from `20`

Assessment:

- `19` remains a completed prerequisite and should not be repeated.
- `19` is not the final production rollout plan.
- `21` exists to move the project from the readiness-complete baseline created by `19` to the exact target-state defined by `20`.

## 4. Current-state assessment

### Already aligned

- the repo has the approved three-scene structure: `Bootstrap`, `MainMenu`, `Gameplay`
- gameplay presentation already routes through config-owned player, hazard, background, theme, audio, and VFX systems
- the project is structurally ready for staged asset replacement and controlled presentation rollout
- safe area handling, instant retry flow, and one-tap gameplay identity already exist
- audits and validators already protect core presentation-readiness and rollout discipline

### Partially aligned

- menu, HUD, pause, result, and settings exist but still look closer to a generic runtime UI shell than a final Live Wire City front door
- progression infrastructure exists, but the live repo still reflects a Neon Night / Candy Pop baseline instead of one evolving Live Wire City world
- player and hazard presentation support exists, but current authored content is still sparse compared with the final target in `20`
- audio and VFX routing is ready, but authored production content is still limited

### Missing versus the final target in `20`

- Phase 1 groundwork is now in place through config-owned `Voltline` title/subtitle/logo support in `BrandingPresentationConfig`
- Phase 1 groundwork is now in place through `ProductionCopyConfig`, which owns menu, pause, result, settings, and milestone copy for current first-release surfaces
- Phase 2 groundwork is now in place through `UIThemeConfig` and token-driven UI construction across menu, HUD, pause, result, and settings
- Phase 3 is now in place through one surfaced `theme.live-wire-city` base theme plus `WorldProgressionConfig` and `WorldProgressionController`
- Phase 4 is now in place through the six-family Live Wire City hazard migration, progression-reactive line/background handling, and gameplay-safe config ownership
- Phase 5 is now in place through the Live Wire City front door shell, visible-but-disabled future entries, and city/power-grid result messaging
- no fully authored production audio and VFX package currently exists to match the final target in `20`
- final authored typography and logo assets are still placeholder-safe rather than final

### Prototype-like or placeholder-like areas

- most player-facing menu, pause, result, settings, and theme-status strings are now config-owned; future rollout phases should continue removing legacy copy as surfaces evolve
- the menu shell is now structurally aligned with the Live Wire City front door, but still awaits final authored font/logo assets
- legacy theme assets still exist in the repo even though the first-release shipping posture now surfaces one Live Wire City theme
- current audio and VFX catalogs still rely heavily on sparse fallback content
- production-facing folders for fonts, UI art, authored VFX, and authored audio remain thin

### Structural migration work still required

- continue extending the placeholder-safe branding and copy ownership path as later optional surfaces are implemented
- continue feeding the shipped UI through the token layer as final typography, icon, and surface assets arrive
- replace sparse fallback-heavy audio and VFX content with authored Live Wire City feedback
- remove legacy Neon Night / Candy Pop assumptions completely during the final hardening pass
- finalize canonical font and logo assets when approved
## 5. Gap analysis

### Architecture readiness

- strong enough to support the rollout
- branding, production copy, UI presentation tokens, and world progression now have dedicated ownership
- first-release runtime progression now matches `20` through one surfaced base theme plus district progression

### Config and extensibility readiness

- strong for player, hazard, background, world progression, audio, and VFX routing
- complete for title/logo/subtitle, copy-library, baseline UI-token ownership, and first-release world progression ownership
- still awaiting final authored typography and authored audio/VFX packages

### Art direction support

- the repo can now accept real player and hazard art safely
- the current content package is still far from the complete Live Wire City target in `20`
- no canonical Live Wire City asset package currently replaces the legacy baseline end to end

### Hazard and environment presentation

- the final target hazard language from `20` is now the current gameplay-facing baseline in the repo
- the background and line now support district progression inside one Live Wire City theme
- authored hazard/background content is still incomplete compared with the final quality target

### HUD and menus

- structure exists, but final typography, panel treatment, iconography, and product hierarchy are not yet in place
- the front door shell now matches the Live Wire City structure; the remaining gap is final authored iconography and typography polish
- settings currently expose theme-related structure that should remain hidden while only one production theme is live

### Results and death flow

- retry-first flow remains correct
- result and death presentation now use short city/grid/power-loss language from config
- the remaining gap is final authored typography/logo polish rather than missing structural support

### Typography and spacing

- safe area and broad hierarchy exist
- the UI token layer exists, but the final authored font package is still missing
- spacing and layout language still need a true production pass to align all surfaces

### Asset replacement readiness

- technically ready because `19` is complete and Phases 3 to 5 are now landed
- player, hazard, background, progression, and front-door assets now have stable runtime ownership paths
- final authored audio/VFX and typography still need replacement content rather than more structural prep

### Audio and VFX cohesion

- pipeline is ready
- authored production content is still missing or sparse
- current fallback-heavy content does not yet match the world identity from `20`

### Scene composition

- the existing three-scene structure remains sufficient for first release
- optional extras from `20` should fit inside the existing scene model unless later promoted with explicit scope change
- no new scene should be assumed by default in this roadmap

### Branding placeholder safety

- internal project naming is already stable enough
- public-facing branding is now config-owned and placeholder-safe; the remaining gap is final approved logo art

### Player-facing polish consistency

- gameplay and front-door structure now align under one Live Wire City shell
- the remaining mismatch is authored polish depth, not product-shell structure
- `20` expects one cohesive product identity across all screens and states

### Maintainability

- maintainability stays strong if the rollout remains config-driven and token-driven
- maintainability will erode quickly if final styling is layered directly into generic UI scripts without dedicated ownership

### Performance-aware presentation support

- the project already has strong budget discipline from `19`
- the final city-world presentation from `20` still needs careful rollout discipline so speed, clarity, and mobile budgets stay intact

## 6. Non-negotiables

- gameplay remains one-tap
- readability beats clutter
- presentation must never damage fairness or collision honesty
- production polish must not slow retry speed
- the final target is Live Wire City as defined by `20`, not a compromise between old themes and the new direction
- `Voltline` branding must stay placeholder-safe until final logo/brand assets are approved
- no branding decision should create refactor debt in repo names, namespaces, scenes, or config IDs
- one progressive Live Wire City theme is the first-release production target
- optional extras stay optional unless formally promoted
- systems should become more configurable and extensible, not more brittle
- audio, VFX, UI, copy, and gameplay presentation must feel like one shipped product
- current-doc baselines are starting points, not blockers, when they differ from the final target in `20`
- whenever implementation lands a new target-state fact that replaces an older baseline, the owning docs must be updated in the same work unit

## 7. Delivery philosophy

- target-state clarity before implementation detail
- preparation before asset flood
- foundation before styling sprawl
- one product direction before parallel theme experimentation
- one evolving theme before multi-theme shipping complexity
- readability first
- polish in intentional layers
- config-driven and token-driven systems preferred
- production quality means cohesive, stable, maintainable, performant, and easy to evolve
- migration work is acceptable when required to reach `20`; half-migration is not

## 8. Phase overview

| Phase | Name | Purpose | Key outcome | Exit gate |
| --- | --- | --- | --- | --- |
| 0 | Goal-to-baseline reconciliation | Lock the migration contract from current state to `20` | Final target differences are explicit and planned, not accidental | Implementation can proceed without product-direction ambiguity |
| 1 | Branding-safe presentation tokens | Make public branding and copy replaceable and config-owned | `Voltline` title, placeholder logo, subtitle, and copy paths exist cleanly | Public-facing language is no longer trapped in runtime strings |
| 2 | UI production support baseline | Create the token/hook layer for a flat, clean, modern Live Wire City UI | Menu, HUD, pause, result, and settings can take final production styling safely | UI can absorb real typography, icons, panels, and branded surfaces |
| 3 | Live Wire City world progression model | Replace whole-theme swapping with one evolving Live Wire City progression model | Score-band and milestone-driven world progression is config-owned and explicit | The repo can express the final world progression target from `20` honestly |
| 4 | Gameplay presentation rollout | Move player, line, hazards, and background toward the final Live Wire City target | In-run gameplay reads as Live Wire City, not a legacy baseline | Gameplay presentation, hazard model, and world state are aligned with `20` |
| 5 | UI and front-door rollout | Bring menu, HUD, result, pause, and settings to the same production level as gameplay | The shell around gameplay matches the same final product language | Front door and result flow feel production-ready and cohesive |
| 6 | Audio and VFX rollout | Land the final city-electric feedback language | Feedback feels authored, premium, and world-cohesive | Core event feedback matches `20` without harming readability |
| 7 | Optional extras | Add only promoted optional surfaces from `20` | Extras fit the product without creating structural debt | Optional additions stay additive and disciplined |
| 8 | Integration and hardening | Clean up, tune, validate, and retire legacy assumptions | The game reads as one final Live Wire City product | Consistency, readability, performance, and audit gates all pass |

Current repo status after the latest implementation pass:

- Phase 0 is complete at the documentation/runtime contract level
- Phase 1 is complete through `BrandingPresentationConfig`, `ProductionCopyConfig`, and config-driven result-copy ownership
- Phase 2 is complete through `UIThemeConfig`, token-driven UI factory support, and hidden first-release theme selection
- Phase 3 is complete through `WorldProgressionConfig`, `WorldProgressionController`, the single surfaced `theme.live-wire-city` shipping posture, and dormant legacy `ThemeSequenceConfig`
- Phase 4 is complete through the six-family Live Wire City hazard migration, progression-reactive line/background presentation, and gameplay-safe config ownership
- Phase 5 is complete through the shipped front door shell, menu entry state gates, HUD district/milestone presentation, and city/power-grid result flow
- Phase 6 is complete through dedicated Live Wire City audio/VFX profiles, expanded semantic cue catalogs, family-aware death feedback, and upgraded procedural city-electric feedback
- Phase 7 is complete through the promoted splash/logo moment, Grid Status overlay, share surface, and expanded first-run tutorial, all kept inside the existing three-scene model
- Phase 8 is complete at the repo level through validator, audit, test, and legacy-profile hardening for the Live Wire City shipping posture

## 9. Detailed phased roadmap

### Phase 0: Goal-to-baseline reconciliation

- Objective: reconcile current repo reality, current source-of-truth baselines, and the final target in `20` into one explicit migration contract.
- Why this phase exists: the project is ready for rollout, but multiple older assumptions still exist in docs and config, especially around theme switching, hazard modeling, copy, and UI scope.
- Scope:
  - identify where current docs and repo reality differ from the final target in `20`
  - explicitly mark those differences as planned migration targets
  - lock first-release scope versus optional extras
  - define which owning docs must be updated when implementation lands target-state changes
- Anti-scope:
  - implementing the theme itself
  - silently preserving old assumptions just because they are older
  - adding unplanned feature sprawl
- Dependencies:
  - `.ai/19-presentation-refresh-readiness-roadmap.md`
  - `.ai/20-live-wire-city-production-style-guide.md`
  - current repo reality
- Systems affected:
  - documentation first
  - future theme, hazard, progression, copy, and UI migration work
- Likely files or areas affected during later implementation:
  - `.ai/07-gameplay-systems.md`
  - `.ai/08-ui-ux-style-guide.md`
  - `.ai/09-audio-vfx-guide.md`
  - `.ai/10-data-content-model.md`
  - runtime theme and hazard config assets
- Exact execution guidance:
  - treat `20` as the final presentation target
  - treat current docs and repo systems as the baseline to migrate from
  - explicitly plan for the six hazard families and descriptions from `20`
  - explicitly plan for one evolving Live Wire City theme with both milestone events and score-band world-state progression
  - explicitly keep optional extras out of first-release implementation unless promoted
  - explicitly record which older docs must change when target-state implementation lands
- Risks:
  - half-migrating to `20`
  - mixing old and new target models at the same time
  - leaving doc ownership unclear while implementation proceeds
- Validation steps:
  - no unresolved target-state ambiguity remains
  - the later phases can execute without inventing product decisions
- Definition of done:
  - the migration contract from baseline to `20` is explicit
- Exit criteria:
  - implementation agents can proceed without relitigating target-state rules
- Docs that would need updates during future implementation:
  - `07`, `08`, `09`, `10`, `20`, `21`, and any owning docs touched by actual code changes
- What the phase unlocks next:
  - Phases 1 through 3 can begin cleanly

### Phase 1: Branding-safe presentation tokens

- Objective: make `Voltline` branding and all core user-facing production copy replaceable, config-owned, and placeholder-safe.
- Why this phase exists: the final target in `20` expects a branded product shell, but the repo still contains hardcoded strings and no clean ownership path for title, logo, subtitle, or result language.
- Scope:
  - `Voltline` title support
  - placeholder-safe logo path
  - subtitle or tagline path
  - result and death copy library
  - milestone text library tied to `20`
  - any other public-facing short-form copy needed for first release
- Anti-scope:
  - finalizing permanent public branding assets before they exist
  - broad localization systems beyond first-release need
  - lore-heavy narrative systems
- Dependencies:
  - Phase 0
  - existing runtime UI and result systems
- Systems affected:
  - main menu title surfaces
  - result panel surfaces
  - milestone text surfaces
  - any future share or splash surfaces that become in scope later
- Likely files or areas affected:
  - `Assets/_Game/Config/UI/`
  - `Assets/_Game/Scripts/Runtime/UI/`
  - `Assets/_Game/Art/Sprites/UI/`
- Exact execution guidance:
  - prefer one small config family for branding and one small config family for production copy if separation is cleaner
  - keep fallback text-first branding available while no final logo exists
  - keep subtitle or tagline optional but supported
  - keep internal project names unchanged even if public branding evolves later
  - use short city/power-grid language from `20` for death, result, and milestone copy
- Risks:
  - new hardcoded branding strings sneaking back into code
  - overdesigning branding before final assets exist
  - copy sprawl without clear ownership
- Validation steps:
  - public-facing title, subtitle, result headers, and milestone messages can change without code edits
  - current placeholder assets can be swapped later without refactors
- Definition of done:
  - public-facing branding and production copy are config-owned and placeholder-safe
- Exit criteria:
  - the product shell is ready for branded UI and result implementation
- Docs that would need updates during future implementation:
  - `08`, `10`, `20`, and any UI-copy ownership docs affected
- What the phase unlocks next:
  - Phase 2 and Phase 5

### Phase 2: UI production support baseline

- Objective: create the production support layer for a flat, clean, modern-font Live Wire City UI across menu, HUD, pause, result, and settings.
- Why this phase exists: the current UI shell is structurally healthy but too generic to absorb the final product styling from `20` cleanly.
- Scope:
  - typography token support
  - panel and button surface tokens
  - icon support
  - aligned treatment rules across menu, HUD, pause, result, and settings
  - minimal-option settings visual upgrade support
- Anti-scope:
  - adding extra first-release settings options
  - bloated menu trees
  - building a second parallel UI framework
- Dependencies:
  - Phase 0
  - Phase 1 for branding and copy ownership
- Systems affected:
  - runtime UI surface creation and styling hooks
  - settings and result layout presentation
  - menu front-door treatment
- Likely files or areas affected:
  - `Assets/_Game/Scripts/Runtime/UI/`
  - `Assets/_Game/Config/UI/`
  - `Assets/_Game/Art/Fonts/`
  - `Assets/_Game/Art/Sprites/UI/`
  - `Assets/_Game/Prefabs/UI/` if prefab-backed surfaces are justified
- Exact execution guidance:
  - keep the UI flat, clean, modern, and visually disciplined
  - align all core screens to the same panel, button, typography, and spacing language
  - keep settings minimal even after the visual upgrade
  - hide theme-selection affordance while only one production theme is live
  - ensure the token layer supports future multi-theme UI variation without making it a first-release dependency
- Risks:
  - overdecorated HUD
  - brittle styling spread across many runtime scripts
  - exposing theme selection too early
- Validation steps:
  - menu, HUD, pause, result, and settings can all consume the same production UI language cleanly
  - no extra settings-scope creep is introduced
- Definition of done:
  - the UI layer can accept final Live Wire City production styling safely and consistently
- Exit criteria:
  - front-door and shell rollout can proceed without ad hoc UI hacks
- Docs that would need updates during future implementation:
  - `08`, `10`, `11`, `20`
- What the phase unlocks next:
  - Phase 5

### Phase 3: Live Wire City world progression model

- Objective: replace whole-theme milestone swapping with one evolving Live Wire City world progression model driven by both milestone events and score-band world states.
- Why this phase exists: `20` is explicit that this is one theme becoming more alive, not random theme churn.
- Scope:
  - one evolving Live Wire City theme
  - score-band world-state progression
  - milestone-driven world reactions
  - config-driven progression data owned through presentation and balance systems
  - rules for hidden theme selection while only one production theme is live
- Anti-scope:
  - shipping multiple production themes in first release
  - hardcoding progression stages directly into scene logic
  - creating a second progression system disconnected from the actual run state
- Dependencies:
  - Phase 0
  - readiness systems from `19`
- Systems affected:
  - theme progression config
  - background presentation
  - milestone presentation
  - UI status surfaces if exposed later
- Likely files or areas affected:
  - `Assets/_Game/Config/Themes/`
  - `Assets/_Game/Config/Gameplay/`
  - gameplay presentation controllers
  - settings/theme UI hooks
- Exact execution guidance:
  - keep one active production theme for first release
  - treat multiple future themes as a later extension, not a current shipping requirement
  - express city progression through both score bands and milestone events
  - keep the progression model configurable by design and balance data rather than hardcoded branching
  - remove legacy whole-theme swapping as the shipping model
- Risks:
  - retaining old Neon Night / Candy Pop assumptions under the hood
  - mixing world-state progression and milestone reactions in confusing ways
  - leaking future multi-theme complexity into first-release UI
- Validation steps:
  - the repo can express the phase progression described in `20` honestly
  - progression reads as one city waking up over score, not palette roulette
- Definition of done:
  - Live Wire City world progression is explicit, configurable, and aligned with `20`
- Exit criteria:
  - gameplay and UI rollout can target the correct world model
- Docs that would need updates during future implementation:
  - `07`, `08`, `09`, `10`, `19`, `20`
- What the phase unlocks next:
  - Phases 4 through 6
### Phase 4: Gameplay presentation rollout

- Objective: migrate the in-run presentation toward the final Live Wire City target in `20` across player, line, hazards, and background.
- Why this phase exists: gameplay must look and feel like the final product before shell polish can feel honest.
- Scope:
  - final player presentation pass
  - final line and conduit presentation pass
  - hazard rollout using the types and descriptions from `20`
  - city background rollout and world-state art support
  - stage progression visuals tied to the new progression model
- Anti-scope:
  - adding unrelated gameplay mechanics just because the art is changing
  - uncontrolled theme experimentation outside Live Wire City
  - spectacle that harms readability
- Dependencies:
  - Phases 0 through 3
  - readiness guardrails from `19`
- Systems affected:
  - player visual systems
  - track and line presentation
  - hazard presentation systems and any gameplay-facing hazard model that must expand or reclassify to reach `20`
  - background presentation systems
- Likely files or areas affected:
  - `Assets/_Game/Art/Sprites/Characters/`
  - `Assets/_Game/Art/Sprites/Hazards/`
  - `Assets/_Game/Art/Themes/`
  - `Assets/_Game/Art/Materials/`
  - `Assets/_Game/Config/Gameplay/`
  - `Assets/_Game/Config/Themes/`
- Exact execution guidance:
  - keep explicit visual bounds and collision honesty rules from `19`
  - use the final hazard families and descriptions from `20` as the target-state model
  - if reaching that target requires gameplay-scope changes, treat them as intentional migration work and update owning docs when implemented
  - keep the lane as the clearest gameplay layer and the background as a support role
- Risks:
  - leaving legacy hazard assumptions in place after target-state migration starts
  - readability loss from overdesigned city visuals
  - partial art rollout that still reads as mixed-theme
- Validation steps:
  - player, line, hazards, and background all read clearly at run speed
  - no fairness or collision honesty drift occurs
  - hazard presentation and world progression visibly align with `20`
- Definition of done:
  - the in-run experience reads as Live Wire City rather than a prepared refresh baseline
- Exit criteria:
  - gameplay presentation has reached the target direction strongly enough to support the final shell pass
- Docs that would need updates during future implementation:
  - `07`, `08`, `09`, `10`, `11`, `18`, `19`, `20`
- What the phase unlocks next:
  - Phase 5 and Phase 6

### Phase 5: UI and front-door rollout

- Objective: bring menu, HUD, result, pause, and settings to the same production standard and world identity as gameplay.
- Why this phase exists: Live Wire City is a full product direction, not just an in-run art pass.
- Scope:
  - `Voltline` title and placeholder-safe logo treatment
  - subtitle/tagline treatment
  - menu layout and city-electric entrance feel
  - HUD typography and score treatment
  - result panel hierarchy, styling, and city/power-grid copy
  - pause and settings visual alignment
  - hidden theme selection while only one production theme is live
- Anti-scope:
  - extra first-release settings options
  - bloated menu architecture
  - slow front-door theatrics that delay play
- Dependencies:
  - Phases 1 through 4
- Systems affected:
  - all runtime UI views
  - branding and copy config
  - UI token systems
  - settings presentation
- Likely files or areas affected:
  - `Assets/_Game/Scripts/Runtime/UI/`
  - `Assets/_Game/Config/UI/`
  - `Assets/_Game/Art/Sprites/UI/`
  - `Assets/_Game/Art/Fonts/`
  - `Assets/_Game/Prefabs/UI/`
- Exact execution guidance:
  - keep the menu minimal but clearly electric, city-based, and line-based
  - keep Play and Retry as dominant actions
  - keep settings structurally available but visually secondary
  - keep theme selection hidden until more than one production theme is intentionally live
  - keep result and milestone language short, sharp, and city/power-grid themed
- Risks:
  - generic shell surviving around a polished run
  - too much text or too many controls in the menu and result flows
  - exposing future multi-theme UI too early
- Validation steps:
  - menu, HUD, pause, result, and settings all feel like the same shipped product
  - Retry remains fast and dominant
  - the front door clearly communicates electricity, line, city, and the one-tap loop
- Definition of done:
  - the product shell matches the final gameplay identity from `20`
- Exit criteria:
  - front door, run, death, and retry all feel cohesive and production-ready
- Docs that would need updates during future implementation:
  - `06`, `08`, `10`, `11`, `20`
- What the phase unlocks next:
  - Phase 6 and Phase 8

### Phase 6: Audio and VFX rollout

- Objective: land the authored city-electric feedback language from `20` across gameplay and UI.
- Why this phase exists: the final target requires more than routing; it requires a distinct premium feedback identity.
- Scope:
  - authored flip, score, near-miss, milestone, death, and menu feedback
  - authored music and ambience if approved in scope
  - authored VFX assets or refined prefab-backed effects
  - city-grid and power-loss language across milestone and death feedback
- Anti-scope:
  - noisy spectacle
  - feedback that masks gameplay clarity
  - soundtrack scope that exceeds first-release needs
- Dependencies:
  - Phases 3 through 5
- Systems affected:
  - audio cue catalog and profiles
  - VFX catalog and profiles
  - runtime audio and VFX services
  - UI feedback surfaces
- Likely files or areas affected:
  - `Assets/_Game/Audio/`
  - `Assets/_Game/Art/Sprites/VFX/`
  - `Assets/_Game/Prefabs/VFX/`
  - `Assets/_Game/Config/Audio/`
  - `Assets/_Game/Config/Themes/`
- Exact execution guidance:
  - keep semantic cue IDs stable
  - keep event hierarchy clear and retry-safe
  - use city-electric and grid-failure flavor, especially around milestones and death
  - maintain strict restraint so audio and VFX support gameplay instead of overwhelming it
- Risks:
  - fatigue from harsh loops or busy layering
  - milestone spectacle becoming too heavy
  - menu and gameplay feedback feeling like different products
- Validation steps:
  - every core event is clearly distinguishable and aligned with the Live Wire City world
  - repeated retries still feel clean and satisfying
  - death and milestone feedback match the city/power language from `20`
- Definition of done:
  - audio and VFX feel authored, cohesive, and specific to Live Wire City
- Exit criteria:
  - the final feedback language from `20` is meaningfully present in the shipped loop
- Docs that would need updates during future implementation:
  - `09`, `10`, `11`, `20`
- What the phase unlocks next:
  - Phase 8 and final release-candidate validation with the full optional shell in place

### Phase 7: Optional extras

- Objective: add only the optional surfaces from `20` that are later promoted into scope.
- Why this phase exists: `20` describes strong optional extras, but the first release does not need all of them to hit the core production target.
- Scope:
  - optional splash/logo moment
  - optional Grid Status or similar progression surface
  - optional share surface
  - optional expanded tutorial treatment
- Anti-scope:
  - treating optional extras as first-release blockers
  - scene sprawl
  - broad meta-feature creep
- Dependencies:
  - Phases 1 through 6
  - explicit promotion of each extra into scope
- Systems affected:
  - UI surfaces
  - branding config
  - copy config
  - progression presentation
- Likely files or areas affected:
  - `Assets/_Game/Scripts/Runtime/UI/`
  - `Assets/_Game/Config/UI/`
  - `Assets/_Game/Art/Sprites/UI/`
- Exact execution guidance:
  - keep all extras additive and disciplined
  - keep them inside the existing scene model unless a later ADR changes that intentionally
  - reuse the same branding, copy, and world-progression systems already created in earlier phases
- Risks:
  - front-door complexity slowing first play
  - adding optional features before the core product feels complete
- Validation steps:
  - extras do not weaken clarity, retry speed, or small-scope product discipline
- Definition of done:
  - any approved extras feel native to the same product
- Exit criteria:
  - optional additions create no new structural debt
- Docs that would need updates during future implementation:
  - `06`, `08`, `10`, `11`, `15`, `20`
- What the phase unlocks next:
  - Phase 8 and final release-candidate validation with the full optional shell in place

### Phase 8: Integration and hardening

- Objective: finish the migration with cleanup, consistency review, readability tuning, performance validation, and retirement of legacy assumptions.
- Why this phase exists: production quality requires the whole game to read as one final Live Wire City product, not a layered mix of old and new decisions.
- Scope:
  - cleanup of pre-Live-Wire-City leftovers
  - consistency review against `20`
  - readability and fairness tuning
  - performance and device validation
  - audit and regression hardening
- Anti-scope:
  - fresh late-stage direction changes
  - new systems created after the product target is already locked
  - leaving legacy theme assumptions half-removed
- Dependencies:
  - Phases 1 through 7 as applicable
- Systems affected:
  - the whole player-facing presentation layer
- Likely files or areas affected:
  - all touched content, config, UI, docs, and audit surfaces
- Exact execution guidance:
  - compare the live game directly against `20`
  - remove remaining Neon Night / Candy Pop leftovers if the Live Wire City rollout has replaced them
  - rerun config, readiness, approval, Edit Mode, and Play Mode validation
  - check startup, menu, gameplay, death, retry, progression, and settings flow as one product
- Risks:
  - half-migrated assets or copy
  - mixed hazard or progression assumptions
  - performance regressions from the final polish pass
- Validation steps:
  - no normal-flow console noise
  - consistent style across all core screens and states
  - readability, fairness, and mobile performance remain intact
  - the game feels like `20`, not a compromise between old and new directions
- Definition of done:
  - the game presents as a production-ready Live Wire City product
- Exit criteria:
  - the team can treat presentation as release-ready for the first Live Wire City launch target
- Docs that would need updates during future implementation:
  - `07`, `08`, `09`, `10`, `11`, `15`, `16`, `20`, `21`
- What the phase unlocks next:
  - final release-candidate work under `.ai/16-production-phases.md`
## 10. Missing prerequisites before theme rollout

Before broad Live Wire City rollout begins safely, the project still needs:

- no additional prerequisite phases remain before gameplay/front-door Live Wire City rollout; Phases 0 through 5 are now complete in the repo
- no blocking prerequisite phases remain before release-candidate validation or optional extras
- the highest remaining needs are final brand/font assets, optional extras only if promoted, and device-level release validation rather than additional architectural rollout work

## 11. Placeholder title and logo strategy

`Voltline` remains the active public-facing title for now.

Rules:

- keep `Voltline` as the stable internal codename, namespace root, assembly root, scene naming baseline, and config naming prefix
- do not spread public branding across runtime strings in multiple scripts
- do not bake final logo assumptions into scene object names or config IDs
- keep logo support optional and text-first friendly until final logo assets exist
- keep subtitle or tagline optional and placeholder-safe

Recommended strategy:

- introduce a small branding config family under `Assets/_Game/Config/UI/`
- support:
  - fallback text title
  - optional logo sprite
  - optional subtitle or tagline
  - result-title overrides
  - future optional splash/share title use without structural refactor
- keep placeholder art and placeholder text clearly identified as temporary

This keeps later branding changes as config and content work, not refactor work.

## 12. UI and UX production standards

### Main menu

- keep it minimal but clearly electric, city-based, and line-based
- support `Voltline` title plus optional subtitle
- showcase live gameplay lane motion without turning the menu into a noisy scene
- keep Play dominant
- keep secondary actions visually subordinate
- hide theme selection while only one production theme is live

### HUD

- score remains dominant and instantly readable
- best score stays secondary
- pause stays lightweight
- typography and spacing must match the same final product language as menu and result

### Pause and settings

- keep pause quick and uncluttered
- keep settings visually upgraded but option-light
- keep settings aligned with the same flat clean design language as the rest of the product
- do not expose future multi-theme controls until they are real shipping features

### Result and death panel

- keep Retry dominant
- keep Home secondary
- keep copy short, sharp, and city/power-grid themed
- pair death language to the world identity from `20` where practical

### Transitions and interaction polish

- short, decisive, premium
- no heavy cinematic delay before retry
- one strong primary action per screen
- clean motion, not noisy motion

### Safe area, spacing, and readability

- keep all critical controls and score elements safe-area correct
- use whitespace for hierarchy
- avoid dense stacked panels
- prioritize screenshot clarity without adding clutter

## 13. Visual production standards

- final player, line, hazard, and world presentation should follow the target-state direction from `20`
- obstacle families should use the final world language, types, and descriptions from `20`
- lane readability remains the top priority over atmosphere and decoration
- glow stays controlled and role-based
- background remains a support layer even when city progression becomes richer
- canonical assets should stay consistent across gameplay, menu preview, and result surfaces
- the product should read as one authored world, not a collection of separate theme experiments
- milestone progression should enrich the world gradually without turning the lane noisy

## 14. Audio and VFX production standards

### Flip feedback

- crisp, premium, and electrically satisfying
- should remain the most repeatable feedback loop in the game

### Score and near-miss feedback

- short, clear, rewarding, and not spammy
- should support skill and motion without crowding the next decision

### Milestone feedback

- should feel like city power routing, district recovery, or surge progression
- should stay short and non-blocking

### Death feedback

- should feel like power loss, overload, interruption, or district failure
- must remain sharp, readable, and retry-friendly

### Menu and UI feedback

- softer than gameplay cues but still inside the same city-electric palette
- should make the shell feel like the same product as the run

### Mix and restraint

- gameplay cue clarity stays above ambience and music
- keep city-electric flavor without harsh fatigue
- prefer short premium bursts over layered noise

## 15. Production quality gates

Before the game can be treated as production-ready under the Live Wire City target, all of these must pass:

- target-alignment gate: the live product clearly matches `20`
- gameplay readability gate: player, line, hazards, and world remain readable at speed
- fairness gate: presentation changes do not damage collision honesty or decision clarity
- UI clarity gate: Play and Retry remain dominant and safe-area correct
- feedback cohesion gate: audio and VFX feel like the same city-electric product as the visuals
- branding-safety gate: public branding can still change later without structural refactor
- config and extensibility gate: branding, copy, progression, and presentation assets are owned cleanly enough to evolve safely
- consistency gate: no major screen or state still looks like a prototype beside polished ones
- performance gate: city motion, VFX, and audio stay within mobile budgets
- migration-completeness gate: legacy Neon Night / Candy Pop assumptions no longer leak into the shipped Live Wire City product

## 16. Risks and anti-patterns

- half-migrating to `20`
- mixing old Neon Night / Candy Pop assumptions with Live Wire City surfaces
- keeping whole-theme swap thinking when the target is one evolving city world
- leaving legacy hazard-model assumptions in place after target-state migration starts
- hardcoding branding, subtitle, result copy, or milestone copy during polish work
- over-designing visuals until hazards and the lane lose readability
- making the menu shell heavier than the gameplay loop needs
- exposing future multi-theme UI before it is actually shipping
- adding optional extras before the core product shell is finished
- failing to update owning source-of-truth docs when implementation lands new target-state facts
- treating older docs as permanent blockers instead of baseline starting points during migration

## 17. Agent execution guidance

Future agents should:

- treat `.ai/20-live-wire-city-production-style-guide.md` as the final creative and product target
- treat `.ai/19-presentation-refresh-readiness-roadmap.md` as completed readiness groundwork
- treat older docs and current repo behavior as the baseline to migrate from, not the final destination
- follow phases in order
- do not skip prerequisite phases because some assets already exist
- do not skip quality gates once implementation begins
- keep one-tap clarity, fairness, and instant retry speed intact at every phase
- prefer config-driven, token-driven, and extensible implementations
- keep the first release focused on one progressive Live Wire City theme
- keep optional extras out unless they are formally promoted
- update owning source-of-truth docs whenever implementation crosses from the old baseline into the new target-state model
- avoid speculative systems unless clearly justified by the roadmap

## 18. Maintenance protocol

Update this roadmap when:

- `.ai/20-live-wire-city-production-style-guide.md` changes materially
- the readiness foundation in `19` changes materially
- first-release scope changes
- branding strategy changes
- world progression strategy changes
- hazard target-state changes
- phase boundaries, quality gates, or current-state assessment become outdated

Do not update this roadmap for routine asset additions alone.

Update it when the roadmap itself, the target-state interpretation, or the migration path changes.







