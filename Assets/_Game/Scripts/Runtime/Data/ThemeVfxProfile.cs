using System.Collections.Generic;
using UnityEngine;
using Voltline.VFX;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_ThemeVfxProfile_Default", menuName = "Voltline/Config/Theme VFX Profile")]
    public sealed class ThemeVfxProfile : ScriptableObject
    {
        [System.Serializable]
        public sealed class ThemeVfxCueOverride
        {
            [SerializeField] private string cueId = VfxCueIds.Flip;
            [SerializeField] private string overrideVfxId = string.Empty;
            [SerializeField] private float scaleMultiplier = 1f;
            [SerializeField] private float alphaMultiplier = 1f;
            [SerializeField] private int maxBurstCount = 0;

            public string CueId => cueId;
            public string OverrideVfxId => overrideVfxId;
            public float ScaleMultiplier => scaleMultiplier;
            public float AlphaMultiplier => alphaMultiplier;
            public int MaxBurstCount => maxBurstCount;

            public string ResolveVfxId(string fallbackCueId)
            {
                return string.IsNullOrWhiteSpace(overrideVfxId) ? fallbackCueId : overrideVfxId;
            }
        }

        [SerializeField] private int maxActiveTransientEffects = 24;
        [SerializeField] private int maxBurstCountPerEffect = 10;
        [SerializeField] private float minimumReplayCooldownSeconds = 0.04f;
        [SerializeField] private bool allowDeathCameraShake = true;
        [SerializeField] private List<ThemeVfxCueOverride> entries = new();

        private Dictionary<string, ThemeVfxCueOverride> lookup;

        public int MaxActiveTransientEffects => maxActiveTransientEffects;
        public int MaxBurstCountPerEffect => maxBurstCountPerEffect;
        public float MinimumReplayCooldownSeconds => minimumReplayCooldownSeconds;
        public bool AllowDeathCameraShake => allowDeathCameraShake;
        public IReadOnlyList<ThemeVfxCueOverride> Entries => entries;

        public bool TryGetOverride(string cueId, out ThemeVfxCueOverride entry)
        {
            EnsureLookup();
            return lookup.TryGetValue(cueId, out entry);
        }

        private void OnEnable()
        {
            lookup = null;
        }

        private void EnsureLookup()
        {
            if (lookup != null)
            {
                return;
            }

            lookup = new Dictionary<string, ThemeVfxCueOverride>();
            for (int i = 0; i < entries.Count; i++)
            {
                ThemeVfxCueOverride entry = entries[i];
                if (entry != null && !string.IsNullOrWhiteSpace(entry.CueId) && !lookup.ContainsKey(entry.CueId))
                {
                    lookup.Add(entry.CueId, entry);
                }
            }
        }
    }
}
