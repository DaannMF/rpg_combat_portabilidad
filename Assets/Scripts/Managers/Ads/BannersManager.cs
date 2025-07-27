using UnityEngine;
using UnityEngine.Advertisements;

public class BannersManager : MonoBehaviour {
#pragma warning disable 0414
    [SerializeField] private string _androidUnityId = "Banner_Android";
    [SerializeField] private string _iosUnityId = "Banner_iOS";
#pragma warning restore 0414

    private string _unityId = null;

    void Start() {
#if UNITY_ANDROID
        _unityId = _androidUnityId;
#elif UNITY_IOS
        _unityId = _iosUnityId;
#endif
    }

    internal void Initialize() {
        BannerLoadOptions bannerLoadOptions = new() {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_RIGHT);
        Advertisement.Banner.Load(_unityId, bannerLoadOptions);
    }

    private void OnBannerLoaded() {
        Debug.Log("Banner loaded successfully");

        ShowBannerAd();
    }

    public void ShowBannerAd() {
        Advertisement.Banner.Show(_unityId);
    }

    public void HideBannerAd() {
        Advertisement.Banner.Hide();
    }

    private void OnBannerError(string message) {
        Debug.Log($"Banner error: {message}");
    }
}
