using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class HitEffectData
    {
        public object[] Parms; 
    }

    public class HitEffectWindow
    {
        private List<HitEffectData> mEffects = new List<HitEffectData>();

        private VisualElement mRootPanel = null;
        private Button mAddHitEffectBtn = null;
        private EnumField mHitEffectField = null;

        public void Init()
        {
            this.mRootPanel = new ScrollView();

            this.mAddHitEffectBtn = new Button();
            this.mAddHitEffectBtn.text = "Add Hit Effect";
            this.mAddHitEffectBtn.style.width = 100;
            this.mAddHitEffectBtn.clicked += this.OnClickAddEffect;
            this.mRootPanel.Add(this.mAddHitEffectBtn);
        }

        public VisualElement GetRootPanel()
        {
            return this.mRootPanel;
        }

        public void LoadData(string aContent)
        {

        }

        public string SaveData()
        {
            return "";
        }

        private void OnClickAddEffect()
        {
            Box rItem = new Box();
            this.mRootPanel.Add(rItem);
            rItem.style.marginLeft = 3;
            rItem.style.marginRight = 3;
            rItem.style.marginTop = 2;
            rItem.style.marginBottom = 2;
            rItem.style.flexDirection = FlexDirection.Row;
            rItem.style.width = Length.Percent(98);
            rItem.style.height = 30;
            rItem.style.alignItems = Align.Center;
            //rEventItem.userData = rEditorData;

            //删除按钮
            Button rDel = new Button();
            rItem.Add(rDel);
            rDel.text = "-";
            rDel.clicked += () =>
            {
                this.mRootPanel.Remove(rItem);
            };

            this.mHitEffectField = new EnumField(HitEffect.DEAL_DAMAGE);
            this.mHitEffectField.style.width = 100;
            this.mHitEffectField.RegisterCallback<ChangeEvent<System.Enum>>(this.OnChangedHitEffect);
            rItem.Add(this.mHitEffectField);

            VisualElement rExtendPanel = CommonUtility.CreateVisualElement(rItem);
            rExtendPanel.style.alignItems = Align.Center;
            rExtendPanel.style.flexDirection = FlexDirection.Row;

            //默认是DEAL_DAMAGE的选项
            CommonUtility.CreateLable("Damage Type:", rExtendPanel, 0, 0, 0, 0);

            EnumField rDamageTypeField = new EnumField(DamageType.FORMULA);
            rExtendPanel.style.width = 100;
            rExtendPanel.Add(rDamageTypeField);

            CommonUtility.CreateLable("Value:", rExtendPanel, 0, 0, 0, 0);
            FloatField rValueField = new FloatField();
            rValueField.style.width = 50;
            rExtendPanel.Add(rValueField);
        }

        private void OnChangedHitEffect(ChangeEvent<System.Enum> aEvt)
        {
            HitEffect rEffectType = (HitEffect)aEvt.newValue;
            VisualElement rRootPanel = (aEvt.currentTarget as EnumField).parent;
            VisualElement rExtendPanel = rRootPanel.ElementAt(2);
            rExtendPanel.Clear();
            if (rEffectType == HitEffect.DEAL_DAMAGE)
            {
                CommonUtility.CreateLable("Damage Type:", rExtendPanel, 0, 0, 0, 0);

                EnumField rDamageTypeField = new EnumField(DamageType.FORMULA);
                rExtendPanel.style.width = 100;
                rExtendPanel.Add(rDamageTypeField);

                CommonUtility.CreateLable("Value:", rExtendPanel, 0, 0, 0, 0);
                FloatField rValueField = new FloatField();
                rValueField.style.width = 50;
                rExtendPanel.Add(rValueField);
            }
            else if (rEffectType == HitEffect.ADD_BUFF)
            {
                CommonUtility.CreateLable("Buff ID:", rExtendPanel, 0, 0, 0, 0);
                IntegerField rValueField = new IntegerField();
                rValueField.style.width = 50;
                rExtendPanel.Add(rValueField);
            }
            else if (rEffectType == HitEffect.KNOCK_DOWN 
                || rEffectType == HitEffect.KNOCK_FLY 
                || rEffectType == HitEffect.STUN)
            {
                CommonUtility.CreateLable("Time:", rExtendPanel, 0, 0, 0, 0);
                FloatField rValueField = new FloatField();
                rValueField.style.width = 50;
                rExtendPanel.Add(rValueField);
            }
        }

    }

}
