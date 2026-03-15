using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Audio
{
    public static class ProceduralAudioClipLibrary
    {
        private const int SampleRate = 44100;
        private static readonly Dictionary<string, AudioClip> Cache = new();

        public static AudioClip GetClip(string cueId)
        {
            if (Cache.TryGetValue(cueId, out AudioClip clip) && clip != null)
            {
                return clip;
            }

            clip = cueId switch
            {
                AudioCueIds.Flip => CreateFlipClip(),
                AudioCueIds.Score => CreateScoreClip(),
                AudioCueIds.NearMiss => CreateNearMissClip(),
                AudioCueIds.Milestone => CreateMilestoneClip(),
                AudioCueIds.Death => CreateDeathClip(),
                AudioCueIds.UiClick => CreateUiClickClip(),
                AudioCueIds.MainLoop => CreateMusicLoopClip(),
                _ => CreateUiClickClip(),
            };

            Cache[cueId] = clip;
            return clip;
        }

        private static AudioClip CreateFlipClip()
        {
            return CreateClip("proc_flip", 0.055f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float envelope = Mathf.Exp(-40f * t);
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(980f, 1480f, t / 0.055f) * t);
                float overtone = Mathf.Sin(2f * Mathf.PI * 2200f * t) * 0.16f;
                return (tone * 0.8f + overtone) * envelope * 0.45f;
            });
        }

        private static AudioClip CreateScoreClip()
        {
            return CreateClip("proc_score", 0.08f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(640f, 940f, t / 0.08f) * t);
                float envelope = Mathf.Exp(-22f * t);
                return tone * envelope * 0.32f;
            });
        }

        private static AudioClip CreateNearMissClip()
        {
            return CreateClip("proc_nearmiss", 0.11f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float shimmer = Mathf.Sin(2f * Mathf.PI * 1620f * t) + (Mathf.Sin(2f * Mathf.PI * 2140f * t) * 0.45f);
                float envelope = Mathf.Exp(-18f * t);
                return shimmer * envelope * 0.18f;
            });
        }

        private static AudioClip CreateMilestoneClip()
        {
            return CreateClip("proc_milestone", 0.16f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float a = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(520f, 920f, t / 0.16f) * t);
                float b = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(780f, 1320f, t / 0.16f) * t) * 0.55f;
                float envelope = Mathf.Exp(-14f * t);
                return (a + b) * envelope * 0.26f;
            });
        }

        private static AudioClip CreateDeathClip()
        {
            return CreateClip("proc_death", 0.18f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float tone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(220f, 88f, t / 0.18f) * t);
                float noise = (Mathf.PerlinNoise(t * 120f, 0.11f) * 2f) - 1f;
                float envelope = Mathf.Exp(-10f * t);
                return ((tone * 0.75f) + (noise * 0.28f)) * envelope * 0.52f;
            });
        }

        private static AudioClip CreateUiClickClip()
        {
            return CreateClip("proc_ui_click", 0.045f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float tone = Mathf.Sin(2f * Mathf.PI * 760f * t);
                float envelope = Mathf.Exp(-36f * t);
                return tone * envelope * 0.16f;
            });
        }

        private static AudioClip CreateMusicLoopClip()
        {
            return CreateClip("proc_music_loop", 4.2f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float beat = Mathf.Sin(2f * Mathf.PI * 2f * t) * 0.03f;
                float bass = Mathf.Sin(2f * Mathf.PI * 110f * t) * 0.07f;
                float pad = Mathf.Sin(2f * Mathf.PI * 220f * t) * 0.045f;
                float arpRate = 6f;
                float step = Mathf.Repeat(t * arpRate, 1f);
                float note = step < 0.25f ? 440f : step < 0.5f ? 554f : step < 0.75f ? 660f : 494f;
                float arp = Mathf.Sin(2f * Mathf.PI * note * t) * 0.028f;
                return bass + pad + arp + beat;
            });
        }

        private static AudioClip CreateClip(string name, float durationSeconds, System.Func<int, float> sampleGenerator)
        {
            int sampleCount = Mathf.Max(1, Mathf.RoundToInt(durationSeconds * SampleRate));
            float[] samples = new float[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                samples[i] = Mathf.Clamp(sampleGenerator(i), -1f, 1f);
            }

            AudioClip clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }
    }
}
