using System.Collections.Generic;
using UnityEngine;
using Voltline.Data;

namespace Voltline.VFX
{
    public sealed class VfxService : MonoBehaviour
    {
        private readonly Dictionary<string, float> lastPlayTimes = new();

        private VfxCatalog catalog;
        private ThemeConfig theme;
        private ThemeVfxProfile activeProfile;
        private WorldDistrictStateDefinition districtState;
        private Camera targetCamera;
        private Transform root;
        private float shakeTimeRemaining;
        private float shakeMagnitude;
        private Vector3 cameraBaseLocalPosition;

        public string CurrentThemeVfxProfileName => activeProfile != null ? activeProfile.name : string.Empty;
        public string CurrentThemeVfxProfileId => activeProfile != null ? activeProfile.ProfileId : string.Empty;
        public string LastPlayedEffectId { get; private set; } = string.Empty;
        public int ActiveTransientCount => root != null ? root.childCount : 0;

        public void Initialize(VfxCatalog effectCatalog, ThemeConfig activeTheme, Camera gameplayCamera)
        {
            catalog = effectCatalog;
            targetCamera = gameplayCamera;
            root ??= new GameObject("VfxRoot").transform;
            root.SetParent(transform, false);
            if (targetCamera != null)
            {
                cameraBaseLocalPosition = targetCamera.transform.localPosition;
            }

            ApplyTheme(activeTheme);
        }

        public void ApplyTheme(ThemeConfig activeTheme)
        {
            theme = activeTheme;
            activeProfile = activeTheme != null ? activeTheme.ThemeVfxProfile : null;
            districtState = null;
        }

        public void ApplyWorldDistrict(WorldDistrictStateDefinition district)
        {
            districtState = district;
        }

        public void PlayEffect(string vfxId, Vector3 position, float scaleMultiplier = 1f)
        {
            ThemeVfxProfile.ThemeVfxCueOverride profileOverride = ResolveProfileOverride(vfxId);
            string resolvedVfxId = profileOverride != null ? profileOverride.ResolveVfxId(vfxId) : vfxId;
            VfxCatalog.VfxDefinition definition = ResolveDefinition(resolvedVfxId);
            if (definition == null)
            {
                return;
            }

            float effectiveScale = definition.Scale * scaleMultiplier * (profileOverride != null ? profileOverride.ScaleMultiplier : 1f);
            if (!CanSpawnTransient(definition.SpawnMode == VfxCatalog.VfxSpawnMode.Prefab ? 1 : EstimateTransientCount(resolvedVfxId, profileOverride)))
            {
                return;
            }

            LastPlayedEffectId = resolvedVfxId;
            if (definition.SpawnMode == VfxCatalog.VfxSpawnMode.Prefab && definition.Prefab != null)
            {
                GameObject instance = Instantiate(definition.Prefab, position, Quaternion.identity, root);
                instance.transform.localScale *= effectiveScale;
                return;
            }

            SpawnProcedural(resolvedVfxId, position, effectiveScale, profileOverride);
        }

        private void Update()
        {
            if (targetCamera == null)
            {
                return;
            }

            if (shakeTimeRemaining > 0f)
            {
                shakeTimeRemaining -= Time.deltaTime;
                Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
                targetCamera.transform.localPosition = cameraBaseLocalPosition + new Vector3(offset.x, offset.y, 0f);
                if (shakeTimeRemaining <= 0f)
                {
                    targetCamera.transform.localPosition = cameraBaseLocalPosition;
                }
            }
        }

