# Feature Spec Template
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: standard structure for proposing and documenting new features
Depends on: 00-index.md, 14-agent-rules.md
Do not duplicate with: backlog notes, raw brainstorm lists

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

Every new meaningful feature should be specified in a consistent format before implementation, unless it is an obviously tiny local fix.

Use this template to reduce ambiguity and stop feature sprawl.

---

# Feature Name

## Status
Draft / Approved / In Progress / Done / Rejected

## Owner
Team / AI / Human

## Date
YYYY-MM-DD

## Summary
One short paragraph explaining the feature in plain language.

## Player Goal
What the player is trying to do or feel.

## User Story
As a player, I want to ______ so that ______.

## Design Intent
Why this feature exists and what problem it solves.

## Non-goals
What this feature must not become.

## Dependencies
Which existing systems, scenes, or configs it depends on.

Examples:
- `Gameplay`
- `ScoreSystem`
- `ThemeCatalog`
- `SettingsOverlay`
- `AudioCueCatalog`

## Impacted Docs
List the source-of-truth docs that must be updated if the feature is implemented.

Examples:
- `07-gameplay-systems.md`
- `08-ui-ux-style-guide.md`
- `09-audio-vfx-guide.md`
- `10-data-content-model.md`

## Impacted Systems
List runtime systems likely to change.

Examples:
- `GameManager`
- `PlayerController`
- `SaveService`
- `HudView`

## UX / UI Changes
Describe visible screen or interaction changes.

## Audio / VFX Changes
Describe event feedback changes.

## Data / Config Changes
List:
- new config assets
- new save fields
- new IDs
- new catalogs or catalog entries

## Technical Constraints
List implementation constraints.

Examples:
- must remain one-tap only
- must not add a new scene
- must remain mobile-safe
- must keep retry flow fast

## Acceptance Criteria
Use a checklist with concrete outcomes.

Examples:
- player can access the feature from menu
- feature works in gameplay
- state persists correctly
- relevant docs updated
- tests added/updated

## Test Plan
### Automated
List Edit Mode / Play Mode tests to add or update.

### Manual
List feel/readability/device checks.

## Risks
List likely failure points or trade-offs.

## Rollback Plan
How to disable or remove the feature safely if it underperforms.

## ADR Requirement
State whether this feature requires a new ADR and why.

## Open Questions
List unresolved questions clearly.

## Implementation Notes
Optional practical notes only after the design is approved.

---

## Template usage rules

- Fill this out before implementation for any non-trivial feature.
- Keep the summary concrete.
- State non-goals explicitly.
- Do not hide save-schema changes.
- Do not add new packages without referencing `03-tech-stack.md`.
- If the feature changes core player behavior, update `07-gameplay-systems.md`.
- If the feature changes screen hierarchy, update `08-ui-ux-style-guide.md`.
- If the feature changes feedback events, update `09-audio-vfx-guide.md`.
- If the feature changes content/config model, update `10-data-content-model.md`.
