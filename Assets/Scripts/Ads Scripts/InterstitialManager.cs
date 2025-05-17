using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class InterstitialManager : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOSAdUnitId = "Interstitial_iOS";
    string _adUnitId;
    bool _adLoaded;

    private void Start()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        _adUnitId = _androidAdUnitId;

#elif UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#endif 

        _adLoaded = false;
    }

    public void InitializeInterstitial()
    {
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        _adLoaded = true;
        Debug.Log("Intertitial cargado con éxito.");

    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Intertitial carga fallida.");
    }

    public void ShowInterstitialAd()
    {
        if (_adLoaded)
        {
            Advertisement.Show(_adUnitId, this);
            InitializeInterstitial();
        }
    }
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.LogWarning($"Muestra de ad fallida:  {message}. Intentando nuevamente en 30 segundos...");
        StartCoroutine(RetryLoadAdCoroutine());
    }

    private IEnumerator RetryLoadAdCoroutine()
    {
        yield return new WaitForSeconds(30f);
        Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        //Que ocurre al momento de que comienze el ad (Ej pausar el juego)
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Que ocurre si el jugador hace clic
    }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        //Que ocurre al momento de que termine el ad (Ej despausar el juego)
    }
}