using System;
using System.Collections.Generic;
using UnityEngine;

namespace Voltline.Data
{
    [Serializable]
    public sealed class ThemeSequenceEntry
    {
        [SerializeField] private int scoreThreshold = 20;
        [SerializeField] private string themeId = "theme.candy-pop";
        [SerializeField] private float transitionDurationSeconds = 0.35f;

        public int ScoreThreshold => scoreThreshold;
        public string ThemeId => themeId;
        public float TransitionDurationSeconds => transitionDurationSeconds;
    }

    [CreateAssetMenu(fileName = "CFG_ThemeSequence_Main", menuName = "Voltline/Config/Theme Sequence")]
    public sealed class ThemeSequenceConfig : ScriptableObject
    {
        [SerializeField] private bool enableRuntimeTransitions = true;
        [SerializeField] private bool requireUnlockedTheme = true;
        [SerializeField] private int minimumScoreForTransitions = 20;
        [SerializeField] private float minimumTransitionDurationSeconds = 0.24f;
        [SerializeField] private float maximumTransitionDurationSeconds = 0.5f;
        [SerializeField] private List<ThemeSequenceEntry> entries = new();

        public bool EnableRuntimeTransitions => enableRuntimeTransitions;
        public bool RequireUnlockedTheme => requireUnlockedTheme;
        public int MinimumScoreForTransitions => minimumScoreForTransitions;
        public float MinimumTransitionDurationSeconds => minimumTransitionDurationSeconds;
        public float MaximumTransitionDurationSeconds => maximumTransitionDurationSeconds;
        public IReadOnlyList<ThemeSequenceEntry> Entries => entries;
    }
}
