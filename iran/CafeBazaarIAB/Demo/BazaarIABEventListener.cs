using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using BazaarPlugin;


public class BazaarIABEventListener : MonoBehaviour {
#if UNITY_ANDROID

    void OnEnable() {
        // Listen to all events for illustration purposes
        IABEventManager.billingSupportedEvent += billingSupportedEvent;
        IABEventManager.billingNotSupportedEvent += billingNotSupportedEvent;
        IABEventManager.queryInventorySucceededEvent += queryInventorySucceededEvent;
        IABEventManager.queryInventoryFailedEvent += queryInventoryFailedEvent;
        IABEventManager.purchaseSucceededEvent += purchaseSucceededEvent;
        IABEventManager.purchaseFailedEvent += purchaseFailedEvent;
        IABEventManager.consumePurchaseSucceededEvent += consumePurchaseSucceededEvent;
        IABEventManager.consumePurchaseFailedEvent += consumePurchaseFailedEvent;
    }

    void OnDisable() {
        // Remove all event handlers
        IABEventManager.billingSupportedEvent -= billingSupportedEvent;
        IABEventManager.billingNotSupportedEvent -= billingNotSupportedEvent;
        IABEventManager.queryInventorySucceededEvent -= queryInventorySucceededEvent;
        IABEventManager.queryInventoryFailedEvent -= queryInventoryFailedEvent;
        IABEventManager.purchaseSucceededEvent -= purchaseSucceededEvent;
        IABEventManager.purchaseFailedEvent -= purchaseFailedEvent;
        IABEventManager.consumePurchaseSucceededEvent -= consumePurchaseSucceededEvent;
        IABEventManager.consumePurchaseFailedEvent -= consumePurchaseFailedEvent;
    }


    void billingSupportedEvent() {
        Debug.LogError("---伊朗支付--------------计费支持事件--billingSupportedEvent");
    }

    void billingNotSupportedEvent(string error) {
        Debug.LogError("---伊朗支付-------------计费不支持事件---billingNotSupportedEvent:==" + error);
    }

    void queryInventorySucceededEvent(List<BazaarPurchase> purchases, List<BazaarSkuInfo> skus) {
        Debug.Log(string.Format("queryInventorySucceededEvent. total purchases:=={0}, total skus: =={1}", purchases.Count, skus.Count));

        for (int i = 0; i < purchases.Count; ++i) {
            Debug.Log(purchases[i].ToString());
        }

        Debug.LogError("---伊朗支付-----------------------查询库存成功事件----------------------");

        for (int i = 0; i < skus.Count; ++i) {
            Debug.Log(skus[i].ToString());
        }
    }

    void queryInventoryFailedEvent(string error) {
        Debug.LogError("---伊朗支付-------------查询库存败了-- -queryInventoryFailedEvent: ==" + error);
    }

    void purchaseSucceededEvent(BazaarPurchase purchase) {
        Debug.LogError("---伊朗支付-----------购买成功事件-----purchaseSucceededEvent: ==" + purchase);
    }

    void purchaseFailedEvent(string error) {
        Debug.LogError("---伊朗支付--------------购买失败事件--purchaseFailedEvent: ==" + error);
    }

    void consumePurchaseSucceededEvent(BazaarPurchase purchase) {
        Debug.LogError("---伊朗支付-------------消费购买事件---consumePurchaseSucceededEvent: " + purchase);
    }

    void consumePurchaseFailedEvent(string error) {
        Debug.LogError("---伊朗支付--------------消费购买失败--consumePurchaseFailedEvent: " + error);
    }

#endif

}


