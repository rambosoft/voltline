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

These are the highest-priority items for the first serious playable.

### Core playable
- implement one-tap side flip
- implement moving line/path
- implement at least 2 hazard families
- implement score, death, and retry loop
- implement base HUD and result panel

### Feel pass
- flip sound
- death burst
- score pop
- near-miss feedback
- main menu animated preview
- safe-area handling
- minimal settings overlay

## Next

These items should happen after the core loop is already fun.

### Content polish
- expand hazard set to approved initial family list
- improve line rendering and pulse
- add first theme unlock
- improve milestone feedback
- refine result messaging
- add pause overlay and settings persistence

### Testing / quality
- add wider Edit Mode coverage
- add Play Mode retry/regression tests
- profile particle/audio overlap on target devices
- validate build stability on Android and iOS hardware

### Product polish
- improve menu presentation
- refine logo/title treatment
- improve screenshot moments
- tighten safe-start difficulty bands

## Later

Only pursue after the game already feels strong.

### Optional features
- daily challenge seed mode
- ghost/replay-style share image
- cosmetic themes/skins expansion
- theme unlock progression
- best score history view
- lightweight leaderboard page outside the game app

### Technical options
- object pooling if profiling justifies it
- Addressables if content scale grows
- analytics/event instrumentation if release strategy requires it

## Technical debt backlog

Track debt explicitly. Initial likely areas:

- if first line/path implementation is visually simple, refine it later without changing mechanics
- if first restart path uses scene reload, consider in-scene reset only if speed profiling justifies the complexity
- unify any duplicated telegraph logic once hazard set stabilizes
- replace temporary debug UI with proper dev tooling if needed

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