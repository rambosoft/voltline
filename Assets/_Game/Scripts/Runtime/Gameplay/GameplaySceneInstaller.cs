using UnityEngine;
using Voltline.Data;
using Voltline.Input;
using Voltline.Save;
using Voltline.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Voltline.Gameplay
{
    public sealed class GameplaySceneInstaller : MonoBehaviour
    {
        [SerializeField] private GameBalanceConfig gameBalance;
        [SerializeField] private DifficultyCurveConfig difficultyCurve;
        [SerializeField] private ObstacleCatalog obstacleCatalog;
        [SerializeField] private ThemeCatalog themeCatalog;
        [SerializeField] private int startingSeed = 1337;

        private void Awake()
        {
            if (gameBalance == null || difficultyCurve == null || obstacleCatalog == null || themeCatalog == null || themeCatalog.DefaultTheme == null)
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

            GameplayInputReader inputReader = GetOrAddComponent<GameplayInputReader>();
            DifficultyDirector difficultyDirector = GetOrAddComponent<DifficultyDirector>();
            ScoreSystem scoreSystem = GetOrAddComponent<ScoreSystem>();
            TrackManager trackManager = GetOrAddComponent<TrackManager>();
            HazardManager hazardManager = GetOrAddComponent<HazardManager>();
            GameManager gameManager = GetOrAddComponent<GameManager>();
            UIStateCoordinator uiStateCoordinator = GetOrAddComponent<UIStateCoordinator>();
            PlayerController playerController = GetOrAddComponent<PlayerController>();
            SaveService saveService = SaveService.EnsureExists();

            inputReader.Initialize();
            difficultyDirector.Initialize(gameBalance, difficultyCurve, obstacleCatalog);
            scoreSystem.Initialize(gameBalance);
            trackManager.Initialize(gameBalance, themeCatalog.DefaultTheme, gameplayCamera);
            playerController.Initialize(gameBalance, trackManager, themeCatalog.DefaultTheme);
            hazardManager.Initialize(gameBalance, obstacleCatalog, difficultyDirector, trackManager, themeCatalog.DefaultTheme);
            gameManager.Initialize(gameBalance, inputReader, difficultyDirector, trackManager, playerController, hazardManager, scoreSystem, startingSeed);
            uiStateCoordinator.Initialize(gameManager, scoreSystem, themeCatalog.DefaultTheme, saveService);
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
        }

        private void AssignDefaultReferences()
        {
            gameBalance ??= AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(ProjectConfigAssetPaths.GameBalance);
            difficultyCurve ??= AssetDatabase.LoadAssetAtPath<DifficultyCurveConfig>(ProjectConfigAssetPaths.DifficultyCurve);
            obstacleCatalog ??= AssetDatabase.LoadAssetAtPath<ObstacleCatalog>(ProjectConfigAssetPaths.ObstacleCatalog);
            themeCatalog ??= AssetDatabase.LoadAssetAtPath<ThemeCatalog>(ProjectConfigAssetPaths.ThemeCatalog);
        }
#endif
    }
}
