namespace Voltline.VFX
{
    using Voltline.Data;

    public static class VfxCueIds
    {
        public const string Flip = "vfx.flip.default";
        public const string FlipLiveWireCity = "vfx.flip.live-wire-city";
        public const string NearMiss = "vfx.near-miss.default";
        public const string NearMissLiveWireCity = "vfx.near-miss.live-wire-city";
        public const string Death = "vfx.death.default";
        public const string DeathLiveWireCityGrounded = "vfx.death.live-wire-city.grounded";
        public const string DeathLiveWireCitySharp = "vfx.death.live-wire-city.sharp";
        public const string DeathLiveWireCityElectric = "vfx.death.live-wire-city.electric";
        public const string DeathLiveWireCityRotating = "vfx.death.live-wire-city.rotating";
        public const string DeathLiveWireCityBroken = "vfx.death.live-wire-city.broken";
        public const string DeathLiveWireCitySide = "vfx.death.live-wire-city.side";
        public const string Milestone = "vfx.milestone.default";
        public const string MilestoneLiveWireCity = "vfx.milestone.live-wire-city";

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
