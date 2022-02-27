using System.Collections;
using System.Collections.Generic;
using System.IO;
using Tabtale.TTPlugins;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using UnityEngine;

public class TTPPrivacySettingsPostProcess
{
    [PostProcessBuild(40006)]
    public static void OnPostProcessBuild(BuildTarget target, string path)
    {
        Debug.Log("TTPPrivacySettingsPostProcess::OnPostprocessBuild");
#if CRAZY_LABS_CLIK
        Debug.Log("TTPPrivacySettingsPostProcess::OnPostprocessBuild: in CLIK");
        var includedServicesPath = "Assets/Tabtale/TTPlugins/CLIK/Resources/ttpIncludedServices.asset";
        if (File.Exists(includedServicesPath))
        {
            Debug.Log("TTPPrivacySettingsPostProcess::OnPostprocessBuild: found included services asset");
            var includedServices = AssetDatabase.LoadAssetAtPath<TTPIncludedServicesScriptableObject>(includedServicesPath);
            if (!includedServices.privacySettings)
            {
                Debug.Log("TTPPrivacySettingsPostProcess::OnPostprocessBuild: privacy settings not included");
                return;
            }
        }
#endif
        string attText = null;
        var additionalConfigPath =
            "Assets/StreamingAssets/ttp/configurations/additionalConfig.json";
        if (File.Exists(additionalConfigPath))
        {
            var json = File.ReadAllText(additionalConfigPath);
            if (!string.IsNullOrEmpty(json))
            {
                var dic = TTPJson.Deserialize(json) as Dictionary<string, object>;
                if (dic != null)
                {
                    if (dic.ContainsKey("attText") && dic["attText"] is string)
                    {
                        attText = dic["attText"] as string;
                    }
                }
            }
        }
        
#if UNITY_IOS
        var pbxProjectPath = UnityEditor.iOS.Xcode.PBXProject.GetPBXProjectPath(path);
        var pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
        pbxProject.ReadFromString(System.IO.File.ReadAllText(pbxProjectPath));
        var plistPath = Path.Combine(path, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        var rootDict = plist.root;
        rootDict.SetString("NSUserTrackingUsageDescription", "By pressing \"Allow\" we will be able to provide you personalized ads.");

        File.WriteAllText(plistPath, plist.WriteToString());
#endif
    }
}