        private void SpawnProcedural(string vfxId, Vector3 position, float scale, ThemeVfxProfile.ThemeVfxCueOverride profileOverride)
        {
            float now = Time.time;
            float cooldown = activeProfile != null ? activeProfile.MinimumReplayCooldownSeconds : 0.04f;
            if (lastPlayTimes.TryGetValue(vfxId, out float previousTime) && now - previousTime < cooldown)
            {
                return;
            }

            lastPlayTimes[vfxId] = now;
            float alphaMultiplier = profileOverride != null ? profileOverride.AlphaMultiplier : 1f;
            Color lineGlow = ResolveLineGlowColor(alphaMultiplier);
            Color accent = ResolveAccentColor(alphaMultiplier);
            Color milestone = ResolveMilestoneColor(alphaMultiplier);
            Color danger = ResolveDangerColor(alphaMultiplier);
            Color whiteCore = WithAlpha(new Color(1f, 1f, 1f, 0.94f), alphaMultiplier);
            Color goldCore = WithAlpha(new Color(1f, 0.87f, 0.35f, 0.9f), alphaMultiplier);

            switch (vfxId)
            {
                case VfxCueIds.Flip:
                case VfxCueIds.FlipLiveWireCity:
                    SpawnPulse(position, lineGlow, new Vector2(0.18f, 0.18f) * scale, new Vector2(0.64f, 0.64f) * scale, 0.12f, 8);
                    SpawnStreak(position, accent, new Vector2(0.08f, 0.42f) * scale, new Vector2(0.03f, 0.88f) * scale, 0.09f, new Vector3(0f, 0.35f * scale, 0f), 32f, 9);
                    SpawnRadialBurst(position, whiteCore, ResolveBurstCount(6, profileOverride), 0.14f * scale, 0.11f, 9, new Vector2(0.035f, 0.12f), new Vector2(0.015f, 0.26f));
                    break;

                case VfxCueIds.NearMiss:
                case VfxCueIds.NearMissLiveWireCity:
                    SpawnStreak(position, whiteCore, new Vector2(0.04f, 0.3f) * scale, new Vector2(0.01f, 0.7f) * scale, 0.08f, new Vector3(0f, 0.18f * scale, 0f), -18f, 9);
                    SpawnPulse(position, WithAlpha(new Color(1f, 0.98f, 0.88f, 0.85f), alphaMultiplier), new Vector2(0.08f, 0.08f) * scale, new Vector2(0.34f, 0.34f) * scale, 0.1f, 8);
                    SpawnRadialBurst(position, accent, ResolveBurstCount(4, profileOverride), 0.12f * scale, 0.07f, 8, new Vector2(0.028f, 0.08f), new Vector2(0.01f, 0.18f));
                    break;

                case VfxCueIds.Milestone:
                case VfxCueIds.MilestoneLiveWireCity:
                    SpawnPulse(position, milestone, new Vector2(0.18f, 0.18f) * scale, new Vector2(0.96f, 0.96f) * scale, 0.2f, 8);
                    SpawnPulse(position, goldCore, new Vector2(0.12f, 0.12f) * scale, new Vector2(0.68f, 0.68f) * scale, 0.16f, 9);
                    SpawnRadialBurst(position, milestone, ResolveBurstCount(10, profileOverride), 0.32f * scale, 0.16f, 9, new Vector2(0.03f, 0.14f), new Vector2(0.02f, 0.28f));
                    SpawnRadialBurst(position, goldCore, ResolveBurstCount(6, profileOverride), 0.24f * scale, 0.18f, 10, new Vector2(0.04f, 0.16f), new Vector2(0.02f, 0.22f));
                    break;

                case VfxCueIds.Death:
                    SpawnPulse(position, danger, new Vector2(0.2f, 0.2f) * scale, new Vector2(1.08f, 1.08f) * scale, 0.24f, 8);
                    SpawnRadialBurst(position, danger, ResolveBurstCount(10, profileOverride), 0.54f * scale, 0.2f, 9, new Vector2(0.05f, 0.18f), new Vector2(0.02f, 0.3f));
                    TriggerDeathShake();
                    break;

                case VfxCueIds.DeathLiveWireCityGrounded:
                    SpawnPulse(position, danger, new Vector2(0.22f, 0.22f) * scale, new Vector2(0.94f, 0.94f) * scale, 0.22f, 8);
                    SpawnRadialBurst(position, WithAlpha(new Color(1f, 0.72f, 0.35f, 0.88f), alphaMultiplier), ResolveBurstCount(8, profileOverride), 0.36f * scale, 0.18f, 9, new Vector2(0.05f, 0.18f), new Vector2(0.02f, 0.24f));
                    SpawnStreak(position + new Vector3(0f, -0.04f, 0f), WithAlpha(new Color(1f, 0.82f, 0.55f, 0.74f), alphaMultiplier), new Vector2(0.18f, 0.12f) * scale, new Vector2(0.42f, 0.04f) * scale, 0.14f, new Vector3(0f, -0.12f, 0f), 0f, 10);
                    TriggerDeathShake();
                    break;

                case VfxCueIds.DeathLiveWireCitySharp:
                    SpawnStreak(position, whiteCore, new Vector2(0.06f, 0.78f) * scale, new Vector2(0.02f, 1.05f) * scale, 0.08f, Vector3.zero, 28f, 10);
                    SpawnStreak(position, danger, new Vector2(0.04f, 0.84f) * scale, new Vector2(0.01f, 1.16f) * scale, 0.09f, Vector3.zero, -22f, 9);
                    SpawnRadialBurst(position, danger, ResolveBurstCount(8, profileOverride), 0.4f * scale, 0.12f, 9, new Vector2(0.02f, 0.12f), new Vector2(0.01f, 0.24f));
                    TriggerDeathShake();
                    break;

                case VfxCueIds.DeathLiveWireCityElectric:
                    SpawnPulse(position, WithAlpha(new Color(1f, 0.28f, 0.8f, 0.9f), alphaMultiplier), new Vector2(0.18f, 0.18f) * scale, new Vector2(1.1f, 1.1f) * scale, 0.18f, 8);
                    SpawnStreak(position, whiteCore, new Vector2(0.05f, 0.86f) * scale, new Vector2(0.02f, 1.18f) * scale, 0.1f, Vector3.zero, 0f, 10);
                    SpawnRadialBurst(position, accent, ResolveBurstCount(10, profileOverride), 0.52f * scale, 0.16f, 10, new Vector2(0.03f, 0.16f), new Vector2(0.01f, 0.3f));
                    TriggerDeathShake();
                    break;

                case VfxCueIds.DeathLiveWireCityRotating:
                    SpawnPulse(position, danger, new Vector2(0.16f, 0.16f) * scale, new Vector2(0.9f, 0.9f) * scale, 0.18f, 8);
                    for (int i = 0; i < 3; i++)
                    {
                        SpawnStreak(position, WithAlpha(new Color(1f, 0.86f, 0.72f, 0.82f), alphaMultiplier), new Vector2(0.03f, 0.56f) * scale, new Vector2(0.015f, 0.92f) * scale, 0.14f, Vector3.zero, 60f * i, 9);
                    }
                    SpawnRadialBurst(position, danger, ResolveBurstCount(8, profileOverride), 0.48f * scale, 0.14f, 9, new Vector2(0.025f, 0.12f), new Vector2(0.01f, 0.26f));
                    TriggerDeathShake();
                    break;

                case VfxCueIds.DeathLiveWireCityBroken:
                    SpawnPulse(position, WithAlpha(new Color(0.92f, 0.97f, 1f, 0.82f), alphaMultiplier), new Vector2(0.14f, 0.14f) * scale, new Vector2(0.72f, 0.72f) * scale, 0.16f, 8);
                    SpawnRadialBurst(position, accent, ResolveBurstCount(6, profileOverride), 0.18f * scale, 0.14f, 9, new Vector2(0.025f, 0.1f), new Vector2(0.01f, 0.2f));
                    SpawnStreak(position + new Vector3(0f, -0.06f, 0f), lineGlow, new Vector2(0.06f, 0.36f) * scale, new Vector2(0.02f, 0.64f) * scale, 0.14f, new Vector3(0f, -0.38f * scale, 0f), 180f, 9);
                    break;

                case VfxCueIds.DeathLiveWireCitySide:
                    SpawnPulse(position, danger, new Vector2(0.22f, 0.18f) * scale, new Vector2(1.04f, 0.82f) * scale, 0.18f, 8);
                    SpawnStreak(position, danger, new Vector2(0.18f, 0.08f) * scale, new Vector2(0.52f, 0.04f) * scale, 0.12f, new Vector3(0.18f * scale, 0f, 0f), 0f, 9);
                    SpawnRadialBurst(position, WithAlpha(new Color(1f, 0.92f, 0.92f, 0.9f), alphaMultiplier), ResolveBurstCount(7, profileOverride), 0.34f * scale, 0.14f, 9, new Vector2(0.03f, 0.12f), new Vector2(0.01f, 0.22f));
                    TriggerDeathShake();
                    break;
            }
        }

