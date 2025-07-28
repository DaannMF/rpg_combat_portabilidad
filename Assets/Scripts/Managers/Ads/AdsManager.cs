using UnityEngine;
using UnityEngine.Advertisements;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener {

    [Header("Game Ids")]
#pragma warning disable 0414    
    [SerializeField] private string androidGameId = "GameId_Android";
    [SerializeField] private string iosGameId = "GameId_iOS";
#pragma warning restore 0414


#pragma warning disable 0414
    private string gameId;
    private bool testMode = true;
#pragma warning restore 0414

    void Awake() {
#if UNITY_ANDROID
        gameId = androidGameId;
        testMode = false;
#elif UNITY_IOS
        gameId = iosGameId;
        testMode = false;
#elif UNITI_EDITOR
        gameId = androidGameId;
        testMode = true;
#endif

#if UNITY_ANDROID || UNITY_IOS
        if (!Advertisement.isInitialized && Advertisement.isSupported)
            Advertisement.Initialize(gameId, testMode, this);
#endif
    }

    public void OnInitializationComplete() {
        InterstitialManager.Instance?.Initialize();
        BannersManager.Instance?.Initialize();
        Debug.Log("Unity Ads Initialization Complete");
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message) {
        Debug.Log($"Unity Ads Initialization Failed: {error} - {message}");
    }
}
