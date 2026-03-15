using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CAT_AudioCueCatalog_Main", menuName = "Voltline/Config/Audio Cue Catalog")]
    public sealed class AudioCueCatalog : ScriptableObject
    {
        public enum AudioCueRoute
        {
            Music = 0,
            GameplaySfx = 1,
            UiSfx = 2,
        }

        [System.Serializable]
        public sealed class AudioCueDefinition
        {
            [SerializeField] private string cueId = "audio.flip.default";
            [SerializeField] private AudioCueRoute route = AudioCueRoute.GameplaySfx;
            [SerializeField] private List<AudioClip> clips = new();
            [SerializeField] private float minVolume = 1f;
            [SerializeField] private float maxVolume = 1f;
            [SerializeField] private float minPitch = 1f;
            [SerializeField] private float maxPitch = 1f;
            [SerializeField] private int maxSimultaneousInstances = 1;

            public AudioCueDefinition()
            {
            }

            public AudioCueDefinition(string cueId, AudioCueRoute route, float minVolume, float maxVolume, float minPitch, float maxPitch, int maxSimultaneousInstances)
            {
                this.cueId = cueId;
                this.route = route;
                this.minVolume = minVolume;
                this.maxVolume = maxVolume;
                this.minPitch = minPitch;
                this.maxPitch = maxPitch;
                this.maxSimultaneousInstances = maxSimultaneousInstances;
            }

            public string CueId => cueId;
            public AudioCueRoute Route => route;
            public IReadOnlyList<AudioClip> Clips => clips;
            public float MinVolume => minVolume;
            public float MaxVolume => maxVolume;
            public float MinPitch => minPitch;
            public float MaxPitch => maxPitch;
            public int MaxSimultaneousInstances => maxSimultaneousInstances;
        }

        [SerializeField] private List<AudioCueDefinition> entries = new();
        private Dictionary<string, AudioCueDefinition> lookup;

        public IReadOnlyList<AudioCueDefinition> Entries => entries;

        public bool TryGetDefinition(string cueId, out AudioCueDefinition definition)
        {
            EnsureLookup();
            return lookup.TryGetValue(cueId, out definition);
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

            lookup = new Dictionary<string, AudioCueDefinition>();
            for (int i = 0; i < entries.Count; i++)
            {
                AudioCueDefinition entry = entries[i];
                if (entry != null && !string.IsNullOrWhiteSpace(entry.CueId) && !lookup.ContainsKey(entry.CueId))
                {
                    lookup.Add(entry.CueId, entry);
                }
            }
        }
    }
}
