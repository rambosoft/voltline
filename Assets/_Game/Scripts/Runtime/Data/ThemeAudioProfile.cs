using System.Collections.Generic;
using UnityEngine;
using Voltline.Audio;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_ThemeAudioProfile_Default", menuName = "Voltline/Config/Theme Audio Profile")]
    public sealed class ThemeAudioProfile : ScriptableObject
    {
        [System.Serializable]
        public sealed class ThemeAudioCueOverride
        {
            [SerializeField] private string cueId = AudioCueIds.Flip;
            [SerializeField] private string overrideCueId = string.Empty;
            [SerializeField] private float volumeMultiplier = 1f;
            [SerializeField] private float pitchMultiplier = 1f;

            public string CueId => cueId;
            public string OverrideCueId => overrideCueId;
            public float VolumeMultiplier => volumeMultiplier;
            public float PitchMultiplier => pitchMultiplier;

            public string ResolveCueId(string fallbackCueId)
            {
                return string.IsNullOrWhiteSpace(overrideCueId) ? fallbackCueId : overrideCueId;
            }
        }

        [SerializeField] private int maxConcurrentGameplayVoices = 4;
        [SerializeField] private int maxConcurrentUiVoices = 1;
        [SerializeField] private float minimumUiClickIntervalSeconds = 0.05f;
        [SerializeField] private float musicVolumeMultiplier = 1f;
        [SerializeField] private List<ThemeAudioCueOverride> entries = new();

        private Dictionary<string, ThemeAudioCueOverride> lookup;

        public int MaxConcurrentGameplayVoices => maxConcurrentGameplayVoices;
        public int MaxConcurrentUiVoices => maxConcurrentUiVoices;
        public float MinimumUiClickIntervalSeconds => minimumUiClickIntervalSeconds;
        public float MusicVolumeMultiplier => musicVolumeMultiplier;
        public IReadOnlyList<ThemeAudioCueOverride> Entries => entries;

        public bool TryGetOverride(string cueId, out ThemeAudioCueOverride entry)
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

            lookup = new Dictionary<string, ThemeAudioCueOverride>();
            for (int i = 0; i < entries.Count; i++)
            {
                ThemeAudioCueOverride entry = entries[i];
                if (entry != null && !string.IsNullOrWhiteSpace(entry.CueId) && !lookup.ContainsKey(entry.CueId))
                {
                    lookup.Add(entry.CueId, entry);
                }
            }
        }
    }
}
