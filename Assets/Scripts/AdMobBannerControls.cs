using UnityEngine;
using GoogleMobileAds.Api;

public class AdMobBannerControls : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
        MobileAds.Initialize(initStatus => { });
        this.RequestBanner();
    }

    private void RequestBanner()
    {
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-5553264284629233/5095518621";
        #else
        string adUnitId = "unexpected_platform";
        #endif

        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);
        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);
    }
}
