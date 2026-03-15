# Visual Refresh and Theme System Guide
Status: Active
Owner: Team / AI
Last updated: 2026-03-15
Source of truth for: visual refresh planning, theme system planning, and presentation-layer change guidance
Depends on: `.ai/00-index.md`, `.ai/01-product-vision.md`, `.ai/02-game-design-pillars.md`, `.ai/03-tech-stack.md`, `.ai/04-architecture.md`, `.ai/05-project-structure.md`, `.ai/06-scene-flow.md`, `.ai/07-gameplay-systems.md`, `.ai/08-ui-ux-style-guide.md`, `.ai/09-audio-vfx-guide.md`, `.ai/10-data-content-model.md`, `.ai/11-testing-quality-bar.md`, `.ai/12-adrs.md`, `.ai/14-agent-rules.md`, `.ai/15-roadmap-backlog.md`, `.ai/16-production-phases.md`
Do not duplicate with: gameplay systems, core architecture, UI style guide, audio/VFX guide, or roadmap docs

## 1. Purpose
This file is the execution guide for presentation refresh work. It owns how to evolve art, collision-safe visuals, themes, background motion, VFX, and audio without changing the game's mechanical identity. It does not own gameplay rules, core architecture, UI behavior rules, or roadmap priority.

## 2. Non-negotiables
- Gameplay stays one-tap.
- Readability beats decoration.
- Collision changes must preserve fairness.
- Theme changes must not confuse danger semantics.
- VFX/SFX must reinforce clarity and satisfaction.
- Mobile performance and small build size matter.
- The game remains premium-looking but simple.
- Presentation work must not introduce feature creep.

## 3. Current-state analysis
- Player visuals are currently procedural: `PlayerController.cs` creates a runtime `SpriteRenderer` using `RuntimeSpriteFactory.WhiteSprite` and tints/scales it.
- Player collision is currently simplified, but the frozen readability/collision baseline now lives in `GameplayPresentationConfig` rather than static helper constants.
- Obstacle visuals are procedural too: `HazardManager.cs` creates multiple runtime `SpriteRenderer`s and styles them per family.
- Obstacle collision and readable spacing now route through `HazardPresentationCatalog` and `HazardManager.cs`, but the visuals themselves are still procedural.
- Gameplay background is minimal: `TrackManager.cs` sets camera background color from `ThemeConfig`; there is no gameplay background layer system yet.
- Themes already exist but are color-only: `ThemeConfig`, `ThemeCatalog`, `SaveService`, and `SettingsOverlayView` support unlock/select persistence, but not asset overrides or in-run switching.
- VFX are mostly procedural: `VfxService.cs` falls back to transient sprite pulses/bursts.
- Audio is semantic and service-based: `AudioService.cs` + `AudioCueCatalog.cs` + mixer routing are in place, but actual content still leans on procedural fallback clips.
- Content folders are still sparse: `Assets/_Game/Art/...` and most audio content folders contain structure more than real authored assets.
- Main architectural limitation: visuals are still heavily code-generated, and theme application is mostly initialization-time only.
- Easy changes: more color themes, richer theme metadata, config additions. Risky changes: swapping player/obstacle art before decoupling collision, adding mid-run theme switching, and adding busy background effects too early.

## 4. Impact map
Affected areas will include:
- player rendering and anchor/pivot handling
- player collision and line-clearance assumptions
- obstacle visual profiles, collision, spacing, and telegraphs
- track/background rendering and motion FX
- theme config/data, persistence, and settings UX
- VFX catalogs and VfxService theme adaptation
- AudioCueCatalog, AudioService, mixer balancing, and optional theme music variations
- relevant tests, manual fairness review, device/performance validation

## 5. Recommended implementation order
1. Audit and document current visual/collision dependencies.
2. Move hardcoded presentation values into focused config assets.
3. Decouple player visuals from collision.
4. Refresh player visuals.
5. Refresh obstacle visuals and revalidate spacing/fairness.
6. Add a dedicated background presentation layer.
7. Expand the theme system beyond color-only.
8. Add dynamic theme switching only after live theme application is safe.
9. Update VFX and audio last so they match the final visual language.

## 6. Player asset refresh guide
- Keep gameplay-critical data separate from art: collision extents, side offset, line clearance, flip timing, and visual minimum size should be data-driven.
- Visual-only data should include sprite/prefab, material, scale, pivot offset, glow/trail references, and theme overrides.
- Recommended repo-specific path: add `PlayerVisualConfig`, then let `PlayerController` read visual setup separately from collision setup before replacing the runtime white sprite.
- Preserve one canonical pivot/orientation rule. Do not let themes silently change the perceived anchor on the line.
- Avoid large decorative appendages that imply collision when they are cosmetic only.

