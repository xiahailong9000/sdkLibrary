using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Topifish.Analytics {
    public class GoogleAnalyticsV4_Demo : MonoBehaviour {
        public Button logButton;
        GoogleAnalyticsV4_GA o_GoogleAnalyticsV4;
        void Start() {
            o_GoogleAnalyticsV4 = gameObject.GetComponent<GoogleAnalyticsV4_GA>();
            o_GoogleAnalyticsV4.Init("dddd");
            o_GoogleAnalyticsV4.SetUserID("userId==hhuu-ooowww-" + Random.Range(80, 120),true,5);
            logButton.onClick.AddListener(delegate () {
                o_GoogleAnalyticsV4.LogException("TestAAAAAAAAAAA--uuwww-"+ Random.Range(800, 12000));
            });
        }
        void Update() {

        }
    }
}