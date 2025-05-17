using UnityEngine;

public class UIPlatformAdaptation : MonoBehaviour
{
    [SerializeField] private GameObject mobileUI;
    [SerializeField] private GameObject pcUI;

    private void Start()
    {
#if UNITY_EDITOR
        mobileUI.SetActive(false);
        pcUI.SetActive(true);

#elif UNITY_ANDROID
        mobileUI.SetActive(true);
        pcUI.SetActive(false);

#elif UNITY_WEBGL
        mobileUI.SetActive(false);
        pcUI.SetActive(true);
#endif
    }
}
