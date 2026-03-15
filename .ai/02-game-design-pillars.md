# Game Design Pillars
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: core gameplay philosophy, fairness rules, addiction loop rules, acceptable mechanic boundaries
Depends on: 01-product-vision.md
Do not duplicate with: 07-gameplay-systems.md

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose of this file

This file protects the game from mechanical drift.  
If a feature sounds exciting but weakens these pillars, it should not be added.

## Pillar 1: One tap, one meaning

The single tap always means the same thing:

> move the player from one side of the line to the other.

It does not become jump, dash, boost, interact, attack, hold, charge, or menu-confirm during active play.

Why this matters:

- keeps input readable
- keeps runs fast
- keeps the game explainable in one sentence
- increases mastery through timing rather than control complexity

### Allowed variations

- slightly different animation styles per theme
- subtle assist tuning through config
- alternate visual trails
- accessibility adjustments that do not change the meaning of tap

### Rejected variations

- hold to charge
- swipe for alternate moves
- two-tap chains
- context-sensitive tap behavior
- power-ups that change tap into another verb

## Pillar 2: Hard but fair

The game should be brutal, but the player must usually feel:

> “That was my mistake.”

Fairness means:

- hazards are readable early enough
- spacing is learnable
- visual telegraphs are consistent
- collision expectations match visuals
- deaths are deterministic from player decisions, not hidden randomness

### A player death is valid if…

- the player had enough readable information to react
- the rule causing death is consistent with earlier examples
- the collision shape matches what was shown
- the obstacle sequence respects the current difficulty rules
- the camera and effects did not hide the failure reason

### A player death is not valid if…

- danger appears too late without deliberate tutorialization
- collision is wider than the visible hazard with no cue
- screen effects obscure the actual fail reason
- the obstacle requires a second input that does not exist
- two systems combine into unreadable ambiguity

## Pillar 3: Instant retry

The loop after failure is part of the game design, not just UI behavior.

Desired result:

- death
- result recognition
- finger already over retry
- next run starts immediately

Anything that delays this loop reduces the game’s core strength.

### Rules

- no long death animations
- no forced summary pages before retry
- no multi-step confirmation for restart
- retry is visually primary
- result panel appears quickly and can be dismissed quickly

## Pillar 4: Readable danger

The player should understand the danger pattern at speed.

Readability comes from:

- strong silhouettes
- contrast between safe and dangerous space
- consistent hazard language
- disciplined color roles
- modest screen clutter
- stable camera behavior

### A mechanic is acceptable if…

- it can be recognized in under a fraction of a second
- it has a clear “safe side / unsafe side / timing” logic
- it can be introduced simply and remixed later

### A mechanic is rejected if…

- it needs paragraph-level explanation
- it adds a second spatial grammar unrelated to the line
- it makes screenshots visually messy
- it cannot be understood while the run is moving fast

## Pillar 5: Almost-made-it tension

The best feeling in this game is not domination.  
It is near-success.

Design for:

- late flips
- narrow escapes
- visible milestones just ahead
- score targets that feel barely out of reach
- death that happens after hope, not before understanding

This is the psychological engine behind “one more try.”

## Pillar 6: Small scope, rich polish

The game becomes memorable by polishing a tiny ruleset, not by adding many mechanics.

Prioritize:

- better feel
- better telegraphing
- better sound
- better particles
- better HUD
- better restart flow

Before prioritizing:

- more content types
- more meta systems
- more currencies
- more progression layers

## Difficulty philosophy

Difficulty should ramp in these dimensions:

1. speed
2. obstacle density
3. obstacle sequencing
4. side-switch timing pressure
5. mix complexity of known hazards

Difficulty should **not** ramp through:

- unreadable visual overload
- excessive randomness
- surprise rule changes
- invisible collision tricks
- inconsistent telegraphs

## Scoring philosophy

The score should feel:

- immediate
- clear
- fair
- always improvable

Scoring should create the feeling that one more point is close.  
The score system must never require math during play.

Preferred design:

- one visible main score
- score increments on passing designed challenge beats
- optional hidden telemetry can track near misses, clean chains, etc.
- optional flair events can celebrate skilled play without replacing the simple score

## Retry-loop philosophy

Every completed failure loop should reinforce:

- I understood what happened
- I know what to try differently
- restarting is effortless
- I want to beat my last number immediately

If the player feels confused instead, the loop is broken.

## Acceptable obstacle criteria

An obstacle or obstacle cluster is acceptable when it:

- tests timing or side choice cleanly
- is readable at current speed
- has clear danger silhouette
- supports fast failure comprehension
- works with the line-centered fantasy
- produces interesting screenshot/clip moments

## Unacceptable obstacle criteria

Reject an obstacle when it:

- requires stopping or slowing the forward flow
- demands multiple verbs
- makes the safe path unclear
- overlaps too many warning languages at once
- consumes too much screen space for too little decision value
- reads worse than it sounds on paper

## Near-miss philosophy

Near misses are a reward for skill and a generator of obsession.

They should:

- feel intentional
- trigger tiny but meaningful feedback
- never fake safety
- avoid excessive particle clutter

Near misses are not a scoring system by default.  
They are a tension-and-reward system first.

## Non-negotiables

- One tap always means one thing.
- Difficulty must remain fair before it becomes severe.
- Restart speed is a design pillar, not polish fluff.
- Danger readability beats decorative complexity.
- New mechanics must support the almost-made-it feeling.
- The game is improved by polish first, complexity second.
