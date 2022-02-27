#if TTP_RATEUS
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Scripting;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

namespace Tabtale.TTPlugins
{

    public class TTPRateUs
    {
#if UNITY_IOS && !TTP_DEV_MODE
        [DllImport ("__Internal")]
        private static extern void ttpRateUsDisplayRateUsModal();

        [DllImport("__Internal")]
        private static extern void ttpRateUsGoToStore();
#endif

        private const string PLAYER_PREFS_NEVER = "TTPRateUsNeverShow";
        private const string PLAYER_PREFS_LATER = "TTPRateUsLaterNextTime";

        private static bool _neverShow;
        private static DateTime? _nextShowTime = null;
        private static double _coolDown = -1;
        private static string _iconFileExtenstion;
        private static TTPRateUsCanvas _rateUsCanvas;
        private static TTPRateUsNotConnectedCanvas _notConnectedCanvas;

        public static void Button()
        {
            if (TTPCore.IncludedServices != null && !TTPCore.IncludedServices.rateUs)
            {
                return;
            }
            Debug.Log("TTPRateUs::Button:");
            if (TTPCore.IsConnectedToTheInternet())
            {
                GoToRateFromButton();
            }
            else
            {
                LogPopUpEvent("rateUsButton", false, false, false, true, "NA");
                InformNoInternet();
            }

        }

        public static void Popup()
        {
            if (TTPCore.IncludedServices != null && !TTPCore.IncludedServices.rateUs)
            {
                return;
            }
            Debug.Log("TTPRateUs::Popup");
            if (ShouldSuggestRateUs())
            {
                SuggestRateUs();
            }
        }

        static void Later()
        {
            Debug.Log("TTPRateUs::Later:Cooldown=" + Cooldown);
            LogPopUpEvent("rateUsPopup", true, false, false, false, "later");
            NextShowTime = DateTime.Now.AddMinutes(Cooldown);
        }

        static void SuggestRateUs()
        {
            Debug.Log("TTPRateUs::SuggestRateUs:");
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IOS && !TTP_DEV_MODE
                ttpRateUsDisplayRateUsModal();        
#endif
            }
            else
            {
#if TTP_POPUPMGR
                TTPPopupMgr.OnShown("rateus");
#endif

                UnityEngine.Object rateUsCanvasResource = Resources.Load("TTPRateUsCanvas");
                if (rateUsCanvasResource != null)
                {
                    GameObject rateUsCanvasGO = (GameObject)UnityEngine.Object.Instantiate(rateUsCanvasResource);
                    _rateUsCanvas = rateUsCanvasGO.GetComponent<TTPRateUsCanvas>();
                    _rateUsCanvas.Init(
                        () =>
                        {
                            GoToRateFromPopup();
                        },
                         () =>
                         {
                             Never(true);
                         },
                          () =>
                          {
                              Later();
                          },
                          () =>
                          {
                              Closed();
                          },
                          IconFileExtenstion
                        );
                }
            }
        }
        [Preserve]
        static bool HandleAndroidBackPressed()
        {
            bool handled = false;
            if (_rateUsCanvas != null)
            {
                Later();
                _rateUsCanvas.OnClickClose();
                handled = true;
            }

            if (_notConnectedCanvas != null)
            {
                _notConnectedCanvas.OnClickClose();
                handled = true;
            }
            return handled;

        }

        static void InformNoInternet()
        {
            Debug.Log("TTPRateUs::InformNoInternet:");
            UnityEngine.Object noConnectionCanvasResource = Resources.Load("TTPRateUsNotConnectedCanvas");
            if (noConnectionCanvasResource != null)
            {
                GameObject go = (GameObject)UnityEngine.Object.Instantiate(noConnectionCanvasResource);
                _notConnectedCanvas = go.GetComponent<TTPRateUsNotConnectedCanvas>();
            }
        }

        private static void GoToRateFromPopup()
        {
            Debug.Log("TTPRateUs::GoToRateFromPopup:");
            LogPopUpEvent("rateUsPopup", true, false, false, false, "rate");
            Never(false);
            GoToRate();
        }

        private static void GoToRateFromButton()
        {
            Debug.Log("TTPRateUs::GoToRateFromButton:");
            LogPopUpEvent("rateUsButton", true, false, false, false, "rate");
            GoToRate();
        }

        private static void GoToRate()
        {
            Debug.Log("TTPRateUs::GoToRate:");
            if (TTPCore.DevMode)
            {
                return;
            }
            if (UnityEngine.Application.platform == UnityEngine.RuntimePlatform.Android ||
                        UnityEngine.Application.platform == UnityEngine.RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IOS && !TTP_DEV_MODE
                ttpRateUsGoToStore();
#elif UNITY_ANDROID
                AndroidJavaObject appLauncher = new AndroidJavaObject("com.tabtale.ttplugins.ttpcore.common.TTPAppLauncher");
                if (appLauncher != null)
                {
                    appLauncher.Call("OpenAppImpl", "google", null, null, ((TTPCore.ITTPCoreInternal)TTPCore.Impl).GetCurrentActivity());
                }
                else
                {
                    Debug.Log("TTPRateUs:: Could not initiate appLauncher - class not found");
                }
#endif
            }

#if UNITY_EDITOR
            Debug.Log("TTPRateUs::GoToRate");
#endif
        }


