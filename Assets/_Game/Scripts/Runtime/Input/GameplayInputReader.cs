using UnityEngine;
using UnityEngine.InputSystem;

namespace Voltline.Input
{
    public sealed class GameplayInputReader : MonoBehaviour
    {
        private InputAction tapAction;
        private InputAction pauseAction;
        private InputAction debugRestartAction;

        public void Initialize()
        {
            if (tapAction != null)
            {
                return;
            }

            tapAction = new InputAction(name: "Tap", type: InputActionType.Button);
            tapAction.AddBinding("<Mouse>/leftButton");
            tapAction.AddBinding("<Touchscreen>/primaryTouch/press");
            tapAction.AddBinding("<Gamepad>/buttonSouth");

            pauseAction = new InputAction(name: "Pause", type: InputActionType.Button);
            pauseAction.AddBinding("<Keyboard>/escape");
            pauseAction.AddBinding("<Gamepad>/start");

            debugRestartAction = new InputAction(name: "DebugRestart", type: InputActionType.Button);
            debugRestartAction.AddBinding("<Keyboard>/r");

            tapAction.Enable();
            pauseAction.Enable();
            debugRestartAction.Enable();
        }

        public bool ConsumeTapPressed()
        {
            return tapAction != null && tapAction.WasPressedThisFrame();
        }

        public bool ConsumePausePressed()
        {
            return pauseAction != null && pauseAction.WasPressedThisFrame();
        }

        public bool ConsumeDebugRestartPressed()
        {
            return debugRestartAction != null && debugRestartAction.WasPressedThisFrame();
        }

        private void OnDisable()
        {
            tapAction?.Disable();
            pauseAction?.Disable();
            debugRestartAction?.Disable();
        }

        private void OnDestroy()
        {
            tapAction?.Dispose();
            pauseAction?.Dispose();
            debugRestartAction?.Dispose();
        }
    }
}