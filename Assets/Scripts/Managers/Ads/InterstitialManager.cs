using UnityEngine;
using UnityEngine.Advertisements;

public class InterstitialManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener {

#pragma warning disable 0414
    [SerializeField] private string _androidUnityId = "Interstitial_Android";
    [SerializeField] private string _iosUnityId = "Interstitial_iOS";
#pragma warning restore 0414

    private string _unityId = null;
    private bool isLoaded;

    private static InterstitialManager _instance;
    public static InterstitialManager Instance {
        get {
            if (_instance == null) {
                _instance = FindFirstObjectByType<InterstitialManager>();
            }
            return _instance;
        }
    }

    void Start() {
#if UNITY_ANDROID
        _unityId = _androidUnityId;
#elif UNITY_IOS
        _unityId = _iosUnityId;
#endif
    }

    internal void Initialize() {
        Advertisement.Load(_unityId, this);
        Debug.Log("Unity Ads - Interstitial ad initialized");
    }

    public void ShowInterstitialAd() {
        if (isLoaded) {
            BannersManager.Instance?.HideBannerAd();
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
        BannersManager.Instance?.ShowBannerAd();
        Initialize();
    }
}
