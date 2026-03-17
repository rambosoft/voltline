# Live Wire City — Production-Ready Visual & Experience Style Guide

Status: Active  
Owner: Team / AI  
Last updated: 2026-03-16  
Source of truth for: final presentation direction for the **Live Wire City** theme across UI, UX, gameplay feedback, audio, VFX, milestones, and social/share moments  
Depends on: product vision, gameplay pillars, UI/UX guide, audio/VFX guide, production phases, presentation refresh readiness roadmap  
Do not duplicate with: gameplay systems, architecture, or implementation-specific asset lists

---

## 1. Purpose

This document defines the **production-ready presentation target** for the **Live Wire City** theme of Voltline.

It answers:

- what the world is
- what the player is
- what the line is
- what the hazards are
- how the city reacts to success and failure
- how every important event should look, sound, and feel
- how UI should be laid out and styled
- how the game should progress visually as score increases
- how all of the above stay readable, fair, performant, and attractive on mobile

This document is not an implementation spec for code architecture.  
It is a **creative direction + UX + feedback map** that a production team or AI agent can use to upgrade the game into a polished, publishable experience.

---

## 2. Core fantasy

### One-line fantasy

You are a living electric pulse racing along a dangerous city power line to keep the city alive.

### Player fantasy

The player is not “a generic ball.”  
The player is a **smart energy core** carrying charge across a damaged urban grid.

### World fantasy

The city below is partially unstable.  
Every successful run restores more power, more light, more life, and more momentum to the skyline.

### Core emotional target

The game should feel:

- fast
- precise
- stylish
- dangerous
- satisfying
- premium
- alive

It should **not** feel:

- noisy
- confusing
- muddy
- childish
- cruel
- cluttered
- generic cyberpunk

### Narrative framing

The city is always on the edge of blackout.  
Each run is a charge transfer.  
The line is a high-voltage conduit.  
Hazards are failures, blockages, overloads, leaks, grounded objects, and sabotage on the line.

When the player dies, power drops.  
When the player succeeds, districts illuminate.

---

## 3. Non-negotiables

- Gameplay remains **one-tap only**
- Portrait mobile remains the target
- Line, player, and hazards must remain readable at a glance
- High contrast is mandatory
- Decoration must never hide danger
- City background must support gameplay, not overpower it
- VFX must feel sharp and premium, never noisy
- Audio must be satisfying but not exhausting across many retries
- Death-to-retry flow must stay extremely fast
- Milestone spectacle must remain lightweight and readable
- All theme progression must reinforce the fantasy of **powering the city**
- Build size and performance discipline must be preserved

---

## 4. Theme identity

## Theme name

**Live Wire City**

## Tagline directions

- Power the night.
- Keep the grid alive.
- One line. One pulse. One more run.
- Every point lights another district.

## Visual identity keywords

- neon utility
- premium arcade
- electric tension
- urban energy
- sharp contrast
- controlled glow
- readable danger
- modern, not messy

---

## 5. Color palette

Use a limited palette with role-based usage.  
Avoid random accent colors.

## Primary palette

### Background base
- **Midnight Navy** — `#09121F`
- **Deep Grid Blue** — `#10233A`
- **Charcoal Steel** — `#11161D`

Use these for:
- sky
- distant city silhouette
- UI backplates
- shadow areas
- low-energy states

### Main energy / line
- **Live Cyan** — `#4DF3FF`
- **Electric Blue** — `#19B5FF`
- **Core White** — `#F5FDFF`

Use these for:
- line glow
- player core highlights
- score emphasis
- active current
- energized city windows

### Danger
- **Overload Magenta** — `#FF3D8E`
- **Alert Red** — `#FF4B4B`
- **Hot Orange** — `#FF9C3A`

Use these for:
- obstacles
- warning flashes
- hazard edges
- overload states
- death screen accents

### Reward / milestone
- **Signal Gold** — `#FFD85B`
- **Voltage Lime** — `#A7FF4B`

Use these for:
- milestone banners
- best-score state
- daily challenge target
- special completion flashes

### Neutral UI
- **Soft White** — `#EAF4FF`
- **Steel Gray** — `#7F95AA`
- **Glass Slate** — `rgba(8,18,31,0.72)`

## Usage rules

- Cyan is the default “safe energy” color
- Magenta/red are reserved for danger
- Gold/lime are reserved for achievement moments
- Keep the number of simultaneously prominent colors low
- If everything glows, nothing matters

---

## 6. Lighting and contrast rules

