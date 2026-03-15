using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CAT_ObstacleCatalog_Main", menuName = "Voltline/Config/Obstacle Catalog")]
    public sealed class ObstacleCatalog : ScriptableObject
    {
        [SerializeField] private List<ObstacleConfig> obstacles = new();

        public IReadOnlyList<ObstacleConfig> Obstacles => obstacles;
    }
}