using System.Collections;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerManager : MonoBehaviour
{
    [SerializeField] private string _androidAdUnitId = "Banner_Android";
    [SerializeField] private string _iOSAdUnitId = "Banner_iOS";

    private string _adUnitId;
    private BannerOptions _bannerOptions;

    private void Start()
    {
#if UNITY_ANDROID
        _adUnitId = _androidAdUnitId;
#elif UNITY_IOS
        _adUnitId = _iOSAdUnitId;
#endif

        Advertisement.Banner.SetPosition(BannerPosition.BOTTOM_CENTER);

        _bannerOptions = new BannerOptions
        {
            showCallback = OnBannerShown,
            hideCallback = OnBannerHidden,
            clickCallback = OnBannerClicked
        };

        LoadBannerAd();
    }

    private void LoadBannerAd()
    {
        var loadOptions = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerLoadFailed
        };

        Advertisement.Banner.Load(_adUnitId, loadOptions);
    }

    private void OnBannerLoaded()
    {
        Debug.Log("Banner cargado con éxito.");
        StartCoroutine(DisplayBannerWithDelay());
    }

    private void OnBannerLoadFailed(string message)
    {
        Debug.LogWarning("Error al cargar banner: " + message);
        StartCoroutine(RetryLoadAdCoroutine());
    }

    private IEnumerator RetryLoadAdCoroutine()
    {
        yield return new WaitForSeconds(30f);
        LoadBannerAd();
    }

    private IEnumerator DisplayBannerWithDelay()
    {
        while (true)
        {
            // Mostrar banner
            Advertisement.Banner.Show(_adUnitId, _bannerOptions);

            Debug.Log("Banner mostrado");
            yield return new WaitForSeconds(5f);

            // Ocultar banner
            Advertisement.Banner.Hide(false); // false = oculta pero sigue cargado

            Debug.Log("Banner ocultado");
            yield return new WaitForSeconds(30f);
        }
    }

    private void OnBannerShown()
    {
        Debug.Log("Callback: Banner mostrado.");
    }

    private void OnBannerHidden()
    {
        Debug.Log("Callback: Banner ocultado.");
    }

    private void OnBannerClicked()
    {
        Debug.Log("Callback: Banner clickeado.");
    }
}
