# Testing and Quality Bar
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: test philosophy, test scope, manual QA, performance budgets, device checks, acceptance criteria for done
Depends on: 03-tech-stack.md, 07-gameplay-systems.md, 10-data-content-model.md
Do not duplicate with: personal testing habits or undocumented checklists

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Quality philosophy

This project is small enough that quality problems will usually be obvious to players immediately.

That means testing must protect:

- fairness
- responsiveness
- restart speed
- readability
- stability
- performance on mobile

A feature is not done because it compiles.  
It is done when it preserves the feel of the game.

## Test categories

### 1. Edit Mode tests
Use for pure logic and data validation.

Examples:
- score calculations
- milestone threshold logic
- difficulty band selection
- save schema upgrade logic
- catalog validation
- theme/config integrity checks
- release-readiness audit checks

### 2. Play Mode tests
Use for runtime interaction and scene-level behavior.

Examples:
- scene bootstrap flow
- gameplay state transitions
- retry state reset
- pause/resume behavior
- score reset on restart
- persistent best-score behavior

### 3. Manual feel testing
Required for anything touch/feel-sensitive.

Examples:
- tap responsiveness
- visual clarity at speed
- near-miss emotional impact
- death readability
- result-panel timing
- menu-to-gameplay flow

## What must be covered by automated tests

At minimum, add automated tests for:

- run state transitions
- score increment and best-score persistence logic
- config validation
- save load/write/version migration behavior
- difficulty progression calculations
- theme ID and catalog integrity
- release-readiness audit checks for scene order, input asset integrity, and debug-safe defaults

## What must be validated manually

Always test manually for:

- actual tap feel
- collision fairness perception
- readability of hazards under real motion
- VFX clutter under stress
- sound balance after many retries
- restart emotional speed
- safe-area correctness on representative phone aspect ratios
- clean-install and relaunch behavior on representative devices before final candidate approval

## Smoke test checklist

Run this checklist before any significant merge/build:

1. App boots into menu correctly.
2. Play starts gameplay correctly.
3. Tap flips the player every time.
4. Score increments correctly.
5. Death occurs when expected.
6. Retry works immediately.
7. Home returns to menu safely.
8. Best score persists after app relaunch.
9. Music and SFX settings persist.
10. No missing references/errors spam in console for normal flows.

## Gameplay fairness checklist

Before shipping a new hazard pattern or tuning change, confirm:

- fail reason is visible
- reaction window is readable
- collision feels consistent with visuals
- near-miss feedback does not hide next danger
- the pattern still respects one-tap design
- the pattern does not create fake/impossible-feeling deaths

## UI checklist

- score visible in safe area
- pause icon reachable
- result panel prioritizes Retry
- buttons remain tappable on small phones
- no clipped text
- no overlapping panels
- menu remains understandable without reading a paragraph

## Audio checklist

- flip is always audible and satisfying
- score blip is not annoying under repetition
- near-miss is not spammy
- death sound is punchy and readable
- menu clicks are subtle
- music does not overpower gameplay SFX

## Performance targets

Primary target:
- **stable 60 FPS** on target mid-range mobile hardware

Secondary target:
- higher refresh support is welcome but not required for release

Working performance constraints:
- gameplay should not create noticeable hitches during common effects
- no avoidable per-frame allocations in hot gameplay paths
- VFX should remain lightweight
- restart should feel immediate without long stalls

## Responsiveness targets

These are critical for game feel:

- input-to-visible flip response: should feel immediate
- death-to-result reveal: fast and decisive
- death-to-retry availability: roughly around one second or better
- retry-to-active-control: should feel near-instant

## Device test matrix

Minimum practical matrix before release:

### Android
- one mid-range Android phone
- one lower-performance Android device if available
- one newer/high-refresh Android device if available

### iOS
- one recent iPhone
- one older supported iPhone if available

### Optional
- one tall-aspect device with aggressive safe area
- one tablet only if tablet support becomes an explicit goal

## Regression checklist

After any change to gameplay systems, verify:

- tap still flips only once per input
- score still increments only when expected
- best score still persists
- no hazard spawns in impossible intro window
- pause still freezes live progression safely
- retry still resets all transient run state
- theme changes do not reduce readability

## Release-blocking bugs

Treat these as release blockers:

- input drop/missed tap during normal play
- death that feels visually incorrect
- impossible obstacle pattern confirmed by testing
- retry taking too long or failing intermittently
- corrupted save or lost best score
- menu/gameplay soft lock
- severe frame hitch in common gameplay
- UI hidden behind notch/safe area

## Acceptance criteria for a feature

A feature is incomplete unless all are true:

1. implementation works in the intended flow
2. relevant docs are updated
3. relevant automated tests are added or updated
4. manual QA confirms feel/readability
5. no new console-error noise in normal flow
6. known risks are stated explicitly

## When to write tests first

Prefer writing tests first for:

- pure progression logic
- score logic
- save logic
- config validators
- scene routing logic

Do not overforce test-first on highly feel-driven VFX polish, but still validate outcomes.

## Build verification

Before creating a candidate mobile build:

- run `Tools > Voltline > Validate Config`
- run `Tools > Voltline > Run Release Audit`
- verify scene list order
- verify portrait orientation settings
- verify package/import sanity
- run Tools > Voltline > Ensure Audio Mixer if the mixer asset or groups are missing
- verify audio routing references and mixer setup
- verify input action asset included and functional
- verify debug-only restart/start-score helpers are not exposed unintentionally in release builds
- verify clean-install and relaunch behavior on representative devices

## Documentation rule

A feature is incomplete unless docs are updated when source-of-truth topics changed.

## Non-negotiables

- Fairness and responsiveness are core quality gates.
- Retry speed is a quality requirement, not a nice-to-have.
- Automated tests cover logic and persistence.
- Manual testing covers feel and readability.
- Mobile safe area and performance must be validated before release.
- No feature is done if docs and tests drift behind implementation.


