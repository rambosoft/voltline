using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Voltline.Core;

namespace Voltline.Input
{
    public sealed class GameplayInputReader : MonoBehaviour
    {
        private InputActionAsset inputActionsInstance;
        private InputAction tapAction;
        private InputAction pauseAction;
        private InputAction debugRestartAction;

        public void Initialize(InputActionAsset inputActionsAsset)
        {
            if (tapAction != null)
            {
                return;
            }

            if (inputActionsAsset == null)
            {
                Debug.LogError("GameplayInputReader requires the project-owned InputActionAsset.");
                enabled = false;
                return;
            }

            inputActionsInstance = Instantiate(inputActionsAsset);
            tapAction = inputActionsInstance.FindAction("Tap");
            pauseAction = inputActionsInstance.FindAction("Pause");
            debugRestartAction = inputActionsInstance.FindAction("DebugRestart");

            if (tapAction == null || pauseAction == null)
            {
                Debug.LogError("GameplayInputReader could not find the required Gameplay actions in the project input asset.");
                enabled = false;
                return;
            }

            tapAction.Enable();
            pauseAction.Enable();

            if (RuntimeBuildFlags.GameplayDebugToolsEnabled && debugRestartAction != null)
            {
                debugRestartAction.Enable();
            }
        }

        public bool ConsumeTapPressed()
        {
            return tapAction != null && tapAction.WasPressedThisFrame();
        }

        public bool ConsumeGameplayTapPressed()
        {
            return ConsumeTapPressed() && !IsPointerOverUi();
        }

        public bool ConsumePausePressed()
        {
            return pauseAction != null && pauseAction.WasPressedThisFrame();
        }

        public bool ConsumeDebugRestartPressed()
        {
            return debugRestartAction != null && debugRestartAction.WasPressedThisFrame();
        }

        private static bool IsPointerOverUi()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void OnDisable()
        {
            tapAction?.Disable();
            pauseAction?.Disable();
            debugRestartAction?.Disable();
        }

        private void OnDestroy()
        {
            tapAction = null;
            pauseAction = null;
            debugRestartAction = null;

            if (inputActionsInstance != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(inputActionsInstance);
                }
                else
                {
#if UNITY_EDITOR
                    DestroyImmediate(inputActionsInstance);
#else
                    Destroy(inputActionsInstance);
#endif
                }

                inputActionsInstance = null;
            }
        }
    }
}
