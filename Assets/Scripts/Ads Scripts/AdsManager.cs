using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

#if UNITY_ANDROID

public class AdsManager : MonoBehaviour , IUnityAdsInitializationListener
{
    public BannerManager banner;
    public InterstitialManager interstitial;
    public RewardedAdsManager rewarded;

    private string gameId;

    public bool isTesting;
    public void OnInitializationComplete()
    {
        interstitial.InitializeInterstitial();
        rewarded.InitializeRewarded();
    }



    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    { }

    private void Awake()
    {
#if UNITY_ANDROID
        gameId = "5854945";
#elif UNITY_IOS
    gameId = "5854944";

#endif

        if (!Advertisement.isInitialized && Advertisement.isSupported)
        {
            Advertisement.Initialize(gameId, isTesting, this); //true es el "test mode" - false manda ads de verdad
            Debug.Log("Advertisement inicializado correctamente");
        }
    }

}
#endif