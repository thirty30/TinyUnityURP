using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace TSkillEditor3D
{
    public class SummonEventData : EventDataBase
    {
        public override SkillEventType GetEventType() { return SkillEventType.SUMMON; }
        public override Color GetDisplayColor() { return Color.blue; }
        public override bool IsPreviewing() { return false; }
    }
}
