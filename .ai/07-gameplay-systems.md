# Gameplay Systems
Status: Active
Owner: Team
Last updated: 2026-03-16
Source of truth for: concrete gameplay rules, state machine, input behavior, hazard rules, score rules, fail conditions, tuning defaults
Depends on: 02-game-design-pillars.md, 04-architecture.md, 10-data-content-model.md
Do not duplicate with: prototype notes, temporary tuning spreadsheets

> Working title: **Voltline**  
> Current public-facing release title: **Voltline**  
> Primary production theme direction: **Live Wire City**

## Gameplay summary

The player rides along a forward-moving line.  
The only active gameplay input is **Tap**, which flips the character from one side of the line to the other.

The run is short, escalating, and harsh.  
The player survives by reading upcoming hazards and switching sides at the last possible safe moment.

## Core verbs

### Primary verb
- **Flip sides**

### Passive system verbs
- move forward automatically
- clear challenge beats
- score
- die
- retry

There are no combat verbs, no charge verbs, and no inventory verbs in the first release.

## Run loop

1. start run
2. line advances automatically
3. player taps to flip
4. hazards/gaps pressure timing
5. score increases as challenge beats are cleared
6. a mistake causes immediate death
7. result panel appears quickly
8. retry starts another run immediately

## Player model

### Player position logic

The player is attached to the lineâ€™s local frame.

The playerâ€™s world position is determined by:

- current distance/progress on the line
- line tangent
- line normal
- current side value (`Top` or `Bottom` relative to the line)
- configured side offset distance

This keeps the character visually â€œridingâ€ the line rather than floating independently in space.

### Player side state

Approved states:

- `Top`
- `Bottom`
- `Flipping`
- `Dead`

During `Flipping`, the transition should be extremely short and readable.

## Input rules

### Active gameplay input
- `Tap`

### Input interpretation
When gameplay is active and the player is not dead:

- a valid tap requests a side flip
- if already in a non-interruptible flip window, the tap can be ignored or buffered based on final tuning decision
- the meaning of tap does not change

### Editor support
Mouse click may map to `Tap` for development.

## Flip mechanic rules

### Functional rule
A tap moves the player from the current side of the line to the opposite side.

### Feel rule
The flip should feel:

- immediate
- crisp
- tiny
- risky
- readable

### Visual rule
The flip should include:

- short movement arc or cross-line tween
- subtle squash/lean/rotation if useful
- tiny spark or pulse
- short audio tick/pop

### Timing rule
Recommended starting target:

- flip duration: **0.08s to 0.12s**

This is tunable via config.

### Buffering rule
Initial default:
- no deep input queue
- optionally allow a very small forgiveness buffer if testing shows it improves feel without reducing clarity

## Track and line rules

### Line behavior
The line is the main spatial anchor of the game.

It should:

- move forward continuously
- curve smoothly
- remain readable
- support local â€œtop/bottomâ€ interpretation at all times
- react lightly to flips and milestones

### Line visual rule
The line is not a flat placeholder.  
It is a glowing ribbon/wire/rail with subtle pulse.

### Gaps
The line may have broken segments.

Rules:
- gaps must be readable ahead of time
- edges must telegraph danger
- gap usage should escalate with difficulty
- gaps must never be hidden by large VFX

## Hazard families

Current first-release Live Wire City hazard families:

1. **Grounded blockers**
   - wood barriers, insulated clamp blocks, grounded metal plates
   - chunky blocking silhouette
   - read as current drain or route denial

2. **Sharp utility hazards**
   - exposed spikes, hooks, shard brackets, torn cable barbs
   - fastest aggressive read
   - rupture the transfer path visually and mechanically

3. **Active electric hazards**
   - shorted nodes, unstable arcs, crossing live wires, pulse gates
   - telegraphed warning then active danger state
   - overload the transfer path

4. **Rotating industrial hazards**
   - cutter discs, fan blades, maintenance rotors
   - readable circular motion pressure
   - mechanical city infrastructure gone wrong

5. **Broken conduit sections**
   - cable gaps, collapsed supports, fractured insulation zones
   - clearly missing path / broken line state
   - infrastructure failure and transfer interruption

6. **Side pressure hazards**
   - compressing blocks, side interrupters, moving armatures
   - heavy readable pressure from lane sides
   - dynamic side denial without hiding the line

These six families are the current gameplay-facing first-release baseline.

## Hazard introduction rules

Use progressive onboarding inside real play:

- score 0â€“3: simplest readable hazards only
- early score range introduces one idea at a time
- mixed patterns appear only after base patterns are understood
- high difficulty remixes known hazards rather than inventing unreadable new logic

## Hazard placement rules

Hazard placement must respect:

- reaction window at current speed
- current difficulty band
- safe intro spacing
- visual composition clarity
- no impossible transitions

### Placement constraints

- do not stack multiple hazards so tightly that one visual shape hides another
- avoid requiring two flips faster than the tested minimum human-readable window
- do not place a new teaching moment inside an already overloaded sequence

## Fail conditions

A run fails when any of the following occurs:

