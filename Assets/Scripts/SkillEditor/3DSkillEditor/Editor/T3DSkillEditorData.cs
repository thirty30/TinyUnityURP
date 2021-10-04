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
        //需要保存的数据
        public string EventName = string.Empty;
        public float StartTime = 0;

        //中间控件引用
        public ObjectField AvatarObjField = null;       //用于演示的Animator对象
        public TextField EventNameField = null;         //事件名字
        public FloatField StartTimeField = null;        //事件开始时间
        protected VisualElement mRootPanel = new VisualElement();

        public virtual SkillEventType GetEventType() { return SkillEventType.INVALID; }
        public virtual void Init() { }
        public virtual void Clear() { }
        public virtual void Update(float aDeltaTime) { }
        public virtual float GetDuration() { return 0; }    //得到Event执行时间
        public virtual void GetFocus() { }      //选择事件的时候调用
        public virtual void LoseFocus() { }     //失去选择的时候调用

        public virtual void Preview() { }
        public abstract bool IsPreviewing();

        public virtual void LoadFromFile(string aContent) 
        {
            //属性序列
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


