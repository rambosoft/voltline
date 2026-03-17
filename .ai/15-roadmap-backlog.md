# Roadmap and Backlog
Status: Active
Owner: Team
Last updated: 2026-03-17
Source of truth for: prioritized future work, polish backlog, technical debt, blocked items, rejected ideas
Depends on: 01-product-vision.md, 15-roadmap-backlog.md
Do not duplicate with: source-of-truth gameplay or architecture docs

> Working title: **Voltline**  
> Current public-facing release title: **Voltline**  
> Primary production theme direction: **Live Wire City**

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

These are the highest-priority items after the completed Live Wire City Phase 3 to 8 rollout, including the promoted Phase 7 optional surfaces.

### Final release validation
- run Tools > Voltline > Validate Config
- run Tools > Voltline > Run Release Audit
- run Tools > Voltline > Run Presentation Readiness Audit
- run Tools > Voltline > Run Presentation Refresh Approval Audit
- run the full Unity Edit Mode and Play Mode suites in editor
- validate no console noise in menu/gameplay/retry/world-progression flows
- validate safe-area, persistence, and feel flows on representative mobile hardware
- measure startup, common gameplay frame pacing, and build size on representative devices before final candidate approval

## Next

These items should happen after release-candidate validation is clean.

### Remaining optional extras
- daily challenge surface

### Final brand asset replacement
- replace placeholder-safe title treatment with final approved logo and font package when available

## Later

Only pursue after the game already feels strong.

### Technical and content options
- additional production themes after the single-theme Live Wire City release posture is stable
- Addressables if content scale grows
- analytics/event instrumentation if release strategy requires it

## Technical debt backlog

Track debt explicitly. Current likely areas:

- retire or archive legacy theme assets once Live Wire City fully replaces them
- replace placeholder-safe logo/text-only branding surfaces when final brand assets are approved
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
