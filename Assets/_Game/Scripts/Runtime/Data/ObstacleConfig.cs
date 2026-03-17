using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_Obstacle", menuName = "Voltline/Config/Obstacle")]
    public sealed class ObstacleConfig : ScriptableObject
    {
        [SerializeField] private string obstacleId = "obstacle.Sharp Utility Hazard.basic";
        [SerializeField] private string displayName = "Sharp Utility Hazard";
        [SerializeField] private ObstacleFamily family = ObstacleFamily.SharpUtilityHazards;
        [SerializeField] private int allowedFromScore;
        [SerializeField] private int allowedToScore = -1;
        [SerializeField] private float minSpawnSpacing = 1.5f;
        [SerializeField] private float maxSpawnSpacing = 2.5f;
        [SerializeField] private bool requiresTelegraph;
        [SerializeField] private float minimumTelegraphSeconds = 0.75f;
        [SerializeField] private float weight = 1f;
        [SerializeField] private bool enabledForRuntime = true;

        public string ObstacleId => obstacleId;
        public string DisplayName => displayName;
        public ObstacleFamily Family => family;
        public int AllowedFromScore => allowedFromScore;
        public int AllowedToScore => allowedToScore;
        public float MinSpawnSpacing => minSpawnSpacing;
        public float MaxSpawnSpacing => maxSpawnSpacing;
        public bool RequiresTelegraph => requiresTelegraph;
        public float MinimumTelegraphSeconds => minimumTelegraphSeconds;
        public float Weight => weight;
        public bool EnabledForRuntime => enabledForRuntime;
    }
}
