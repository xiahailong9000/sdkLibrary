using System.Collections;
using System.Collections.Generic;
using admob;
using UnityEngine;
public class AdmobAgent : MonoBehaviour {
    static AdmobAgent instance;
    public static AdmobAgent GetInstance {
        get {
            if (instance == null) {
                GameObject gg = new GameObject("AdmobAgent");
                MonoBehaviour.DontDestroyOnLoad(gg);
                instance = gg.AddComponent<AdmobAgent>();
                instance.initAdmob();
            }
            return instance;
        }
    }
    /// <summary>
    /// 显示奖赏视频---
    /// </summary>
    public void ShowVideo() {
        //Debug.Log("touch video button -------------");
        if (ad.isRewardedVideoReady()) {
            ad.showRewardedVideo();
        } else {
            ad.loadRewardedVideo(videoID);
        }
    }
    /// <summary>
    /// 显示横幅广告-----------Banner==横幅
    /// </summary>
    public void ShowBottomAds() {
        Admob.Instance().showBannerRelative(AdSize.SmartBanner, AdPosition.BOTTOM_CENTER, 0);
    }
    public void showInterstitial() {
        Debug.Log("touch inst button -------------");
        if (ad.isInterstitialReady()) {
            ad.showInterstitial();
        } else {
            ad.loadInterstitial();
        }
    }
    public void showBannerAbsolute() {
        Admob.Instance().showBannerAbsolute(AdSize.Banner, 20, 300);
    }
    public void removeBanner() {
        Admob.Instance().removeBanner();
    }
    /// <summary>
    /// Native==本地人
    /// </summary>
    public void showNativeBannerRelative() {
        Admob.Instance().showNativeBannerRelative(new AdSize(320, 100), AdPosition.BOTTOM_CENTER, 0, nativeBannerID);
    }
    public void showNativeBannerAbsolute() {
        Admob.Instance().showNativeBannerAbsolute(new AdSize(-1, 132), 20, 300, nativeBannerID);
    }
    public void removeNativeBanner() {
        Admob.Instance().removeNativeBanner();
    }
    Admob ad;
    string appID, bannerID, interstitialID, videoID, nativeBannerID;

    void initAdmob() {
#if UNITY_IOS
        appID="ca-app-pub-3940256099942544~1458002511";
		bannerID="ca-app-pub-3940256099942544/2934735716";
		interstitialID="ca-app-pub-3940256099942544/4411468910";
		videoID="ca-app-pub-3940256099942544/1712485313";
		nativeBannerID = "ca-app-pub-3940256099942544/3986624511";
#elif UNITY_ANDROID
        appID = "ca-app-pub-3940256099942544~3347511713";
        bannerID = "ca-app-pub-3940256099942544/6300978111";
        interstitialID = "ca-app-pub-3940256099942544/1033173712";
        videoID = "ca-app-pub-3940256099942544/5224354917";
        nativeBannerID = "ca-app-pub-3940256099942544/2247696110";
#endif
        appID = "ca-app-pub-3718311388385619~4677941061";
        string RewardedID = "ca-app-pub-3718311388385619/7529543102";
        string bannerID2 = "ca-app-pub-3718311388385619/9052300341";
        ad = Admob.Instance();
        ad.bannerEventHandler += onBannerEvent;
        ad.interstitialEventHandler += onInterstitialEvent;
        ad.rewardedVideoEventHandler += onRewardedVideoEvent;
        ad.nativeBannerEventHandler += onNativeBannerEvent;
        ad.initSDK(appID);//optional
        ad.initAdmob(bannerID, interstitialID);
        //all id are admob test id,change those to your
        //ad.setTesting(true);//show test ad
        //ad.setNonPersonalized(true);//if want load NonPersonalized only,set true
        // ad.setIsDesignedForFamilies(true);//if is Is Designed For Families set true
        // ad.setGender(AdmobGender.MALE);
        //  string[] keywords = { "game","crash","male game"};
        //  ad.setKeywords(keywords);//set keywords for ad
        Debug.Log("admob inited -------------" + appID);

    }
    void onInterstitialEvent(string eventName, string msg) {
        Debug.Log("handler onAdmobEvent---" + eventName + "   " + msg);
        if (eventName == AdmobEvent.onAdLoaded) {
            Admob.Instance().showInterstitial();
        }
    }
    void onBannerEvent(string eventName, string msg) {
        Debug.Log("handler onAdmobBannerEvent---" + eventName + "   " + msg);
    }
    void onRewardedVideoEvent(string eventName, string msg) {
        Debug.Log("handler onRewardedVideoEvent---" + eventName + "  rewarded: " + msg);
    }
    void onNativeBannerEvent(string eventName, string msg) {
        Debug.Log("handler onAdmobNativeBannerEvent---" + eventName + "   " + msg);
    }
}
