# UI / UX Style Guide
Status: Active
Owner: Team
Last updated: 2026-03-15
Source of truth for: screen behavior, hierarchy, HUD, menu rules, death flow, typography, spacing, accessibility, screenshot composition
Depends on: 01-product-vision.md, 06-scene-flow.md, 07-gameplay-systems.md
Do not duplicate with: art moodboards or isolated UI mock notes

> Working title: **Voltline**  
> Core concept / public-facing design name: **STAY ON THE LINE**

## UX goals

The interface should feel:

- immediate
- premium
- uncluttered
- readable
- modern
- mobile-native
- fast to navigate
- visually consistent with the neon arcade fantasy

The UI must support the game loop, not compete with it.

## High-level UI philosophy

During gameplay, UI should do only what is necessary:

- show score
- show essential meta information
- stay out of the way

Outside gameplay, UI should guide the player back into play quickly.

## Visual direction

UI style keywords:

- soft dark panels
- rounded corners
- clean spacing
- strong contrast
- white or bright text
- subtle glow accents
- minimal outlines
- restrained motion
- premium arcade feel

The UI should feel stylish enough for adults, but friendly enough for kids.

## Screen hierarchy

Approved screen set for first release:

1. Main Menu
2. Gameplay HUD
3. Pause Overlay
4. Result Panel
5. Settings Overlay

Future optional screens:

- Daily Challenge panel
- Themes/Skins panel
- Best score/history panel

## Main Menu

### Purpose
- sell the game instantly
- explain the mechanic visually
- get the player into a run quickly

### Required elements
- game logo / working title
- animated gameplay demo or idle preview
- large primary Play button
- best score display
- smaller buttons for Settings and future modes
- short mechanic hint: “Tap to flip sides”

### Layout priority
Top:
- logo/title

Middle:
- animated line + character preview

Bottom:
- big Play button
- small secondary actions

### Menu rules
- Play is the most visually prominent action
- demo/preview should run without requiring text explanation
- no cluttered top bars
- no long submenu chain before the first run

## Gameplay HUD

### Required elements
- current score
- optional best score (small)
- pause button

### Approved layout
- score: top center
- pause: top left
- best/rank badge: top right or small beneath score, depending on final layout testing

### HUD rules
- maximum visible gameplay HUD count at once: **3 primary elements**
- score must remain legible at a glance
- avoid decorative panels that cover danger area
- no large tutorial boxes during normal runs
- no live text spam during basic play

## Score presentation

Score should be:

- large
- centered
- immediately readable
- slightly animated on increment
- visible in screenshots

Recommended treatment:

- bright white or premium bright accent
- soft shadow/glow
- short scale pop on score gain

## Pause overlay

Purpose:
- stop the run safely
- allow resume, restart, settings, and home

Rules:
- pause should be a lightweight overlay, not a full scene detour
- Resume and Restart should be easy to access
- visual treatment should dim gameplay, not replace it completely
- avoid deep settings trees inside pause

## Result panel

This screen is crucial to retention.

### Required result content
- final score
- best score
- short emotional message
- big Retry button
- smaller Home button
- optional Share button

### Emotional microcopy examples
- So close
- Almost 20
- New Best
- One more run
- You survived 18

### Layout rules
- Retry is center priority
- Retry must be visually larger than Home/Share
- result reveal is fast
- panel should feel conclusive but not heavy
- do not cover the entire screen with opaque noise unless testing proves it helps

## Settings overlay

Keep it minimal in first release.

### Allowed settings
- Music volume
- SFX volume
- Vibration toggle (if implemented)
- Theme selection (if unlocked themes exist)
- maybe color-blind/readability options later if needed

### Rules
- no bloated account/settings model
- settings should not overshadow gameplay
- use simple toggles/sliders
- keep copy short

## Typography rules

Use TextMeshPro.

### Typography goals
- crisp on mobile
- bold for score and CTAs
- readable at small sizes
- few font styles total

### Recommended role system
- Display: title / logo
- Heading: menu section labels
- UI Body: buttons and labels
- Micro: helper text only when truly necessary

### Text rules
- short strings over long explanations
- avoid paragraphs in normal flows
- button labels should be one to three words where possible

## Spacing and layout rules

- generous tap targets
- consistent margins
- keep gameplay top area readable around notch/safe area
- prefer one clear primary action over several equal buttons
- use whitespace to imply hierarchy instead of adding more panels

### Mobile tap target rule
Buttons should be comfortably tappable on small phones.

## Color role system

Each theme should still respect the same semantic roles:

- background base
- primary line color
- player accent
- danger color
- reward/milestone color
- neutral text/UI color

### Default working theme
**Neon Night**
- background: deep navy / purple
- line: cyan glow
- player: white with cyan/pink accents
- danger: magenta-red
- milestone/reward: warm gold or bright white

## Animation behavior

UI motion should feel smooth and premium, never bouncy and noisy.

### Approved motion types
- fade
- scale pop
- short slide
- slight panel rise
- score pulse
- button emphasis

### Duration guidance
- tiny micro-feedback: **0.08–0.18s**
- panel transitions: **0.18–0.30s**
- result entry: short and decisive

Avoid long easing chains that slow re-entry into gameplay.

## Safe area behavior

The game is mobile portrait-first.

Rules:

- main UI must respect safe area insets
- score/pause/buttons must never sit under notches or rounded corners
- layout should hold across small and tall phones
- gameplay line composition should still feel centered after safe-area padding

## Screenshot-worthiness rules

A good screenshot should include:

- visible score
- glowing line
- character on one side of the line
- visible danger close by
- strong contrast
- clean UI
- a near-death or high-tension composition

### Screenshot composition rules
- do not let HUD cover the most interesting hazard zone
- keep score and player readable in stills
- prefer clear silhouette moments over crowded effect bursts

## Accessibility/readability notes

- maintain strong contrast between danger and background
- avoid red/green-only meaning separation
- keep motion optional where platform expectations require it
- preserve tap target size
- keep important information large enough on smaller phones

## Good UI examples, conceptually

Good:
- one big Play button
- one big Retry button
- visible score with tiny pop
- minimal pause icon
- clean settings panel with 2–4 controls

Bad:
- five equal-size menu buttons
- gameplay screen full of labels
- achievement banners covering hazards
- decorative chrome around every number
- slow death summary before retry

## Premium-but-simple definition

For this project, “premium but simple” means:

- clean dark surfaces
- bright accent colors
- polished transitions
- high readability
- small number of UI elements
- no clutter pretending to be value

## Non-negotiables

- Gameplay HUD stays minimal.
- Play and Retry are the dominant calls to action.
- UI motion stays short and supportive.
- Safe-area handling is required.
- Screenshots must look exciting without looking crowded.
- UI must never make the run harder to read.
