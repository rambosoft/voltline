using UnityEngine;
using Voltline.Data;

namespace Voltline.Gameplay
{
    public sealed class PlayerController : MonoBehaviour
    {
        private GameBalanceConfig gameBalance;
        private GameplayPresentationConfig presentationConfig;
        private PlayerVisualConfig visualConfig;
        private ThemeConfig theme;
        private TrackManager trackManager;
        private PlayerVisualView visualView;

        private PlayerSide currentSide;
        private PlayerSide targetSide;
        private float currentX;
        private float currentOffsetFromCenter;
        private float flipElapsed;
        private float flipFromOffset;
        private float flipToOffset;
        private bool isFlipping;
        private bool isDead;

        public event System.Action<Vector3> Flipped;

        public PlayerSide CurrentSide => currentSide;
        public bool IsDead => isDead;
        public bool IsFlipping => isFlipping;
        public float CurrentX => currentX;
        public float CollisionHalfWidth => presentationConfig.PlayerCollisionHalfWidth;
        public float CollisionHalfHeight => presentationConfig.PlayerCollisionHalfHeight;
        public Vector3 WorldPosition => new(currentX, trackManager != null ? trackManager.PlayerAnchorY : 0f, 0f);
        public bool HasVisualView => visualView != null && visualView.IsInitialized;

        public void Initialize(
            GameBalanceConfig balanceConfig,
            GameplayPresentationConfig gameplayPresentation,
            PlayerVisualConfig playerVisual,
            TrackManager track,
            ThemeConfig activeTheme)
        {
            gameBalance = balanceConfig;
            presentationConfig = gameplayPresentation;
            visualConfig = playerVisual;
            trackManager = track;
            theme = activeTheme;
            EnsureVisualView();
            ResetRun();
        }

        public void ResetRun()
        {
            currentSide = PlayerSide.Top;
            targetSide = PlayerSide.Top;
            isFlipping = false;
            isDead = false;
            flipElapsed = 0f;
            currentOffsetFromCenter = GetOffsetForSide(PlayerSide.Top);
            currentX = trackManager.GetTrackCenterX(trackManager.PlayerAnchorY) + currentOffsetFromCenter;
            ApplyVisualState(theme.PlayerAccentColor, 0f);
        }

        public bool RequestFlip()
        {
            if (isDead || isFlipping)
            {
                return false;
            }

            targetSide = currentSide == PlayerSide.Top ? PlayerSide.Bottom : PlayerSide.Top;
            flipFromOffset = currentOffsetFromCenter;
            flipToOffset = GetOffsetForSide(targetSide);
            flipElapsed = 0f;
            isFlipping = true;
            Flipped?.Invoke(WorldPosition);
            return true;
        }

        public void Tick(float deltaTime)
        {
            float trackCenterX = trackManager.GetTrackCenterX(trackManager.PlayerAnchorY);
            if (isFlipping)
            {
                flipElapsed += deltaTime;
                float t = Mathf.Clamp01(flipElapsed / gameBalance.FlipDurationSeconds);
                float eased = Mathf.SmoothStep(0f, 1f, t);
                currentOffsetFromCenter = Mathf.Lerp(flipFromOffset, flipToOffset, eased);
                currentX = trackCenterX + currentOffsetFromCenter;
                if (t >= 1f)
                {
                    isFlipping = false;
                    currentSide = targetSide;
                    currentOffsetFromCenter = flipToOffset;
                    currentX = trackCenterX + currentOffsetFromCenter;
                }
            }
            else
            {
                currentOffsetFromCenter = GetOffsetForSide(currentSide);
                currentX = trackCenterX + currentOffsetFromCenter;
            }

            ApplyVisualState(isDead ? theme.DangerColor : theme.PlayerAccentColor, isFlipping ? 25f : 0f);
        }

        public void MarkDead()
        {
            isDead = true;
            isFlipping = false;
            ApplyVisualState(theme.DangerColor, 45f);
        }

        private void EnsureVisualView()
        {
            if (visualView != null)
            {
                return;
            }

            GameObject viewRoot = new("PlayerVisualRoot");
            viewRoot.transform.SetParent(trackManager.PlayerRoot, false);
            visualView = viewRoot.AddComponent<PlayerVisualView>();
            visualView.Initialize(trackManager.PlayerRoot, visualConfig);
        }

        private void ApplyVisualState(Color tint, float zRotation)
        {
            if (visualView == null)
            {
                return;
            }

            visualView.Apply(new PlayerVisualState(WorldPosition, tint, zRotation));
        }

        private float GetOffsetForSide(PlayerSide side)
        {
            return side == PlayerSide.Top ? gameBalance.SideOffset : -gameBalance.SideOffset;
        }
    }
}
