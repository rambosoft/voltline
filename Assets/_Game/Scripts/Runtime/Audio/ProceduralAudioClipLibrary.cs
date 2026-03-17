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
                AudioCueIds.FlipLiveWireCity => CreateLiveWireFlipClip(),
                AudioCueIds.Score => CreateScoreClip(),
                AudioCueIds.ScoreLiveWireCity => CreateLiveWireScoreClip(),
                AudioCueIds.NearMiss => CreateNearMissClip(),
                AudioCueIds.NearMissLiveWireCity => CreateLiveWireNearMissClip(),
                AudioCueIds.Milestone => CreateMilestoneClip(),
                AudioCueIds.MilestoneLiveWireCity => CreateLiveWireMilestoneClip(),
                AudioCueIds.Death => CreateDeathClip(),
                AudioCueIds.DeathLiveWireCityGrounded => CreateGroundedDeathClip(),
                AudioCueIds.DeathLiveWireCitySharp => CreateSharpDeathClip(),
                AudioCueIds.DeathLiveWireCityElectric => CreateElectricDeathClip(),
                AudioCueIds.DeathLiveWireCityRotating => CreateRotatingDeathClip(),
                AudioCueIds.DeathLiveWireCityBroken => CreateBrokenDeathClip(),
                AudioCueIds.DeathLiveWireCitySide => CreateSidePressureDeathClip(),
                AudioCueIds.UiClick => CreateUiClickClip(),
                AudioCueIds.UiClickLiveWireCity => CreateLiveWireUiClickClip(),
                AudioCueIds.MainLoop => CreateMusicLoopClip(),
                AudioCueIds.MainLoopLiveWireCity => CreateLiveWireMusicLoopClip(),
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

        private static AudioClip CreateLiveWireFlipClip()
        {
            return CreateClip("proc_flip_live_wire", 0.07f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float sweep = Mathf.Lerp(760f, 1960f, Mathf.Clamp01(t / 0.045f));
                float click = Mathf.Sin(2f * Mathf.PI * sweep * t) * Mathf.Exp(-54f * t);
                float spark = Mathf.Sin(2f * Mathf.PI * 3120f * t) * Mathf.Exp(-72f * t) * 0.2f;
                float subPop = Mathf.Sin(2f * Mathf.PI * 210f * t) * Mathf.Exp(-32f * t) * 0.12f;
                return (click * 0.72f) + spark + subPop;
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

        private static AudioClip CreateLiveWireScoreClip()
        {
            return CreateClip("proc_score_live_wire", 0.095f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float toneA = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(680f, 1010f, t / 0.095f) * t);
                float toneB = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(1020f, 1320f, t / 0.095f) * t) * 0.26f;
                float envelope = Mathf.Exp(-18f * t);
                return (toneA + toneB) * envelope * 0.26f;
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

        private static AudioClip CreateLiveWireNearMissClip()
        {
            return CreateClip("proc_nearmiss_live_wire", 0.13f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float tone = Mathf.Sin(2f * Mathf.PI * 1960f * t);
                float sparkle = Mathf.Sin(2f * Mathf.PI * 2880f * t) * 0.35f;
                float envelope = Mathf.Exp(-14f * t);
                return (tone * 0.18f + sparkle * 0.12f) * envelope;
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

        private static AudioClip CreateLiveWireMilestoneClip()
        {
            return CreateClip("proc_milestone_live_wire", 0.24f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float baseTone = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(420f, 760f, t / 0.24f) * t) * 0.22f;
                float harmony = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(640f, 1260f, t / 0.24f) * t) * 0.16f;
                float sheen = Mathf.Sin(2f * Mathf.PI * 2120f * t) * Mathf.Exp(-18f * t) * 0.08f;
                float envelope = Mathf.Exp(-8f * t);
                return ((baseTone + harmony) * envelope) + sheen;
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

        private static AudioClip CreateGroundedDeathClip()
        {
            return CreateClip("proc_death_grounded", 0.22f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float bass = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(180f, 70f, t / 0.22f) * t) * 0.42f;
                float buzz = Mathf.Sin(2f * Mathf.PI * 92f * t) * Mathf.Exp(-12f * t) * 0.14f;
                float cut = ((Mathf.PerlinNoise(t * 86f, 0.12f) * 2f) - 1f) * Mathf.Exp(-18f * t) * 0.08f;
                return bass + buzz + cut;
            });
        }

        private static AudioClip CreateSharpDeathClip()
        {
            return CreateClip("proc_death_sharp", 0.17f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float crack = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(2800f, 820f, t / 0.04f) * t) * Mathf.Exp(-64f * t) * 0.28f;
                float impact = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(320f, 120f, t / 0.17f) * t) * Mathf.Exp(-12f * t) * 0.34f;
                return crack + impact;
            });
        }

        private static AudioClip CreateElectricDeathClip()
        {
            return CreateClip("proc_death_electric", 0.19f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float zap = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(1480f, 320f, t / 0.19f) * t) * Mathf.Exp(-18f * t) * 0.24f;
                float glitch = (((Mathf.PerlinNoise(t * 240f, 0.3f) * 2f) - 1f) > 0f ? 1f : -1f) * Mathf.Exp(-26f * t) * 0.14f;
                float sub = Mathf.Sin(2f * Mathf.PI * 96f * t) * Mathf.Exp(-8f * t) * 0.16f;
                return zap + glitch + sub;
            });
        }

        private static AudioClip CreateRotatingDeathClip()
        {
            return CreateClip("proc_death_rotating", 0.2f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float whine = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(960f, 240f, t / 0.2f) * t) * Mathf.Exp(-16f * t) * 0.18f;
                float grind = ((Mathf.PerlinNoise(t * 180f, 0.5f) * 2f) - 1f) * Mathf.Exp(-14f * t) * 0.12f;
                float impact = Mathf.Sin(2f * Mathf.PI * 132f * t) * Mathf.Exp(-10f * t) * 0.22f;
                return whine + grind + impact;
            });
        }

        private static AudioClip CreateBrokenDeathClip()
        {
            return CreateClip("proc_death_broken", 0.2f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float drop = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(320f, 42f, t / 0.2f) * t) * Mathf.Exp(-8f * t) * 0.24f;
                float spark = Mathf.Sin(2f * Mathf.PI * 1640f * t) * Mathf.Exp(-26f * t) * 0.1f;
                return drop + spark;
            });
        }

        private static AudioClip CreateSidePressureDeathClip()
        {
            return CreateClip("proc_death_side", 0.19f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float thud = Mathf.Sin(2f * Mathf.PI * Mathf.Lerp(140f, 68f, t / 0.19f) * t) * Mathf.Exp(-9f * t) * 0.32f;
                float snap = Mathf.Sin(2f * Mathf.PI * 1240f * t) * Mathf.Exp(-40f * t) * 0.14f;
                return thud + snap;
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

        private static AudioClip CreateLiveWireUiClickClip()
        {
            return CreateClip("proc_ui_click_live_wire", 0.055f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float tone = Mathf.Sin(2f * Mathf.PI * 920f * t) * Mathf.Exp(-28f * t) * 0.12f;
                float sparkle = Mathf.Sin(2f * Mathf.PI * 1720f * t) * Mathf.Exp(-44f * t) * 0.04f;
                return tone + sparkle;
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

        private static AudioClip CreateLiveWireMusicLoopClip()
        {
            return CreateClip("proc_music_loop_live_wire", 6f, sampleIndex =>
            {
                float t = sampleIndex / (float)SampleRate;
                float bass = Mathf.Sin(2f * Mathf.PI * 82f * t) * 0.07f;
                float subPulse = Mathf.Sin(2f * Mathf.PI * 2.25f * t) * 0.025f;
                float pad = Mathf.Sin(2f * Mathf.PI * 164f * t) * 0.028f;
                float shimmer = Mathf.Sin(2f * Mathf.PI * 328f * t) * 0.012f;
                float arpStep = Mathf.Repeat(t * 4f, 1f);
                float note = arpStep < 0.2f ? 246f : arpStep < 0.4f ? 328f : arpStep < 0.6f ? 392f : arpStep < 0.8f ? 492f : 328f;
                float arp = Mathf.Sin(2f * Mathf.PI * note * t) * 0.022f;
                float gated = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(t * 2f, 0.5f)) * 0.01f;
                return bass + subPulse + pad + shimmer + arp + gated;
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
