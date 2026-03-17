# Project Index
Status: Active
Owner: Team
Last updated: 2026-03-16
Source of truth for: document authority, reading order, conflict resolution, project-wide update protocol
Depends on: 01-product-vision.md, 03-tech-stack.md, 04-architecture.md
Do not duplicate with: feature notes, ad-hoc implementation comments, backlog items

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Project in 5 lines

**Voltline** is a portrait mobile one-tap arcade game built around a single action: tap to flip the player from one side of a glowing line to the other.  
The line moves forward automatically. Hazards, gaps, cutters, and electric obstacles force split-second decisions.  
The game is designed to create a brutal but fair “one more try” loop through very short runs, immediate restarts, readable danger, and a score that always feels beatable by one more point.  
The visual direction is premium neon arcade: dark soft backgrounds, glowing line, cute-but-stylish character, clean HUD, and highly shareable near-death moments.  
The technical target is **Unity 6.x LTS**, **C#**, **URP 2D**, **Input System**, **Canvas UI**, **Particle System**, and **Audio Mixer**.

## What this `.ai` folder is for

This folder is the operating system for human and AI contributors.  
Its job is to make the project hard to misunderstand.  
It must answer:

- what experience the game protects
- how the codebase is structured
- how features are allowed to evolve
- how to avoid design drift, tech drift, and documentation drift
- what must be updated whenever the project changes

## Reading order for an AI agent

Read these in order before making any cross-cutting change:

1. `14-agent-rules.md`
2. `00-index.md`
3. `01-product-vision.md`
4. `02-game-design-pillars.md`
5. `03-tech-stack.md`
6. `04-architecture.md`
7. `05-project-structure.md`
8. `06-scene-flow.md`
9. `07-gameplay-systems.md`
10. `08-ui-ux-style-guide.md`
11. `09-audio-vfx-guide.md`
12. `10-data-content-model.md`
13. `11-testing-quality-bar.md`
14. `12-adrs.md`
15. `15-roadmap-backlog.md`
16. `16-production-phases.md`
17. `17-repo-alignment-audit.md`
18. `18-visual-refresh-and-theme-system-guide.md`
19. `19-presentation-refresh-readiness-roadmap.md`
20. `20-live-wire-city-production-style-guide.md`
21. `21-live-wire-city-production-roadmap.md`

Read `13-feature-spec-template.md` before proposing or implementing any new feature.

## Authority order

When documents disagree, follow this authority order:

1. `14-agent-rules.md`
2. `03-tech-stack.md`
3. `04-architecture.md`
4. `07-gameplay-systems.md`
5. `08-ui-ux-style-guide.md`
6. `09-audio-vfx-guide.md`
7. feature-specific specs created from `13-feature-spec-template.md`
8. `15-roadmap-backlog.md`
9. `16-production-phases.md`
10. `17-repo-alignment-audit.md`
11. `18-visual-refresh-and-theme-system-guide.md`
12. `19-presentation-refresh-readiness-roadmap.md`
13. `20-live-wire-city-production-style-guide.md`
14. `21-live-wire-city-production-roadmap.md`

If a lower-authority document conflicts with a higher-authority document, the higher one wins.

## Conflict resolution rules

When docs disagree:

1. Prefer **non-negotiables** over examples.
2. Prefer **exact names and explicit rules** over vague wording.
3. Prefer **system boundaries** over convenience.
4. Prefer **fairness, readability, and restart speed** over extra flair.
5. If the disagreement changes architecture, stack, or ownership, record it in `12-adrs.md`.
6. Do not silently “split the difference” by inventing a new hybrid rule.

## Stable project names

These names are intentionally stable even if the public game title changes later.

- Working title / codename: **Voltline**
- Core concept name: **STAY ON THE LINE**
- Root asset folder: `Assets/_Game`
- Primary runtime namespace: `Voltline`
- Primary runtime assembly: `Voltline.Runtime`
- Main scene names: `Bootstrap`, `MainMenu`, `Gameplay`

The public name can change later without forcing a repository rename.

## Document ownership map

