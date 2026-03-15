namespace Voltline.Core
{
    public static class RuntimeBuildFlags
    {
        public static bool GameplayDebugToolsEnabled
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#else
                return false;
#endif
            }
        }
    }
}
