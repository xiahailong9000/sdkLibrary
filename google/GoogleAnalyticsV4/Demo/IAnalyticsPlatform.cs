////
////  Created by wanglidong on 10/21/2017.
////
using System.Collections.Generic;
namespace Topifish.Analytics {
	/// 统计平台接口
	interface IAnalyticsPlatform {
		/// 初始化
		void Init(string sessionId);
		/// 结束会话
		void StopSession();
		/// 打印debug流程
		void LogFlow(string flowName);
		/// 当前屏幕
		void LogScreen(string screenName);
		/// 崩溃和异常
		void LogException(string message);
		/// event收集
		void LogEvent(AnalyticsEvent eventData);
        /// event收集
        /// 简单event直接使用event
        void LogEvent(string eventCategory, string eventAction, string eventLabel, long eventValue = 0);
        /// 设置用户id
        void SetUserID(string userId, bool isNewPlayer, int loginType);
        /// <summary>
        /// 采样数据
        /// </summary>
        void LogSample();
        #if UNITY_EDITOR
        void LogEvent_Editor(AnalyticsEvent eventData);
		#endif
	}
	/// 数据收集基类
	public abstract class AnalyticsEvent {
		/// 会话状态记录
		private const int GA_CD_ST = 6;
		protected void SetSessionStatus(string status) {
			_ga.SetCustomDimension(GA_CD_ST, status);
		}
		// 重复使用同一个对象
		protected static EventHitBuilder _ga;
		/// 转化为ga数据
		public EventHitBuilder GetGA() {
			if(_ga == null) {
				_ga = new EventHitBuilder();
			} else {
				_ga.SetEventCategory("");
				_ga.SetEventAction("");
				_ga.SetEventLabel("");
				_ga.GetCustomDimensions().Clear();
				_ga.GetCustomMetrics().Clear();
			}
			SetGAData();
			#if UNITY_EDITOR
			// 打印参数
		    UnityEngine.Debug.LogFormat("cat={0},act={1},lbl={2},cd={3}", _ga.GetEventCategory(),_ga.GetEventAction(),_ga.GetEventLabel(),string.Join(";", (new List<string>(_ga.GetCustomDimensions().Values)).ToArray()));
			#endif
			return _ga;
		}
		/// event Custom properties set
		abstract protected void SetGAData();
	}
}
