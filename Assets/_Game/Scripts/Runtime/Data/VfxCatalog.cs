using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CAT_VfxCatalog_Main", menuName = "Voltline/Config/VFX Catalog")]
    public sealed class VfxCatalog : ScriptableObject
    {
        public enum VfxLifetimeCategory
        {
            Instant = 0,
            Short = 1,
            Persistent = 2,
        }

        public enum VfxSpawnMode
        {
            Procedural = 0,
            Prefab = 1,
        }

        [System.Serializable]
        public sealed class VfxDefinition
        {
            [SerializeField] private string vfxId = "vfx.flip.default";
            [SerializeField] private VfxSpawnMode spawnMode = VfxSpawnMode.Procedural;
            [SerializeField] private GameObject prefab;
            [SerializeField] private float scale = 1f;
            [SerializeField] private VfxLifetimeCategory lifetimeCategory = VfxLifetimeCategory.Short;
            [SerializeField] private bool usePoolingHint;

            public VfxDefinition()
            {
            }

            public VfxDefinition(string vfxId, VfxSpawnMode spawnMode, float scale, VfxLifetimeCategory lifetimeCategory, bool usePoolingHint)
            {
                this.vfxId = vfxId;
                this.spawnMode = spawnMode;
                this.scale = scale;
                this.lifetimeCategory = lifetimeCategory;
                this.usePoolingHint = usePoolingHint;
            }

            public string VfxId => vfxId;
            public VfxSpawnMode SpawnMode => spawnMode;
            public GameObject Prefab => prefab;
            public float Scale => scale;
            public VfxLifetimeCategory LifetimeCategory => lifetimeCategory;
            public bool UsePoolingHint => usePoolingHint;
        }

        [SerializeField] private List<VfxDefinition> entries = new();
        private Dictionary<string, VfxDefinition> lookup;

        public IReadOnlyList<VfxDefinition> Entries => entries;

        public bool TryGetDefinition(string vfxId, out VfxDefinition definition)
        {
            EnsureLookup();
            return lookup.TryGetValue(vfxId, out definition);
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

            lookup = new Dictionary<string, VfxDefinition>();
            for (int i = 0; i < entries.Count; i++)
            {
                VfxDefinition entry = entries[i];
                if (entry != null && !string.IsNullOrWhiteSpace(entry.VfxId) && !lookup.ContainsKey(entry.VfxId))
                {
                    lookup.Add(entry.VfxId, entry);
                }
            }
        }
    }
}