### Contrast hierarchy

1. Player
2. Immediate hazard zone
3. Line
4. Score / essential HUD
5. Background world
6. Ambient decorative effects

### Glow rules

- Player glow: medium-strength, focused, tight radius
- Line glow: soft but always readable
- Hazard glow: sharper, more aggressive edges than line glow
- Milestone flash: short and controlled
- City lights: subtle unless activated during milestones

### Darkness rules

The background should be dark enough that the line and hazards always dominate.  
The city should feel alive, but never brighter than the game lane.

---

## 7. Environment story

The game space is not abstract. It is a **high-voltage cable zone above a living city**.

### World layers

#### Layer 1: Distant skyline
- dark building silhouettes
- rooftop antennas
- occasional windows
- some districts darker than others

#### Layer 2: Utility depth
- poles
- cable towers
- substations
- transformer nodes
- warning lights
- faint moving traffic glow below

#### Layer 3: Gameplay support motion
- soft energy particles drifting
- atmospheric electric haze
- occasional distant power flickers

#### Layer 4: Gameplay lane
- the line
- player
- hazards
- immediate reactive effects

### Story progression through score

As score rises, the city should feel more powered:
- more windows turn on
- more signs flicker alive
- more rooftop lights stabilize
- subtle color richness increases
- background motion becomes slightly more energized

This progression must be visible but restrained.

---

## 8. Splash screen

### Goal

Immediately sell premium identity in under 2 seconds.

### Visual

- black-to-navy background
- one dark city silhouette at bottom
- a single electric line draws across the screen with a pulse
- the player core rides the line for a short arc
- small sparks jump from the line
- the game logo appears as if energized from within

### Logo direction

**Voltline** wordmark:
- compact
- geometric sans style
- neon cyan core with soft white highlight
- slight electric cut through the letters
- optional tiny spark between “Volt” and “line”

### Motion

- line traces in
- pulse runs through
- logo charges on
- background windows flicker alive
- immediate transition to menu

### Audio

- low electric hum swell
- quick charge-up spark
- soft premium confirmation ping

---

## 9. Main menu

## Layout

### Top area
- logo centered
- tiny subtitle: “Keep the grid alive”

### Mid area
- live animated demo of gameplay lane
- player running across line
- city background active
- one or two hazard examples
- no busy menu background art beyond this

### Lower area
Primary:
- **Play** button centered and dominant

Secondary row:
- Daily
- Themes
- Best
- Settings

### Bottom edge
- version text, small
- privacy/legal only if needed, hidden and not noisy

## Menu style

- dark glass panels
- rounded but not childish corners
- thin cyan edge highlight
- white text
- subtle glow on focus/press
- no oversized panels unless essential

## Demo behavior

The menu background demo should:
- show the actual line
- show the player flipping sides every few seconds
- show one near-miss moment
- never kill the player on loop
- subtly communicate how the game works without tutorial text overload

## Menu animation rules

- logo idle pulse every few seconds
- Play button slight energy shimmer
- distant city windows flicker gently
- no constant flashing

---

## 10. Settings screen

## Purpose

Minimal, readable, premium.

## Options

- Music on/off or slider
- SFX on/off or slider
- Haptics on/off
- Reduced effects mode
- Screen shake on/off
- Pause behavior note if needed
- Restore defaults

## Style

- same glass-slate panel style
- icon + label rows
- sliders use cyan active track and white knob
- toggles use cyan active / gray inactive
- headings in small uppercase tech style

## Background

Keep city background visible but dimmed.

## Audio

- soft UI click
- slider tick subtle
- toggle snap soft but satisfying

---

## 11. Theme / world progression screen

If theme progression or districts are presented outside gameplay:

- call it **Grid Status** instead of “Themes”
- show city districts as cards or map nodes
- each unlocked visual state corresponds to more energized parts of the skyline
- keep it simple; no sprawling world map needed for launch

---

## 12. Gameplay lane composition

### Core lane elements

- line centered visually within active play space
- enough side padding from device edges
- city visible behind but softened
- hazards slightly larger visually than line thickness for readability

### Camera

- fixed portrait framing
- subtle look-ahead feel via movement of world relative to player
- no dramatic camera movement
- only tiny pulse zoom on major milestones or death

---

## 13. Player design

## Base concept

The player is a **living power core**:
- bright center
- translucent electric shell
- small personality through shape and reactive animation

## Final shape recommendation

### Option A — best choice
A small circular core with:
- solid bright center
- thin electric halo
- two optional tiny eye slits or lens markings
- short ghost trail

