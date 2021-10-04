using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public class T3DAttachVFXEditorData : T3DSkillEditorDataBase
    {
        //需要保存的数据
        private SkillEffectAttachPart mAttachPart = SkillEffectAttachPart.FOOT; //附着对象
        private float mDuration = 0;    //持续时间

        //中间空间引用
        private ObjectField mEffectPrefabField = null;  //特效资源
        private EnumField mAttachPartField = null;
        private FloatField mDurationField = null;
        private ParticleSystem mEffectObj = null;       //特效对象
        private float mCurSimulateTime = 0;             //已经播放了的时间
        private bool mSimulating = false;               //是否开始播放

        public override SkillEventType GetEventType() { return SkillEventType.ATTACH_VFX; }
        public override Color GetDisplayColor() { return new Color(1, 0.6f, 0.2f, 1); }

        public override void Init()
        {
            this.mEffectPrefabField = new ObjectField("Effect Prefab:");
            this.mEffectPrefabField.objectType = typeof(ParticleSystem);
            this.mRootPanel.Add(this.mEffectPrefabField);

            this.mAttachPartField = new EnumField("Attach Part:", SkillEffectAttachPart.FOOT);
            this.mAttachPartField.RegisterCallback<ChangeEvent<System.Enum>>((aEvt) => 
            {
                this.mAttachPart = (SkillEffectAttachPart)aEvt.newValue;
            });
            this.mRootPanel.Add(this.mAttachPartField);

            this.mDurationField = new FloatField("Duration:");
            this.mDurationField.RegisterCallback<ChangeEvent<float>>((aEvt) =>
            {
                this.mDuration = aEvt.newValue;
            });
            this.mRootPanel.Add(this.mDurationField);

            VisualElement rPanel = SkillEditorCommon.CreateVisualElement(this.mRootPanel);
            rPanel.style.minHeight = 20;
            SkillEditorCommon.CreateLable("Preview", this.mRootPanel, 3, 3, 1, 1);
            Button rPreviewBtn = new Button();
            this.mRootPanel.Add(rPreviewBtn);
            rPreviewBtn.text = "Preview";
            rPreviewBtn.style.width = 60;
            rPreviewBtn.clicked += this.OnClickPreview;
        }

        public override void Clear()
        {
            if (this.mEffectObj != null)
            {
                GameObject.DestroyImmediate(this.mEffectObj.gameObject);
            }
        }

        public override void Update(float aDeltaTime)
        {
            this.SimulateEffect(aDeltaTime);
        }

        public override float GetDuration()
        {
            return this.mDuration;
        }

        public override void LoadFromFile(string aContent)
        {
            //属性序列
            //0       1         2         3        4          5
            //EventID EventName StartTime Duration AttachPart EffectAssetPath

            base.LoadFromFile(aContent);
            string[] parms = aContent.Split(' ');
            this.mDurationField.value = this.mDuration = System.Convert.ToSingle(parms[3]);
            this.mAttachPartField.value = this.mAttachPart = (SkillEffectAttachPart)System.Convert.ToInt32(parms[4]);
            this.mEffectPrefabField.value = AssetDatabase.LoadAssetAtPath<ParticleSystem>(parms[5]);
        }

        public override string SaveToFile()
        {
            ParticleSystem rPS = this.mEffectPrefabField.value as ParticleSystem;
            if (rPS == null)
            {
                EditorUtility.DisplayDialog("Error", "The AnimationClip can't be null!", "OK");
                return string.Empty;
            }

            return base.SaveToFile() + " "
                + this.mDuration + " "
                + (int)this.mAttachPart + " "
                + AssetDatabase.GetAssetPath(rPS);
        }

        public override void Preview()
        {
            this.OnClickPreview();
        }

        public override bool IsPreviewing()
        {
            return this.mSimulating;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void OnClickPreview()
        {
            ParticleSystem rPS = this.mEffectPrefabField.value as ParticleSystem;
            if (rPS == null)
            {
                EditorUtility.DisplayDialog("Error", "The Particle Object is null!", "OK");
                return;
            }

            if (this.mEffectObj == null)
            {
                this.mEffectObj = GameObject.Instantiate(rPS);
                GameObject rAvatar = this.AvatarObjField.value as GameObject;
                if (this.mAttachPart == SkillEffectAttachPart.FOOT)
                {
                    this.mEffectObj.transform.SetParent(rAvatar.transform, false);
                }
                else if (this.mAttachPart == SkillEffectAttachPart.WEAPON)
                {
                    GameObject rMountNode = TFramework.LogicUtilTool.FindChildByName(rAvatar, "WeaponEffectMount");
                    this.mEffectObj.transform.SetParent(rMountNode.transform, true);
                    this.mEffectObj.transform.localPosition = Vector3.zero;
                }
            }

            if (this.mSimulating == true)
            {
                this.mEffectObj.Simulate(0, true, true);
                this.mCurSimulateTime = 0;
            }

            this.mSimulating = true;
        }

        private void SimulateEffect(float aDeltaTime)
        {
            if (this.mSimulating == false)
            {
                return;
            }

            this.mEffectObj.Simulate(aDeltaTime, true, false);
            SceneView.RepaintAll();

            this.mCurSimulateTime += aDeltaTime;
            if (this.mCurSimulateTime >= this.mDuration)
            {
                this.mSimulating = false;
                this.mCurSimulateTime = 0;
                GameObject.DestroyImmediate(this.mEffectObj.gameObject);
                this.mEffectObj = null;
            }
        }
    }
}