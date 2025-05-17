using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;
using TMPro;


public class RewardedAdsManager : MonoBehaviour , IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Rewarded_Android";
    [SerializeField] string _iOSAdUnitId = "Rewarded_iOS";
    string _adUnitId = null;
    bool _adLoaded;


    [SerializeField] private TMP_Text rewardText;

    [SerializeField] private Mobile_TapGameManager mobile_TapGameManager;


    private void Start()
    {
#if UNITY_ANDROID || UNITY_EDITOR
        _adUnitId = _androidAdUnitId;
#elif UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#endif 

        _adLoaded = false;
    }
    public void InitializeRewarded()
    {
       Advertisement.Load(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string placementId)
    {
        _adLoaded = true;
        Debug.Log("Rewarded cargado con éxito.");
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log("Rewarded cargado carga fallida.");
    }

    public void ShowRewardedAd()
    {
        if (_adLoaded)
        {
            Advertisement.Show(_adUnitId, this);
            Debug.LogWarning("Rewarded ad mostrado");

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
        Debug.Log("El anuncio reward ha comenzado.");
    }

    public void OnUnityAdsShowClick(string placementId)
    {
        //Que ocurre si el jugador hace clic en el ad
    }

 
    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == _adUnitId && showCompletionState.Equals(UnityAdsCompletionState.COMPLETED))
        {
            Debug.Log("Se ha dado el reward");
            UpdateTimer();
        }
    }

    public void UpdateTimer()
    {
        if (mobile_TapGameManager.timer == 10f)
        {
            mobile_TapGameManager.timer = 12f;
            mobile_TapGameManager.timerText.text = "12";

            rewardText.text = "Extra Time";
            rewardText.color = Color.black;

        }
        else if (mobile_TapGameManager.timer == 12f)
        {
            mobile_TapGameManager.timer = 10f;

            mobile_TapGameManager.timerText.text = "10";
            //rewardText.text = "Can't add more time!";
            rewardText.color = Color.white;
        }
    }
}