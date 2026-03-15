using UnityEngine;

namespace Voltline.UI
{
    public sealed class OverlayTransitionController : MonoBehaviour
    {
        private const float TransitionDurationSeconds = 0.22f;
        private const float HiddenScale = 0.96f;

        private RectTransform overlayRoot;
        private RectTransform panelRoot;
        private CanvasGroup overlayCanvasGroup;
        private CanvasGroup panelCanvasGroup;
        private Vector2 shownPosition;
        private Vector2 hiddenPosition;
        private float progress;
        private bool targetVisible;

        public bool IsVisible => targetVisible;

        public void Initialize(RectTransform overlay, RectTransform panel, float hiddenYOffset = -24f)
        {
            overlayRoot = overlay;
            panelRoot = panel;
            overlayCanvasGroup = EnsureCanvasGroup(overlayRoot);
            panelCanvasGroup = EnsureCanvasGroup(panelRoot);
            shownPosition = panelRoot.anchoredPosition;
            hiddenPosition = shownPosition + new Vector2(0f, hiddenYOffset);
            progress = 0f;
            targetVisible = false;
            ApplyState();
            overlayRoot.gameObject.SetActive(false);
            enabled = false;
        }

        public void Show()
        {
            if (overlayRoot == null)
            {
                return;
            }

            overlayRoot.gameObject.SetActive(true);
            targetVisible = true;
            enabled = true;
            ApplyInteractionState();
        }

        public void Hide()
        {
            if (overlayRoot == null)
            {
                return;
            }

            targetVisible = false;
            enabled = true;
            ApplyInteractionState();
        }

        public void HideImmediate()
        {
            if (overlayRoot == null)
            {
                return;
            }

            targetVisible = false;
            progress = 0f;
            ApplyState();
            overlayRoot.gameObject.SetActive(false);
            enabled = false;
        }

        private void Update()
        {
            if (overlayRoot == null || panelRoot == null)
            {
                enabled = false;
                return;
            }

            float target = targetVisible ? 1f : 0f;
            progress = Mathf.MoveTowards(progress, target, Time.unscaledDeltaTime / TransitionDurationSeconds);
            ApplyState();

            if (Mathf.Approximately(progress, target))
            {
                if (!targetVisible)
                {
                    overlayRoot.gameObject.SetActive(false);
                }

                enabled = false;
            }
        }

        private void ApplyState()
        {
            if (overlayRoot == null || panelRoot == null)
            {
                return;
            }

            overlayCanvasGroup = EnsureCanvasGroup(overlayRoot);
            panelCanvasGroup = EnsureCanvasGroup(panelRoot);
            if (overlayCanvasGroup == null || panelCanvasGroup == null)
            {
                return;
            }

            float eased = Mathf.SmoothStep(0f, 1f, progress);
            overlayCanvasGroup.alpha = eased;
            panelCanvasGroup.alpha = eased;
            panelRoot.anchoredPosition = Vector2.Lerp(hiddenPosition, shownPosition, eased);
            float scale = Mathf.Lerp(HiddenScale, 1f, eased);
            panelRoot.localScale = new Vector3(scale, scale, 1f);
            ApplyInteractionState();
        }

        private void ApplyInteractionState()
        {
            if (overlayRoot == null)
            {
                return;
            }

            overlayCanvasGroup = EnsureCanvasGroup(overlayRoot);
            if (overlayCanvasGroup == null)
            {
                return;
            }

            overlayCanvasGroup.blocksRaycasts = targetVisible && progress > 0.2f;
            overlayCanvasGroup.interactable = targetVisible && progress > 0.95f;
        }

        private static CanvasGroup EnsureCanvasGroup(RectTransform rectTransform)
        {
            if (rectTransform == null)
            {
                return null;
            }

            CanvasGroup canvasGroup = rectTransform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rectTransform.gameObject.AddComponent<CanvasGroup>();
            }

            return canvasGroup;
        }
    }
}
