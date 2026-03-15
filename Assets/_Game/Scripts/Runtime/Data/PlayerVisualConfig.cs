using UnityEngine;
using Voltline.Utilities;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CFG_PlayerVisual_Default", menuName = "Voltline/Config/Player Visual")]
    public sealed class PlayerVisualConfig : ScriptableObject
    {
        [SerializeField] private GameObject visualPrefab;
        [SerializeField] private Sprite fallbackSprite;
        [SerializeField] private Material overrideMaterial;
        [SerializeField] private Color fallbackColor = Color.white;
        [SerializeField] private bool applyRuntimeTint = true;
        [SerializeField] private Vector2 visibleBoundsScale = new(0.68f, 0.68f);
        [SerializeField] private Vector2 localOffset = Vector2.zero;
        [SerializeField] private float baseRotationOffset;
        [SerializeField] private int sortingOrder = 4;
        [SerializeField] private Vector2 menuPreviewSize = new(88f, 88f);
        [SerializeField] private Vector2 menuPreviewOffset = new(84f, 116f);

        public GameObject VisualPrefab => visualPrefab;
        public Sprite FallbackSprite => fallbackSprite != null ? fallbackSprite : RuntimeSpriteFactory.WhiteSprite;
        public Material OverrideMaterial => overrideMaterial;
        public Color FallbackColor => fallbackColor;
        public bool ApplyRuntimeTint => applyRuntimeTint;
        public Vector2 VisibleBoundsScale => visibleBoundsScale;
        public Vector2 LocalOffset => localOffset;
        public float BaseRotationOffset => baseRotationOffset;
        public int SortingOrder => sortingOrder;
        public Vector2 MenuPreviewSize => menuPreviewSize;
        public Vector2 MenuPreviewOffset => menuPreviewOffset;
        public float VisibleHalfWidth => visibleBoundsScale.x * 0.5f;
        public float VisibleHalfHeight => visibleBoundsScale.y * 0.5f;
    }
}
