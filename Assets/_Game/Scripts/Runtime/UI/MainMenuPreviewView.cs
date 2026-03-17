using UnityEngine;
using UnityEngine.UI;
using Voltline.Data;
using Voltline.Utilities;

namespace Voltline.UI
{
    public sealed class MainMenuPreviewView : MonoBehaviour
    {
        private RectTransform playerRect;
        private RectTransform frontHazardRect;
        private RectTransform rearHazardRect;
        private RectTransform lineRect;
        private RectTransform glowRect;
        private float elapsed;
        private Vector2 playerBaseOffset;

        public void Initialize(RectTransform parent, ThemeConfig activeTheme, ThemeCatalog themeCatalog, PlayerVisualConfig playerVisualConfig, ObstacleVisualCatalog obstacleVisualCatalog)
        {
            UIThemeConfig uiThemeConfig = themeCatalog != null ? themeCatalog.UiThemeConfig : null;
            WorldDistrictStateDefinition district = activeTheme != null && activeTheme.WorldProgressionConfig != null
                ? activeTheme.WorldProgressionConfig.GetRequiredDistrictForScore(0)
                : null;
            Color backgroundColor = district != null ? Color.Lerp(district.BackgroundTopColor, district.BackgroundBottomColor, 0.5f) : activeTheme.BackgroundBottomColor;
            Color accentColor = district != null ? district.AccentColor : activeTheme.PlayerAccentColor;
            Color dangerColor = district != null ? district.DangerColor : activeTheme.DangerColor;
            Color lineGlowColor = district != null ? district.LineGlowColor : activeTheme.LineGlowColor;

            RectTransform root = UIFactory.CreateSurface("PreviewRoot", parent, uiThemeConfig, UiSurfaceRole.Panel, activeTheme);
            UIFactory.SetAnchors(root, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, -10f), new Vector2(560f, 620f));

            RectTransform innerFrame = UIFactory.CreateSurface("InnerFrame", root, uiThemeConfig, UiSurfaceRole.HighlightPanel, activeTheme);
            UIFactory.Stretch(innerFrame, 18f);

            RectTransform skyBand = UIFactory.CreatePanel("SkyBand", innerFrame, backgroundColor);
            UIFactory.SetAnchors(skyBand, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -92f), new Vector2(520f, 180f));

            CreateCityBlock(innerFrame, new Vector2(-150f, -48f), new Vector2(90f, 180f), new Color(0.05f, 0.1f, 0.17f, 0.9f), accentColor);
            CreateCityBlock(innerFrame, new Vector2(-40f, -28f), new Vector2(120f, 220f), new Color(0.06f, 0.11f, 0.18f, 0.92f), accentColor);
            CreateCityBlock(innerFrame, new Vector2(95f, -38f), new Vector2(110f, 200f), new Color(0.05f, 0.095f, 0.165f, 0.9f), accentColor);

            RectTransform utilityBand = UIFactory.CreatePanel("UtilityBand", innerFrame, new Color(lineGlowColor.r, lineGlowColor.g, lineGlowColor.b, 0.1f));
            UIFactory.SetAnchors(utilityBand, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 18f), new Vector2(420f, 120f));

            glowRect = UIFactory.CreatePanel("PreviewGlow", innerFrame, new Color(lineGlowColor.r, lineGlowColor.g, lineGlowColor.b, 0.08f));
            UIFactory.SetAnchors(glowRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(180f, 480f));

            lineRect = UIFactory.CreatePanel("PreviewLine", innerFrame, lineGlowColor);
            UIFactory.SetAnchors(lineRect, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(18f, 500f));

            ObstacleVisualProfile sharpUtility = obstacleVisualCatalog.GetRequiredProfile(ObstacleFamily.SharpUtilityHazards);
            ObstacleVisualProfile sidePressure = obstacleVisualCatalog.GetRequiredProfile(ObstacleFamily.SidePressureHazards);

            frontHazardRect = UIFactory.CreatePanel("PreviewFrontHazard", innerFrame, dangerColor);
            UIFactory.SetAnchors(frontHazardRect, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(118f, -158f), new Vector2(92f, 138f));
            frontHazardRect.GetComponent<Image>().sprite = sharpUtility.MainSprite;

            rearHazardRect = UIFactory.CreatePanel("PreviewRearHazard", innerFrame, new Color(dangerColor.r, dangerColor.g, dangerColor.b, 0.85f));
            UIFactory.SetAnchors(rearHazardRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-128f, 166f), new Vector2(120f, 164f));
            rearHazardRect.GetComponent<Image>().sprite = sidePressure.MainSprite;

            playerRect = UIFactory.CreatePanel("PreviewPlayer", innerFrame, accentColor);
            playerBaseOffset = playerVisualConfig.MenuPreviewOffset;
            UIFactory.SetAnchors(playerRect, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), playerBaseOffset, playerVisualConfig.MenuPreviewSize);

            Image playerImage = playerRect.GetComponent<Image>();
            playerImage.sprite = playerVisualConfig.FallbackSprite;
            playerImage.preserveAspect = true;
        }

        private static void CreateCityBlock(RectTransform parent, Vector2 anchoredPosition, Vector2 size, Color blockColor, Color windowColor)
        {
            RectTransform block = UIFactory.CreatePanel("CityBlock", parent, blockColor);
            UIFactory.SetAnchors(block, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), anchoredPosition, size);

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 2; column++)
                {
                    RectTransform window = UIFactory.CreatePanel($"Window_{row}_{column}", block, new Color(windowColor.r, windowColor.g, windowColor.b, 0.18f + ((row + column) * 0.05f)));
                    UIFactory.SetAnchors(window, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f + (column * 28f), -28f - (row * 34f)), new Vector2(14f, 18f));
                }
            }
        }

        private void Update()
        {
            if (playerRect == null)
            {
                return;
            }

            elapsed += Time.deltaTime;
            float cycle = Mathf.PingPong(elapsed * 0.82f, 1f);
            float side = Mathf.Lerp(-84f, 84f, cycle);
            float pulse = 1f + (Mathf.Sin(elapsed * 4f) * 0.04f);
            float glowPulse = 1f + (Mathf.Sin(elapsed * 2.1f) * 0.08f);
            float frontHazardPulse = 1f + (Mathf.Sin(elapsed * 3f) * 0.05f);

            playerRect.anchoredPosition = new Vector2(side, playerBaseOffset.y);
            playerRect.localScale = new Vector3(pulse, pulse, 1f);
            frontHazardRect.localScale = new Vector3(1f, frontHazardPulse, 1f);
            rearHazardRect.localScale = new Vector3(1f + (Mathf.Sin(elapsed * 2.4f) * 0.05f), 1f, 1f);
            lineRect.localScale = new Vector3(1f + (Mathf.Sin(elapsed * 2f) * 0.04f), 1f, 1f);
            glowRect.localScale = new Vector3(glowPulse, 1f, 1f);
        }
    }
}
