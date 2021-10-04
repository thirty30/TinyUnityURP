using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace SkillEditor
{
    public class T3DHitEditorData : T3DSkillEditorDataBase
    {
        public override SkillEventType GetEventType() { return SkillEventType.HIT; }
        public override bool IsPreviewing() { return false; }
    }
}