- player collides with an active hazard
- player occupies forbidden space during an electric gate activation window
- player enters a broken/gap fail state as defined by the line system
- future hazard types, if added, violate the same fair-read collision rules

Failure must be immediate and readable.

## Near-miss rules

A near miss occurs when the player clears danger within a configured narrow threshold.

Purpose:

- reinforce skill
- intensify tension
- produce tiny rewarding feedback

Rules:

- no fake near misses
- no near-miss spam from the same event
- no large screen clutter
- near-miss feedback must never hide the next obstacle

## Score rules

### Main score
One visible number.  
No complex during-run scoring math.

### Scoring event
The score increases when the player fully clears a defined challenge beat.

A challenge beat is usually:

- a hazard cluster
- a scoring gate positioned after a readable obstacle
- a danger segment designed to represent one â€œsurvival successâ€

### Score behavior
- increment by 1 per cleared beat by default
- pop visually on increase
- best score tracked persistently
- milestones at meaningful thresholds

### Best-score rule
Best score updates only on confirmed run-end or at safe persistence points, not constantly every frame.

## Milestones

Current first-release Live Wire City milestone thresholds:

- 10
- 20
- 30
- 40
- 50

Milestones should trigger:

- short celebratory UI emphasis
- mild VFX burst
- stronger emotional framing, not long interruption
- city-electric copy such as district recovery, grid stabilization, and full-charge messaging

## Live Wire City world progression

First-release presentation progression is one evolving city, not random whole-theme swapping.

District states:

- `0-9`: **Failing Grid**
- `10-19`: **Local Power Restored**
- `20-29`: **Grid Stabilization**
- `30-39`: **Surge City**
- `40+`: **Overclock City**

Rules:

- district state is driven by config-owned score bands
- milestone reactions layer on top of the current district state
- the base surfaced theme identity remains `theme.live-wire-city`
- progression should enrich the world without changing one-tap rules, fairness, or retry speed

## Difficulty ramp

Difficulty ramps through:

- forward speed increase
- obstacle density increase
- smaller safe windows
- more complex sequencing of known hazard types
- more demanding gap/timing combinations

### Difficulty must not ramp through

- invisible hazards
- unreadable camera tricks
- arbitrary rule changes
- excessive visual noise
- surprise off-screen danger

## State machine

### Run states

#### `Ready`
- scene loaded
- systems reset
- player ready
- minimal delay before play

#### `Starting`
- optional ultra-short settle/intro state
- no long countdown
- gameplay about to begin

#### `Active`
- line advances
- hazards live
- score can change
- input accepted

#### `Dying`
- fail event processed
- death VFX/audio played
- run motion halted or settled cleanly

#### `Results`
- result panel shown
- retry available immediately

#### `Restarting`
- systems reset and re-enter `Ready`/`Starting`

## Restart rules

Restart is one of the most important systems in the game.

Requirements:

- very fast
- predictable
- no heavy transitions
- preserve player urge to retry
- no unnecessary loading UI

### Implementation freedom
Allowed:
- in-scene run reset  
or
- scene reload if performance and feel remain excellent

The chosen path must meet the quality bar in `11-testing-quality-bar.md`.

## Daily challenge seed rules

If daily challenge exists:

- gameplay rules stay the same
- only seed/config differ
- score remains directly comparable within that seed
- daily seed must still respect fairness rules
- daily challenge should feel â€œtough but believable,â€ not random punishment

## Debug hooks

Allowed development/debug actions:

- restart run
- set seed
- set score start point
- force hazard family
- toggle invulnerability for layout debugging
- visualize near-miss threshold
- visualize collision bounds
- speed multiplier for testing

These must be gated out of release behavior appropriately.

## Numerical tuning defaults

These are initial targets, not immutable truths.

### Core feel defaults

- base forward speed: **6.0 units/sec**
- early speed ramp: **+0.12 units/sec per point** through early curve
- player side offset from line center: **0.82 units**
- flip duration: **0.09 sec**
- minimum intro safe window at run start: **1.20 sec**
- minimum readable telegraph window for new/active hazards: **0.75 sec**
- near-miss threshold: **0.18 units**
- result panel initial reveal target: **<= 0.60 sec after death**
- death-to-retry interaction availability target: **<= 1.00 sec**
- total restart loop target: **feels under 2 seconds end-to-end**

### Difficulty defaults

- first mixed hazard sequences allowed after score **5**
- first electric-gate sequences allowed after score **8**
- first tighter gap + hazard combos allowed after score **12**
- no â€œimpossible-feelingâ€ rhythm spikes before score **15**

These values now live in config assets and should be tuned through playtesting rather than rewritten in runtime code.

## Fairness checklist for every new obstacle pattern

Before accepting a new pattern, confirm:

- Is the fail reason visible?
- Is the safe choice readable?
- Is the reaction window appropriate for current speed?
- Does the pattern respect one-tap control?
- Can a player explain why they died?
- Does the pattern create useful tension instead of confusion?

## Non-negotiables

- The only gameplay action is side flip.
- The line remains the main spatial reference.
- Hazard readability beats novelty.
- Score remains simple and visible.
- Death stays immediate and understandable.
- Restart remains central to the emotional loop.