        private int EstimateTransientCount(string vfxId, ThemeVfxProfile.ThemeVfxCueOverride profileOverride)
        {
            return vfxId switch
            {
                VfxCueIds.Flip => 8,
                VfxCueIds.FlipLiveWireCity => 8,
                VfxCueIds.NearMiss => 6,
                VfxCueIds.NearMissLiveWireCity => 6,
                VfxCueIds.Milestone => 14,
                VfxCueIds.MilestoneLiveWireCity => 18,
                _ => 12,
            };
        }

        private int ResolveBurstCount(int defaultCount, ThemeVfxProfile.ThemeVfxCueOverride profileOverride)
        {
            int count = defaultCount;
            if (profileOverride != null && profileOverride.MaxBurstCount > 0)
            {
                count = Mathf.Min(count, profileOverride.MaxBurstCount);
            }

            if (activeProfile != null)
            {
                count = Mathf.Min(count, activeProfile.MaxBurstCountPerEffect);
            }

            return Mathf.Max(1, count);
        }

        private bool CanSpawnTransient(int transientCount)
        {
            int activeBudget = activeProfile != null ? activeProfile.MaxActiveTransientEffects : 24;
            return ActiveTransientCount + transientCount <= activeBudget;
        }

        private ThemeVfxProfile.ThemeVfxCueOverride ResolveProfileOverride(string cueId)
        {
            return activeProfile != null && activeProfile.TryGetOverride(cueId, out ThemeVfxProfile.ThemeVfxCueOverride profileOverride)
                ? profileOverride
                : null;
        }

