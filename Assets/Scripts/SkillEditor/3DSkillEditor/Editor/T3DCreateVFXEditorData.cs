using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public class T3DCreateVFXEditorData : T3DSkillEditorDataBase
    {
        //需要保存的数据
        private ObjectField mEffectPrefabField = null; //动作对象
        private Vector3 mPosition = Vector3.zero;
        private Vector3 mRotation = Vector3.zero;

        //中间空间引用
        private ParticleSystem mEffectObj = null;       //特效对象
        private float mEffectDuration = 0;              //特效总时间
        private float mCurSimulateTime = 0;             //已经播放了的时间
        private bool mSimulating = false;               //是否开始播放
        private GameObject mPositionObj = null;         //特效位置
        private Vector3Field mPositionField = null;
        private Vector3Field mRotationField = null;


        public override SkillEventType GetEventType() { return SkillEventType.CREATE_VFX; }

        public override void Init()
        {
            this.mEffectPrefabField = new ObjectField("Effect Prefab:");
            this.mEffectPrefabField.objectType = typeof(ParticleSystem);
            this.mRootPanel.Add(this.mEffectPrefabField);

            this.mPositionField = new Vector3Field("Position:");
            this.mPositionField.SetEnabled(false);
            this.mRootPanel.Add(this.mPositionField);

            this.mRotationField = new Vector3Field("Rotation:");
            this.mRotationField.SetEnabled(false);
            this.mRootPanel.Add(this.mRotationField);

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
            GameObject.DestroyImmediate(this.mPositionObj);
        }

        public override void Update(float aDeltaTime)
        {
            this.SimulateEffect(aDeltaTime);

            //更新坐标 和 方向
            if (this.mPositionObj != null)
            {
                this.mPositionField.value = this.mPosition = this.mPositionObj.transform.position;
                this.mRotationField.value = this.mRotation = this.mPositionObj.transform.rotation.eulerAngles;
            }
        }

        public override float GetDuration()
        {
            ParticleSystem rPS = this.mEffectPrefabField.value as ParticleSystem;
            if (rPS == null)
            {
                return 0;
            }

            //rPS.useAutoRandomSeed = false;
            float fTime = rPS.main.duration;
            for (int i = 0; i < rPS.transform.childCount; i++)
            {
                Transform trans = rPS.transform.GetChild(i);
                ParticleSystem rChildPS = trans.GetComponent<ParticleSystem>();
                //rChildPS.useAutoRandomSeed = false;
                if (fTime < rChildPS.main.duration)
                {
                    fTime = rChildPS.main.duration;
                }
            }

            return fTime;
        }

        public override void GetFocus()
        {
            this.mPositionObj = new GameObject();
            this.mPositionObj.name = this.EventName + "Position";
            this.mPositionObj.transform.position = this.mPosition;
            this.mPositionObj.transform.rotation = Quaternion.Euler(this.mRotation);
            GameObject rAvatar = this.AvatarObjField.value as GameObject;
            if (rAvatar != null && this.mPosition == Vector3.zero && this.mRotation == Vector3.zero)
            {
                this.mPositionObj.transform.position = rAvatar.transform.position;
            }
            Selection.activeObject = this.mPositionObj;
        }

        public override void LoseFocus()
        {
            if (this.mPositionObj != null)
            {
                GameObject.DestroyImmediate(this.mPositionObj);
            }
            Selection.activeObject = null;
        }

        public override void LoadFromFile(string aContent)
        {
            //属性序列
            //0       1         2         3        4      5    6          7          8         9
            //EventID EventName StartTime PosAngle PosDis PosY rotation.x Rotation.y Rotationz EffectAssetPath

            base.LoadFromFile(aContent);
            string[] parms = aContent.Split(' ');
            float fAngle = System.Convert.ToSingle(parms[3]);
            float fDis = System.Convert.ToSingle(parms[4]);
            float fH = System.Convert.ToSingle(parms[5]);
            Vector3 rRelativeRotation = new Vector3(System.Convert.ToSingle(parms[6]), System.Convert.ToSingle(parms[7]), System.Convert.ToSingle(parms[8]));
            this.mEffectPrefabField.value = AssetDatabase.LoadAssetAtPath<ParticleSystem>(parms[9]);

            //还原相对位置
            GameObject rAvatar = this.AvatarObjField.value as GameObject;
            Vector3 rAvatarPos = rAvatar == null ? Vector3.zero : rAvatar.transform.position;
            Vector3 rAvatarDir = rAvatar == null ? Vector3.forward : rAvatar.transform.forward;
            Vector3 rAvatarRotation = rAvatar == null ? Vector3.zero : rAvatar.transform.rotation.eulerAngles;
            this.mRotation = rAvatarRotation + rRelativeRotation;

            Vector3 rDir = Quaternion.AngleAxis(fAngle, Vector3.up) * rAvatarDir;
            this.mPosition = rAvatarPos + rDir * fDis + new Vector3(0, fH, 0);
        }

        public override string SaveToFile()
        {
            ParticleSystem rPS = this.mEffectPrefabField.value as ParticleSystem;
            if (rPS == null)
            {
                EditorUtility.DisplayDialog("Error", "The AnimationClip can't be null!", "OK");
                return string.Empty;
            }

            //因为人物在游戏中方向位置都不一样，所以要算出相对位置和方向
            GameObject rAvatar = this.AvatarObjField.value as GameObject;
            Vector3 rAvatarDir = rAvatar == null ? Vector3.forward : rAvatar.transform.forward;
            Vector3 rAvatarPos = rAvatar == null ? Vector3.zero : rAvatar.transform.position;
            Vector3 rAvatarRotation = rAvatar == null ? Vector3.zero : rAvatar.transform.rotation.eulerAngles;
            Vector3 rRelativeRotation = this.mRotation - rAvatarRotation;

            Vector3 rXZPos = new Vector3(this.mPosition.x, 0, this.mPosition.z);
            Vector3 rDir = (rXZPos - rAvatarPos).normalized;
            float fAngle = Vector3.SignedAngle(rAvatarDir, rDir, Vector3.up);
            float fDis = Vector3.Distance(rXZPos, rAvatarPos);
            float fH = this.mPosition.y;

            return base.SaveToFile() + " "
                + fAngle + " "
                + fDis + " "
                + fH + " "
                + rRelativeRotation.x.ToString() + " "
                + rRelativeRotation.y.ToString() + " "
                + rRelativeRotation.z.ToString() + " "
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
                this.mEffectDuration = this.GetDuration();
                this.mEffectObj = GameObject.Instantiate(rPS, this.mPosition, Quaternion.Euler(this.mRotation));
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
            if (this.mCurSimulateTime >= this.mEffectDuration)
            {
                this.mSimulating = false;
                this.mCurSimulateTime = 0;
                GameObject.DestroyImmediate(this.mEffectObj.gameObject);
                this.mEffectObj = null;
            }
        }
    }
}