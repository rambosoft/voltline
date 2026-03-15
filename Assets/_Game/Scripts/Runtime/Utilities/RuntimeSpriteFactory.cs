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
                        new Rect(0f, 0f, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
                        new Vector2(0.5f, 0.5f),
                        100f);
                    whiteSprite.name = "RuntimeWhiteSprite";
                }

                return whiteSprite;
            }
        }
    }
}