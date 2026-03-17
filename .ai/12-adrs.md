# Architecture Decision Records
Status: Active
Owner: Team
Last updated: 2026-03-16
Source of truth for: major cross-cutting decisions, rationale, alternatives considered, consequences
Depends on: 00-index.md
Do not duplicate with: feature-specific implementation details

> Working title: **Voltline**  
> Current public-facing release title: **Voltline**  
> Primary production theme direction: **Live Wire City**

## How to use this file

Add a new ADR when a decision changes:

- engine or package strategy
- scene model
- architectural boundaries
- content loading strategy
- save strategy
- major UI/audio/gameplay infrastructure choices

Format to follow:

- ID
- Date
- Status
- Decision
- Context
- Alternatives considered
- Consequences

---

## ADR-001
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Build the game in Unity, not React/Next as the gameplay runtime.

**Context:**  
The project is a portrait mobile 2D arcade game that needs one-tap gameplay, mobile builds, particles, clean UI, touch input, and audio feedback inside one production workflow.

**Alternatives considered:**  
- React/Next as primary runtime
- Web-first gameplay shell with custom canvas loop

**Consequences:**  
- simpler mobile-focused production path
- native fit for sprites, touch, particles, audio, and store release
- React/Next remains appropriate only for website, leaderboard page, promo page, or community features later

---

## ADR-002
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use Unity 6.x LTS as the engine line.

**Context:**  
The project benefits from a stable, supported modern Unity line rather than short-lived experimental version hopping.

**Alternatives considered:**  
- older Unity LTS line
- preview/non-LTS line

**Consequences:**  
- prioritize editor/package stability
- engine upgrades become deliberate decisions
- tech docs must stay aligned with the selected LTS line

---

## ADR-003
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use URP with a 2D Renderer.

**Context:**  
The desired look is premium neon 2D with controlled glow/light support and mobile-friendly rendering.

**Alternatives considered:**  
- built-in render pipeline
- HDRP
- highly custom rendering path

**Consequences:**  
- strong fit for 2D lighting workflows
- supports a polished look without overbuilding the render stack
- rendering setup must stay disciplined for readability and performance

---

## ADR-004
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use the Input System as the project input standard.

**Context:**  
The game needs touch-first action-based input with clean mapping for mobile and editor testing.

**Alternatives considered:**  
- legacy Input Manager
- raw device polling spread through gameplay scripts

**Consequences:**  
- action asset becomes source of truth for input actions
- gameplay consumes abstract actions
- input handling remains centralized and easier to test

---

## ADR-005
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use Canvas-based UI with TextMeshPro for gameplay and menu UI.

**Context:**  
The UI needs are screen-based, mobile-friendly, and modest in scope: menu, HUD, result panel, settings.

**Alternatives considered:**  
- UI Toolkit as the core gameplay UI
- custom runtime UI framework

**Consequences:**  
- straightforward authoring for mobile scenes and overlays
- safe-area and overlay behavior remain simple
- gameplay UI should stay lean and not take on gameplay ownership

---

## ADR-006
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use three production scenes: `Bootstrap`, `MainMenu`, `Gameplay`.

**Context:**  
The game needs clean app startup, separate menu ownership, and isolated gameplay state without scene sprawl.

**Alternatives considered:**  
- one giant scene for everything
- many small scenes including a separate results scene

**Consequences:**  
- simple scene model
- result panel stays in `Gameplay`
- retry flow stays fast without routing through menu

---

## ADR-007
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Use ScriptableObjects for static config and catalogs.

**Context:**  
The project needs tuneable values, theme data, obstacle definitions, and audio/VFX lookup catalogs without scene-value sprawl.

**Alternatives considered:**  
- hardcoded constants across scripts
- scene-only inspector tuning
- save-data misuse as static config

**Consequences:**  
- balancing becomes cleaner
- config can be validated
- runtime save state remains separate from authored config

---

## ADR-008
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Organize code with explicit `.asmdef` assemblies.

**Context:**  
Even a small arcade game becomes harder to maintain if runtime, editor, and tests blur together.

**Alternatives considered:**  
- default giant Assembly-CSharp workflow

**Consequences:**  
- clearer dependency boundaries
- cleaner test separation
- better long-term maintainability

---

## ADR-009
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Keep the first release one-tap only with no alternate gameplay verbs.

**Context:**  
The core identity of the game depends on instant understanding and timing mastery from a single repeated action.

**Alternatives considered:**  
- adding hold, dash, jump, or context-sensitive abilities

**Consequences:**  
- stronger clarity
- easier balancing
- future features must not redefine the tap meaning

---

## ADR-010
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Keep the initial release scope small and polish-driven.

**Context:**  
The gameâ€™s value comes from feel, fairness, and style, not system count.

**Alternatives considered:**  
- progression-heavy design
- many game modes up front
- backend-reliant release

**Consequences:**  
- prioritize core loop polish
- defer complexity until retention-worthy feel is proven
- roadmap remains discipline-focused

---

## ADR-011
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Do not use `Resources` as the default content-loading pattern.

**Context:**  
The project aims to keep asset ownership, config, and references explicit and maintainable.

**Alternatives considered:**  
- broad `Resources`-based loading

**Consequences:**  
- asset references stay visible and structured
- exceptions require documented justification
- future loading strategies can stay intentional

---

## ADR-012
**Date:** 2026-03-15  
**Status:** Accepted  
**Decision:** Do not require Addressables for the first release.

**Context:**  
The first release is intentionally small and does not need remote or large-scale asset loading by default.

**Alternatives considered:**  
- adopting Addressables from day one

**Consequences:**  
- lower setup complexity
- faster initial implementation
- Addressables remain available later if content scale justifies them
---

## ADR-013
**Date:** 2026-03-16  
**Status:** Accepted  
**Decision:** Ship first-release presentation as one surfaced `theme.live-wire-city` base theme with config-driven world progression and six Live Wire City hazard families.

**Context:**  
The Live Wire City production direction became the target-state source of truth. The project had already completed refresh-readiness work, but still reflected a multi-theme milestone-swap baseline and an older five-family hazard model. Reaching the final target required a deliberate migration rather than treating older docs as permanent blockers.

**Alternatives considered:**  
- continue shipping whole-theme milestone swapping through `ThemeSequenceConfig`
- keep the older five-family hazard baseline and treat doc-20 families as visual aliases only
- surface multiple production themes in the first release shell

**Consequences:**  
- `ThemeSequenceConfig` remains only as a dormant compatibility asset in the first-release shipping posture
- `WorldProgressionConfig` and `WorldProgressionController` become the shipping runtime progression path
- the first-release menu shell keeps one surfaced production theme while leaving later multi-theme support structurally possible
- hazard catalogs, obstacle configs, validation, and tests now align to six Live Wire City hazard families
- future implementation must keep docs updated when remaining legacy assumptions are retired

