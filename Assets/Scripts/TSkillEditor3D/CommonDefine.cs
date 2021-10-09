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
        AUDIO,                      //��Ч
    }

    public enum EffectAttachPart
    {
        FOOT,       //�Ų�
        WEAPON,     //����
    }

    public enum TargetType
    {
        ENEMY,  //�з�
        ALLY,   //�ѷ�
    }
    
    public enum HitAreaType
    {
        CURRENT_TARGET,     //��ǰ��������
        FAR_TARGET,         //��Զ�Ķ���
        LOW_HP_TARGET,      //Ѫ����͵Ķ���
        ALL_TARGET,         //ȫ�����
        RANDOM_TARGET,      //�������
        SECTOR,             //��������
        CIRCLE,             //Բ������
    }

    public enum HitEffect
    {
        DEAL_DAMAGE,        //����˺�
        ADD_BUFF,           //���buff
        KNOCK_DOWN,         //����
        KNOCK_FLY,          //����
        STUN,               //��ѣ
    }

    public enum DamageType
    {
        FORMULA,    //��ʽ����ɳ��˺�
        FIXED,      //��ֵ�˺�
        PERCENT,    //�ٷֱ��˺�
    }
}
