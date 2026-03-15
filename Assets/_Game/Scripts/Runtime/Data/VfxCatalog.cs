using System;
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

        [Serializable]
        public sealed class VfxDefinition
        {
            [SerializeField] private string vfxId = "vfx.flip.default";
            [SerializeField] private GameObject prefab;
            [SerializeField] private float scale = 1f;
            [SerializeField] private VfxLifetimeCategory lifetimeCategory = VfxLifetimeCategory.Short;
            [SerializeField] private bool usePoolingHint;

            public string VfxId => vfxId;
            public GameObject Prefab => prefab;
            public float Scale => scale;
            public VfxLifetimeCategory LifetimeCategory => lifetimeCategory;
            public bool UsePoolingHint => usePoolingHint;
        }

        [SerializeField] private List<VfxDefinition> entries = new();

        public IReadOnlyList<VfxDefinition> Entries => entries;
    }
}