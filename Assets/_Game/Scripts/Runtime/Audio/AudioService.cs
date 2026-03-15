using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using Voltline.Data;
using Voltline.Save;

namespace Voltline.Audio
{
    public sealed class AudioService : MonoBehaviour
    {
        private sealed class AudioSourceSlot
        {
            public AudioSource Source;
            public string CueId;
            public float BaseVolume;
        }

        private static AudioService instance;

        private readonly List<AudioSourceSlot> gameplaySlots = new();
        private readonly List<AudioSourceSlot> uiSlots = new();

        private AudioCueCatalog cueCatalog;
        private SaveService saveService;
        private AudioSource musicSource;
        private AudioMixer audioMixer;
        private AudioMixerGroup musicGroup;
        private AudioMixerGroup gameplayGroup;
        private AudioMixerGroup uiGroup;
        private float musicBaseVolume = 1f;

        public static AudioService Instance => EnsureExists();

        public static AudioService EnsureExists()
        {
            if (instance != null)
            {
                return instance;
            }

            instance = FindFirstObjectByType<AudioService>();
            if (instance != null)
            {
                instance.InitializeIfNeeded();
                return instance;
            }

            GameObject root = new("AudioService");
            instance = root.AddComponent<AudioService>();
            instance.InitializeIfNeeded();
            return instance;
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }

            instance = this;
            InitializeIfNeeded();
        }

        private void OnDestroy()
        {
            if (saveService != null)
            {
                saveService.ProfileChanged -= HandleProfileChanged;
            }
        }

        public void Configure(AudioCueCatalog catalog, SaveService service, AudioMixer mixer = null)
        {
            cueCatalog = catalog;

            if (saveService != null)
            {
                saveService.ProfileChanged -= HandleProfileChanged;
            }

            saveService = service;
            if (saveService != null)
            {
                saveService.ProfileChanged += HandleProfileChanged;
            }

            if (mixer != null)
            {
                audioMixer = mixer;
            }

            ResolveMixerGroups();
            ApplyVolumes();
        }

        public void PlayCue(string cueId)
        {
            AudioCueCatalog.AudioCueDefinition definition = ResolveDefinition(cueId);
            if (definition == null)
            {
                return;
            }

            if (definition.Route == AudioCueCatalog.AudioCueRoute.Music)
            {
                PlayMusicLoop(cueId);
                return;
            }

            List<AudioSourceSlot> pool = definition.Route == AudioCueCatalog.AudioCueRoute.UiSfx ? uiSlots : gameplaySlots;
            CleanupPool(pool);
            if (CountActive(pool, cueId) >= definition.MaxSimultaneousInstances)
            {
                return;
            }

            AudioSourceSlot slot = GetOrCreateSlot(pool, definition.Route == AudioCueCatalog.AudioCueRoute.UiSfx ? "UiSfx" : "GameplaySfx");
            slot.CueId = cueId;
            slot.BaseVolume = Random.Range(definition.MinVolume, definition.MaxVolume);
            slot.Source.loop = false;
            slot.Source.clip = ResolveClip(definition);
            slot.Source.pitch = Random.Range(definition.MinPitch, definition.MaxPitch);
            slot.Source.outputAudioMixerGroup = ResolveRouteGroup(definition.Route);
            slot.Source.volume = ResolveRouteVolume(definition.Route) * slot.BaseVolume;
            slot.Source.Play();
        }

        public void PlayMusicLoop(string cueId)
        {
            AudioCueCatalog.AudioCueDefinition definition = ResolveDefinition(cueId);
            if (definition == null)
            {
                return;
            }

            AudioClip clip = ResolveClip(definition);
            musicBaseVolume = definition.MaxVolume;
            musicSource.outputAudioMixerGroup = ResolveRouteGroup(AudioCueCatalog.AudioCueRoute.Music);
            if (musicSource.clip == clip && musicSource.isPlaying)
            {
                ApplyVolumes();
                return;
            }

            musicSource.clip = clip;
            musicSource.loop = true;
            musicSource.pitch = 1f;
            musicSource.volume = ResolveRouteVolume(AudioCueCatalog.AudioCueRoute.Music) * musicBaseVolume;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource.Stop();
        }

        public void PlayButtonClick()
        {
            PlayCue(AudioCueIds.UiClick);
        }

        private void InitializeIfNeeded()
        {
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }

            if (musicSource != null)
            {
                return;
            }

