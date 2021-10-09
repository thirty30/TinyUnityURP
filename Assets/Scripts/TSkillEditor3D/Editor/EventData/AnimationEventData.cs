using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class AnimationEventData : EventDataBase
    {
        //需要保存的数据
        private ObjectField mAnimationClipField = null; //动作对象

        //中间控件引用
        private bool mIsAutoSampleAnimation;
        private float mCurSampleTime = 0;
        private Slider mAnimationProgress = null;       //动作进度条
        private Label mCurAnimationTimeLabel = null;    //动作当前时间

        public override SkillEventType GetEventType() { return SkillEventType.ANIMATION; }
        public override Color GetDisplayColor() { return Color.green; }

        public override void Init()
        {
            this.mAnimationClipField = new ObjectField("Animation Clip:");
            this.mAnimationClipField.objectType = typeof(AnimationClip);
            this.mRootPanel.Add(this.mAnimationClipField);

            VisualElement rPanel = CommonUtility.CreateVisualElement(this.mRootPanel);
            rPanel.style.minHeight = 20;
            CommonUtility.CreateLable("Preview", this.mRootPanel, 3, 3, 1, 1);

            rPanel = CommonUtility.CreateVisualElement(this.mRootPanel);
            rPanel.style.flexDirection = FlexDirection.Row;
            {
                CommonUtility.CreateLable("Current Time:", rPanel, 3, 3, 1, 1);
                this.mCurAnimationTimeLabel = CommonUtility.CreateLable("0", rPanel, 3, 3, 1, 1);
                this.mCurAnimationTimeLabel.style.width = 40;

                this.mAnimationProgress = new Slider(0, 1);
                this.mAnimationProgress.style.width = 200;
                this.mAnimationProgress.value = 0;
                this.mAnimationProgress.style.marginLeft = 3;
                this.mAnimationProgress.style.marginRight = 3;
                this.mAnimationProgress.style.marginTop = 1;
                this.mAnimationProgress.style.marginBottom = 1;
                this.mAnimationProgress.RegisterValueChangedCallback<float>(this.OnAnimationProgressChanged);
                rPanel.Add(this.mAnimationProgress);
            }
        }

        public override void Update(float aDeltaTime)
        {
            this.SampleAnimation(aDeltaTime);
        }

        public override float GetDuration()
        {
            AnimationClip rClip = this.mAnimationClipField.value as AnimationClip;
            if (rClip == null)
            {
                return 0;
            }
            return rClip.length;
        }

        public override void Preview() 
        {
            this.mIsAutoSampleAnimation = true;
            this.mCurSampleTime = 0;
            this.mAnimationProgress.SetEnabled(false);
        }

        public override bool IsPreviewing()
        {
            return this.mIsAutoSampleAnimation;
        }

        public override void LoadFromFile(string aContent)
        {
            //属性序列
            //0       1         2         3             4
            //EventID EventName StartTime AnimationName AnimationAssetPath

            base.LoadFromFile(aContent);
            string[] parms = aContent.Split(' ');
            string strAnimationPath = parms[4];
            this.mAnimationClipField.value = AssetDatabase.LoadAssetAtPath<AnimationClip>(strAnimationPath);
        }

        public override string SaveToFile()
        {
            AnimationClip rAC = this.mAnimationClipField.value as AnimationClip;
            if (rAC == null)
            {
                EditorUtility.DisplayDialog("Error", "The AnimationClip can't be null!", "OK");
                return string.Empty;
            }
            return base.SaveToFile() + " " + rAC.name + " " + AssetDatabase.GetAssetPath(rAC);
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //预览播放动作
        private void SampleAnimation(float aDeltaTime)
        {
            if (this.mIsAutoSampleAnimation == false)
            {
                return;
            }

            AnimationClip rClip = this.mAnimationClipField.value as AnimationClip;
            if (rClip == null)
            {
                return;
            }

            GameObject rAvatar = this.AvatarObjField.value as GameObject;
            if (rAvatar == null)
            {
                return;
            }

            this.mCurSampleTime += aDeltaTime;
            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(rAvatar, rClip, this.mCurSampleTime);
            AnimationMode.EndSampling();
            SceneView.RepaintAll();

            if (this.mCurSampleTime >= rClip.length)
            {
                this.mIsAutoSampleAnimation = false;
                this.mCurSampleTime = 0;
                this.mAnimationProgress.SetEnabled(true);
            }

        }

        private void OnAnimationProgressChanged(ChangeEvent<float> aEvt)
        {
            AnimationClip rClip = this.mAnimationClipField.value as AnimationClip;
            if (rClip == null)
            {
                return;
            }

            float fCurValue = aEvt.newValue;
            this.mCurSampleTime = rClip.length * fCurValue;
            this.mCurAnimationTimeLabel.text = string.Format("{0:N2}s", this.mCurSampleTime);

            AnimationMode.BeginSampling();
            AnimationMode.SampleAnimationClip(this.AvatarObjField.value as GameObject, rClip, this.mCurSampleTime);
            AnimationMode.EndSampling();

            SceneView.RepaintAll();
        }







    }
}

