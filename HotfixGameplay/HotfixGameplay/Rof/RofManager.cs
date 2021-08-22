using UnityEngine;
using System;
using System.Reflection;
using Rof;
using HotfixGameplay.Framework;
using TFramework;

namespace HotfixGameplay
{
    public interface IRofBase
    {
        int ReadBody(byte[] rData, int nOffset);
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ROFPathAttribute : System.Attribute
    {
        public string AssetName;
        public ROFPathAttribute(string rAssetName)
        {
            this.AssetName = rAssetName;
        }
    }

    public class RofManager : HotfixSingleton<RofManager>
    {
        public int CurrentLanguage = 0;

        //add below
        [ROFPath("RofLanguage")]
        public RofLanguageTable LanguageTable { get; private set; }


        ////////////////////////////////////////////////////////////////////////////////////////////
        //Don't modify this function
        public string GetMultiLanguage(int nID)
        {
            RofLanguageRow RofRow = RofManager.GetSingleton().LanguageTable.GetDataByID(nID);
            if (RofRow == null)
            {
                return nID.ToString();
            }
            if (this.CurrentLanguage == 1)
            {
                return RofRow.Chinese;
            }
            else
            {
                return RofRow.English;
            }
        }

        public void Initialize(string rConfigAssetPath)
        {
            var rAssetsRequest = AssetLoader.LoadAllAssets(rConfigAssetPath);
            if (rAssetsRequest == null) return;

            Type rType = this.GetType();
            var rBindingFlags = BindingFlags.GetProperty | BindingFlags.SetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rPropInfos = rType.GetProperties(rBindingFlags);
            for (int i = 0; i < rPropInfos.Length; i++)
            {
                var rAttrObjs = rPropInfos[i].GetCustomAttributes(typeof(ROFPathAttribute), false);
                if (rAttrObjs == null || rAttrObjs.Length == 0) continue;

                var rConfigPathAttr = rAttrObjs[0] as ROFPathAttribute;
                if (rConfigPathAttr == null) continue;

                var rRofAssetName = rConfigPathAttr.AssetName;
                TextAsset rTextAsset = null;
                for (int k = 0; k < rAssetsRequest.Length; k++)
                {
                    var rTempAsset = rAssetsRequest[k];
                    if (rTempAsset == null) continue;

                    if (rRofAssetName.Equals(rTempAsset.name))
                    {
                        rTextAsset = rTempAsset as TextAsset;
                        break;
                    }
                }
                if (rTextAsset != null)
                {
                    var rFiledObj = HotfixReflectionAssist.Construct(rPropInfos[i].PropertyType);
                    if (rFiledObj != null)
                    {
                        HotfixReflectionAssist.MethodMember(rFiledObj, "Init", HotfixReflectionAssist.flags_method_inst, rTextAsset.bytes);
                        rPropInfos[i].SetValue(this, rFiledObj);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("Cannot find rof asset {0}.", rRofAssetName);
                }
            }
        }
    }

}
