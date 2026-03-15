using UnityEngine;
using Voltline.Utilities;

namespace Voltline.VFX
{
    public sealed class TransientVfxSprite : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;
        private Vector3 velocity;
        private float lifetime;
        private float elapsed;
        private float startScale;
        private float endScale;
        private float startAlpha;
        private float endAlpha;

        public void Initialize(Color color, float startScale, float endScale, float lifetime, Vector3 velocity, int sortingOrder)
        {
            this.lifetime = Mathf.Max(0.01f, lifetime);
            this.velocity = velocity;
            this.startScale = startScale;
            this.endScale = endScale;
            startAlpha = color.a;
            endAlpha = 0f;

            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = RuntimeSpriteFactory.WhiteSprite;
            spriteRenderer.sortingOrder = sortingOrder;
            spriteRenderer.color = color;
            transform.localScale = Vector3.one * startScale;
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            transform.position += velocity * Time.deltaTime;
            transform.localScale = Vector3.one * Mathf.Lerp(startScale, endScale, eased);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, eased);
                spriteRenderer.color = color;
            }

            if (elapsed >= lifetime)
            {
                Destroy(gameObject);
            }
        }
    }
}
