using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class HitEventData : EventDataBase
    {
        //需要保存的数据
        private TargetType mTargetType = TargetType.ENEMY;
        private HitAreaType mAreaType = HitAreaType.CURRENT_TARGET;
        private float mAvailableRadius = 0;  //攻击有效范围，比如在某一个范围内血量最低的对象
        private float mAngle = 0;
        private float mRadius = 0;

        //中间控件
        private EnumField mTargetTypeField = null;
        private EnumField mHitAreaField = null;
        private VisualElement mHitAreaExtendPanel = null;
        private FloatField mAvailableRadiusField = null;
        private FloatField mAngleField = null;
        private FloatField mRadiusField = null;
        private Button mAddHitEffectBtn = null;
        private HitEffectWindow mHitEffectField = null;

        private T3DSkillEditorAttackArea mAreaObject = null;

        public override SkillEventType GetEventType() { return SkillEventType.HIT; }
        public override Color GetDisplayColor() { return Color.red; }

        public override void Init()
        {
            this.mTargetTypeField = new EnumField("Target Type:", TargetType.ENEMY);
            this.mTargetTypeField.RegisterCallback<ChangeEvent<System.Enum>>((aEvt) => 
            {
                this.mTargetType = (TargetType)aEvt.newValue;
            });
            this.mRootPanel.Add(this.mTargetTypeField);

            this.mHitAreaField = new EnumField("Hit Area Type:", HitAreaType.CURRENT_TARGET);
            this.mHitAreaField.RegisterCallback<ChangeEvent<System.Enum>>(this.OnChangedHitAreaType);
            this.mRootPanel.Add(this.mHitAreaField);

            this.mHitAreaExtendPanel = CommonUtility.CreateVisualElement(this.mRootPanel);

            this.mAvailableRadiusField = new FloatField("Available Radius:");
            this.mAvailableRadiusField.RegisterCallback<ChangeEvent<float>>((aEvt) =>
            {
                this.mAvailableRadius = aEvt.newValue;
                this.RefreshHitArea();
            });

            this.mAngleField = new FloatField("Sector Angle:");
            this.mAngleField.RegisterCallback<ChangeEvent<float>>((aEvt) => 
            {
                this.mAngle = aEvt.newValue;
                this.RefreshHitArea();
            });

            this.mRadiusField = new FloatField("Hit Area Radius:");
            this.mRadiusField.RegisterCallback<ChangeEvent<float>>((aEvt) =>
            {
                this.mRadius = aEvt.newValue;
                this.RefreshHitArea();
            });

            this.mHitEffectField = new HitEffectWindow();
            this.mHitEffectField.Init();
            this.mRootPanel.Add(this.mHitEffectField.GetRootPanel());
        }

        public override void Clear()
        {
            if (this.mAreaObject != null)
            {
                GameObject.DestroyImmediate(this.mAreaObject.gameObject);
            }
        }

        public override void GetFocus()
        {
            if (this.mAreaObject == null)
            {
                GameObject go = new GameObject("HitArea");
                this.mAreaObject = go.AddComponent<T3DSkillEditorAttackArea>();
            }
        }

        public override void LoseFocus()
        {
            if (this.mAreaObject != null)
            {
                GameObject.DestroyImmediate(this.mAreaObject.gameObject);
            }
        }

        public override bool IsPreviewing() { return false; }

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        private void RefreshHitArea()
        {
            if (this.mAreaObject == null)
            {
                return;
            }
            GameObject rAvatar = this.AvatarObjField.value as GameObject;
            Vector3 rCenter = rAvatar == null ? Vector3.zero : rAvatar.transform.position;
            Vector3 rForward = rAvatar == null ? Vector3.forward : rAvatar.transform.forward;

            switch (this.mAreaType)
            {
                case HitAreaType.CURRENT_TARGET:
                case HitAreaType.FAR_TARGET:
                case HitAreaType.LOW_HP_TARGET:
                case HitAreaType.ALL_TARGET:
                case HitAreaType.RANDOM_TARGET:
                    {
                        this.mAreaObject.DrawTarget(this.mAreaType, this.mAvailableRadius);
                    }
                    break;
                case HitAreaType.SECTOR:
                    {
                        this.mAreaObject.DrawSector(rCenter, rForward, this.mAngle, this.mRadius);
                    }
                    break;
                case HitAreaType.CIRCLE:
                    {
                        this.mAreaObject.DrawCircle(rCenter, this.mRadius);
                    }
                    break;
                default:
                    break;
            }
        }

        private void OnChangedHitAreaType(ChangeEvent<System.Enum> aEvt)
        {
            this.mAreaType = (HitAreaType)aEvt.newValue;
            this.mHitAreaExtendPanel.Clear();

            switch (this.mAreaType)
            {
                case HitAreaType.FAR_TARGET:
                case HitAreaType.LOW_HP_TARGET:
                case HitAreaType.ALL_TARGET:
                case HitAreaType.RANDOM_TARGET:
                    {
                        this.mAvailableRadiusField.value = 0;
                        this.mAvailableRadius = 0;
                        this.mHitAreaExtendPanel.Add(this.mAvailableRadiusField);
                    }
                    break;
                case HitAreaType.SECTOR:
                    {
                        this.mAngleField.value = 0;
                        this.mRadiusField.value = 0;
                        this.mAngle = 0;
                        this.mRadius = 0;
                        this.mHitAreaExtendPanel.Add(this.mAngleField);
                        this.mHitAreaExtendPanel.Add(this.mRadiusField);
                    }
                    break;
                case HitAreaType.CIRCLE:
                    {
                        this.mRadiusField.value = 0;
                        this.mRadius = 0;
                        this.mHitAreaExtendPanel.Add(this.mRadiusField);
                    }
                    break;
                default:
                    break;
            }
            this.RefreshHitArea();
        }
    }
}
