/*
  Copyright 2014 Google Inc. All rights reserved.

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

      http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/*
  GoogleAnalyticsV4 is an interface for developers to send hits to Google
  Analytics.
  The class will delegate the hits to the appropriate helper class depending on
  the platform being built for - Android, iOS or Measurement Protocol for all
  others.

  Each method has a simple form with the hit parameters, or developers can
  pass a builder to the same method name in order to add custom metrics or
  custom dimensions to the hit.
*/
public class GoogleAnalyticsV4 : MonoBehaviour {
    private string uncaughtExceptionStackTrace = null;
    private bool initialized = false;

    public enum DebugMode {
        ERROR,
        WARNING,
        INFO,
        VERBOSE
    };

    // [Tooltip("The tracking code to be used for Android. Example value: UA-XXXX-Y.")]
    //用于Android的跟踪代码。实例值
    public string androidTrackingCode = "UA-96512667-1";
    // [Tooltip("The tracking code to be used for iOS. Example value: UA-XXXX-Y.")]
    //用于iOS的跟踪代码。实例值
    public string IOSTrackingCode = "UA-96512667-4";
    // [Tooltip("The tracking code to be used for platforms other than Android and iOS. Example value: UA-XXXX-Y.")]
    //跟踪代码将用于Android和iOS以外的平台。实例值
    public string otherTrackingCode = "UA-96512667-1";//---------------------------------

    //[Tooltip("The application name. This value should be modified in the Unity Player Settings.")]
    public string productName = "WarOfBioech";//-------------------------------------

    //[Tooltip("The application identifier. Example value: com.company.app.")]
    //应用程序标识符。示例值：COM公司应用程序
    public string bundleIdentifier = "com.topifish.mech";//------------------------------

    //[Tooltip("The application version. Example value: 1.2")]
    //应用程序版本。实例值
    public string bundleVersion = "0.16.3.63"; //------------------------------------------

    // [RangedTooltip("The dispatch period in seconds. Only required for Android and iOS.", 0, 3600)]
    //以秒为单位的调度周期。只需要Android和iOS。
    public int dispatchPeriod = 1;

    //[RangedTooltip("The sample rate to use. Only required for Android and iOS.", 0, 100)]
    //样品使用率。只需要Android和iOS
    public int sampleFrequency = 100;

    //[Tooltip("The log level. Default is WARNING.")]
    //日志级别。默认为警告
    public DebugMode logLevel = DebugMode.VERBOSE;  //-------------------------------------

    // [Tooltip("If checked, the IP address of the sender will be anonymized.")]
    //如果选中，发送方的IP地址将匿名化。
    public bool anonymizeIP = false;

    //[Tooltip("Automatically report uncaught exceptions.")]
    //自动报告异常
    public bool UncaughtExceptionReporting = false;

    // [Tooltip("Automatically send a launch event when the game starts up.")]
    //当游戏启动时自动发送启动事件
    public bool sendLaunchEvent = true;

    // [Tooltip("If checked, hits will not be dispatched. Use for testing.")]
    //如果选中，点击将不会被发送。用于测试
    public bool dryRun = false;

    // TODO: Create conditional textbox attribute
    // [Tooltip("The amount of time in seconds your application can stay in" + "the background before the session is ended. Default is 30 minutes" + " (1800 seconds). A value of -1 will disable session management.")]
    public int sessionTimeout = 1800;

    // [AdvertiserOptIn()]
    //如果启用此集合，请确保检查并遵守用于SDK和广告功能的Google Analytics策略。点击下面的按钮在浏览器中查看它们。
    //https://support.google.com/analytics/answer/2700409
    public bool enableAdId = false;

    public static GoogleAnalyticsV4 instance = null;

