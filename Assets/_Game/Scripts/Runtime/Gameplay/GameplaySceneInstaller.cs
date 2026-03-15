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
        [SerializeField] private PlayerVisualConfig playerVisualConfig;
        [SerializeField] private HazardPresentationCatalog hazardPresentationCatalog;
        [SerializeField] private ObstacleVisualCatalog obstacleVisualCatalog;
        [SerializeField] private DifficultyCurveConfig difficultyCurve;
        [SerializeField] private ObstacleCatalog obstacleCatalog;
        [SerializeField] private ThemeCatalog themeCatalog;
        [SerializeField] private AudioCueCatalog audioCueCatalog;
        [SerializeField] private VfxCatalog vfxCatalog;
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private int startingSeed = 1337;
        [SerializeField] private int debugStartingScore = 0;

        public int DebugStartingScore => RuntimeBuildFlags.GameplayDebugToolsEnabled ? Mathf.Max(0, debugStartingScore) : 0;

        private void Awake()
        {
            if (gameBalance == null
                || gameplayPresentation == null
                || playerVisualConfig == null
                || hazardPresentationCatalog == null
                || obstacleVisualCatalog == null
                || difficultyCurve == null
                || obstacleCatalog == null
                || themeCatalog == null
                || themeCatalog.DefaultTheme == null
                || audioCueCatalog == null
                || vfxCatalog == null
                || inputActions == null)
            {
                Debug.LogError("GameplaySceneInstaller is missing required config references.");
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
            audioService.PlayMusicLoop(AudioCueIds.MainLoop);

            GameplayInputReader inputReader = GetOrAddComponent<GameplayInputReader>();
            DifficultyDirector difficultyDirector = GetOrAddComponent<DifficultyDirector>();
            ScoreSystem scoreSystem = GetOrAddComponent<ScoreSystem>();
            TrackManager trackManager = GetOrAddComponent<TrackManager>();
            HazardManager hazardManager = GetOrAddComponent<HazardManager>();
            GameManager gameManager = GetOrAddComponent<GameManager>();
            UIStateCoordinator uiStateCoordinator = GetOrAddComponent<UIStateCoordinator>();
            PlayerController playerController = GetOrAddComponent<PlayerController>();
            VfxService vfxService = GetOrAddComponent<VfxService>();
            GameplayFeedbackCoordinator feedbackCoordinator = GetOrAddComponent<GameplayFeedbackCoordinator>();

            inputReader.Initialize(inputActions);
            difficultyDirector.Initialize(gameBalance, difficultyCurve, obstacleCatalog);
            scoreSystem.Initialize(gameBalance);
            trackManager.Initialize(gameBalance, gameplayPresentation, activeTheme, gameplayCamera);
            playerController.Initialize(gameBalance, gameplayPresentation, playerVisualConfig, trackManager, activeTheme);
            hazardManager.Initialize(gameBalance, obstacleCatalog, hazardPresentationCatalog, obstacleVisualCatalog, difficultyDirector, trackManager, activeTheme);
            gameManager.Initialize(gameBalance, inputReader, difficultyDirector, trackManager, playerController, hazardManager, scoreSystem, startingSeed, DebugStartingScore);
            uiStateCoordinator.Initialize(gameManager, scoreSystem, activeTheme, themeCatalog, saveService);
            vfxService.Initialize(vfxCatalog, activeTheme, gameplayCamera);
            feedbackCoordinator.Initialize(gameManager, playerController, hazardManager, scoreSystem, trackManager, audioService, vfxService);
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
            playerVisualConfig ??= AssetDatabase.LoadAssetAtPath<PlayerVisualConfig>(ProjectConfigAssetPaths.PlayerVisualConfig);
            hazardPresentationCatalog ??= AssetDatabase.LoadAssetAtPath<HazardPresentationCatalog>(ProjectConfigAssetPaths.HazardPresentationCatalog);
            obstacleVisualCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleVisualCatalog>(ProjectConfigAssetPaths.ObstacleVisualCatalog);
            difficultyCurve ??= AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            obstacleCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            themeCatalog ??= AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
            audioCueCatalog ??= AssetDatabase.LoadAssetAtPath<AudioCueCatalog>(ProjectConfigAssetPaths.AudioCueCatalog);
            vfxCatalog ??= AssetDatabase.LoadAssetAtPath<VfxCatalog>(ProjectConfigAssetPaths.VfxCatalog);
            audioMixer ??= AssetDatabase.LoadAssetAtPath<AudioMixer>(ProjectConfigAssetPaths.AudioMixer);
            inputActions ??= AssetDatabase.LoadAssetAtPath<InputActionAsset>(ProjectConfigAssetPaths.InputActions);
        }
#endif
    }
}
