# Agent Rules
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: implementation behavior, documentation update rules, change boundaries, output format, when to create ADRs, when to refuse drift
Depends on: 00-index.md, all source-of-truth docs
Do not duplicate with: casual coding preferences, hidden contributor assumptions

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file defines how an AI contributor must behave on this project.

Its goal is consistency, not creativity for its own sake.

## Prime directive

Protect the game’s identity:

- one-tap only
- hard but fair
- instant retry
- premium but simple
- readable danger
- polished feedback
- small maintainable scope

## Required working order

Before changing anything substantial:

1. identify which source-of-truth docs are relevant
2. read them
3. restate the change in terms of the existing architecture
4. implement the smallest clean change
5. update docs if ownership topics changed
6. update tests
7. report risks/follow-ups

## Coding principles

- prefer clarity over cleverness
- prefer explicit ownership over convenience
- prefer extending an existing coherent system over creating a parallel one
- prefer data-driven tuning over scattered constants
- prefer readable names over short names
- prefer small classes with one responsibility

## Implementation rules

### Rule 1: Do not change the core verb
Do not introduce a new gameplay verb without explicit approval and documentation updates.

### Rule 2: Do not add stack drift
Do not introduce new packages, frameworks, or major tech paths without updating `03-tech-stack.md`.

### Rule 3: Do not create architecture drift
Do not add a new major runtime system without updating `04-architecture.md`.

### Rule 4: Do not create folder drift
Do not create new top-level repository folders without updating `05-project-structure.md`.

### Rule 5: Do not create scene drift
Do not add a production scene without updating `06-scene-flow.md` and likely `12-adrs.md`.

### Rule 6: Do not change gameplay silently
If player behavior, score rules, fail rules, or difficulty rules change, update `07-gameplay-systems.md`.

### Rule 7: Do not change UI silently
If HUD/menu/result/settings behavior changes, update `08-ui-ux-style-guide.md`.

### Rule 8: Do not change feedback silently
If sound or VFX event behavior changes, update `09-audio-vfx-guide.md`.

### Rule 9: Do not hide data changes
If new config assets, IDs, or save fields are introduced, update `10-data-content-model.md`.

### Rule 10: Do not ship undocumented quality changes
If acceptance criteria or testing expectations shift, update `11-testing-quality-bar.md`.

## Non-negotiable implementation constraints

- gameplay remains one-tap only
- UI stays lightweight during play
- retry stays central and fast
- fairness beats novelty
- readability beats effect density
- config beats scene-value sprawl
- no duplicate managers/services solving the same problem

## Preferred implementation order

When building a feature:

1. data/config model
2. architecture boundary
3. runtime logic
4. UI binding
5. audio/VFX integration
6. tests
7. docs cleanup
8. polish pass

This order keeps the core rule system clean before presentation details pile on.

## When an ADR is required

Create or update an ADR when:

- engine version strategy changes
- scene model changes
- package strategy changes
- save strategy changes
- rendering/input/UI foundation changes
- new major service or subsystem is introduced
- one of the project non-negotiables changes

## When to refuse or escalate

Refuse or flag a request when it would:

- add a second gameplay verb
- make deaths intentionally unfair
- clutter the gameplay HUD
- weaken retry speed
- add backend dependency to core play unnecessarily
- bypass config and hardcode cross-cutting values everywhere
- create multiple competing systems for the same responsibility
- introduce undocumented top-level architecture changes

## Documentation update rules

When implementing, update docs in the same work unit whenever source-of-truth topics changed.

Do not leave documentation “for later” if the change affects project truth.

## Testing rules

- add or update automated tests for logic and persistence changes
- manually verify feel-sensitive changes
- do not claim a change is complete without a test story
- state what was not tested if anything remains uncertain

## Naming rules

- use exact documented scene names
- use existing namespace roots
- use stable IDs for config-backed content
- avoid vague temp names
- use semantic cue/effect names, not “newSound” / “effect2”

## Temporary code rules

If a temporary shortcut is introduced:

- label it clearly
- isolate it
- add a backlog follow-up
- do not let “temporary” become silent architecture

## Required change summary format

For any meaningful implementation, the agent should report:

### What changed
Concrete summary of the implementation.

### Why
How it supports the documented goals.

### Files touched
Key code/config/doc files changed.

### Docs updated
Which source-of-truth docs were updated.

### Tests
What automated/manual validation was done.

### Risks
What remains risky or unverified.

### Follow-ups
Any intentionally deferred cleanup or polish.

## Practical behavior examples

### Good behavior
- extends `DifficultyCurveConfig` instead of hardcoding new thresholds
- updates `07` after changing near-miss logic
- reuses `AudioService` instead of adding another sound manager
- keeps the Retry button primary after adjusting result flow

### Bad behavior
- adds swipe input because it “feels modern”
- adds a new scene for results with no documented reason
- hardcodes theme colors inside UI prefab scripts
- creates `MegaGameManager` that owns everything
- modifies save schema without documenting it

## Final delivery rule

A task is not complete when the code exists.  
It is complete when code, data, docs, tests, and reported risks are aligned.

## Non-negotiables

- Protect identity over novelty.
- Use the documented stack and structure.
- Update the right docs in the same change.
- Keep systems small and explicit.
- Preserve fairness, clarity, and retry speed.
- Never solve a local problem by creating global chaos.