        private static bool ShouldSuggestRateUs()
        {
            Debug.Log("TTPRateUs::ShouldSuggestRateUs:");
            if (!TTPCore.IsConnectedToTheInternet())
            {
                LogPopUpEvent("rateUsPopup", false, false, false, true, "NA");
                Debug.Log("TTPRateUs::ShouldSuggestRateUs: not connected to the internet. will not show.");
                return false;
            }
            if (_neverShow)
            {
                LogPopUpEvent("rateUsPopup", false, true, false, false, "NA");
                Debug.Log("TTPRateUs::ShouldSuggestRateUs: neverShow marked. will not show.");
                return false;
            }
            if (PlayerPrefs.GetInt(PLAYER_PREFS_NEVER, 0) == 1)
            {
                LogPopUpEvent("rateUsPopup", false, true, false, false, "NA");
                Debug.Log("TTPRateUs::ShouldSuggestRateUs: neverShow marked in persistency. will not show.");
                Never(true);
                return false;
            }
            if (NextShowTime != null && DateTime.Now.CompareTo(NextShowTime) < 0)
            {
                LogPopUpEvent("rateUsPopup", false, false, true, false, "NA");
                Debug.Log("TTPRateUs::ShouldSuggestRateUs: coolDown has not passed. will not show.");
                return false;
            }
#if TTP_POPUPMGR        
            if(!TTPPopupMgr.ShouldShow("RateUs"))
            {
                Debug.Log("TTPRateUs::ShouldSuggestRateUs: PopupMgr return false. will not show.");
                return false;
            }
#endif
            return true;
        }

        private static void Never(bool sendAnalytics)
        {
            if (sendAnalytics)
            {
                LogPopUpEvent("rateUsPopup", true, false, false, false, "never");
            }
            Debug.Log("TTPRateUs::Never");
            _neverShow = true;
            PlayerPrefs.SetInt(PLAYER_PREFS_NEVER, 1);
        }

        private static void Closed()
        {
#if TTP_POPUPMGR
            TTPPopupMgr.OnClosed("rateus");
#endif

            Debug.Log("TTPRateUs::Closed");
        }

        [Serializable]
        private class RateUsConfiguration
        {
            public string iconUrl = ".png";
            public double coolDown = 0;
        }


        private static double Cooldown
        {
            get
            {
                if (_coolDown <= -1)
                {
                    string configurationJson = ((TTPCore.ITTPCoreInternal)TTPCore.Impl).GetConfigurationJson("rateUs");
                    if (!string.IsNullOrEmpty(configurationJson))
                    {
                        RateUsConfiguration configuration = JsonUtilityWrapper.FromJson<RateUsConfiguration>(configurationJson);
                        if (configuration != null)
                        {
                            _coolDown = configuration.coolDown;
                        }
                    }
                }
                return _coolDown;
            }
        }

        private static string IconFileExtenstion
        {
            get
            {
                if(_iconFileExtenstion == null)
                {
                    string configurationJson = ((TTPCore.ITTPCoreInternal)TTPCore.Impl).GetConfigurationJson("rateUs");
                    if (!string.IsNullOrEmpty(configurationJson))
                    {
                        RateUsConfiguration configuration = JsonUtilityWrapper.FromJson<RateUsConfiguration>(configurationJson);
                        if (configuration != null)
                        {
                            _iconFileExtenstion = configuration.iconUrl.Substring(configuration.iconUrl.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase)+1);
                        }
                    }
                }
                return _iconFileExtenstion;
            }
        }

        private static DateTime? NextShowTime
        {
            get
            {
                if(_nextShowTime == null)
                {
                    string timeStr = PlayerPrefs.GetString(PLAYER_PREFS_LATER);
                    if(!string.IsNullOrEmpty(timeStr))
                        _nextShowTime = DateTime.Parse(timeStr);
                }
                return _nextShowTime;
            }
            set
            {
                PlayerPrefs.SetString(PLAYER_PREFS_LATER, _nextShowTime.ToString());
                _nextShowTime = value;
            }
        }

        private static void LogPopUpEvent(string eventName, bool show, bool never, bool later, bool noInternet, string response)
        {
            Debug.Log("TTPRateUs::LogPopUpEvent:eventName=" + eventName + " response=" + response);
#if UNITY_ANDROID
            IDictionary<string, object> logEventParams = new Dictionary<string, object>()
            {
                {"showrateus", show},
                {"never", never},
                {"later", later},
                {"noInternet", noInternet},
                {"response", response}
            };

            System.Type analyticsClsType = System.Type.GetType("Tabtale.TTPlugins.TTPAnalytics");
            if (analyticsClsType != null)
            {
                System.Reflection.MethodInfo method = analyticsClsType.GetMethod("LogEvent", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                if (method != null)
                {
                    long ttAnalyticsConst = 1;
                    long ddnaAnalyticsConst = 1 << 1;
                    long firebaseAnalyticsConst = 1 << 2;
                    method.Invoke(null, new object[] { ttAnalyticsConst | ddnaAnalyticsConst | firebaseAnalyticsConst, eventName, logEventParams, false, true });
                }
                else
                {
                    Debug.LogWarning("CallAnalyticsByReflection:: could not find method - LogEvent");
                }
            }
            else
            {
                Debug.LogWarning("CallAnalyticsByReflection:: could not find TTPAnalytics class");
            }
#endif
        }
    }
}
#endif