## 7. Collision update guide
- For Voltline, simplified gameplay collision is preferred over full silhouette collision.
- Recommended default: compact player ellipse/rounded box, simplified obstacle hit areas inside the visible danger core, explicit forbidden-space regions for gates/gaps.
- Collision should roughly match the visible dangerous core or be slightly smaller for fairness. Do not make it larger than what the player sees.
- Move collision tuning into config: player collision size/offset, obstacle collision size/offset, line clearance, near-miss threshold, spacing multipliers.
- Required validation: front hit, side graze, flip-through timing, near miss, repeated runs at speed, no line-overlap confusion.

## 8. Background refresh guide
- The gameplay background should evolve from flat camera color into a restrained layered backdrop.
- Preferred layers: base gradient, distant parallax, mid-depth motion layer, subtle streak layer.
- Keep the line, player, and hazards visually dominant.
- Danger color must remain distinct from the background.
- Avoid high-frequency detail, bright flicker, or shapes that resemble hazards.
- Profile any background motion on device before expanding it.

## 9. High-speed motion background FX guide
Use speed-feel effects that support motion without competing with gameplay:
- subtle downward or opposing streaks
- sparse directional particle drift
- soft glow bands or ribbon-like ambience
- layered parallax with different speeds
- small camera-relative ambience that never occupies the core danger lane

Avoid:
- obstacle-like silhouettes in the background
- full-screen distortion in normal play
- dense particle clouds near hazards
- flashing effects during decision windows

Effect budget rules:
- few layers
- low alpha
- low spawn count
- short or efficiently recycled lifetimes
- no persistent effect directly over the main lane unless extremely subtle

## 10. Obstacle refresh guide
- Do not refresh hazard art ad hoc per script branch. Formalize visual profile data first.
- Maintain family readability: spikes, cutters, gates, gaps, and blockers must remain recognizable in a fraction of a second.
- Store visual extents and collision extents separately.
- Decorative glow may extend beyond collision, but the visible dangerous core should still explain the hit area.
- Re-check minimum hit-distance spacing, silhouette overlap, telegraph readability, and screenshot clarity after every obstacle art pass.
- Themes may change palette/material treatment, but not the meaning of danger or which side is safe.

## 11. Theme system design
A theme in Voltline is a presentation package, not a gameplay ruleset.

Recommended theme responsibilities:
- background palette and layer styling
- line appearance
- player look
- obstacle palette/style treatment
- UI accents
- VFX color/style overrides
- optional audio/music variations

Recommended direction for this repo:
- keep `ThemeConfig` as the semantic top-level asset
- add focused child assets only when needed, such as `PlayerVisualConfig`, `ObstacleVisualConfig`, `BackgroundFxConfig`, optional `ThemeVfxProfile`, and optional `ThemeAudioProfile`
- avoid `switch(themeId)` logic in gameplay code

## 12. Dynamic theme switching
- Best trigger model: milestone- or difficulty-band-based through a future `ThemeSequenceConfig`.
- Switching should be infrequent, short, and readable.
- Preferred transition behavior: brief background crossfade, line blend, subtle ambient VFX pulse, optional mild audio transition.
- Do not switch themes during death resolution, pause, the first moments of a run, or during a teaching pattern.
- Preserve fairness: danger color and silhouette meaning must remain stable during transitions.
- Make thresholds, transition durations, and switching enablement configurable.

## 13. VFX refresh guide
Map VFX to the existing semantic gameplay events:
- flip
- score
- near miss
- milestone
- death
- gap readability
- electric telegraph
- optional theme unlock/theme transition

Guidance:
- theme variation should mostly change palette/material/style, not event meaning
- keep the current semantic registry model via `VfxCatalog`
- use prefab-backed VFX only for signature effects; keep cheap feedback lightweight
- a future `ThemeVfxProfile` is a good fit if theme-specific overrides grow

## 14. SFX/audio refresh guide
Keep the current semantic event map:
- flip, score, near miss, milestone, death, menu click, unlock, main music loop

Guidance:
- theme audio variation should stay light
- prefer reusing core cue identity and varying timbre only where it adds real value
- preserve mixer discipline and gameplay clarity
- avoid creating a second parallel audio lookup path
- if needed later, use a small `ThemeAudioProfile` or themed clip groups inside `AudioCueCatalog`

## 15. Data/config model recommendations
Likely needed additions for a proper refresh:
- `PlayerVisualConfig`
- `CollisionTuningConfig`
- `ObstacleVisualConfig`
- `BackgroundFxConfig`
- `ThemeSequenceConfig`
- optional `ThemeVfxProfile`
- optional `ThemeAudioProfile`

Recommended placement:
- `Assets/_Game/Config/Themes/` for theme and theme-sequence assets
- `Assets/_Game/Config/Gameplay/` for player/obstacle/collision/background gameplay presentation assets
- `Assets/_Game/Config/Audio/` for optional themed audio profiles
- `Assets/_Game/Prefabs/Characters/`, `.../Gameplay/Hazards/`, and `.../VFX/` once authored assets begin replacing procedural visuals

