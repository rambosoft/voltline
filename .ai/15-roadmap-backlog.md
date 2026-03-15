# Roadmap and Backlog
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: prioritized future work, polish backlog, technical debt, blocked items, rejected ideas
Depends on: 01-product-vision.md, 15-roadmap-backlog.md
Do not duplicate with: source-of-truth gameplay or architecture docs

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file holds future work without polluting the source-of-truth docs with half-decided implementation details.

Use it for priority control, not truth ownership.

## Priority philosophy

Prioritize in this order:

1. core game feel
2. fairness/readability
3. retry loop polish
4. screenshot/video appeal
5. device performance and stability
6. simple replayable extras
7. content expansion
8. optional meta systems

## Now

These are the highest-priority items for the next serious playable.

### Final release validation
- run `Tools > Voltline > Validate Config`
- run `Tools > Voltline > Run Release Audit`
- run the full Unity Edit Mode and Play Mode suites in editor
- validate no console noise in menu/gameplay/retry/theme-selection flows
- validate safe-area, persistence, and feel flows on representative mobile hardware
- measure startup, common gameplay frame pacing, and build size on representative devices before final candidate approval

### Release-candidate blockers
- run `Tools > Voltline > Ensure Audio Mixer` and confirm the final `Music`, `SFX`, `Gameplay`, and `UI` routing before final release-candidate sign-off
- replace the first-pass procedural audio clips and procedural VFX with authored assets if playtest fatigue or release polish review shows the current pass is insufficient

## Next

These items should happen after the current build is stable and validated.

### Publishing prep
- perform clean-install and relaunch checks on candidate builds
- finalize store copy, icons, screenshots, and submission metadata outside gameplay scope
- verify release-safe debug posture in a non-development build

### Presentation readiness continuation
- establish background presentation architecture and a strict performance budget before any gameplay background refresh
- expand the theme system beyond the current color-oriented model before any broad theme art rollout

## Later

Only pursue after the game already feels strong.

### Optional features
- daily challenge seed mode
- ghost/replay-style share image
- cosmetic themes/skins expansion
- best score history view
- lightweight leaderboard page outside the game app

### Technical options
- object pooling if profiling justifies it
- Addressables if content scale grows
- analytics/event instrumentation if release strategy requires it

## Technical debt backlog

Track debt explicitly. Initial likely areas:

- refine the current curved line/path presentation only if playtesting shows readability or performance issues
- add lightweight automated safe-area coverage only if the Unity test harness or device setup justifies it

## Blocked ideas

These are not rejected forever, but blocked until core loop quality is proven.

- multiple game modes
- long progression trees
- power-up systems
- story wrapper
- backend-dependent live events
- broad cosmetic economy
- elaborate achievements UI

## Rejected ideas

These do not fit current product direction.

- turning gameplay into a direct Flappy Bird clone
- adding a second core action
- menu-heavy onboarding
- visually noisy effects that obscure danger
- ad-spam-first UX
- building the gameplay runtime in a web app framework
- overcomplicated live-service architecture for first release

## Decision triggers

Move an item from Later to Next only when one of these is true:

- core loop is repeatedly fun in playtesting
- current build meets fairness and retry quality bar
- the feature strengthens identity instead of distracting from it
- implementation cost is justified by player value

## Notes for future prioritization

A feature can be exciting and still be wrong for the project right now.  
This backlog should reward discipline.

Questions to ask before promoting an item:

- does it improve the retry loop?
- does it improve readability?
- does it improve feel?
- does it improve screenshot appeal?
- does it avoid new complexity debt?

## Non-negotiables

- Core feel and fairness come before expansion.
- Polish beats feature count.
- Roadmap notes do not override source-of-truth docs.
- Blocked items stay blocked until the core loop earns complexity.
- Rejected ideas should not quietly re-enter implementation through side doors.