This preserves readability at speed.

### Surface details

- white-hot center
- cyan edge glow
- occasional tiny electric arcs around shell
- no over-detailed face

## States

### Idle / running
- slight hover pulse
- tiny flicker variations
- stable glow

### Flip
- compress horizontally then stretch slightly on flip
- leave a small electric arc trail
- outer shell leans toward movement direction

### Near miss
- shell spikes outward for a frame
- halo intensity increases briefly

### Milestone
- shell brightens and expands in a clean pulse
- tiny golden spark ring added

### Death
- center collapses
- shell breaks into electric fragments
- sparks fall away

## Readability rules

- silhouette must remain clear on 6-inch mobile screens
- no shape that looks too similar to hazards
- player must remain brightest moving element on screen

---

## 14. Player trail design

### Default trail

- short cyan-white energy streak
- fades quickly
- slightly curved to imply motion
- no long ribbon that obscures hazards

### During intense moments

At higher score brackets:
- add micro sparks in the trail
- increase speed impression, not clutter

### During near-miss

- trail briefly sharpens to white
- one thin sparkle line appears

---

## 15. Line design

## Identity

The line is not just a line.  
It is the **main power conduit**.

## Visual construction

- medium-thick core stroke
- inner bright white-cyan current channel
- darker cyan outer edge
- subtle flowing energy texture moving through it
- occasional tiny electrical pulse traveling forward

## Geometry

- smooth curves, never rigid jagged corners
- bends should feel cable-like, not abstract spline noise
- thickness can vary slightly at tension points, but only subtly

## Behavior by event

### Normal
- steady current flow
- soft glow

### Flip
- a pulse ripple runs along the line at the contact point

### Near miss
- line brightens for a split second near the player

### Milestone
- line glows stronger across visible segment
- distant current pulse races ahead

### Death
- current flickers, then line dims for a beat

---

## 16. Obstacle families

All obstacles should feel like real threats to an electrical transfer.

## Family 1 — Grounded blockers
Examples:
- wood barriers
- insulated clamp blocks
- grounded metal plates

Visual:
- chunky shapes
- strong silhouette
- red/magenta warning edges
- occasional hazard stripe markings

Narrative:
These drain or block current.

## Family 2 — Sharp utility hazards
Examples:
- exposed spikes
- metal hooks
- shard brackets
- torn cable barbs

Visual:
- sharper silhouette
- hotter edges
- aggressive red/orange highlights

Narrative:
These physically rupture the transfer path.

## Family 3 — Active electric hazards
Examples:
- shorted nodes
- unstable arcs
- crossing live wires
- pulse gates

Visual:
- animated
- warning flash before peak danger
- magenta/pink with white electrical crack

Narrative:
These overload the signal.

## Family 4 — Rotating industrial hazards
Examples:
- utility saws
- rotating cutter discs
- fan blades
- maintenance rotors

Visual:
- circular industrial forms
- caution lights
- sparks on edges

Narrative:
Mechanical city repair systems gone wrong.

## Family 5 — Broken conduit sections
Examples:
- cable gaps
- collapsed support areas
- fractured insulation zones

Visual:
- torn glowing ends
- falling spark dust
- line clearly missing

Narrative:
Infrastructure damage.

## Family 6 — Side pressure hazards
Examples:
- compressing blocks
- side-mounted interrupters
- moving armatures

Visual:
- heavy shapes approaching lane edge
- readable anticipation

Narrative:
Dynamic grid pressure.

---

## 17. Obstacle readability rules

- Each obstacle family needs a distinct silhouette
- Danger must be readable in under 0.3 seconds
- Active hazards need a warning state before contact zones become lethal
- Decorative details must never change the collision truth visually
- A hazard should feel dangerous because of shape and spacing, not only because of color

---

## 18. Background design

## Role

The background should make the city feel alive and fast, without competing with gameplay.

## Composition

### Layer A — sky gradient
- deep navy at top
- faint electric blue haze near horizon

### Layer B — distant skyline
- simple building blocks
- varied heights
- small energized windows

### Layer C — middle utility band
- cable poles
- rooftop structures
- warning lights
- transformer silhouettes

### Layer D — foreground atmosphere
- soft glow streaks
- tiny drifting sparks
- occasional mist haze

## Motion

- parallax layers move at different speeds
- distant skyline slowest
- utility layer slightly faster
- subtle streak layer fastest

---

## 19. High-speed background effect

