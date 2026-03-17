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
        private Vector2 startScale;
        private Vector2 endScale;
        private float startAlpha;
        private float endAlpha;

        public void Initialize(Color color, float startScale, float endScale, float lifetime, Vector3 velocity, int sortingOrder)
        {
            Initialize(color, new Vector2(startScale, startScale), new Vector2(endScale, endScale), lifetime, velocity, 0f, sortingOrder);
        }

        public void Initialize(Color color, Vector2 startScale, Vector2 endScale, float lifetime, Vector3 velocity, float rotationDegrees, int sortingOrder)
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
            transform.localScale = new Vector3(startScale.x, startScale.y, 1f);
            transform.localRotation = Quaternion.Euler(0f, 0f, rotationDegrees);
        }

        private void Update()
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lifetime);
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            transform.position += velocity * Time.deltaTime;
            Vector2 currentScale = Vector2.Lerp(startScale, endScale, eased);
            transform.localScale = new Vector3(currentScale.x, currentScale.y, 1f);

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
