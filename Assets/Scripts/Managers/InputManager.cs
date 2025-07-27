using System;
using UnityEngine;

public class InputManager : MonoBehaviour {
    private bool enableInput = true;

    private BaseInputSystem currentInputSystem;
    public BaseInputSystem CurrentInputSystem => currentInputSystem;

    private void Awake() {
        InitializeInputSystem();
    }

    private void Update() {
        if (enableInput) currentInputSystem?.HandleInput();
    }

    private void OnDestroy() {
        if (currentInputSystem != null && currentInputSystem is MobileInputSystem mobileSystem)
            mobileSystem.Cleanup();
    }

    private void InitializeInputSystem() {
#if UNITY_ANDROID || UNITY_IOS
        currentInputSystem = new MobileInputSystem();
#elif UNITY_STANDALONE || UNITY_EDITOR
        currentInputSystem = new KeyboardInputSystem();
#endif
    }

    public void Initialize(PlayerActionController controller, GridVisualFeedbackSystem visualSystem) {
        if (currentInputSystem is MobileInputSystem mobileSystem)
            mobileSystem.InitializeWithVisualFeedback(controller, visualSystem);
        else
            currentInputSystem?.Initialize(controller);
    }

    public void SetActivePlayer(BaseCharacter player) {
        currentInputSystem?.SetActivePlayer(player);
    }

    public void EndPlayerTurn() {
        currentInputSystem?.EndPlayerTurn();
    }

    public void EnableInput(bool enable) {
        enableInput = enable;
    }
}