## 16. Required code and prefab touchpoints
Most likely implementation touchpoints in the current repo:
- `Assets/_Game/Scripts/Runtime/Gameplay/PlayerController.cs`
- `Assets/_Game/Scripts/Runtime/Gameplay/HazardManager.cs`
- `Assets/_Game/Scripts/Runtime/Gameplay/TrackManager.cs`
- `Assets/_Game/Scripts/Runtime/Data/GameplayPresentationConfig.cs`
- `Assets/_Game/Scripts/Runtime/Data/HazardPresentationCatalog.cs`
- `Assets/_Game/Scripts/Runtime/Gameplay/GameplaySceneInstaller.cs`
- `Assets/_Game/Scripts/Runtime/Gameplay/GameplayFeedbackCoordinator.cs`
- `Assets/_Game/Scripts/Runtime/UI/MainMenuPreviewView.cs`
- `Assets/_Game/Scripts/Runtime/UI/MainMenuView.cs`
- `Assets/_Game/Scripts/Runtime/UI/SettingsOverlayView.cs`
- `Assets/_Game/Scripts/Runtime/Audio/AudioService.cs`
- `Assets/_Game/Scripts/Runtime/VFX/VfxService.cs`
- `Assets/_Game/Scripts/Runtime/Save/SaveService.cs`
- `Assets/_Game/Scripts/Runtime/Data/ThemeConfig.cs`
- `Assets/_Game/Scripts/Runtime/Data/ThemeCatalog.cs`
- `Assets/_Game/Scripts/Runtime/Data/AudioCueCatalog.cs`
- `Assets/_Game/Scripts/Runtime/Data/VfxCatalog.cs`
- `Assets/_Game/Scenes/MainMenu.unity`
- `Assets/_Game/Scenes/Gameplay.unity`

## 17. Validation and testing plan
Edit Mode:
- config integrity for player/obstacle/background/theme-sequence data
- collision tuning integrity
- theme ID and persistence integrity
- VFX/audio catalog and theme override integrity

Play Mode:
- active theme applies in menu and gameplay
- selected theme persists after relaunch
- retry resets transient presentation state
- dynamic theme switching only occurs at allowed thresholds
- UI/home/pause/results remain stable in themed runs

Manual:
- player readability at speed
- obstacle readability at speed
- collision honesty vs visuals
- theme transition clarity
- background motion distraction risk
- VFX clutter and audio fatigue over repeated retries
- device performance, startup, build size, and restart speed

Highest regression risks:
- visual/collision mismatch
- unfair spacing after art size changes
- dynamic theme changes hiding danger cues
- background motion overpowering the lane
- asset-heavy themes hurting restart speed or frame pacing

## 18. Documentation update requirements
Update the owning docs later if implementation changes their topics:
- `04-architecture.md` for new runtime owners such as a background controller or theme transition owner
- `05-project-structure.md` for new config/prefab/art folder rules
- `06-scene-flow.md` if theme switching changes initialization ownership or scene behavior
- `07-gameplay-systems.md` if collision, spacing, near-miss, or theme-transition timing becomes canonical gameplay truth
- `08-ui-ux-style-guide.md` if theme preview/settings UX changes materially
- `09-audio-vfx-guide.md` if semantic feedback policy changes
- `10-data-content-model.md` for new config families or save fields
- `11-testing-quality-bar.md` for new required checks or performance validation
- `12-adrs.md` for cross-cutting presentation architecture decisions

## 19. Suggested execution phases for this refresh
- Mini-phase A: audit current presentation dependencies and hardcoded assumptions
- Mini-phase B: decouple player visuals from collision
- Mini-phase C: decouple obstacle visuals from collision and spacing
- Mini-phase D: add a dedicated background/speed-feel presentation owner
- Mini-phase E: expand the theme system from color-only to presentation packages
- Mini-phase F: add dynamic theme switching with strict readability rules
- Mini-phase G: align VFX/audio with the refreshed visual language

Exit criteria for each phase:
- A: dependencies are explicit
- B: player art can change without changing fairness silently
- C: obstacle art can change without spacing/collision drift
- D: speed feeling improves without clutter or performance regressions
- E: themes are data-driven and consistent
- F: switching is readable and gameplay-safe
- G: feedback feels cohesive without becoming noisy

## 20. Recommendations
Highest-value path for this repo right now:
- do not start with art swapping
- start by decoupling player visuals and obstacle visuals from collision and spacing ownership
- keep the new presentation config assets as the frozen fairness baseline while decoupling happens
- add the background motion system after silhouettes are stable
- expand themes next
- add dynamic theme switching only after live theme application is safe
- refresh VFX/audio last so they match the final visual language

Safest sequence:
- config first
- collision honesty second
- visual assets third
- theme expansion fourth
- dynamic switching fifth
- final VFX/audio cohesion pass last