    [HideInInspector]
    public readonly static string currencySymbol = "USD";
    public readonly static string EVENT_HIT = "createEvent";
    public readonly static string APP_VIEW = "createAppView";
    public readonly static string SET = "set";
    public readonly static string SET_ALL = "setAll";
    public readonly static string SEND = "send";
    public readonly static string ITEM_HIT = "createItem";
    public readonly static string TRANSACTION_HIT = "createTransaction";
    public readonly static string SOCIAL_HIT = "createSocial";
    public readonly static string TIMING_HIT = "createTiming";
    public readonly static string EXCEPTION_HIT = "createException";

#if UNITY_ANDROID && !UNITY_EDITOR
    private GoogleAnalyticsAndroidV4 androidTracker = new GoogleAnalyticsAndroidV4();
#elif UNITY_IPHONE && !UNITY_EDITOR
  private GoogleAnalyticsiOSV3 iosTracker = new GoogleAnalyticsiOSV3();
#else
    private GoogleAnalyticsMPV3 mpTracker = new GoogleAnalyticsMPV3();
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
    public GoogleAnalyticsAndroidV4 GetTracker() {
        return androidTracker;
    }
#endif

    void Awake() {
        InitializeTracker();
        if (sendLaunchEvent) {
            LogEvent("Google Analytics", "Auto Instrumentation", "Game Launch", 0);
        }

        if (UncaughtExceptionReporting) {
#if UNITY_5 || UNITY_2017
            Application.logMessageReceived += HandleException;
#else
            Application.RegisterLogCallback(HandleException);
#endif
            if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
                Debug.Log("Enabling uncaught exception reporting.");
            }
        }
    }

    void Update() {
        //#if !UNITY_EDITOR && ANALYSIS_ON
#if ANALYSIS_ON
        if (!string.IsNullOrEmpty(uncaughtExceptionStackTrace)) {
            LogException(uncaughtExceptionStackTrace, true);
            uncaughtExceptionStackTrace = null;
        }
#endif
    }

    private void HandleException(string condition, string stackTrace, LogType type) {
        if (type == LogType.Exception) {
            uncaughtExceptionStackTrace = condition + "\n" + stackTrace
                + UnityEngine.StackTraceUtility.ExtractStackTrace();
        }
    }

    // TODO: Error checking on initialization parameters
    private void InitializeTracker() {
        if (!initialized) {
            instance = this;

            //      DontDestroyOnLoad(instance);

            // automatically set app parameters from player settings if they are left empty
            if (string.IsNullOrEmpty(productName)) {
                productName = Application.productName;
            }
            if (string.IsNullOrEmpty(bundleIdentifier)) {
#if UNITY_5
                bundleIdentifier = Application.bundleIdentifier;
#elif UNITY_2017
        bundleIdentifier = Application.identifier;
#endif
            }
            if (string.IsNullOrEmpty(bundleVersion)) {
                bundleVersion = Application.version;
            }

            Debug.Log("Initializing Google Analytics 0.2.");
#if UNITY_ANDROID && !UNITY_EDITOR
            androidTracker.SetTrackingCode(androidTrackingCode);
            androidTracker.SetAppName(productName);
            androidTracker.SetBundleIdentifier(bundleIdentifier);
            androidTracker.SetAppVersion(bundleVersion);
            androidTracker.SetDispatchPeriod(dispatchPeriod);
            androidTracker.SetSampleFrequency(sampleFrequency);
            androidTracker.SetLogLevelValue(logLevel);
            androidTracker.SetAnonymizeIP(anonymizeIP);
            androidTracker.SetAdIdCollection(enableAdId);
            androidTracker.SetDryRun(dryRun);
            androidTracker.InitializeTracker();
#elif UNITY_IPHONE && !UNITY_EDITOR
      iosTracker.SetTrackingCode(IOSTrackingCode);
      iosTracker.SetAppName(productName);
      iosTracker.SetBundleIdentifier(bundleIdentifier);
      iosTracker.SetAppVersion(bundleVersion);
      iosTracker.SetDispatchPeriod(dispatchPeriod);
      iosTracker.SetSampleFrequency(sampleFrequency);
      iosTracker.SetLogLevelValue(logLevel);
      iosTracker.SetAnonymizeIP(anonymizeIP);
      iosTracker.SetAdIdCollection(enableAdId);
      iosTracker.SetDryRun(dryRun);
      iosTracker.InitializeTracker();
#else
            Debug.LogFormat("GoogleAnalytics注册------------otherTrackingCode=={0}------bundleIdentifier=={1}------productName=={2}------bundleVersion=={3}------logLevel=={4}--", otherTrackingCode,
                bundleIdentifier, productName, bundleVersion, logLevel, anonymizeIP, dryRun);
            mpTracker.SetTrackingCode(otherTrackingCode);
            mpTracker.SetBundleIdentifier(bundleIdentifier);
            mpTracker.SetAppName(productName);
            mpTracker.SetAppVersion(bundleVersion);
            mpTracker.SetLogLevelValue(logLevel);
            mpTracker.SetAnonymizeIP(anonymizeIP);
            mpTracker.SetDryRun(dryRun);
            mpTracker.InitializeTracker();
#endif
            initialized = true;
            SetOnTracker(Fields.DEVELOPER_ID, "GbOCSs");
        }
    }

    public void SetAppLevelOptOut(bool optOut) {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.SetOptOut(optOut);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.SetOptOut(optOut);
#else
        mpTracker.SetOptOut(optOut);
#endif
    }

    public void SetUserIDOverride(string userID) {
        SetOnTracker(Fields.USER_ID, userID);
    }

    public void ClearUserIDOverride() {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.ClearUserIDOverride();
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.ClearUserIDOverride();
#else
        mpTracker.ClearUserIDOverride();
#endif
    }

    public void DispatchHits() {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.DispatchHits();
#elif UNITY_IPHONE && !UNITY_EDITOR
  iosTracker.DispatchHits();
#else
        //Do nothing
#endif
    }

    public void StartSession() {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.StartSession();
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.StartSession();
#else
        mpTracker.StartSession();
#endif
    }

    public void StopSession() {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.StopSession();
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.StopSession();
#else
        mpTracker.StopSession();
#endif
    }

    // Use values from Fields for the fieldName parameter ie. Fields.SCREEN_NAME
    public void SetOnTracker(Field fieldName, object value) {
        InitializeTracker();
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.SetTrackerVal(fieldName, value);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.SetTrackerVal(fieldName, value);
#else
        mpTracker.SetTrackerVal(fieldName, value);
#endif
    }

    public void LogScreen(string title) {
        AppViewHitBuilder builder = new AppViewHitBuilder().SetScreenName(title);
        LogScreen(builder);
    }

    public void LogScreen(AppViewHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging screen.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogScreen(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogScreen(builder);
#else
        mpTracker.LogScreen(builder);
#endif
    }

    public void LogEvent(string eventCategory, string eventAction,
        string eventLabel, long value) {
        EventHitBuilder builder = new EventHitBuilder()
            .SetEventCategory(eventCategory)
            .SetEventAction(eventAction)
            .SetEventLabel(eventLabel)
            .SetEventValue(value);

        LogEvent(builder);
    }

    public void LogEvent(EventHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging event.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogEvent(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogEvent(builder);
#else
        mpTracker.LogEvent(builder);
#endif
    }

    public void LogTransaction(string transID, string affiliation,
        double revenue, double tax, double shipping) {
        LogTransaction(transID, affiliation, revenue, tax, shipping, "");
    }

    public void LogTransaction(string transID, string affiliation,
        double revenue, double tax, double shipping, string currencyCode) {
        TransactionHitBuilder builder = new TransactionHitBuilder()
            .SetTransactionID(transID)
            .SetAffiliation(affiliation)
            .SetRevenue(revenue)
            .SetTax(tax)
            .SetShipping(shipping)
            .SetCurrencyCode(currencyCode);

        LogTransaction(builder);
    }

    public void LogTransaction(TransactionHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging transaction.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogTransaction(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogTransaction(builder);
#else
        mpTracker.LogTransaction(builder);
#endif
    }

    public void LogItem(string transID, string name, string sku,
        string category, double price, long quantity) {
        LogItem(transID, name, sku, category, price, quantity, null);
    }

    public void LogItem(string transID, string name, string sku,
        string category, double price, long quantity, string currencyCode) {
        ItemHitBuilder builder = new ItemHitBuilder()
            .SetTransactionID(transID)
            .SetName(name)
            .SetSKU(sku)
            .SetCategory(category)
            .SetPrice(price)
            .SetQuantity(quantity)
            .SetCurrencyCode(currencyCode);

        LogItem(builder);
    }

    public void LogItem(ItemHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging item.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogItem(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogItem(builder);
#else
        mpTracker.LogItem(builder);
#endif
    }

    public void LogException(string exceptionDescription, bool isFatal) {
        ExceptionHitBuilder builder = new ExceptionHitBuilder()
            .SetExceptionDescription(exceptionDescription)
            .SetFatal(isFatal);

        LogException(builder);
    }

    public void LogException(ExceptionHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging exception.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogException(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogException(builder);
#else
        mpTracker.LogException(builder);
#endif
    }

    public void LogSocial(string socialNetwork, string socialAction,
        string socialTarget) {
        SocialHitBuilder builder = new SocialHitBuilder()
            .SetSocialNetwork(socialNetwork)
            .SetSocialAction(socialAction)
            .SetSocialTarget(socialTarget);

        LogSocial(builder);
    }

    public void LogSocial(SocialHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging social.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogSocial(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogSocial(builder);
#else
        mpTracker.LogSocial(builder);
#endif
    }

    public void LogTiming(string timingCategory, long timingInterval,
        string timingName, string timingLabel) {
        TimingHitBuilder builder = new TimingHitBuilder()
            .SetTimingCategory(timingCategory)
            .SetTimingInterval(timingInterval)
            .SetTimingName(timingName)
            .SetTimingLabel(timingLabel);

        LogTiming(builder);
    }

    public void LogTiming(TimingHitBuilder builder) {
        InitializeTracker();
        if (builder.Validate() == null) {
            return;
        }
        if (GoogleAnalyticsV4.belowThreshold(logLevel, GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            Debug.Log("Logging timing.");
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.LogTiming(builder);
#elif UNITY_IPHONE && !UNITY_EDITOR
    iosTracker.LogTiming(builder);
#else
        mpTracker.LogTiming(builder);
#endif
    }

    public void Dispose() {
        initialized = false;
#if UNITY_ANDROID && !UNITY_EDITOR
        androidTracker.Dispose();
#elif UNITY_IPHONE && !UNITY_EDITOR
#else
#endif
    }

    public static bool belowThreshold(GoogleAnalyticsV4.DebugMode userLogLevel,
        GoogleAnalyticsV4.DebugMode comparelogLevel) {
        if (comparelogLevel == userLogLevel) {
            return true;
        } else if (userLogLevel == GoogleAnalyticsV4.DebugMode.ERROR) {
            return false;
        } else if (userLogLevel == GoogleAnalyticsV4.DebugMode.VERBOSE) {
            return true;
        } else if (userLogLevel == GoogleAnalyticsV4.DebugMode.WARNING &&
          (comparelogLevel == GoogleAnalyticsV4.DebugMode.INFO ||
          comparelogLevel == GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            return false;
        } else if (userLogLevel == GoogleAnalyticsV4.DebugMode.INFO &&
          (comparelogLevel == GoogleAnalyticsV4.DebugMode.VERBOSE)) {
            return false;
        }
        return true;
    }

    // Instance for running Coroutines from platform specific classes
    public static GoogleAnalyticsV4 getInstance() {
        return instance;
    }
}
