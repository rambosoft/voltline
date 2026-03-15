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
    }
}