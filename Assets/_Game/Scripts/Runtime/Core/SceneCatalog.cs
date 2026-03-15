namespace Voltline.Core
{
    public static class SceneCatalog
    {
        public const string Bootstrap = "Bootstrap";
        public const string MainMenu = "MainMenu";
        public const string Gameplay = "Gameplay";

        public const string BootstrapScenePath = "Assets/_Game/Scenes/Bootstrap.unity";
        public const string MainMenuScenePath = "Assets/_Game/Scenes/MainMenu.unity";
        public const string GameplayScenePath = "Assets/_Game/Scenes/Gameplay.unity";

        public static readonly string[] BuildOrder =
        {
            BootstrapScenePath,
            MainMenuScenePath,
            GameplayScenePath,
        };
    }
}