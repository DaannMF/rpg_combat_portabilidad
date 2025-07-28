using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [Header("Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    void Awake() {
        continueButton.onClick.AddListener(OnContinueButtonClicked);
#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
#else
        if (quitButton != null) {
            quitButton.onClick.AddListener(() => {
                UIEvents.OnGameQuit?.Invoke();
            });
        }
#endif
    }

    private void OnContinueButtonClicked() {
        UIEvents.OnGameResumed?.Invoke();
    }
}
