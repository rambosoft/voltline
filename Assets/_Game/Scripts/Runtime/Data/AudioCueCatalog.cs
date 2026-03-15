using System;
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

        [Serializable]
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

        public IReadOnlyList<AudioCueDefinition> Entries => entries;
    }
}