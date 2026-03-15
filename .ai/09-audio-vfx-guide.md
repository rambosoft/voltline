# Audio and VFX Guide
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: sound philosophy, music philosophy, mixer routing, event-to-sound mapping, event-to-VFX mapping, budgets and restrictions
Depends on: 01-product-vision.md, 07-gameplay-systems.md, 08-ui-ux-style-guide.md
Do not duplicate with: prototype one-off effect experiments

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## Purpose

This file defines how the game should **feel** moment to moment through sound and visual feedback.

The goal is not maximal spectacle.  
The goal is **high-value clarity and satisfaction**.

## Feedback philosophy

Every major gameplay event should answer one of these:

- help readability
- reward precision
- intensify tension
- make failure feel fair
- make success feel satisfying

If an audio/VFX idea does none of those, cut it.

## Audio direction

Audio style keywords:

- clean
- arcade
- soft synth
- cute digital
- punchy
- premium
- not noisy
- not harsh
- not exhausting over many retries

## Music direction

Music should feel:

- energetic
- modern
- lightly tense
- loopable
- non-annoying after many failures

### Approved music shape
- short looping track
- clean rhythm bed
- subtle melodic identity
- possible intensity layering later, but not required for first release

### Music constraints
- no hyper-busy lead that competes with moment-to-moment SFX
- no sad/depressing death mood
- no harsh EDM overload for first release

## Mixer routing

Required top-level mixer groups:

- `Music`
- `SFX/Gameplay`
- `SFX/UI`

Optional later groups:

- `SFX/Ambience`
- `SFX/Voice` if ever needed

Rules:

- every clip routes intentionally
- UI clicks should never be as loud as death or milestone events
- gameplay clarity beats music fullness

## Core sound event map

### 1. Flip
**Purpose:** make the main input addictive and responsive  
**Sound character:** short tick-pop / zip / click  
**Rules:**
- extremely fast attack
- very short tail
- allow slight pitch variation
- must never feel dull

### 2. Score increase
**Purpose:** confirm survival success  
**Sound character:** soft upward blip  
**Rules:**
- pleasant
- short
- should not overpower flip
- can rise subtly if chained tastefully

### 3. Near miss
**Purpose:** reward precision and create tension memory  
**Sound character:** airy shing / sharp shimmer  
**Rules:**
- only on real narrow escape
- do not spam
- should feel special, not constant

### 4. Milestone
**Purpose:** celebrate progress without slowing the game down  
**Sound character:** bright synth chime  
**Rules:**
- cleaner and fuller than score blip
- short enough not to interrupt next action

### 5. Death
**Purpose:** make failure feel final, fair, and motivating  
**Sound character:** pop + glitch + soft bass hit  
**Rules:**
- punchy, not depressing
- fast read of failure
- no long sad sting

### 6. Menu click
**Purpose:** polished interface feedback  
**Sound character:** soft rounded tap  
**Rules:**
- subtle
- should not compete with gameplay SFX

### 7. Unlock / reward
**Purpose:** reward cosmetics or milestones  
**Sound character:** sparkle / bright success ping  
**Rules:**
- cheerful
- used sparingly

## Core VFX event map

### 1. Flip spark
**Trigger:** player flips sides  
**Effect package:**
- small burst
- tiny ring pulse
- short trail streak

**Purpose:** make tap feel alive

### 2. Near-miss shine
**Trigger:** real narrow escape  
**Effect package:**
- tiny white spark or line shine
- optional screen pulse
- very restrained

**Purpose:** reinforce skill without clutter

### 3. Score pop
**Trigger:** score increases  
**Effect package:**
- score scale pulse
- tiny sparkle around number or subtle HUD emphasis

**Purpose:** reward beat clearance

### 4. Milestone burst
**Trigger:** 10 / 20 / 30 etc.  
**Effect package:**
- ring pulse from player
- small celebratory particles
- brief UI emphasis

**Purpose:** make thresholds feel memorable

### 5. Death burst
**Trigger:** fail state  
**Effect package:**
- player fragments / glow breakup
- short shockwave
- one quick screen shake

**Purpose:** give strong closure and support instant retry

### 6. Gap edge particles
**Trigger:** broken line segments visible  
**Effect package:**
- small glowing dust on torn edges

**Purpose:** improve readability and style

### 7. Electric arc effect
**Trigger:** electric gate active / charging  
**Effect package:**
- tiny animated sparks between poles
- optional warning flicker before activation

**Purpose:** telegraph danger clearly

## Camera feedback policy

Approved camera feedback:

- subtle zoom emphasis in dangerous clusters
- one short shake on death
- mild pulse or impulse on milestone if tested positively

Disallowed for first release:

- constant camera shake
- dramatic zoom swings that reduce readability
- effects that make the line difficult to track

## Loudness / overlap rules

- flip must remain clearly audible even late into a run
- death must cut through the mix without clipping
- score sounds should stay light
- milestone should feel notable but short
- near-miss should not overlap so often that it loses meaning

## Sound concurrency rules

- avoid stacking many identical sounds on the same frame
- gate or limit repeated UI clicks
- near-miss should have a cooldown window per obstacle event
- gameplay one-shots should remain clean during rapid retries

## VFX budget rules

This is a high-readability arcade game, not a particle showcase.

Rules:

- particle counts stay low
- effect lifetimes stay short
- avoid large persistent emitters near active hazards
- do not cover the player during key decision windows
- every effect should be identifiable in one glance

## Suggested effect prefab list

### Required first-release prefabs
- `PFB_VFX_FlipSpark`
- `PFB_VFX_NearMiss`
- `PFB_VFX_DeathBurst`
- `PFB_VFX_MilestoneBurst`
- `PFB_VFX_LinePulse`
- `PFB_VFX_GapDust`
- `PFB_VFX_ElectricArc`

### Optional later prefabs
- `PFB_VFX_PerfectDodgeText`
- `PFB_VFX_ThemeUnlockBurst`
- `PFB_VFX_GhostTrailBoost`

## Suggested sound asset list

### Required first-release clips
- `SFX_Flip_01`
- `SFX_Score_01`
- `SFX_NearMiss_01`
- `SFX_Milestone_01`
- `SFX_Death_01`
- `SFX_UI_Click_01`
- `MUS_MainLoop_01`

### Optional variation set
- alternate flip pitches
- alternate score blips
- second death variation
- reward sparkle variant

## Implementation rules

- gameplay code requests semantic cues, not raw asset paths
- audio lookup should happen via catalog/service
- VFX spawning should happen via catalog/service
- feedback tuning must be centralized enough to rebalance quickly
- avoid duplicating cue definitions across many prefabs

## When not to play feedback

Do not play full feedback when:

- game is paused
- result panel UI button is repeating due to held input
- near-miss event repeats from the same object in an unreadable way
- death has already transitioned fully to results and duplicate death effects would be confusing

## First-release audio/VFX priority order

Build first:

1. flip sound
2. death burst + death sound
3. score pop
4. near-miss effect
5. main loop music
6. milestone burst
7. menu click polish

## Non-negotiables

- Flip feedback must feel great.
- Death feedback must feel fair and conclusive.
- Near-miss feedback must stay meaningful, not spammy.
- Audio routing must stay organized through mixer groups.
- VFX must support readability, never overpower it.
- Feedback implementation should remain data-driven and reusable.
