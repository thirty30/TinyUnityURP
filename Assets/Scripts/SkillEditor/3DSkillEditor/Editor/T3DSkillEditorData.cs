using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public abstract class T3DSkillEditorDataBase
    {
        //��Ҫ���������
        public string EventName = string.Empty;
        public float StartTime = 0;

        //�м�ؼ�����
        public ObjectField AvatarObjField = null;       //������ʾ��Animator����
        public TextField EventNameField = null;         //�¼�����
        public FloatField StartTimeField = null;        //�¼���ʼʱ��
        protected VisualElement mRootPanel = new VisualElement();

        public virtual SkillEventType GetEventType() { return SkillEventType.INVALID; }
        public virtual void Init() { }
        public virtual void Clear() { }
        public virtual void Update(float aDeltaTime) { }
        public virtual float GetDuration() { return 0; }    //�õ�Eventִ��ʱ��
        public virtual void GetFocus() { }      //ѡ���¼���ʱ�����
        public virtual void LoseFocus() { }     //ʧȥѡ���ʱ�����

        public virtual void Preview() { }
        public abstract bool IsPreviewing();

        public virtual void LoadFromFile(string aContent) 
        {
            //��������
            //EventID EventName StartTime
            string[] parms = aContent.Split(' ');
            this.EventNameField.value = this.EventName = parms[1];
            this.StartTimeField.value = this.StartTime = System.Convert.ToSingle(parms[2]);
        }

        public virtual string SaveToFile() 
        {
            return string.Format("{0} {1} {2:N2}", (int)this.GetEventType(), this.EventName, this.StartTime);
        }


        public VisualElement GetRootPanel() { return mRootPanel; }

    }
}


