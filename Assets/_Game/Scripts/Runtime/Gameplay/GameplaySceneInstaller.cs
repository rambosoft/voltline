using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using Voltline.Audio;
using Voltline.Core;
using Voltline.Data;
using Voltline.Input;
using Voltline.Save;
using Voltline.UI;
using Voltline.VFX;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Voltline.Gameplay
{
    public sealed class GameplaySceneInstaller : MonoBehaviour
    {
        [SerializeField] private GameBalanceConfig gameBalance;
        [SerializeField] private GameplayPresentationConfig gameplayPresentation;
        [SerializeField] private BackgroundPresentationConfig backgroundPresentationConfig;
        [SerializeField] private PlayerVisualConfig playerVisualConfig;
        [SerializeField] private HazardPresentationCatalog hazardPresentationCatalog;
        [SerializeField] private ObstacleVisualCatalog obstacleVisualCatalog;
        [SerializeField] private DifficultyCurveConfig difficultyCurve;
        [SerializeField] private ObstacleCatalog obstacleCatalog;
        [SerializeField] private ThemeCatalog themeCatalog;
        [SerializeField] private ThemeSequenceConfig themeSequenceConfig;
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private VfxCatalog vfxCatalog;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private int startingSeed = 1337;
        [SerializeField] private int debugStartingScore = 0;

#if UNITY_EDITOR
        private const string EditorSessionDebugStartingScoreOverrideEnabledKey = "Voltline.Gameplay.DebugStartingScore.OverrideEnabled";
        private const string EditorSessionDebugStartingScoreValueKey = "Voltline.Gameplay.DebugStartingScore.Value";
#endif

        public int ReleaseBaselineDebugStartingScore => Mathf.Max(0, debugStartingScore);
        public int DebugStartingScore
        {
            get
            {
                if (!RuntimeBuildFlags.GameplayDebugToolsEnabled)
                {
                    return 0;
                }

#if UNITY_EDITOR
                if (EditorSessionDebugStartingScoreOverrideEnabled)
                {
                    return EditorSessionDebugStartingScore;
                }
#endif

                return ReleaseBaselineDebugStartingScore;
            }
        }

        public ThemeCatalog ThemeCatalog => themeCatalog;
        public ThemeSequenceConfig ThemeSequenceConfig => themeSequenceConfig;
        public BackgroundPresentationConfig BackgroundPresentationConfig => backgroundPresentationConfig;

#if UNITY_EDITOR
        public static bool EditorSessionDebugStartingScoreOverrideEnabled
        {
            get => SessionState.GetBool(EditorSessionDebugStartingScoreOverrideEnabledKey, false);
            set => SessionState.SetBool(EditorSessionDebugStartingScoreOverrideEnabledKey, value);
        }

        public static int EditorSessionDebugStartingScore
        {
            get => Mathf.Max(0, SessionState.GetInt(EditorSessionDebugStartingScoreValueKey, 0));
            set => SessionState.SetInt(EditorSessionDebugStartingScoreValueKey, Mathf.Max(0, value));
        }

        public static void ClearEditorSessionDebugStartingScoreOverride()
        {
            EditorSessionDebugStartingScoreOverrideEnabled = false;
            EditorSessionDebugStartingScore = 0;
        }
#endif

        private void Awake()
        {
#if UNITY_EDITOR
            AssignDefaultReferences();
#endif

            if (!HasRequiredConfigReferences(out string missingReferences))
            {
                Debug.LogError($"GameplaySceneInstaller is missing required config references: {missingReferences}");
                enabled = false;
                return;
            }

            Camera gameplayCamera = Camera.main ?? FindAnyObjectByType<Camera>();
            if (gameplayCamera == null)
            {
                Debug.LogError("GameplaySceneInstaller could not find a gameplay camera.");
                enabled = false;
                return;
            }

            SaveService saveService = SaveService.EnsureExists();
            saveService.SynchronizeThemeUnlocks(themeCatalog);
            ThemeConfig activeTheme = saveService.ResolveSelectedTheme(themeCatalog) ?? themeCatalog.DefaultTheme;
            AudioService audioService = AudioService.EnsureExists();
            audioService.Configure(audioCueCatalog, saveService, audioMixer);
            audioService.ApplyTheme(activeTheme);
            audioService.PlayMusicLoop(AudioCueIds.MainLoop);

            GameplayInputReader inputReader = GetOrAddComponent<GameplayInputReader>();
            DifficultyDirector difficultyDirector = GetOrAddComponent<DifficultyDirector>();
            ScoreSystem scoreSystem = GetOrAddComponent<ScoreSystem>();
            TrackManager trackManager = GetOrAddComponent<TrackManager>();
            HazardManager hazardManager = GetOrAddComponent<HazardManager>();
            GameManager gameManager = GetOrAddComponent<GameManager>();
            UIStateCoordinator uiStateCoordinator = GetOrAddComponent<UIStateCoordinator>();
            PlayerController playerController = GetOrAddComponent<PlayerController>();
            BackgroundPresentationController backgroundPresentationController = GetOrAddComponent<BackgroundPresentationController>();
            ThemePresentationController themePresentationController = GetOrAddComponent<ThemePresentationController>();
            WorldProgressionController worldProgressionController = GetOrAddComponent<WorldProgressionController>();
            VfxService vfxService = GetOrAddComponent<VfxService>();
            GameplayFeedbackCoordinator feedbackCoordinator = GetOrAddComponent<GameplayFeedbackCoordinator>();

            inputReader.Initialize(inputActions);
            difficultyDirector.Initialize(gameBalance, difficultyCurve, obstacleCatalog);
            scoreSystem.Initialize(gameBalance);
            trackManager.Initialize(gameBalance, gameplayPresentation, activeTheme, gameplayCamera);
            backgroundPresentationController.Initialize(backgroundPresentationConfig, activeTheme, gameplayCamera, trackManager);
            playerController.Initialize(gameBalance, gameplayPresentation, playerVisualConfig, trackManager, activeTheme);
            hazardManager.Initialize(gameBalance, obstacleCatalog, hazardPresentationCatalog, obstacleVisualCatalog, difficultyDirector, trackManager, activeTheme);
            gameManager.Initialize(gameBalance, inputReader, difficultyDirector, trackManager, playerController, hazardManager, scoreSystem, startingSeed, DebugStartingScore);
            uiStateCoordinator.Initialize(gameManager, scoreSystem, activeTheme, themeCatalog, saveService);
            vfxService.Initialize(vfxCatalog, activeTheme, gameplayCamera);
            feedbackCoordinator.Initialize(gameManager, playerController, hazardManager, scoreSystem, trackManager, audioService, vfxService);

            if (RuntimeBuildFlags.GameplayDebugToolsEnabled)
            {
                GameplayDebugScoreOverlay debugScoreOverlay = GetOrAddComponent<GameplayDebugScoreOverlay>();
                debugScoreOverlay.Initialize(scoreSystem, worldProgressionController, gameManager);
            }

            themePresentationController.Initialize(
                themeCatalog,
                themeSequenceConfig,
                saveService,
                gameManager,
                scoreSystem,
                backgroundPresentationController,
                backgroundPresentationConfig,
                trackManager,
                playerController,
                hazardManager,
                worldProgressionController,
                uiStateCoordinator,
                audioService,
                vfxService,
                activeTheme);
        }

        private bool HasRequiredConfigReferences(out string missingReferences)
        {
            System.Collections.Generic.List<string> missing = new();

            if (gameBalance == null) missing.Add(nameof(gameBalance));
            if (gameplayPresentation == null) missing.Add(nameof(gameplayPresentation));
            if (backgroundPresentationConfig == null) missing.Add(nameof(backgroundPresentationConfig));
            if (playerVisualConfig == null) missing.Add(nameof(playerVisualConfig));
            if (hazardPresentationCatalog == null) missing.Add(nameof(hazardPresentationCatalog));
            if (obstacleVisualCatalog == null) missing.Add(nameof(obstacleVisualCatalog));
            if (difficultyCurve == null) missing.Add(nameof(difficultyCurve));
            if (obstacleCatalog == null) missing.Add(nameof(obstacleCatalog));
            if (themeCatalog == null)
            {
                missing.Add(nameof(themeCatalog));
            }
            else
            {
                if (themeCatalog.DefaultTheme == null) missing.Add("themeCatalog.DefaultTheme");
                if (themeCatalog.BrandingPresentationConfig == null) missing.Add("themeCatalog.BrandingPresentationConfig");
                if (themeCatalog.ProductionCopyConfig == null) missing.Add("themeCatalog.ProductionCopyConfig");
                if (themeCatalog.UiThemeConfig == null) missing.Add("themeCatalog.UiThemeConfig");
                if (themeCatalog.DefaultTheme != null && themeCatalog.DefaultTheme.ResolveWorldProgressionConfig() == null) missing.Add("themeCatalog.DefaultTheme.WorldProgressionConfig");
            }

            if (themeSequenceConfig == null) missing.Add(nameof(themeSequenceConfig));
            if (audioCueCatalog == null) missing.Add(nameof(audioCueCatalog));
            if (vfxCatalog == null) missing.Add(nameof(vfxCatalog));
            if (inputActions == null) missing.Add(nameof(inputActions));

            missingReferences = string.Join(", ", missing);
            return missing.Count == 0;
        }
        private T GetOrAddComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            return component != null ? component : gameObject.AddComponent<T>();
        }

#if UNITY_EDITOR
        private void Reset()
        {
            AssignDefaultReferences();
        }

        private void OnValidate()
        {
            AssignDefaultReferences();
            debugStartingScore = Mathf.Max(0, debugStartingScore);
        }

        private void AssignDefaultReferences()
        {
            gameBalance ??= AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            gameplayPresentation ??= AssetDatabase.LoadAssetAtPath<GameplayPresentationConfig>(ProjectConfigAssetPaths.GameplayPresentation);
            backgroundPresentationConfig ??= AssetDatabase.LoadAssetAtPath<BackgroundPresentationConfig>(ProjectConfigAssetPaths.BackgroundPresentation);
            playerVisualConfig ??= AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            hazardPresentationCatalog ??= AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            obstacleVisualCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            difficultyCurve ??= AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            obstacleCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            themeCatalog ??= AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            themeSequenceConfig ??= AssetDatabase.LoadAssetAtPath<ThemeSequenceConfig>(ProjectConfigAssetPaths.ThemeSequence);
            audioCueCatalog ??= AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            vfxCatalog ??= AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);
            audioMixer ??= AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
            inputActions ??= AssetDatabase.LoadAssetAtPath<InputActionAsset>(ProjectConfigAssetPaths.InputActions);
        }
#endif
    }
}




