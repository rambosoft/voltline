namespace Voltline.Audio
{
    using Voltline.Data;

    public static class AudioCueIds
    {
        public const string Flip = "audio.flip.default";
        public const string FlipLiveWireCity = "audio.flip.live-wire-city";
        public const string Score = "audio.score.default";
        public const string ScoreLiveWireCity = "audio.score.live-wire-city";
        public const string NearMiss = "audio.near-miss.default";
        public const string NearMissLiveWireCity = "audio.near-miss.live-wire-city";
        public const string Milestone = "audio.milestone.default";
        public const string MilestoneLiveWireCity = "audio.milestone.live-wire-city";
        public const string Death = "audio.death.default";
        public const string DeathLiveWireCityGrounded = "audio.death.live-wire-city.grounded";
        public const string DeathLiveWireCitySharp = "audio.death.live-wire-city.sharp";
        public const string DeathLiveWireCityElectric = "audio.death.live-wire-city.electric";
        public const string DeathLiveWireCityRotating = "audio.death.live-wire-city.rotating";
        public const string DeathLiveWireCityBroken = "audio.death.live-wire-city.broken";
        public const string DeathLiveWireCitySide = "audio.death.live-wire-city.side";
        public const string UiClick = "audio.ui.click.default";
        public const string UiClickLiveWireCity = "audio.ui.click.live-wire-city";
        public const string MainLoop = "audio.music.main-loop";
        public const string MainLoopLiveWireCity = "audio.music.main-loop.live-wire-city";

        public static string ResolveDeathCueId(ObstacleFamily? family)
        {
            return family switch
            {
                ObstacleFamily.GroundedBlockers => DeathLiveWireCityGrounded,
                ObstacleFamily.SharpUtilityHazards => DeathLiveWireCitySharp,
                ObstacleFamily.ActiveElectricHazards => DeathLiveWireCityElectric,
                ObstacleFamily.RotatingIndustrialHazards => DeathLiveWireCityRotating,
                ObstacleFamily.BrokenConduitSections => DeathLiveWireCityBroken,
                ObstacleFamily.SidePressureHazards => DeathLiveWireCitySide,
                _ => Death,
            };
        }
    }
}
