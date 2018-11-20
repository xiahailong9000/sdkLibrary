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
            logButton.onClick.AddListener(delegate () {
                o_GoogleAnalyticsV4.LogException("dddddddddddddddddddddd");
            });
        }
        void Update() {

        }
    }
}