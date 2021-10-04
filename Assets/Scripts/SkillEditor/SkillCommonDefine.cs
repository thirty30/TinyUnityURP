using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public class SkillEditorCommon
    {
        public const int MinHeight = 22;

        public static Label CreateLable(string aText, VisualElement aParentPanel, float aMarginLeft, float aMarginRight, float aMarginTop, float aMarginBottom)
        {
            Label label = new Label(aText);
            label.style.marginLeft = aMarginLeft;
            label.style.marginRight = aMarginRight;
            label.style.marginTop = aMarginTop;
            label.style.marginBottom = aMarginBottom;
            aParentPanel.Add(label);
            return label;
        }

        public static VisualElement CreateVisualElement(VisualElement aParentPanel)
        {
            VisualElement panel = new VisualElement();
            aParentPanel.Add(panel);
            return panel;
        }
    }

    public enum SKillType
    {
        ACTIVE,     //主动技能
        PASSIVE,    //被动技能
    }

    public enum SkillEventType
    {
        INVALID,
    
        ANIMATION,                  //动作
        CREATE_VFX,                 //生成一个独立坐标的特效
        ATTACH_VFX,                 //生成一个依附特效
        HIT,                        //打击逻辑
        SUMMON,                     //召唤逻辑
    }

    public enum SkillEffectAttachPart
    {
        FOOT,       //脚部
        WEAPON,     //武器
    }

    public enum SKillDealTarget
    {
        ALLY,
        ENEMY,
    }

    public enum SKillHitRange
    {
        TARGET,
        CIRCLE,
        SECTOR,
    }

    public enum SkillHitStatus
    {
        DEAL_DAMAGE,
        KNOCK_DOWN,
        KNOCK_FLY,
        STUN,
    }
}
