using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {

#pragma warning disable 0414
    [SerializeField] private string _androidUnityId = "Interstitial_Android";
    [SerializeField] private string _iosUnityId = "Interstitial_iOS";
#pragma warning restore 0414

    private string _unityId = null;
    private bool isLoaded;

    void Start() {
#if UNITY_ANDROID
        _unityId = _androidUnityId;
#elif UNITY_IOS
        _unityId = _iosUnityId;
#endif
    }

    private BannersManager bannersManager;
    private GameManager gameManager;

    private void Awake() {
        bannersManager = FindFirstObjectByType<BannersManager>();
        gameManager = FindFirstObjectByType<GameManager>();

        if (gameManager != null)
            gameManager.OnInterstitialAdRequested += ShowInterstitialAd;
    }

    internal void Initialize() {
        Advertisement.Load(_unityId, this);
    }

    public void ShowInterstitialAd() {
        if (isLoaded) {
            if (bannersManager != null) bannersManager.HideBannerAd();
            Advertisement.Show(_unityId, this);
            isLoaded = false;
        }
        else {
            Debug.Log("Interstitial ad not loaded yet.");
        }
    }

    public void OnUnityAdsAdLoaded(string placementId) {
        isLoaded = true;
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message) {
        Debug.Log($"Interstitial failed to load: {error} - {message}");
    }

    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message) {
        Debug.Log($"Interstitial ad failed to show: {error} - {message}");
    }

    public void OnUnityAdsShowStart(string placementId) {
        Debug.Log("Interstitial ad started showing.");
    }

    public void OnUnityAdsShowClick(string placementId) {
        Debug.Log("Interstitial ad clicked.");
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState) {
        if (bannersManager != null) bannersManager.ShowBannerAd();
        Initialize();
    }
}
