using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Topifish.Analytics { 
    public class GoogleAnalyticsV4_GA : MonoBehaviour,IAnalyticsPlatform {
        private GoogleAnalyticsV4 m_googleAnalytics;
        private bool _isOff = false;
        /// 会话状态记录
        private const int GA_CD_ST = 6;
        private AppViewHitBuilder _viewHitBuilder;
        private EventHitBuilder _eventHitBuilder;
        private void Awake() {
            m_googleAnalytics = gameObject.GetComponent<GoogleAnalyticsV4>();
        }
        /// 是否关闭ga统计
        public void SetDisable(bool value) {
            _isOff = value;
        }
        public void EnableSample(bool v) {}
        public void Init(string sessionId) {
            if (_isOff)
                return;
    #if !GA_RELEASE_TRACK
            // 除了GooglePlay发布的版本其他都用other
            m_googleAnalytics.androidTrackingCode = m_googleAnalytics.otherTrackingCode;
#endif
            // set app version
            m_googleAnalytics.bundleVersion = Application.version;// TFDevice.GameVersion();
            m_googleAnalytics.DispatchHits();
            m_googleAnalytics.StartSession();
        }
        public void StopSession() {
            if (_isOff)
                return;
            m_googleAnalytics.StopSession();
        }
        /// 设置用户id
        public void SetUserID(string userId, bool isNewPlayer, int loginType) {
            if (_isOff)
                return;
            // 不启用ga user-id
            //m_googleAnalytics.SetUserIDOverride(userId);
        }
        /// <summary>
        /// ga不统计debug流程信息
        /// </summary>
        public void LogFlow(string flowName) {}
        /// <summary>
        /// ga不统计sample流程信息
        /// </summary>
        public void LogSample() {}
        public void LogScreen(string screenName) {
            if (_isOff)
                return;
            if (_viewHitBuilder == null)
                _viewHitBuilder = new AppViewHitBuilder();
            else {
                _viewHitBuilder.GetCustomDimensions().Clear();
                _viewHitBuilder.GetCustomMetrics().Clear();
            }
            _viewHitBuilder.SetScreenName(screenName);
            _viewHitBuilder.SetCustomDimension(GA_CD_ST, screenName);
            m_googleAnalytics.LogScreen(_viewHitBuilder);
        }
        public void LogException(string message) {
            m_googleAnalytics.LogException(new ExceptionHitBuilder().SetExceptionDescription(message));
            // debug error log
            m_googleAnalytics.LogScreen("[E]" + message);
        }
        public void LogEvent(AnalyticsEvent eventData) {
            m_googleAnalytics.LogEvent(eventData.GetGA());
        }
        public void LogEvent(string eventCategory, string eventAction, string eventLabel, long eventValue = 0) {
            if (_eventHitBuilder == null)
                _eventHitBuilder = new EventHitBuilder();
            else {
                //_eventHitBuilder.GetCustomDimensions().Clear();
                //_eventHitBuilder.GetCustomMetrics().Clear();
            }
            if (string.IsNullOrEmpty(eventCategory))
                return;
            if (string.IsNullOrEmpty(eventAction))
                return;
            if (string.IsNullOrEmpty(eventLabel))
                return;
            _eventHitBuilder.SetEventCategory(eventCategory).SetEventAction(eventAction) .SetEventLabel(eventLabel).SetEventValue(eventValue);
            m_googleAnalytics.LogEvent(_eventHitBuilder);
        }
    #if UNITY_EDITOR
        public void LogEvent_Editor(AnalyticsEvent eventData) {
            eventData.GetGA();
        }
    #endif
    }
}