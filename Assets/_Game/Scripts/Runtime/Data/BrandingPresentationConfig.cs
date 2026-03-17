using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    public enum BrandingMenuEntryId
    {
        Daily = 0,
        Themes = 1,
        Best = 2,
        Settings = 3,
    }

    public enum BrandingMenuEntryState
    {
        Hidden = 0,
        Disabled = 1,
        Enabled = 2,
    }

    [Serializable]
    public sealed class BrandingMenuEntryDefinition
    {
        [SerializeField] private BrandingMenuEntryId entryId;
        [SerializeField] private BrandingMenuEntryState state = BrandingMenuEntryState.Enabled;

        public BrandingMenuEntryId EntryId => entryId;
        public BrandingMenuEntryState State => state;
    }

    [CreateAssetMenu(fileName = "CFG_BrandingPresentation_Voltline", menuName = "Voltline/Config/Branding Presentation")]
    public sealed class BrandingPresentationConfig : ScriptableObject
    {
        [SerializeField] private string publicTitle = "Voltline";
        [SerializeField] private string subtitle = "Keep the grid alive";
        [SerializeField] private Sprite logoSprite;
        [SerializeField] private bool showThemeSelectionInSettings;
        [SerializeField] private bool showSplashLogoMoment = true;
        [SerializeField] private float splashDurationSeconds = 1.35f;
        [SerializeField] private bool showGridStatusEntry = true;
        [SerializeField] private bool showShareEntry = true;
        [SerializeField] private bool showExpandedTutorialHint = true;
        [SerializeField] private List<BrandingMenuEntryDefinition> menuEntries = new();

        public string PublicTitle => publicTitle;
        public string Subtitle => subtitle;
        public Sprite LogoSprite => logoSprite;
        public bool ShowThemeSelectionInSettings => showThemeSelectionInSettings;
        public bool ShowSplashLogoMoment => showSplashLogoMoment;
        public float SplashDurationSeconds => splashDurationSeconds;
        public bool ShowGridStatusEntry => showGridStatusEntry;
        public bool ShowShareEntry => showShareEntry;
        public bool ShowExpandedTutorialHint => showExpandedTutorialHint;
        public IReadOnlyList<BrandingMenuEntryDefinition> MenuEntries => menuEntries;
        public bool ShowDailyEntry => GetMenuEntryState(BrandingMenuEntryId.Daily) != BrandingMenuEntryState.Hidden;
        public bool ShowBestEntry => GetMenuEntryState(BrandingMenuEntryId.Best) != BrandingMenuEntryState.Hidden;

        public BrandingMenuEntryState GetMenuEntryState(BrandingMenuEntryId entryId)
        {
            for (int i = 0; i < menuEntries.Count; i++)
            {
                BrandingMenuEntryDefinition candidate = menuEntries[i];
                if (candidate != null && candidate.EntryId == entryId)
                {
                    return candidate.State;
                }
            }

            return BrandingMenuEntryState.Hidden;
        }

        public bool IsMenuEntryVisible(BrandingMenuEntryId entryId)
        {
            return GetMenuEntryState(entryId) != BrandingMenuEntryState.Hidden;
        }

        public bool IsMenuEntryEnabled(BrandingMenuEntryId entryId)
        {
            return GetMenuEntryState(entryId) == BrandingMenuEntryState.Enabled;
        }
    }
}
