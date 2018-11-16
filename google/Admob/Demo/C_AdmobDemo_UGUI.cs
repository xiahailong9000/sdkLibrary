using UnityEngine;
using System.Collections;
using admob;
using UnityEngine.UI;
public class C_AdmobDemo_UGUI : MonoBehaviour {

    public Button showInterstitial, showRewardVideo,
        showbanner, showbannerABS, removebanner,
        showNative, showNativeABS, removeNative;
    void Start() {
        Debug.LogError("---------开始支持--------------");
        showInterstitial.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.showInterstitial();
        });
        showRewardVideo.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.ShowVideo();
        });


        showbanner.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.ShowBottomAds();
        });
        showbannerABS.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.showBannerAbsolute();
        });
        removebanner.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.removeBanner();
        });
        showNative.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.showNativeBannerRelative();
        });
        showNativeABS.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.showNativeBannerAbsolute();
        });
        removeNative.onClick.AddListener(delegate () {
            AdmobAgent.GetInstance.removeNativeBanner();
        });
    }
}
