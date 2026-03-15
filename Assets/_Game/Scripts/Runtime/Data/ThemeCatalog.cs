using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [CreateAssetMenu(fileName = "CAT_ThemeCatalog_Main", menuName = "Voltline/Config/Theme Catalog")]
    public sealed class ThemeCatalog : ScriptableObject
    {
        [SerializeField] private ThemeConfig defaultTheme;
        [SerializeField] private List<ThemeConfig> themes = new();

        public ThemeConfig DefaultTheme => defaultTheme;
        public IReadOnlyList<ThemeConfig> Themes => themes;
        public string DefaultThemeId => defaultTheme != null ? defaultTheme.ThemeId : string.Empty;

        public ThemeConfig ResolveThemeOrDefault(string themeId)
        {
            if (!string.IsNullOrWhiteSpace(themeId))
            {
                for (int i = 0; i < themes.Count; i++)
                {
                    ThemeConfig theme = themes[i];
                    if (theme != null && theme.ThemeId == themeId)
                    {
                        return theme;
                    }
                }
            }

            return defaultTheme != null ? defaultTheme : (themes.Count > 0 ? themes[0] : null);
        }

        public bool TryGetTheme(string themeId, out ThemeConfig theme)
        {
            theme = null;
            if (string.IsNullOrWhiteSpace(themeId))
            {
                return false;
            }

            for (int i = 0; i < themes.Count; i++)
            {
                ThemeConfig candidate = themes[i];
                if (candidate != null && candidate.ThemeId == themeId)
                {
                    theme = candidate;
                    return true;
                }
            }

            return false;
        }
    }
}