- `01-product-vision.md` owns **why the game exists**
- `02-game-design-pillars.md` owns **the core play philosophy**
- `03-tech-stack.md` owns **approved technology**
- `04-architecture.md` owns **runtime boundaries and dependencies**
- `05-project-structure.md` owns **folders, assemblies, naming**
- `06-scene-flow.md` owns **scene model and transitions**
- `07-gameplay-systems.md` owns **actual gameplay rules**
- `08-ui-ux-style-guide.md` owns **screen behavior and visual interaction**
- `09-audio-vfx-guide.md` owns **feedback rules**
- `10-data-content-model.md` owns **config, content, and persistence**
- `11-testing-quality-bar.md` owns **done criteria and quality gates**
- `12-adrs.md` owns **major decisions and rationale**
- `13-feature-spec-template.md` owns **how new features are specified**
- `14-agent-rules.md` owns **implementation behavior for AI contributors**
- `15-roadmap-backlog.md` owns **future work and rejected ideas**
- `16-production-phases.md` owns **phased delivery from current repo state to release**
- `17-repo-alignment-audit.md` owns **current repo-to-doc alignment status and baseline remediation order**
- `18-visual-refresh-and-theme-system-guide.md` owns **visual refresh planning and theme-system evolution guidance**
- `19-presentation-refresh-readiness-roadmap.md` owns **strict readiness and gating before refresh implementation begins**
- `20-live-wire-city-production-style-guide.md` owns **the final Live Wire City presentation direction across UI, gameplay presentation, audio, VFX, and production polish targets**
- `21-live-wire-city-production-roadmap.md` owns **the phased production transition from the current repo state to a production-ready Live Wire City release presentation**

## Update protocol

When implementing a feature, update:

- `04-architecture.md` if system boundaries or responsibilities changed
- `05-project-structure.md` if folders, assemblies, namespaces, or naming changed
- `06-scene-flow.md` if scene ownership or transitions changed
- `07-gameplay-systems.md` if player behavior, rules, or tuning changed
- `08-ui-ux-style-guide.md` if HUD, menus, or interaction hierarchy changed
- `09-audio-vfx-guide.md` if any feedback event changed
- `10-data-content-model.md` if config assets, save schema, or IDs changed
- `11-testing-quality-bar.md` if test scope or acceptance criteria changed
- `12-adrs.md` if a cross-cutting technical or design decision changed
- `15-roadmap-backlog.md` if a planned item moved between Now / Next / Later

## Non-negotiables

- Gameplay remains **one-tap only**.
- Runs must become engaging within **seconds**, not minutes.
- Death-to-retry flow must feel **nearly instant**.
- Difficulty can be brutal, but never intentionally unreadable or unfair.
- Screenshots must remain readable at a glance.
- UI must stay minimal during gameplay.
- Tunable values must come from config, not scattered hardcoded scene values.
- The first release stays technically simple: no unnecessary backend dependency, no heavy live-service scope, no overbuilt content pipeline.

## What not to put in these docs

Do not put the following into source-of-truth docs:

- temporary scratch notes
- half-decided ideas
- meeting leftovers
- contradictory examples
- personal preference comments without a decision
- “we might do this” items that belong in backlog instead
- duplicate copies of the same rule in multiple docs

## Expected implementation behavior

A contributor is considered reliable when they:

- read the relevant source-of-truth docs first
- change the smallest system that solves the problem cleanly
- update the right docs in the same change
- avoid parallel systems
- preserve the one-tap, fast-retry identity of the game
- state risks and follow-ups clearly

## Definition of a healthy `.ai` system

This `.ai` folder is healthy when:

- every topic has one owner
- new contributors can infer the intended game from docs alone
- the same feature would be implemented similarly by two different agents
- docs stay short enough to be read, but concrete enough to constrain decisions
- cross-cutting changes are captured in ADRs instead of hidden in code

## Quick navigation

- Need the **feel** of the game? Read `01` and `02`.
- Need the **approved stack**? Read `03`.
- Need the **system map**? Read `04`.
- Need the **folder or namespace rule**? Read `05`.
- Need the **scene or flow rule**? Read `06`.
- Need the **actual gameplay rules**? Read `07`.
- Need the **screen/HUD/menu rules**? Read `08`.
- Need the **SFX/VFX/event mapping**? Read `09`.
- Need the **config or save-data model**? Read `10`.
- Need the **quality bar**? Read `11`.
- Need the **history of major decisions**? Read `12`.
- Need to spec a new feature? Start with `13`.
- Need to know how an AI agent should behave? Read `14`.
- Need future priorities? Read `15`.
- Need phased delivery order and release gates? Read `16`.
- Need the current repo alignment snapshot before baseline cleanup? Read `17`.
- Need the visual-refresh/theme-system implementation map? Read `18`.
- Need the pre-refresh gating roadmap before any presentation implementation starts? Read `19`.
- Need the Live Wire City creative production target? Read `20`.
- Need the execution roadmap from readiness-complete repo state to Live Wire City production quality? Read `21`.




