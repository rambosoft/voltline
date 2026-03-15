using UnityEngine;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.Gameplay
{
    public sealed class PlayerController : MonoBehaviour
    {
        private GameBalanceConfig gameBalance;
        private ThemeConfig theme;
        private TrackManager trackManager;
        private Transform visualRoot;
        private SpriteRenderer spriteRenderer;

        private PlayerSide currentSide;
        private PlayerSide targetSide;
        private float currentX;
        private float flipElapsed;
        private float flipFromX;
        private float flipToX;
        private bool isFlipping;
        private bool isDead;

        public PlayerSide CurrentSide => currentSide;
        public bool IsDead => isDead;
        public bool IsFlipping => isFlipping;
        public float CurrentX => currentX;
        public float CollisionHalfWidth => GameplayPresentationTuning.PlayerCollisionHalfWidth;
        public float CollisionHalfHeight => GameplayPresentationTuning.PlayerCollisionHalfHeight;

        public void Initialize(GameBalanceConfig balanceConfig, TrackManager track, ThemeConfig activeTheme)
        {
            gameBalance = balanceConfig;
            trackManager = track;
            theme = activeTheme;
            EnsureVisual();
            ResetRun();
        }

        public void ResetRun()
        {
            currentSide = PlayerSide.Top;
            targetSide = PlayerSide.Top;
            isFlipping = false;
            isDead = false;
            flipElapsed = 0f;
            currentX = trackManager.GetSideX(PlayerSide.Top);
            ApplyVisualState(theme.PlayerAccentColor, 0f);
        }

        public bool RequestFlip()
        {
            if (isDead || isFlipping)
            {
                return false;
            }

            targetSide = currentSide == PlayerSide.Top ? PlayerSide.Bottom : PlayerSide.Top;
            flipFromX = currentX;
            flipToX = trackManager.GetSideX(targetSide);
            flipElapsed = 0f;
            isFlipping = true;
            return true;
        }

        public void Tick(float deltaTime)
        {
            if (isFlipping)
            {
                flipElapsed += deltaTime;
                float t = Mathf.Clamp01(flipElapsed / gameBalance.FlipDurationSeconds);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                currentX = Mathf.Lerp(flipFromX, flipToX, eased);
                if (t >= 1f)
                {
                    isFlipping = false;
                    currentSide = targetSide;
                    currentX = flipToX;
                }
            }

            ApplyVisualState(isDead ? theme.DangerColor : theme.PlayerAccentColor, isFlipping ? 25f : 0f);
        }

        public void MarkDead()
        {
            isDead = true;
            isFlipping = false;
            ApplyVisualState(theme.DangerColor, 45f);
        }

        private void EnsureVisual()
        {
            visualRoot ??= new GameObject("Player").transform;
            visualRoot.SetParent(trackManager.PlayerRoot, false);

            spriteRenderer = visualRoot.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = visualRoot.gameObject.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = RuntimeSpriteFactory.WhiteSprite;
                spriteRenderer.sortingOrder = 4;
            }
        }

        private void ApplyVisualState(Color tint, float zRotation)
        {
            if (visualRoot == null || spriteRenderer == null)
            {
                return;
            }

            visualRoot.position = new Vector3(currentX, trackManager.PlayerAnchorY, 0f);
            visualRoot.localScale = new Vector3(GameplayPresentationTuning.PlayerScale, GameplayPresentationTuning.PlayerScale, 1f);
            visualRoot.rotation = Quaternion.Euler(0f, 0f, zRotation);
            spriteRenderer.color = tint;
        }
    }
}
