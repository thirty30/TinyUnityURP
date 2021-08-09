using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class RenderResolutionSetting
    {
        private static int mOriginWidth = 720;
        private static int mOriginHeight = 1584;

        public static void Initialize()
        {
            Application.targetFrameRate = 30;
            QualitySettings.vSyncCount = 2;

            mOriginWidth = Screen.width;
            mOriginHeight = Screen.height;
        }

        public static void SetResolution(int nLevelWidth)
        {
            float fOriginRatio = (float)mOriginHeight / (float)mOriginWidth;
            nLevelWidth = (nLevelWidth > mOriginWidth) ? mOriginWidth : nLevelWidth;// 取小
            int nLevelHeight = (int)(nLevelWidth * fOriginRatio);

            Screen.SetResolution(nLevelWidth, nLevelHeight, Screen.fullScreen);
        }

        public static void SetOriginResolution()
        {
            Screen.SetResolution(mOriginWidth, mOriginHeight, Screen.fullScreen);
        }
    }
}

