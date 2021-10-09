using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Reflection;
using System;

namespace TSkillEditor3D
{
    public class AudioEventData : EventDataBase
    {
        private ObjectField mAudioField;

        public override SkillEventType GetEventType() { return SkillEventType.AUDIO; }
        public override Color GetDisplayColor() { return new Color(0.627f, 0.125f, 0.941f); }

        public override void Init()
        {
            this.mAudioField = new ObjectField("Audio Clip:");
            this.mAudioField.objectType = typeof(AudioClip);
            this.mRootPanel.Add(this.mAudioField);
        }

        public override bool IsPreviewing() { return false; }

        public override void LoadFromFile(string aContent)
        {
            // Ù–‘–Ú¡–
            //0       1         2         3
            //EventID EventName StartTime AudioAssetPath

            base.LoadFromFile(aContent);
            string[] parms = aContent.Split(' ');
            string strAudioPath = parms[3];
            this.mAudioField.value = AssetDatabase.LoadAssetAtPath<AudioClip>(strAudioPath);
        }

        public override string SaveToFile()
        {
            AudioClip rAC = this.mAudioField.value as AudioClip;
            if (rAC == null)
            {
                EditorUtility.DisplayDialog("Error", "The AudioClip can't be null!", "OK");
                return string.Empty;
            }
            return base.SaveToFile() + " " + AssetDatabase.GetAssetPath(rAC);
        }

    }
}
