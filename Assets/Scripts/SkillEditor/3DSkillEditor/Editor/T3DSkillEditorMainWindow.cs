using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public class T3DSkillEditorMainWindow : EditorWindow
    {
        private VisualElement mPropertyPanel = null;    //属性面板
        private ObjectField mAvatarObjField = null;     //用于演示的Animator对象
        private ObjectField mSkillDataObjField = null;  //技能文件
        private IntegerField mSkillIDTextField = null;  //技能ID
        private EnumField mSkillTypeField = null;       //技能类型
        private ScrollView mEventsScrollView = null;    //Event 列表控件
        private Box mLastSelectedEventItem = null;      //上一次选中的Event控件

        private Label mPreviewProgressLabel = null;     //技能预览进度时间
        private bool mIsPreviewing = false;             //是否在预览
        private double mLastPreviewTime = 0;            //上一次预览tick的时间
        private float mCurPreviewTime = 0;              //当前预览的时间
        private float mTotalPreviewTime = 0;              //技能总时间

        [MenuItem("TTool/Skill Editor 3D")]
        public static void OpenEditor()
        {
            T3DSkillEditorMainWindow win = GetWindow<T3DSkillEditorMainWindow>();
            win.titleContent = new GUIContent("Skill Editor 3D");
            win.minSize = new Vector2(910, 300);
        }

        public void OnEnable()
        {
            this.rootVisualElement.style.flexDirection = FlexDirection.Row;

            //左侧布局
            VisualElement leftPanel = SkillEditorCommon.CreateVisualElement(this.rootVisualElement);
            leftPanel.style.width = Length.Percent(60);
            leftPanel.style.height = Length.Percent(100);
            leftPanel.style.flexDirection = FlexDirection.Column;
            leftPanel.style.borderRightColor = Color.gray;
            leftPanel.style.borderRightWidth = 1;
            this.LeftLayout(leftPanel);

            //右侧布局
            this.mPropertyPanel = SkillEditorCommon.CreateVisualElement(this.rootVisualElement);
            this.mPropertyPanel.style.width = Length.Percent(40);
            this.mPropertyPanel.style.height = Length.Percent(100);
            this.mPropertyPanel.style.flexDirection = FlexDirection.Column;
            SkillEditorCommon.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);

            
            this.mLastPreviewTime = EditorApplication.timeSinceStartup; //tick time
            AnimationMode.StartAnimationMode(); //开启编辑器动作模式
        }

        public void OnDestroy()
        {
            AnimationMode.StopAnimationMode();  
        }

        private void Update()
        {
            float fDeltaTime = (float)(EditorApplication.timeSinceStartup - this.mLastPreviewTime);
            this.mLastPreviewTime = EditorApplication.timeSinceStartup;

            foreach (var child in this.mEventsScrollView.Children())
            {
                (child.userData as T3DSkillEditorDataBase).Update(fDeltaTime);
            }

            //预览
            this.Preview(fDeltaTime);
        }

        //左边部分布局
        private void LeftLayout(VisualElement aParentPanel)
        {
            //用于展示的角色动作对象
            VisualElement rPanel = SkillEditorCommon.CreateVisualElement(aParentPanel);
            rPanel.style.flexDirection = FlexDirection.Row;
            rPanel.style.minHeight = SkillEditorCommon.MinHeight;
            {
                this.mAvatarObjField = new ObjectField("Avatar GameObject:");
                this.mAvatarObjField.style.width = 400;
                this.mAvatarObjField.objectType = typeof(GameObject);
                rPanel.Add(this.mAvatarObjField);

                Button btnPreview = new Button();
                btnPreview.text = "Preview";
                btnPreview.style.width = 60;
                btnPreview.clicked += this.OnClickPreviewSkill;
                rPanel.Add(btnPreview);

                this.mPreviewProgressLabel = new Label();
                this.mPreviewProgressLabel.style.alignSelf = Align.Center;
                this.mPreviewProgressLabel.style.display = DisplayStyle.None;
                rPanel.Add(this.mPreviewProgressLabel);
            }

            //技能数据文件
            rPanel = SkillEditorCommon.CreateVisualElement(aParentPanel);
            rPanel.style.flexDirection = FlexDirection.Row;
            rPanel.style.minHeight = SkillEditorCommon.MinHeight;
            {
                this.mSkillDataObjField = new ObjectField("Skill File:");
                this.mSkillDataObjField.style.width = 400;
                this.mSkillDataObjField.objectType = typeof(TextAsset);
                this.mSkillDataObjField.RegisterCallback<ChangeEvent<Object>>(this.OnSkillDataChanged);
                rPanel.Add(this.mSkillDataObjField);

                Button btnCreateSkillObj = new Button();
                btnCreateSkillObj.text = "New";
                btnCreateSkillObj.style.width = 60;
                btnCreateSkillObj.clicked += this.OnClickCreateNewSkill;
                rPanel.Add(btnCreateSkillObj);

                Button btnSaveSkillData = new Button();
                btnSaveSkillData.text = "Save";
                btnSaveSkillData.style.width = 60;
                btnSaveSkillData.clicked += this.OnClickSaveSkill;
                rPanel.Add(btnSaveSkillData);

                Button btnClearSkillData = new Button();
                btnClearSkillData.text = "Clear";
                btnClearSkillData.style.width = 60;
                btnClearSkillData.clicked += () => { this.mSkillDataObjField.value = null; };
                rPanel.Add(btnClearSkillData);
            }

            //技能ID
            this.mSkillIDTextField = new IntegerField("Skill ID:");
            this.mSkillIDTextField.style.width = 400;
            this.mSkillIDTextField.style.minHeight = SkillEditorCommon.MinHeight;
            this.mSkillIDTextField.value = 0;
            aParentPanel.Add(this.mSkillIDTextField);

            //技能类型
            this.mSkillTypeField = new EnumField("Skill Type:", SKillType.ACTIVE);
            this.mSkillTypeField.style.width = 400;
            this.mSkillTypeField.style.minHeight = SkillEditorCommon.MinHeight;
            aParentPanel.Add(this.mSkillTypeField);

            //事件区域
            Box rBG = new Box();
            rBG.style.width = Length.Percent(100);
            rBG.style.height = Length.Percent(100);
            aParentPanel.Add(rBG);
            //右键Context Menu
            rBG.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction(SkillEventType.ANIMATION.ToString(), (x) => { this.OnCreateEvent(SkillEventType.ANIMATION); });
                evt.menu.AppendAction(SkillEventType.CREATE_VFX.ToString(), (x) => { this.OnCreateEvent(SkillEventType.CREATE_VFX); });
                evt.menu.AppendAction(SkillEventType.ATTACH_VFX.ToString(), (x) => { this.OnCreateEvent(SkillEventType.ATTACH_VFX); });
                evt.menu.AppendAction(SkillEventType.HIT.ToString(), (x) => { this.OnCreateEvent(SkillEventType.HIT); });
                evt.menu.AppendAction(SkillEventType.SUMMON.ToString(), (x) => { this.OnCreateEvent(SkillEventType.SUMMON); });
            }));

            //事件Title
            SkillEditorCommon.CreateLable("Events", rBG, 3, 3, 1, 1);

            //事件列表
            this.mEventsScrollView = new ScrollView();
            this.mEventsScrollView.style.width = Length.Percent(100);
            this.mEventsScrollView.style.height = Length.Percent(100);
            rBG.Add(this.mEventsScrollView);

        }

        //计算预览需要的时间
        private float CalcTotalPreviewTime()
        {
            float fSkillTime = 0;
            foreach (var child in this.mEventsScrollView.Children())
            {
                T3DSkillEditorDataBase data = child.userData as T3DSkillEditorDataBase;
                float fTime = data.StartTime + data.GetDuration();
                if (fTime > fSkillTime)
                {
                    fSkillTime = fTime;
                }
            }
            return fSkillTime;
        }

        private void OnClickPreviewSkill()
        {
            if (this.mIsPreviewing == true)
            {
                return;
            }

            this.mIsPreviewing = true;
            this.mCurPreviewTime = 0;
            this.mTotalPreviewTime = this.CalcTotalPreviewTime();
            this.mPreviewProgressLabel.style.display = DisplayStyle.Flex;
        }

        private void Preview(float aDeltaTime)
        {
            if (this.mIsPreviewing == false)
            {
                return;
            }

            this.mCurPreviewTime += aDeltaTime;
            foreach (var child in this.mEventsScrollView.Children())
            {
                T3DSkillEditorDataBase data = child.userData as T3DSkillEditorDataBase;
                float fEndTime = data.StartTime + data.GetDuration();
                if (this.mCurPreviewTime >= data.StartTime && this.mCurPreviewTime < fEndTime && data.IsPreviewing() == false)
                {
                    data.Preview();
                }
            }

            this.mPreviewProgressLabel.text = string.Format("{0:N2}/{1:N2}", this.mCurPreviewTime, this.mTotalPreviewTime);

            if (this.mCurPreviewTime >= this.mTotalPreviewTime)
            {
                this.mIsPreviewing = false;
                this.mPreviewProgressLabel.style.display = DisplayStyle.None;
            }
        }

        //创建事件
        private void OnCreateEvent(SkillEventType aEventType)
        {
            if (aEventType == SkillEventType.INVALID) { return; }
            T3DSkillEditorDataBase rEditorData = this.NewEditorData(aEventType);

            Box rEventItem = new Box();
            this.mEventsScrollView.Add(rEventItem);
            rEventItem.style.marginLeft = 3;
            rEventItem.style.marginRight = 3;
            rEventItem.style.marginTop = 2;
            rEventItem.style.marginBottom = 2;
            rEventItem.style.flexDirection = FlexDirection.Row;
            rEventItem.style.width = Length.Percent(98);
            rEventItem.style.height = 30;
            rEventItem.style.alignItems = Align.Center;
            rEventItem.RegisterCallback<MouseDownEvent>(this.OnClickEventItem);
            rEventItem.userData = rEditorData;

            Box rColorMark = new Box();
            rEventItem.Add(rColorMark);
            rColorMark.style.width = 3;
            rColorMark.style.height = Length.Percent(100);

            if (aEventType == SkillEventType.ANIMATION) { rColorMark.style.backgroundColor = Color.green; }
            else if (aEventType == SkillEventType.CREATE_VFX) { rColorMark.style.backgroundColor = Color.yellow; }
            else if (aEventType == SkillEventType.ATTACH_VFX) { rColorMark.style.backgroundColor = new Color(1, 0.6f, 0.2f, 1); }
            else if (aEventType == SkillEventType.HIT) { rColorMark.style.backgroundColor = Color.red; }
            else if (aEventType == SkillEventType.SUMMON) { rColorMark.style.backgroundColor = Color.blue; }

            TextField rEventNameTextField = new TextField();
            rEventItem.Add(rEventNameTextField);
            rEventNameTextField.value = aEventType.ToString();
            rEventNameTextField.style.width = 150;
            rEventNameTextField.style.height = 20;
            rEventNameTextField.RegisterCallback<ChangeEvent<string>>((aEvt) => 
            {
                //更新Event名字
                rEditorData.EventName = aEvt.newValue;

                //不能包含空格
                if (rEditorData.EventName.Contains(" ") == true)
                {
                    rEditorData.EventName = rEditorData.EventName.Replace(" ", "");
                    rEventNameTextField.value = rEditorData.EventName;
                }
            });
            rEditorData.EventName = aEventType.ToString();
            rEditorData.EventNameField = rEventNameTextField;

            SkillEditorCommon.CreateLable("Start Time:", rEventItem, 0, 0, 0, 0);

            FloatField rStartTimeField = new FloatField();
            rEventItem.Add(rStartTimeField);
            rStartTimeField.value = 0;
            rStartTimeField.style.width = 50;
            rStartTimeField.style.height = 20;
            rStartTimeField.RegisterCallback<ChangeEvent<float>>((aEvt) =>
            {
                //更新开始时间
                rEditorData.StartTime = aEvt.newValue;
            });
            rEditorData.StartTimeField = rStartTimeField;

            VisualElement rButtonPanel = SkillEditorCommon.CreateVisualElement(rEventItem);
            rButtonPanel.style.flexDirection = FlexDirection.Row;
            rButtonPanel.style.width = Length.Percent(100);
            rButtonPanel.style.alignItems = Align.Center;
            rButtonPanel.style.justifyContent = Justify.FlexEnd;

            //排序按钮
            Button rMoveUp = new Button();
            rButtonPanel.Add(rMoveUp);
            rMoveUp.text = "↑";
            rMoveUp.clicked += () =>
            {
                //表现层
                int nIDX = this.mEventsScrollView.IndexOf(rEventItem);
                if (nIDX == 0) { return; }
                this.mEventsScrollView.RemoveAt(nIDX);
                this.mEventsScrollView.Insert(nIDX - 1, rEventItem);
            };

            Button rMoveDown = new Button();
            rButtonPanel.Add(rMoveDown);
            rMoveDown.text = "↓";
            rMoveDown.clicked += () =>
            {
                //表现层
                int nIDX = this.mEventsScrollView.IndexOf(rEventItem);
                if (nIDX == this.mEventsScrollView.childCount - 1) { return; }
                this.mEventsScrollView.RemoveAt(nIDX);
                this.mEventsScrollView.Insert(nIDX + 1, rEventItem);
            };

            //删除按钮
            Button rDel = new Button();
            rButtonPanel.Add(rDel);
            rDel.text = "Del";
            rDel.clicked += () =>
            {
                if (rEventItem == this.mLastSelectedEventItem)
                {
                    rEditorData.LoseFocus();
                    this.mLastSelectedEventItem = null;
                    //刷新属性面板
                    this.mPropertyPanel.Clear();
                    SkillEditorCommon.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);
                }
                this.mEventsScrollView.Remove(rEventItem);
                rEditorData.Clear();
            };
        }

        private T3DSkillEditorDataBase NewEditorData(SkillEventType aEventType)
        {
            //根据Event类型创建中间数据
            T3DSkillEditorDataBase rEditorData = null;
            if (aEventType == SkillEventType.ANIMATION)
            {
                rEditorData = new T3DAnimationEditorData();
            }
            else if (aEventType == SkillEventType.CREATE_VFX)
            {
                rEditorData = new T3DCreateVFXEditorData();
            }
            else if (aEventType == SkillEventType.ATTACH_VFX)
            {
                rEditorData = new T3DAttachVFXEditorData();
            }
            else if (aEventType == SkillEventType.HIT)
            {
                rEditorData = new T3DHitEditorData();
            }
            else if (aEventType == SkillEventType.SUMMON)
            {
                rEditorData = new T3DSummonEditorData();
            }
            rEditorData.AvatarObjField = this.mAvatarObjField;
            rEditorData.Init();
            return rEditorData;
        }

        //选中Event
        private void OnClickEventItem(MouseDownEvent aEvt)
        {
            if (aEvt.button != 0) { return; }   //只有左键点击才算选中

            Box rItem = aEvt.currentTarget as Box;

            //选中同一个不刷新
            if (this.mLastSelectedEventItem == rItem)
            {
                return;
            }

            this.mPropertyPanel.Clear();
            SkillEditorCommon.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);

            if (this.mLastSelectedEventItem != null)
            {
                this.mLastSelectedEventItem.style.backgroundColor = new Color(0, 0, 0, 0.1f);
                (this.mLastSelectedEventItem.userData as T3DSkillEditorDataBase).LoseFocus();
            }

            rItem.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            this.mLastSelectedEventItem = rItem;
            T3DSkillEditorDataBase rSkillData = (rItem.userData as T3DSkillEditorDataBase);
            rSkillData.GetFocus();
            this.mPropertyPanel.Add(rSkillData.GetRootPanel());
        }

        private void OnClickCreateNewSkill()
        {
            string strPath = EditorUtility.SaveFilePanel("Select Path", "", "SkillData", "txt");
            if (strPath == string.Empty)
            {
                return;
            }

            FileStream fs = new FileStream(strPath, FileMode.Create);
            fs.Close();
            strPath = strPath.Replace(Application.dataPath, "Assets");
            AssetDatabase.Refresh();
            TextAsset rSkillData = AssetDatabase.LoadAssetAtPath<TextAsset>(strPath);
            this.mSkillDataObjField.value = rSkillData;
        }

        private void OnSkillDataChanged(ChangeEvent<Object> aEvt)
        {
            foreach (var child in this.mEventsScrollView.Children())
            {
                (child.userData as T3DSkillEditorDataBase).Clear();
            }
            this.mSkillIDTextField.value = 0;
            this.mEventsScrollView.Clear();
            this.mPropertyPanel.Clear();
            SkillEditorCommon.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);
            
            //加载数据
            TextAsset rSkillData = aEvt.newValue as TextAsset;
            if (rSkillData == null)
            {
                return;
            }
            string strPath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(rSkillData);

            FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            if (sr.EndOfStream == true)
            {
                //新文件
                fs.Close();
                return;
            }
            string line = sr.ReadLine();
            string[] parms = line.Split(' ');
            this.mSkillIDTextField.value = System.Convert.ToInt32(parms[0]);
            this.mSkillTypeField.value = (SKillType)System.Convert.ToInt32(parms[1]);

            int nIDX = 0;
            while(sr.EndOfStream == false)
            {
                line = sr.ReadLine();
                if (line.Length == 0)
                {
                    continue;
                }
                parms = line.Split(' ');
                int nType = System.Convert.ToInt32(parms[0]);
                this.OnCreateEvent((SkillEventType)nType);
                T3DSkillEditorDataBase data = this.mEventsScrollView.ElementAt(nIDX).userData as T3DSkillEditorDataBase;
                data.LoadFromFile(line);
                nIDX++;
            }
            fs.Close();
        }

        private void OnClickSaveSkill()
        {
            TextAsset rSkillData = this.mSkillDataObjField.value as TextAsset;
            if (rSkillData == null)
            {
                EditorUtility.DisplayDialog("Error", "Can't find the skill data file!", "OK");
                return;
            }

            string strPath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(rSkillData);
            FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            System.Text.StringBuilder sbText = new System.Text.StringBuilder();

            float fSkillTime = this.CalcRealSkillTime();

            //0       1         2
            //SkillID SkillType SKillTime
            string strSkillInfo = string.Format("{0} {1} {2:N2}", 
                this.mSkillIDTextField.value,
                (int)(SKillType)this.mSkillTypeField.value,
                fSkillTime);
            sbText.AppendLine(strSkillInfo);

            //存事件
            foreach (var child in this.mEventsScrollView.Children())
            {
                T3DSkillEditorDataBase data = child.userData as T3DSkillEditorDataBase;
                string strContent = data.SaveToFile();
                if (strContent == string.Empty)
                {
                    fs.Close();
                    return;
                }
                sbText.AppendLine(strContent);
            }

            sw.Write(sbText.ToString());
            sw.Flush();
            fs.Close();

            EditorUtility.DisplayDialog("Tip", "Saved", "OK");
        }

        //计算技能时长
        private float CalcRealSkillTime()
        {
            //其他事件开始时间不能超过动作结束时间，最后一个动作结束时间是技能时长
            float fSkillTime = 0;
            foreach (var child in this.mEventsScrollView.Children())
            {
                T3DSkillEditorDataBase data = child.userData as T3DSkillEditorDataBase;
                if (data.GetEventType() != SkillEventType.ANIMATION)
                {
                    continue;
                }
                float fTime = data.StartTime + data.GetDuration();
                if (fTime > fSkillTime)
                {
                    fSkillTime = fTime;
                }
            }
            return fSkillTime;
        }
    }
}
