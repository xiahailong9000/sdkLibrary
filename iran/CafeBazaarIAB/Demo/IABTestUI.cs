using UnityEngine;
using BazaarPlugin;
public class IABTestUI : MonoBehaviour {
#if UNITY_ANDROID

    void OnGUI() {
        //GUILayout.BeginArea(new Rect(10f, 10f, Screen.width - 15f, Screen.height - 15f));
        //GUI.skin.button.fixedHeight = 80;
        GUI.skin.button.fontSize = 35;

        if (Button("Initialize IAB")) {
            Debug.LogError("---伊朗支付--1----------------Initialize IAB------");
            var key = "MIHNMA0GCSqGSIb3DQEBAQUAA4G7ADCBtwKBrwDN72wlPXo4pFK78rElKD+nwc9OnHHL+YYAt0o2Fm6H+7pNoOKLk/fbXrmV3jaL2cz99IClllFKEAvo6VbyRyIOD5cWpBCV+IFVobCPs9dtCV0M4DDqpVY2NUR9WownlNMwr/AwmwW750xS8BvQ9zt5+u7VEhLkAJPVxWJfr+kLHI7519s9T5eb58cdAM+bvJ1vT0pGx6te5DrV8IHUUCKpDYPy7kBfc9wdcT6EBUMCAwEAAQ==";
            BazaarIAB.init(key);
        }

        if (Button("Query Inventory-查询清单")) {
            Debug.LogError("---伊朗支付--1-------------------查询清单----");
            // enter all the available skus from the CafeBazaar Developer Portal in this array so that item information can be fetched for them
            //从这个数组中的CAFEBAZAAR开发门户中输入所有可用的SKU，以便可以为它们获取项目信息。
            var skus = new string[] { "com.fanafzar.bazaarplugin.test1"
                , "com.fanafzar.bazaarplugin.test2"
                , "com.fanafzar.bazaarplugin.test3"
                , "com.fanafzar.bazaarplugin.monthly_subscribtion_test"
                , "com.fanafzar.bazaarplugin.annually_subscribtion_test"};

            BazaarIAB.queryInventory(skus);
        }

        if (Button("Are subscriptions supported?--是否支持订阅")) {
            Debug.LogError("---伊朗支付--1---------------------是否支持订阅----");
            Debug.Log("subscriptions supported: " + BazaarIAB.areSubscriptionsSupported());
        }

        if (Button("Purchase Product Test1--采购产品 测试1")) {
            Debug.LogError("---伊朗支付--1---------------------采购产品 测试1----");
            BazaarIAB.purchaseProduct("com.fanafzar.bazaarplugin.test1");
        }

        if (Button("Purchase Product Test2---采购产品 测试2")) {
            Debug.LogError("---伊朗支付--1---------------------采购产品 测试2----");
            BazaarIAB.purchaseProduct("com.fanafzar.bazaarplugin.test2");
        }

        if (Button("Consume Purchase Test1--消费购买 test1")) {
            Debug.LogError("---伊朗支付--1---------------------消费购买 test1----");
            BazaarIAB.consumeProduct("com.fanafzar.bazaarplugin.test1");
        }

        if (Button("Consume Purchase Test2--消费购买 Test2")) {
            Debug.LogError("---伊朗支付--1---------------------消费购买 Test2----");
            BazaarIAB.consumeProduct("com.fanafzar.bazaarplugin.test2");
        }

        if (Button("Consume Multiple Purchases--多消费购买")) {
            Debug.LogError("---伊朗支付--1---------------------多消费购买----");
            var skus = new string[] { "com.fanafzar.bazaarplugin.test1", "com.fanafzar.bazaarplugin.test2" };
            BazaarIAB.consumeProducts(skus);
        }

        if (Button("Test Unavailable Item--测试不可用的")) {
            Debug.LogError("---伊朗支付--1---------------------测试不可用的----");
            BazaarIAB.purchaseProduct("com.fanafzar.bazaarplugin.unavailable");
        }

        if (Button("Purchase Monthly Subscription---购买月度订购")) {
            Debug.LogError("---伊朗支付--1---------------------购买月度订购----");
            BazaarIAB.purchaseProduct("com.fanafzar.bazaarplugin.monthly_subscribtion_test", "subscription payload");
        }

        if (Button("Purchase Annually Subscription---每年订购")) {
            Debug.LogError("---伊朗支付--1---------------------每年订购----");
            BazaarIAB.purchaseProduct("com.fanafzar.bazaarplugin.annually_subscribtion_test", "subscription payload");
        }

        if (Button("Enable High Details Logs--启用高细节日志")) {
            Debug.LogError("---伊朗支付--1---------------------启用高细节日志----");
            BazaarIAB.enableLogging(true);
        }
      //  GUILayout.EndArea();
    }

    bool Button(string label) {
       // GUILayout.Space(5);
        return GUILayout.Button(label, GUILayout.MinWidth(1200), GUILayout.Height(95));
    }

#endif

}

