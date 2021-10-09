using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class CommonUtility
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
        AUDIO,                      //音效
    }

    public enum EffectAttachPart
    {
        FOOT,       //脚部
        WEAPON,     //武器
    }

    public enum TargetType
    {
        ENEMY,  //敌方
        ALLY,   //友方
    }
    
    public enum HitAreaType
    {
        CURRENT_TARGET,     //当前独立对象
        FAR_TARGET,         //最远的对象
        LOW_HP_TARGET,      //血量最低的对象
        ALL_TARGET,         //全体对象
        RANDOM_TARGET,      //随机对象
        SECTOR,             //扇形区域
        CIRCLE,             //圆形区域
    }

    public enum HitEffect
    {
        DEAL_DAMAGE,        //造成伤害
        ADD_BUFF,           //添加buff
        KNOCK_DOWN,         //击倒
        KNOCK_FLY,          //击飞
        STUN,               //晕眩
    }

    public enum DamageType
    {
        FORMULA,    //公式计算成长伤害
        FIXED,      //定值伤害
        PERCENT,    //百分比伤害
    }
}
