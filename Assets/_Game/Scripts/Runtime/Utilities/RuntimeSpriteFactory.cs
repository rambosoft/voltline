using UnityEngine;

namespace Voltline.Utilities
{
    public static class RuntimeSpriteFactory
    {
        private static Sprite whiteSprite;

        public static Sprite WhiteSprite
        {
            get
            {
                if (whiteSprite == null)
                {
                    whiteSprite = Sprite.Create(
                        Texture2D.whiteTexture,
                        new Rect(0f, 0f, 1f, 1f),
                        new Vector2(0.5f, 0.5f),
                        1f,
                        0u,
                        SpriteMeshType.FullRect,
                        Vector4.zero);
                    whiteSprite.name = "RuntimeWhiteSprite";
                }

                return whiteSprite;
            }
        }
    }
}