## Goal

Make the player feel like they are moving at dangerous speed without cluttering the playfield.

## Approved effects

### Effect A — energy streak drift
- very thin translucent horizontal/diagonal streaks
- move downward or upward relative to lane motion
- low density
- strongest in peripheral screen areas

### Effect B — window flicker trails
- distant windows create tiny light streaks as speed rises

### Effect C — atmospheric electric particles
- small drifting motes
- cyan and blue
- occasional magenta spark in danger-heavy zones

### Effect D — heat-haze shimmer
- subtle distortion around energized sections of background

## Avoid

- dense starfield-like particle floods
- thick smoke
- giant fast objects crossing the lane
- anything that can be mistaken for a hazard

---

## 20. HUD style

## Core rules

Show only what matters:
- score
- best or target
- pause

Optional:
- subtle milestone band or district meter

## Score

### Position
Top center

### Style
- large
- bold
- white with faint cyan rim glow
- slightly condensed techno font
- scales up a little on each point

### Best score
- small
- top right or directly under score if needed
- steel gray when inactive
- gold when currently being beaten

### Pause
- top left
- simple icon in glass chip button
- no label

## HUD behavior

- HUD should never bounce too much
- score pop should be fast and clean
- new best should be visually distinct without giant interruption

---

## 21. Score behavior

### Each point
- score increments with a subtle scale-up
- tiny sparkle around number
- soft upward blip sound

### Every 5 points
- slightly stronger score emphasis
- city response more noticeable

### New best threshold crossed
- score flashes gold-white
- “NEW BEST” appears briefly below it
- one celebratory but sharp sound

---

## 22. Milestone progression

## Milestone logic concept

The city becomes more alive at score thresholds.

### Score 10
- nearby windows light up
- one rooftop sign flickers on
- line glow intensifies slightly
- milestone text: **DISTRICT ONLINE**

### Score 20
- utility towers energize
- more background traffic glow
- larger power pulse through line
- milestone text: **GRID STABLE**

### Score 30
- skyline becomes richer
- distant billboard animates
- player halo gets a stronger secondary pulse
- milestone text: **CITY AWAKENS**

### Score 40
- storm haze in the sky reacts with subtle electric sheets
- line current moves faster visually
- milestone text: **FULL CHARGE**

### Score 50+
- premium “impossible” look
- city feels fully powered
- visual richness increases slightly, but gameplay remains clear
- milestone text examples:
  - **NO BLACKOUT**
  - **SURGE LEVEL**
  - **LEGENDARY RUN**

## Milestone visual rules

- no long blocking banner
- use quick center-top flash or world-reactive event
- do not stop gameplay
- text appears for under 1 second

---

## 23. Theme changes across milestones

This is still one theme: **Live Wire City**.  
Milestones should feel like **district progression**, not random theme swaps.

## Phase 1 — Failing grid
Score 0–9
- darker city
- fewer lights
- quieter background

## Phase 2 — Local power restored
Score 10–19
- neighborhood windows activate
- utility glows wake up

## Phase 3 — Grid stabilization
Score 20–29
- skyline more vibrant
- signs and moving lights appear

## Phase 4 — Surge city
Score 30–39
- stronger electric weather
- background streaks more energetic

## Phase 5 — Overclock city
Score 40+
- city feels fully alive
- line and player feel more charged
- strongest premium visual state

Progression must be gradual and smooth.

---

## 24. Flip feedback map

Flip is the most important action in the game.  
It must feel amazing every single time.

## On flip trigger

### Player
- fast squash/stretch
- quick shell lean
- small electric flare at contact point

### Line
- emits a pulse ring where flip occurred
- one current ripple travels along visible line

### Background
- tiny ambient response, usually none
- at higher milestones, a very small synchronized flicker in city lights may occur

### VFX
- 4–8 micro spark particles
- one thin arc streak
- very short glow burst

### Audio
- primary flip SFX: short “tick-zip”
- optional slight pitch variance
- must be satisfying across hundreds of repeats

### Haptics
- tiny tap pulse if enabled

## Edge case: repeated fast flips
- prevent audio stacking from becoming harsh
- retain responsiveness
- vary pitch within a narrow safe band

---

## 25. Near-miss feedback map

## Trigger

When player passes extremely close to hazard without collision.

## Visual
- one sharp white spark
- player shell spikes briefly
- tiny line brightening
- optional quick hazard edge shimmer

## Audio
- airy “shing” or sharp sparkle
- must feel rewarding, not alarming