            musicSource = CreateChildSource("Music");
            musicSource.playOnAwake = false;
            musicSource.loop = true;
        }

        private void HandleProfileChanged()
        {
            ApplyVolumes();
        }

        private void ApplyVolumes()
        {
            if (musicSource != null)
            {
                musicSource.outputAudioMixerGroup = ResolveRouteGroup(AudioCueCatalog.AudioCueRoute.Music);
                musicSource.volume = ResolveRouteVolume(AudioCueCatalog.AudioCueRoute.Music) * musicBaseVolume;
            }

            ApplyPoolVolumes(gameplaySlots, AudioCueCatalog.AudioCueRoute.GameplaySfx);
            ApplyPoolVolumes(uiSlots, AudioCueCatalog.AudioCueRoute.UiSfx);
        }

        private float ResolveRouteVolume(AudioCueCatalog.AudioCueRoute route)
        {
            if (saveService == null)
            {
                return 1f;
            }

            return route == AudioCueCatalog.AudioCueRoute.Music ? saveService.MusicVolume : saveService.SfxVolume;
        }

        private void ApplyPoolVolumes(List<AudioSourceSlot> pool, AudioCueCatalog.AudioCueRoute route)
        {
            float routeVolume = ResolveRouteVolume(route);
            AudioMixerGroup routeGroup = ResolveRouteGroup(route);
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Source != null && pool[i].Source.isPlaying)
                {
                    pool[i].Source.outputAudioMixerGroup = routeGroup;
                    pool[i].Source.volume = routeVolume * Mathf.Max(0f, pool[i].BaseVolume);
                }
            }
        }

        private void ResolveMixerGroups()
        {
            if (audioMixer == null)
            {
                musicGroup = null;
                gameplayGroup = null;
                uiGroup = null;
                return;
            }

            musicGroup = FindExactGroup(audioMixer, "Music");
            gameplayGroup = FindExactGroup(audioMixer, "Gameplay");
            uiGroup = FindExactGroup(audioMixer, "UI");
        }

        private AudioMixerGroup ResolveRouteGroup(AudioCueCatalog.AudioCueRoute route)
        {
            return route switch
            {
                AudioCueCatalog.AudioCueRoute.Music => musicGroup,
                AudioCueCatalog.AudioCueRoute.UiSfx => uiGroup,
                _ => gameplayGroup,
            };
        }

        private AudioCueCatalog.AudioCueDefinition ResolveDefinition(string cueId)
        {
            if (cueCatalog != null && cueCatalog.TryGetDefinition(cueId, out AudioCueCatalog.AudioCueDefinition definition))
            {
                return definition;
            }

            return DefaultAudioCueDefinitions.Get(cueId);
        }

        private static AudioClip ResolveClip(AudioCueCatalog.AudioCueDefinition definition)
        {
            if (definition.Clips != null)
            {
                for (int i = 0; i < definition.Clips.Count; i++)
                {
                    if (definition.Clips[i] != null)
                    {
                        return definition.Clips[i];
                    }
                }
            }

            return ProceduralAudioClipLibrary.GetClip(definition.CueId);
        }

        private static void CleanupPool(List<AudioSourceSlot> pool)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Source != null && !pool[i].Source.isPlaying)
                {
                    pool[i].CueId = null;
                    pool[i].BaseVolume = 0f;
                }
            }
        }

        private static int CountActive(List<AudioSourceSlot> pool, string cueId)
        {
            int count = 0;
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Source != null && pool[i].Source.isPlaying && pool[i].CueId == cueId)
                {
                    count++;
                }
            }

            return count;
        }

        private AudioSourceSlot GetOrCreateSlot(List<AudioSourceSlot> pool, string childName)
        {
            for (int i = 0; i < pool.Count; i++)
            {
                if (pool[i].Source != null && !pool[i].Source.isPlaying)
                {
                    return pool[i];
                }
            }

            AudioSourceSlot slot = new() { Source = CreateChildSource(childName + "_" + pool.Count) };
            pool.Add(slot);
            return slot;
        }

        private AudioSource CreateChildSource(string childName)
        {
            GameObject child = new(childName);
            child.transform.SetParent(transform, false);
            AudioSource source = child.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.loop = false;
            return source;
        }

        private static AudioMixerGroup FindExactGroup(AudioMixer mixer, string groupName)
        {
            if (mixer == null || string.IsNullOrWhiteSpace(groupName))
            {
                return null;
            }

            return mixer.FindMatchingGroups(groupName)
                .FirstOrDefault(group => group != null && group.name == groupName);
        }
    }

    internal static class DefaultAudioCueDefinitions
    {
        private static readonly Dictionary<string, AudioCueCatalog.AudioCueDefinition> Definitions = new()
        {
            { AudioCueIds.Flip, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.Flip, AudioCueCatalog.AudioCueRoute.GameplaySfx, 0.9f, 1f, 0.96f, 1.05f, 1) },
            { AudioCueIds.Score, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.Score, AudioCueCatalog.AudioCueRoute.GameplaySfx, 0.6f, 0.75f, 1f, 1.04f, 2) },
            { AudioCueIds.NearMiss, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.NearMiss, AudioCueCatalog.AudioCueRoute.GameplaySfx, 0.55f, 0.65f, 1f, 1f, 1) },
            { AudioCueIds.Milestone, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.Milestone, AudioCueCatalog.AudioCueRoute.GameplaySfx, 0.72f, 0.82f, 1f, 1f, 1) },
            { AudioCueIds.Death, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.Death, AudioCueCatalog.AudioCueRoute.GameplaySfx, 0.92f, 1f, 1f, 1f, 1) },
            { AudioCueIds.UiClick, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.UiClick, AudioCueCatalog.AudioCueRoute.UiSfx, 0.45f, 0.55f, 1f, 1f, 1) },
            { AudioCueIds.MainLoop, new AudioCueCatalog.AudioCueDefinition(AudioCueIds.MainLoop, AudioCueCatalog.AudioCueRoute.Music, 0.22f, 0.28f, 1f, 1f, 1) }
        };

        public static AudioCueCatalog.AudioCueDefinition Get(string cueId)
        {
            return Definitions.TryGetValue(cueId, out AudioCueCatalog.AudioCueDefinition definition)
                ? definition
                : Definitions[AudioCueIds.UiClick];
        }
    }
}
