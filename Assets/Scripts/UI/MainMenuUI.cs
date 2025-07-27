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
        quitButton.onClick.AddListener(OnQuitButtonClicked);
#endif
    }

    private void OnContinueButtonClicked() {
        UIEvents.OnGameResumed?.Invoke();
    }

    private void QuitAndroid() {
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        activity.Call<bool>("moveTaskToBack", true);
    }

    private void OnQuitButtonClicked() {
#if UNITY_ANDROID
        QuitAndroid();
#elif UNITY_STANDALONE
        Application.Quit();
#elif UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