## Screen
- no large shake
- maybe tiny pulse only

## Purpose
Reinforces skill and keeps “one more try” loop strong.

---

## 26. Collision feedback map

## Collision with grounded blocker
- player spark sputters
- obstacle emits one grounding flash
- city lights brown out slightly
- death sound includes a power cut element
- death text leans “grid failed”

## Collision with sharp utility hazard
- brighter, sharper fragment burst
- one red-white cut flash
- audio includes crack + short impact
- death text leans “signal ruptured”

## Collision with electric hazard
- overload bloom
- strong magenta-white arc burst
- audio includes glitching zap
- death text leans “overload”

## Collision with rotating industrial hazard
- mechanical spark spray
- metallic cut accent
- city flicker plus industrial hit tone

## Collision with gap / broken line
- player drops charge and fades
- line ends spark outward
- death text leans “line broken”

## Collision side note
Death VFX should vary slightly by hazard family but remain short and readable.

---

## 27. Side-collision behavior

If collision happens from a side transition or edge overlap:
- prioritize fairness visual language
- use a clear hit spark exactly at contact zone
- do not let the player appear to die “from air”
- if hit was close and ambiguous, enlarge contact feedback slightly for clarity

---

## 28. Death screen and loss flow

## Timing
- death occurs
- burst and blackout response
- result panel appears quickly
- retry is immediately available

## Background behavior on death
- visible local blackout sweep
- some windows go dark
- line loses intensity
- city remains visible, not fully hidden

## Result panel layout
Center:
- Score large
- Best below
- Retry dominant button
- small Home / Share buttons
- optional “Daily target” reminder

## Style
- glass dark panel
- red-magenta accent edge
- subtle damaged-electric flicker

## Death text rotation
Examples:
- GRID FAILURE
- SIGNAL LOST
- DISTRICT DOWN
- OVERLOAD
- LINE INTERRUPTED
- POWER CUT
- TRANSFER FAILED

These should be paired to cause of death where possible.
Keep them short, city/power-grid themed, and retry-friendly.

## Subtext examples
- “One more pulse.”
- “The city almost held.”
- “Retry the transfer.”
- “You restored 3 districts.”
- “So close to full charge.”

---

## 29. Milestone text library

### Positive progression
- DISTRICT ONLINE
- GRID STABLE
- CURRENT LOCKED
- POWER ROUTED
- CITY AWAKENS
- FULL CHARGE
- SURGE LEVEL
- UNBROKEN LINE
- NO BLACKOUT
- POWER RUN

### New best
- NEW BEST
- RECORD CHARGE
- GRID RECORD
- BEST TRANSFER

### Daily challenge
- DAILY TARGET REACHED
- BONUS DISTRICT ONLINE

Keep text short and punchy.

---

## 30. Pause state

## On pause
- gameplay freezes cleanly
- line energy flow pauses
- city background keeps a very faint idle life if desired, but gameplay elements stop
- dim overlay appears
- menu is immediate and uncluttered

## Pause menu options
- Resume
- Restart
- Settings
- Quit to menu

## Audio on pause
- music low-pass or volume dip
- ambient hum continues softly or pauses based on implementation preference
- UI click remains clean

## Resume
- quick pulse-in
- tiny recharge sound
- no big countdown unless accessibility mode asks for it

---

## 31. New best score celebration

## Trigger
When score exceeds previous best for first time in run.

## Visual
- score turns gold-white
- small burst around score
- city windows pop brighter for a moment
- player emits proud surge ring

## Audio
- bright but not overly long chime
- should not sound like level complete

## Text
- NEW BEST

## Rules
- no large popup
- do not interrupt gameplay

---

## 32. Sound palette

## Core sound character

- modern arcade
- electric but clean
- short and tactile
- premium, not noisy
- synthetic with subtle industrial texture

## Sound families

### Flip
- click + zip + soft spark
- fast, crisp, satisfying
- pitch variance minimal

### Score
- small upward blip
- optimistic and clean

### Near-miss
- thin shimmer / sharp sparkle

### Milestone
- richer synth chime with utility-energy undertone

### Death
- short crack + power cut + low hit

### UI
- rounded clicks
- soft tab snaps

### Ambient
- transformer hum
- distant urban electrical texture
- occasional wire buzz

---

## 33. Music direction

## Main loop style
Hybrid of:
- light synthwave
- urban electrical ambience
- subtle rhythmic pulse

## Structure
### Start
- sparse
- low kick pulse or bass heartbeat
- restrained pads

