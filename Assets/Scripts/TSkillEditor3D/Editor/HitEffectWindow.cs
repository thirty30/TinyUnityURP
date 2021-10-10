using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class HitEffectWindow
    {
        private VisualElement mRootPanel = null;
        private ScrollView mEffectListPanel = null;
        private Button mAddHitEffectBtn = null;

        public void Init()
        {
            this.mRootPanel = new VisualElement();

            this.mAddHitEffectBtn = new Button();
            this.mAddHitEffectBtn.text = "Add Hit Effect";
            this.mAddHitEffectBtn.style.width = 100;
            this.mAddHitEffectBtn.clicked += this.OnClickAddEffect;
            this.mRootPanel.Add(this.mAddHitEffectBtn);

            this.mEffectListPanel = new ScrollView();
            this.mRootPanel.Add(this.mEffectListPanel);
        }

        public VisualElement GetRootPanel()
        {
            return this.mRootPanel;
        }

        public void LoadData(string aContent)
        {
            string[] strEffects = aContent.Split(' ');
            for (int i = 0; i < strEffects.Length; i++)
            {
                if (strEffects[i].Length == 0)
                {
                    continue;
                }
                string[] strParms = strEffects[i].Split('|');
                HitEffect rEffectType = (HitEffect)System.Convert.ToInt32(strParms[0]);
                VisualElement rExtendPanel = this.CreateEffectItem();
                (rExtendPanel.parent.ElementAt(1) as EnumField).value = rEffectType;
                if (rEffectType == HitEffect.DEAL_DAMAGE)
                {
                    DamageType rDamageType = (DamageType)System.Convert.ToInt32(strParms[1]);
                    float fValue = System.Convert.ToSingle(strParms[2]);
                    this.DrawDamageField(rExtendPanel, rDamageType, fValue);
                }
                else if (rEffectType == HitEffect.ADD_BUFF)
                {
                    int nBuffID = System.Convert.ToInt32(strParms[1]);
                    this.DrawBuffField(rExtendPanel, nBuffID);
                }
                else if (rEffectType == HitEffect.KNOCK_DOWN
                    || rEffectType == HitEffect.KNOCK_FLY
                    || rEffectType == HitEffect.STUN)
                {
                    float fValue = System.Convert.ToSingle(strParms[1]);
                    this.DrawStatusField(rExtendPanel, fValue);
                }
            }
        }

        public string SaveData()
        {
            string strContent = "";
            foreach(VisualElement child in this.mEffectListPanel.Children())
            {
                EnumField rEffectTypeField = child.ElementAt(1) as EnumField;
                VisualElement rExtendPanel = child.ElementAt(2) as VisualElement;
                HitEffect rEffectType = (HitEffect)rEffectTypeField.value;
                if (rEffectType == HitEffect.DEAL_DAMAGE)
                {
                    EnumField rDamageTypeField = rExtendPanel.ElementAt(1) as EnumField;
                    DamageType rDamageType = (DamageType)rDamageTypeField.value;
                    FloatField rValueField = rExtendPanel.ElementAt(3) as FloatField;
                    strContent += string.Format("{0}|{1}|{2} ", (int)rEffectType, (int)rDamageType, rValueField.value);
                }
                else if (rEffectType == HitEffect.ADD_BUFF)
                {
                    IntegerField rBuffIDField = rExtendPanel.ElementAt(1) as IntegerField;
                    strContent += string.Format("{0}|{1} ", (int)rEffectType, rBuffIDField.value);
                }
                else if (rEffectType == HitEffect.KNOCK_DOWN
                    || rEffectType == HitEffect.KNOCK_FLY
                    || rEffectType == HitEffect.STUN)
                {
                    FloatField rTimeField = rExtendPanel.ElementAt(1) as FloatField;
                    strContent += string.Format("{0}|{1} ", (int)rEffectType, rTimeField.value);
                }
            }
            if (strContent.Length > 0)
            {
                strContent = strContent.Remove(strContent.Length - 1);   //把空格去掉
            }
            return strContent;
        }

        private void OnClickAddEffect()
        {
            VisualElement rExtendPanel = this.CreateEffectItem();
            //默认是DEAL_DAMAGE的选项
            this.DrawDamageField(rExtendPanel);
        }

        private VisualElement CreateEffectItem()
        {
            Box rItem = new Box();
            this.mEffectListPanel.Add(rItem);
            rItem.style.marginLeft = 3;
            rItem.style.marginRight = 3;
            rItem.style.marginTop = 2;
            rItem.style.marginBottom = 2;
            rItem.style.flexDirection = FlexDirection.Row;
            rItem.style.width = Length.Percent(98);
            rItem.style.height = 30;
            rItem.style.alignItems = Align.Center;

            //删除按钮
            Button rDel = new Button();
            rItem.Add(rDel);
            rDel.text = "-";
            rDel.clicked += () =>
            {
                this.mEffectListPanel.Remove(rItem);
            };

            EnumField rHitEffectField = new EnumField(HitEffect.DEAL_DAMAGE);
            rHitEffectField.style.width = 100;
            rHitEffectField.RegisterCallback<ChangeEvent<System.Enum>>(this.OnChangedHitEffect);
            rItem.Add(rHitEffectField);

            VisualElement rExtendPanel = CommonUtility.CreateVisualElement(rItem);
            rExtendPanel.style.alignItems = Align.Center;
            rExtendPanel.style.flexDirection = FlexDirection.Row;

            return rExtendPanel;
        }

        private void OnChangedHitEffect(ChangeEvent<System.Enum> aEvt)
        {
            HitEffect rEffectType = (HitEffect)aEvt.newValue;
            VisualElement rRootPanel = (aEvt.currentTarget as EnumField).parent;
            VisualElement rExtendPanel = rRootPanel.ElementAt(2);
            rExtendPanel.Clear();
            if (rEffectType == HitEffect.DEAL_DAMAGE)
            {
                this.DrawDamageField(rExtendPanel);
            }
            else if (rEffectType == HitEffect.ADD_BUFF)
            {
                this.DrawBuffField(rExtendPanel);
            }
            else if (rEffectType == HitEffect.KNOCK_DOWN 
                || rEffectType == HitEffect.KNOCK_FLY 
                || rEffectType == HitEffect.STUN)
            {
                this.DrawStatusField(rExtendPanel);
            }
        }

        private void DrawDamageField(VisualElement aParent, params object[] aPrams)
        {
            CommonUtility.CreateLable("Damage Type:", aParent, 0, 0, 0, 0);

            EnumField rDamageTypeField = new EnumField(DamageType.FORMULA);
            aParent.style.width = 100;
            aParent.Add(rDamageTypeField);

            CommonUtility.CreateLable("Value:", aParent, 0, 0, 0, 0);
            FloatField rValueField = new FloatField();
            rValueField.style.width = 50;
            aParent.Add(rValueField);

            if (aPrams.Length > 0)
            {
                rDamageTypeField.value = (DamageType)aPrams[0];
                rValueField.value = (float)aPrams[1];
            }
        }

        private void DrawBuffField(VisualElement aParent, params object[] aPrams)
        {
            CommonUtility.CreateLable("Buff ID:", aParent, 0, 0, 0, 0);
            IntegerField rValueField = new IntegerField();
            rValueField.style.width = 100;
            aParent.Add(rValueField);

            if (aPrams.Length > 0)
            {
                rValueField.value = (int)aPrams[0];
            }
        }

        private void DrawStatusField(VisualElement aParent, params object[] aPrams)
        {
            CommonUtility.CreateLable("Time:", aParent, 0, 0, 0, 0);
            FloatField rValueField = new FloatField();
            rValueField.style.width = 100;
            aParent.Add(rValueField);

            if (aPrams.Length > 0)
            {
                rValueField.value = (float)aPrams[0];
            }
        }
    }

}