        private VfxCatalog.VfxDefinition ResolveDefinition(string vfxId)
        {
            if (catalog != null && catalog.TryGetDefinition(vfxId, out VfxCatalog.VfxDefinition definition))
            {
                return definition;
            }

            return DefaultVfxDefinitions.Get(vfxId);
        }

        private void SpawnPulse(Vector3 position, Color color, Vector2 startScale, Vector2 endScale, float lifetime, int sortingOrder)
        {
            GameObject effect = new("VfxPulse");
            effect.transform.SetParent(root, false);
            effect.transform.position = position;
            TransientVfxSprite transient = effect.AddComponent<TransientVfxSprite>();
            transient.Initialize(color, startScale, endScale, lifetime, Vector3.zero, 0f, sortingOrder);
        }

        private void SpawnStreak(Vector3 position, Color color, Vector2 startScale, Vector2 endScale, float lifetime, Vector3 velocity, float rotationDegrees, int sortingOrder)
        {
            GameObject effect = new("VfxStreak");
            effect.transform.SetParent(root, false);
            effect.transform.position = position;
            TransientVfxSprite transient = effect.AddComponent<TransientVfxSprite>();
            transient.Initialize(color, startScale, endScale, lifetime, velocity, rotationDegrees, sortingOrder);
        }

        private void SpawnRadialBurst(Vector3 position, Color color, int count, float radius, float lifetime, int sortingOrder, Vector2 startScale, Vector2 endScale)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (Mathf.PI * 2f * i) / count;
                Vector3 velocity = new(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                GameObject effect = new("VfxBurst");
                effect.transform.SetParent(root, false);
                effect.transform.position = position;
                TransientVfxSprite transient = effect.AddComponent<TransientVfxSprite>();
                transient.Initialize(color, startScale, endScale, lifetime, velocity * radius, Mathf.Rad2Deg * angle, sortingOrder);
            }
        }