### Mid-run
- more percussion texture
- added arpeggio or pulse layer

### High score
- brighter top-line synth
- more urgency
- still loop-safe

## Important rule
Music should support tension without becoming annoying after 50 retries.

## Menu music
- more atmospheric
- lower intensity
- same harmonic palette as gameplay

## Death transition
- very short dip or cut, then result screen bed resumes softly

---

## 34. Continuous ambient video-effect style

If you want the game to feel more alive, the “video effect” should be world-based and lightweight.

## Approved ongoing effects
- distant city shimmer
- cable current movement
- occasional rooftop sign flicker
- small rain-like electric dust in some score phases
- subtle skyline parallax
- power pulses through distant lines at milestones

## Do not use
- full-screen overlays constantly moving
- giant animated billboards in center play area
- excessive distortion
- noisy particle storms

---

## 35. UI typography

## Recommended style
- geometric techno sans
- slightly condensed
- bold for score and CTAs
- medium for labels
- uppercase for milestone and status text

## Visual treatment
- white text
- soft shadow or low glow
- cyan highlight only where important
- gold reserved for achievement

---

## 36. Buttons

## Primary button
- rounded rectangle
- cyan edge glow
- dark fill
- white label
- slight animated current line inside on hover/idle

## Secondary button
- dark fill
- thin gray-blue edge
- less glow

## Danger / destructive button
- magenta/red edge
- use sparingly

## Press animation
- slight compress
- short glow pulse
- crisp click sound

---

## 37. Share-screen concept

## Goal
Make people want to post the result.

## Composition
- top: logo or icon small
- center: frozen dramatic gameplay frame
- visible score large
- milestone state or district status
- one short line of flavor text:
  - “Kept the grid alive to 32”
  - “District 4 online”
  - “No blackout until 27”

## Visual extras
- frame uses dark glass with cyan edge
- city background visible
- player and hazard near-miss moment captured

## Why it works
People immediately understand:
- score
- danger
- style
- world

---

## 38. Tutorial language

Keep tutorial minimal.

## First-run tutorial
- “Tap to flip sides”
- small finger icon once
- live animated example
- maybe one short second line:
  - “Keep the charge alive”

No long instructions.

---

## 39. Edge cases and special states

## Very low score death
- quick fail
- death text smaller
- avoid over-dramatizing

## High-score death
- slightly bigger blackout reaction
- stronger best-score emphasis if record broken

## Pause during milestone
- freeze current state cleanly
- milestone text should not resume in a broken way

## Resume after near-miss chain
- no stale audio loops
- no leftover flare spam

## Consecutive retries
- no long transitions
- music should restart or continue in a way that feels snappy
- retry should never feel like waiting

---

## 40. Accessibility and readability

- maintain high contrast between gameplay objects and background
- avoid red-green dependency
- large enough score text for portrait phones
- minimum touch targets in menus
- reduced-effects option disables extra streaks, shake, and some particles
- do not rely on sound alone for feedback

---

## 41. Performance rules

- background effects must remain lightweight
- particle bursts should be short and pooled if implemented
- no large alpha-heavy fog over gameplay zone
- avoid too many simultaneous glows
- keep animated background signs limited
- all milestone changes should be state-based, not huge dynamic scene rebuilds

---

## 42. What “premium” means here

Premium does **not** mean:
- adding more effects everywhere
- making every UI element glow
- adding complex story text
- using 12 accent colors

Premium **does** mean:
- discipline
- contrast
- consistency
- purposeful motion
- satisfying feedback
- clean typography
- beautiful screenshots
- a strong world fantasy

---

## 43. Production-ready checklist for this theme

The Live Wire City presentation is production-ready when:

- the player is instantly readable
- the line feels like a real energized conduit
- every hazard family is visually distinct
- the city reacts meaningfully to score progression
- the UI is clean and modern
- flip feedback is addictive
- death feedback is sharp and fair
- score progression feels like restoring a city
- audio is satisfying after many retries
- screenshot moments look exciting without context
- performance remains stable on target mobile devices

---

## 44. Final direction summary

Live Wire City should feel like a premium one-tap arcade survival game where a living electric core races through a blackout-prone city power line, dodging grounded hazards and unstable infrastructure while the skyline slowly comes alive around every good run.

The final player experience should communicate:

- “I understand it instantly.”
- “It looks much better than a simple mobile prototype.”
- “Every flip feels good.”
- “Every death feels fair.”
- “I was so close.”
- “One more run.”

