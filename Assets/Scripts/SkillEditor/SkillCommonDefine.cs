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
        ACTIVE,     //��������
        PASSIVE,    //��������
    }

    public enum SkillEventType
    {
        INVALID,
    
        ANIMATION,                  //����
        CREATE_VFX,                 //����һ�������������Ч
        ATTACH_VFX,                 //����һ��������Ч
        HIT,                        //����߼�
        SUMMON,                     //�ٻ��߼�
    }

    public enum SkillEffectAttachPart
    {
        FOOT,       //�Ų�
        WEAPON,     //����
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