        private void TriggerDeathShake()
        {
            if (activeProfile == null || activeProfile.AllowDeathCameraShake)
            {
                shakeTimeRemaining = 0.12f;
                shakeMagnitude = 0.09f;
            }
        }

        private Color ResolveLineGlowColor(float alphaMultiplier)
        {
            Color color = districtState != null ? districtState.LineGlowColor : (theme != null ? theme.LineGlowColor : Color.white);
            return WithAlpha(color, alphaMultiplier);
        }

        private Color ResolveAccentColor(float alphaMultiplier)
        {
            Color color = districtState != null ? districtState.AccentColor : (theme != null ? theme.PlayerAccentColor : Color.white);
            return WithAlpha(color, alphaMultiplier);
        }

        private Color ResolveMilestoneColor(float alphaMultiplier)
        {
            Color color = districtState != null ? districtState.MilestoneColor : (theme != null ? theme.MilestoneColor : Color.white);
            return WithAlpha(color, alphaMultiplier);
        }

        private Color ResolveDangerColor(float alphaMultiplier)
        {
            Color color = districtState != null ? districtState.DangerColor : (theme != null ? theme.DangerColor : Color.white);
            return WithAlpha(color, alphaMultiplier);
        }

        private static Color WithAlpha(Color color, float alphaMultiplier)
        {
            color.a *= Mathf.Clamp(alphaMultiplier, 0.1f, 1f);
            return color;
        }
    }

    internal static class DefaultVfxDefinitions
    {
        private static readonly Dictionary<string, VfxCatalog.VfxDefinition> Definitions = new()
        {
            { VfxCueIds.Flip, new VfxCatalog.VfxDefinition(VfxCueIds.Flip, VfxCatalog.VfxSpawnMode.Procedural, 1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.FlipLiveWireCity, new VfxCatalog.VfxDefinition(VfxCueIds.FlipLiveWireCity, VfxCatalog.VfxSpawnMode.Procedural, 1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.NearMiss, new VfxCatalog.VfxDefinition(VfxCueIds.NearMiss, VfxCatalog.VfxSpawnMode.Procedural, 1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.NearMissLiveWireCity, new VfxCatalog.VfxDefinition(VfxCueIds.NearMissLiveWireCity, VfxCatalog.VfxSpawnMode.Procedural, 1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.Death, new VfxCatalog.VfxDefinition(VfxCueIds.Death, VfxCatalog.VfxSpawnMode.Procedural, 1.1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCityGrounded, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCityGrounded, VfxCatalog.VfxSpawnMode.Procedural, 1.05f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCitySharp, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCitySharp, VfxCatalog.VfxSpawnMode.Procedural, 1.08f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCityElectric, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCityElectric, VfxCatalog.VfxSpawnMode.Procedural, 1.12f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCityRotating, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCityRotating, VfxCatalog.VfxSpawnMode.Procedural, 1.08f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCityBroken, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCityBroken, VfxCatalog.VfxSpawnMode.Procedural, 1.02f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.DeathLiveWireCitySide, new VfxCatalog.VfxDefinition(VfxCueIds.DeathLiveWireCitySide, VfxCatalog.VfxSpawnMode.Procedural, 1.06f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.Milestone, new VfxCatalog.VfxDefinition(VfxCueIds.Milestone, VfxCatalog.VfxSpawnMode.Procedural, 1.15f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.MilestoneLiveWireCity, new VfxCatalog.VfxDefinition(VfxCueIds.MilestoneLiveWireCity, VfxCatalog.VfxSpawnMode.Procedural, 1.18f, VfxCatalog.VfxLifetimeCategory.Short, false) },
        };

        public static VfxCatalog.VfxDefinition Get(string vfxId)
        {
            return Definitions.TryGetValue(vfxId, out VfxCatalog.VfxDefinition definition)
                ? definition
                : Definitions[VfxCueIds.Flip];
        }
    }
}
