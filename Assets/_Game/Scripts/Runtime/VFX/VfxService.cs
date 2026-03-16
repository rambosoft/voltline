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
        private Camera targetCamera;
        private Transform root;
        private float shakeTimeRemaining;
        private float shakeMagnitude;
        private Vector3 cameraBaseLocalPosition;

        public string CurrentThemeVfxProfileName => activeProfile != null ? activeProfile.name : string.Empty;
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
            Color activeLineGlow = theme != null ? theme.LineGlowColor : Color.white;
            Color activePlayerAccent = theme != null ? theme.PlayerAccentColor : Color.white;
            Color activeMilestone = theme != null ? theme.MilestoneColor : Color.white;
            Color activeDanger = theme != null ? theme.DangerColor : Color.white;

            switch (vfxId)
            {
                case VfxCueIds.Flip:
                    SpawnPulse(position, WithAlpha(activeLineGlow, alphaMultiplier), 0.16f * scale, 0.56f * scale, 0.14f, 6);
                    SpawnPulse(position + new Vector3(0f, 0.08f, 0f), WithAlpha(activePlayerAccent, alphaMultiplier), 0.08f * scale, 0.3f * scale, 0.1f, 7);
                    break;

                case VfxCueIds.NearMiss:
                    SpawnPulse(position, WithAlpha(new Color(1f, 0.96f, 0.8f, 0.95f), alphaMultiplier), 0.1f * scale, 0.38f * scale, 0.12f, 8);
                    SpawnBurst(position, WithAlpha(activePlayerAccent, alphaMultiplier), ResolveBurstCount(4, profileOverride), 0.16f * scale, 0.08f, 8);
                    break;

                case VfxCueIds.Milestone:
                    SpawnPulse(position, WithAlpha(activeMilestone, alphaMultiplier), 0.18f * scale, 0.9f * scale, 0.22f, 6);
                    SpawnBurst(position, WithAlpha(activeMilestone, alphaMultiplier), ResolveBurstCount(8, profileOverride), 0.42f * scale, 0.18f, 7);
                    break;

                case VfxCueIds.Death:
                    SpawnPulse(position, WithAlpha(activeDanger, alphaMultiplier), 0.2f * scale, 1.1f * scale, 0.28f, 8);
                    SpawnBurst(position, WithAlpha(activeDanger, alphaMultiplier), ResolveBurstCount(10, profileOverride), 0.62f * scale, 0.24f, 9);
                    if (activeProfile == null || activeProfile.AllowDeathCameraShake)
                    {
                        shakeTimeRemaining = 0.12f;
                        shakeMagnitude = 0.09f;
                    }
                    break;
            }
        }

        private int EstimateTransientCount(string vfxId, ThemeVfxProfile.ThemeVfxCueOverride profileOverride)
        {
            return vfxId switch
            {
                VfxCueIds.Flip => 2,
                VfxCueIds.NearMiss => 1 + ResolveBurstCount(4, profileOverride),
                VfxCueIds.Milestone => 1 + ResolveBurstCount(8, profileOverride),
                VfxCueIds.Death => 1 + ResolveBurstCount(10, profileOverride),
                _ => 1,
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

        private void SpawnPulse(Vector3 position, Color color, float startScale, float endScale, float lifetime, int sortingOrder)
        {
            GameObject effect = new("VfxPulse");
            effect.transform.SetParent(root, false);
            effect.transform.position = position;
            TransientVfxSprite transient = effect.AddComponent<TransientVfxSprite>();
            transient.Initialize(color, startScale, endScale, lifetime, Vector3.zero, sortingOrder);
        }

        private void SpawnBurst(Vector3 position, Color color, int count, float radius, float lifetime, int sortingOrder)
        {
            for (int i = 0; i < count; i++)
            {
                float angle = (Mathf.PI * 2f * i) / count;
                Vector3 velocity = new(Mathf.Cos(angle), Mathf.Sin(angle), 0f);
                GameObject effect = new("VfxBurst");
                effect.transform.SetParent(root, false);
                effect.transform.position = position;
                TransientVfxSprite transient = effect.AddComponent<TransientVfxSprite>();
                transient.Initialize(color, 0.08f, 0.16f, lifetime, velocity * radius, sortingOrder);
            }
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
            { VfxCueIds.NearMiss, new VfxCatalog.VfxDefinition(VfxCueIds.NearMiss, VfxCatalog.VfxSpawnMode.Procedural, 1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.Death, new VfxCatalog.VfxDefinition(VfxCueIds.Death, VfxCatalog.VfxSpawnMode.Procedural, 1.1f, VfxCatalog.VfxLifetimeCategory.Short, false) },
            { VfxCueIds.Milestone, new VfxCatalog.VfxDefinition(VfxCueIds.Milestone, VfxCatalog.VfxSpawnMode.Procedural, 1.15f, VfxCatalog.VfxLifetimeCategory.Short, false) },
        };

        public static VfxCatalog.VfxDefinition Get(string vfxId)
        {
            return Definitions.TryGetValue(vfxId, out VfxCatalog.VfxDefinition definition)
                ? definition
                : Definitions[VfxCueIds.Flip];
        }
    }
}
