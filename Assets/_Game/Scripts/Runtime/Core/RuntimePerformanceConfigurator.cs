using UnityEngine;

namespace Voltline.Core
{
    public static class RuntimePerformanceConfigurator
    {
        private const int TargetFrameRate = 60;

        public static void Apply()
        {
            Application.targetFrameRate = TargetFrameRate;
            QualitySettings.vSyncCount = 0;

            if (Application.isMobilePlatform)
            {
                Screen.autorotateToPortrait = true;
                Screen.autorotateToPortraitUpsideDown = false;
                Screen.autorotateToLandscapeLeft = false;
                Screen.autorotateToLandscapeRight = false;
                Screen.orientation = ScreenOrientation.Portrait;
                Screen.sleepTimeout = SleepTimeout.NeverSleep;
            }
        }
    }
}
