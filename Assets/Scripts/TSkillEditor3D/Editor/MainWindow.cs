using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class MainWindow : EditorWindow
    {
        private VisualElement mPropertyPanel = null;    //�������
        private ObjectField mAvatarObjField = null;     //������ʾ��Animator����
        private ObjectField mSkillDataObjField = null;  //�����ļ�
        private IntegerField mSkillIDTextField = null;  //����ID
        private EnumField mSkillTypeField = null;       //��������
        private ScrollView mEventsScrollView = null;    //Event �б�ؼ�
        private Box mLastSelectedEventItem = null;      //��һ��ѡ�е�Event�ؼ�

        private Label mPreviewProgressLabel = null;     //����Ԥ������ʱ��
        private bool mIsPreviewing = false;             //�Ƿ���Ԥ��
        private double mLastPreviewTime = 0;            //��һ��Ԥ��tick��ʱ��
        private float mCurPreviewTime = 0;              //��ǰԤ����ʱ��
        private float mTotalPreviewTime = 0;              //������ʱ��

        [MenuItem("TTool/Skill Editor 3D")]
        public static void OpenEditor()
        {
            MainWindow win = GetWindow<MainWindow>();
            win.titleContent = new GUIContent("Skill Editor 3D");
            win.minSize = new Vector2(1100, 300);
        }

        public void OnEnable()
        {
            this.rootVisualElement.style.flexDirection = FlexDirection.Row;

            //��಼��
            VisualElement leftPanel = CommonUtility.CreateVisualElement(this.rootVisualElement);
            leftPanel.style.width = Length.Percent(60);
            leftPanel.style.height = Length.Percent(100);
            leftPanel.style.flexDirection = FlexDirection.Column;
            leftPanel.style.borderRightColor = Color.gray;
            leftPanel.style.borderRightWidth = 1;
            this.LeftLayout(leftPanel);

            //�Ҳ಼��
            this.mPropertyPanel = CommonUtility.CreateVisualElement(this.rootVisualElement);
            this.mPropertyPanel.style.width = Length.Percent(40);
            this.mPropertyPanel.style.height = Length.Percent(100);
            this.mPropertyPanel.style.flexDirection = FlexDirection.Column;
            CommonUtility.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);

            
            this.mLastPreviewTime = EditorApplication.timeSinceStartup; //tick time
            AnimationMode.StartAnimationMode(); //�����༭������ģʽ
        }

        public void OnDestroy()
        {
            foreach (var child in this.mEventsScrollView.Children())
            {
                (child.userData as EventDataBase).Clear();
            }
            AnimationMode.StopAnimationMode();  
        }

        private void Update()
        {
            float fDeltaTime = (float)(EditorApplication.timeSinceStartup - this.mLastPreviewTime);
            this.mLastPreviewTime = EditorApplication.timeSinceStartup;

            foreach (var child in this.mEventsScrollView.Children())
            {
                (child.userData as EventDataBase).Update(fDeltaTime);
            }

            //Ԥ��
            this.Preview(fDeltaTime);
        }

        //��߲��ֲ���
        private void LeftLayout(VisualElement aParentPanel)
        {
            //����չʾ�Ľ�ɫ��������
            VisualElement rPanel = CommonUtility.CreateVisualElement(aParentPanel);
            rPanel.style.flexDirection = FlexDirection.Row;
            rPanel.style.minHeight = CommonUtility.MinHeight;
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

            //���������ļ�
            rPanel = CommonUtility.CreateVisualElement(aParentPanel);
            rPanel.style.flexDirection = FlexDirection.Row;
            rPanel.style.minHeight = CommonUtility.MinHeight;
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

            //����ID
            this.mSkillIDTextField = new IntegerField("Skill ID:");
            this.mSkillIDTextField.style.width = 400;
            this.mSkillIDTextField.style.minHeight = CommonUtility.MinHeight;
            this.mSkillIDTextField.value = 0;
            aParentPanel.Add(this.mSkillIDTextField);

            //��������
            this.mSkillTypeField = new EnumField("Skill Type:", SKillType.ACTIVE);
            this.mSkillTypeField.style.width = 400;
            this.mSkillTypeField.style.minHeight = CommonUtility.MinHeight;
            aParentPanel.Add(this.mSkillTypeField);

            //�¼�����
            Box rBG = new Box();
            rBG.style.width = Length.Percent(100);
            rBG.style.height = Length.Percent(100);
            aParentPanel.Add(rBG);
            //�Ҽ�Context Menu
            rBG.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
            {
                evt.menu.AppendAction(SkillEventType.ANIMATION.ToString(), (x) => { this.OnCreateEvent(SkillEventType.ANIMATION); });
                evt.menu.AppendAction(SkillEventType.CREATE_VFX.ToString(), (x) => { this.OnCreateEvent(SkillEventType.CREATE_VFX); });
                evt.menu.AppendAction(SkillEventType.ATTACH_VFX.ToString(), (x) => { this.OnCreateEvent(SkillEventType.ATTACH_VFX); });
                evt.menu.AppendAction(SkillEventType.HIT.ToString(), (x) => { this.OnCreateEvent(SkillEventType.HIT); });
                evt.menu.AppendAction(SkillEventType.SUMMON.ToString(), (x) => { this.OnCreateEvent(SkillEventType.SUMMON); });
                evt.menu.AppendAction(SkillEventType.AUDIO.ToString(), (x) => { this.OnCreateEvent(SkillEventType.AUDIO); });
            }));

            //�¼�Title
            CommonUtility.CreateLable("Events", rBG, 3, 3, 1, 1);

            //�¼��б�
            this.mEventsScrollView = new ScrollView();
            this.mEventsScrollView.style.width = Length.Percent(100);
            this.mEventsScrollView.style.height = Length.Percent(100);
            rBG.Add(this.mEventsScrollView);

        }

        //����Ԥ����Ҫ��ʱ��
        private float CalcTotalPreviewTime()
        {
            float fSkillTime = 0;
            foreach (var child in this.mEventsScrollView.Children())
            {
                EventDataBase data = child.userData as EventDataBase;
                float fTime = data.StartTime + data.GetDuration();
                if (fTime > fSkillTime)
                {
                    fSkillTime = fTime;
                }
            }
            return fSkillTime;
        }

        //���㼼����ʵʱ��
        private float CalcRealSkillTime()
        {
            //�����¼���ʼʱ�䲻�ܳ�����������ʱ�䣬���һ����������ʱ���Ǽ���ʱ��
            float fSkillTime = 0;
            foreach (var child in this.mEventsScrollView.Children())
            {
                EventDataBase data = child.userData as EventDataBase;
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
                EventDataBase data = child.userData as EventDataBase;
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

        //�����¼�
        private void OnCreateEvent(SkillEventType aEventType)
        {
            if (aEventType == SkillEventType.INVALID) { return; }
            EventDataBase rEditorData = this.NewEditorData(aEventType);

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
            rColorMark.style.backgroundColor = rEditorData.GetDisplayColor();

            TextField rEventNameTextField = new TextField();
            rEventItem.Add(rEventNameTextField);
            rEventNameTextField.value = aEventType.ToString();
            rEventNameTextField.style.width = 150;
            rEventNameTextField.style.height = 20;
            rEventNameTextField.RegisterCallback<ChangeEvent<string>>((aEvt) => 
            {
                //����Event����
                rEditorData.EventName = aEvt.newValue;

                //���ܰ����ո�
                if (rEditorData.EventName.Contains(" ") == true)
                {
                    rEditorData.EventName = rEditorData.EventName.Replace(" ", "");
                    rEventNameTextField.value = rEditorData.EventName;
                }
            });
            rEditorData.EventName = aEventType.ToString();
            rEditorData.EventNameField = rEventNameTextField;

            CommonUtility.CreateLable("Start Time:", rEventItem, 0, 0, 0, 0);

            FloatField rStartTimeField = new FloatField();
            rEventItem.Add(rStartTimeField);
            rStartTimeField.value = 0;
            rStartTimeField.style.width = 50;
            rStartTimeField.style.height = 20;
            rStartTimeField.RegisterCallback<ChangeEvent<float>>((aEvt) =>
            {
                //���¿�ʼʱ��
                rEditorData.StartTime = aEvt.newValue;
            });
            rEditorData.StartTimeField = rStartTimeField;

            VisualElement rButtonPanel = CommonUtility.CreateVisualElement(rEventItem);
            rButtonPanel.style.flexDirection = FlexDirection.Row;
            rButtonPanel.style.width = Length.Percent(100);
            rButtonPanel.style.alignItems = Align.Center;
            rButtonPanel.style.justifyContent = Justify.FlexEnd;

            //����ť
            Button rMoveUp = new Button();
            rButtonPanel.Add(rMoveUp);
            rMoveUp.text = "��";
            rMoveUp.clicked += () =>
            {
                //����������
                int nIDX = this.mEventsScrollView.IndexOf(rEventItem);
                if (nIDX == 0) { return; }
                this.mEventsScrollView.RemoveAt(nIDX);
                this.mEventsScrollView.Insert(nIDX - 1, rEventItem);
            };

            Button rMoveDown = new Button();
            rButtonPanel.Add(rMoveDown);
            rMoveDown.text = "��";
            rMoveDown.clicked += () =>
            {
                //����������
                int nIDX = this.mEventsScrollView.IndexOf(rEventItem);
                if (nIDX == this.mEventsScrollView.childCount - 1) { return; }
                this.mEventsScrollView.RemoveAt(nIDX);
                this.mEventsScrollView.Insert(nIDX + 1, rEventItem);
            };

            //ɾ����ť
            Button rDel = new Button();
            rButtonPanel.Add(rDel);
            rDel.text = "Del";
            rDel.clicked += () =>
            {
                if (rEventItem == this.mLastSelectedEventItem)
                {
                    rEditorData.LoseFocus();
                    this.mLastSelectedEventItem = null;
                    //ˢ���������
                    this.mPropertyPanel.Clear();
                    CommonUtility.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);
                }
                this.mEventsScrollView.Remove(rEventItem);
                rEditorData.Clear();
            };
        }

        private EventDataBase NewEditorData(SkillEventType aEventType)
        {
            //����Event���ʹ����м�����
            EventDataBase rEditorData = null;
            if (aEventType == SkillEventType.ANIMATION)
            {
                rEditorData = new AnimationEventData();
            }
            else if (aEventType == SkillEventType.CREATE_VFX)
            {
                rEditorData = new CreateVFXEventData();
            }
            else if (aEventType == SkillEventType.ATTACH_VFX)
            {
                rEditorData = new AttachVFXEventData();
            }
            else if (aEventType == SkillEventType.HIT)
            {
                rEditorData = new HitEventData();
            }
            else if (aEventType == SkillEventType.SUMMON)
            {
                rEditorData = new SummonEventData();
            }
            else if (aEventType == SkillEventType.AUDIO)
            {
                rEditorData = new AudioEventData();
            }
            rEditorData.AvatarObjField = this.mAvatarObjField;
            rEditorData.Init();
            return rEditorData;
        }

        //ѡ��Event
        private void OnClickEventItem(MouseDownEvent aEvt)
        {
            if (aEvt.button != 0) { return; }   //ֻ������������ѡ��

            Box rItem = aEvt.currentTarget as Box;

            //ѡ��ͬһ����ˢ��
            if (this.mLastSelectedEventItem == rItem)
            {
                return;
            }

            this.mPropertyPanel.Clear();
            CommonUtility.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);

            if (this.mLastSelectedEventItem != null)
            {
                this.mLastSelectedEventItem.style.backgroundColor = new Color(0, 0, 0, 0.1f);
                (this.mLastSelectedEventItem.userData as EventDataBase).LoseFocus();
            }

            rItem.style.backgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
            this.mLastSelectedEventItem = rItem;
            EventDataBase rSkillData = (rItem.userData as EventDataBase);
            rSkillData.GetFocus();
            this.mPropertyPanel.Add(rSkillData.GetRootPanel());
        }

        //�½������ļ�
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

        //���������ļ�
        private void OnSkillDataChanged(ChangeEvent<Object> aEvt)
        {
            foreach (var child in this.mEventsScrollView.Children())
            {
                (child.userData as EventDataBase).Clear();
            }
            this.mSkillIDTextField.value = 0;
            this.mEventsScrollView.Clear();
            this.mPropertyPanel.Clear();
            CommonUtility.CreateLable("Property", this.mPropertyPanel, 3, 3, 1, 1);
            
            //��������
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
                //���ļ�
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
                EventDataBase data = this.mEventsScrollView.ElementAt(nIDX).userData as EventDataBase;
                data.LoadFromFile(line);
                nIDX++;
            }
            fs.Close();
        }

        //�����ļ�
        private void OnClickSaveSkill()
        {
            TextAsset rSkillData = this.mSkillDataObjField.value as TextAsset;
            if (rSkillData == null)
            {
                this.SaveAs();  //���û�д����ļ���ֱ�ӱ༭
                //EditorUtility.DisplayDialog("Error", "Can't find the skill data file!", "OK");
                return;
            }

            string strPath = Application.dataPath.Replace("Assets", "") + AssetDatabase.GetAssetPath(rSkillData);
            FileStream fs = new FileStream(strPath, FileMode.Open, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string strContent = this.FormatSaveContent();
            if (strContent == null)
            {
                fs.Close();
                return;
            }

            sw.Write(strContent);
            sw.Flush();
            fs.Close();

            EditorUtility.DisplayDialog("Tip", "Saved", "OK");
        }

        //���û�д����ļ���ֱ�ӱ༭�����´���һ���ļ�Ȼ��洢
        private void SaveAs()
        {
            string strPath = EditorUtility.SaveFilePanel("Select Path", "", "SkillData", "txt");
            if (strPath == string.Empty)
            {
                return;
            }

            FileStream fs = new FileStream(strPath, FileMode.Create, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs);
            string strContent = this.FormatSaveContent();
            if (strContent == null)
            {
                fs.Close();
                return;
            }

            sw.Write(strContent);
            sw.Flush();
            fs.Close();

            strPath = strPath.Replace(Application.dataPath, "Assets");
            AssetDatabase.Refresh();
            TextAsset rSkillData = AssetDatabase.LoadAssetAtPath<TextAsset>(strPath);
            this.mSkillDataObjField.value = rSkillData;
        }

        private string FormatSaveContent()
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder();

            float fSkillTime = this.CalcRealSkillTime();

            //0       1         2
            //SkillID SkillType SKillTime
            string strSkillInfo = string.Format("{0} {1} {2:N2}",
                this.mSkillIDTextField.value,
                (int)(SKillType)this.mSkillTypeField.value,
                fSkillTime);
            sbText.AppendLine(strSkillInfo);

            //���¼�
            foreach (var child in this.mEventsScrollView.Children())
            {
                EventDataBase data = child.userData as EventDataBase;
                string strContent = data.SaveToFile();
                if (strContent == string.Empty)
                {
                    return null;
                }
                sbText.AppendLine(strContent);
            }

            return sbText.ToString();
        }
    }
}
