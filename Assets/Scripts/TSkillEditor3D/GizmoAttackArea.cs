using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TSkillEditor3D
{
    public class T3DSkillEditorAttackArea : MonoBehaviour
    {
        private HitAreaType mAreaType;
        private Vector3 mCenter;
        private Vector3 mSectorFrom;
        private float mAngle;
        private float mRadius;


        private void OnDrawGizmos()
        {
            UnityEditor.Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            switch (this.mAreaType)
            {
                case HitAreaType.CURRENT_TARGET:
                    break;
                case HitAreaType.FAR_TARGET:
                case HitAreaType.LOW_HP_TARGET:
                case HitAreaType.ALL_TARGET:
                case HitAreaType.RANDOM_TARGET:
                    {
                        UnityEditor.Handles.color = new Color(0, 1, 0, 0.1f);
                        UnityEditor.Handles.DrawSolidDisc(this.mCenter, Vector3.up, this.mRadius);
                    }
                    break;
                case HitAreaType.SECTOR:
                    {
                        UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
                        UnityEditor.Handles.DrawSolidArc(this.mCenter, Vector3.up, this.mSectorFrom, this.mAngle, this.mRadius);
                    }
                    break;
                case HitAreaType.CIRCLE:
                    {
                        UnityEditor.Handles.color = new Color(1, 0, 0, 0.1f);
                        UnityEditor.Handles.DrawSolidDisc(this.mCenter, Vector3.up, this.mRadius);
                    }
                    break;
                default:
                    break;
            }
        }

        public void DrawTarget(HitAreaType aType, float aRadius)
        {
            this.mAreaType = aType;
            this.mRadius = aRadius;
        }

        public void DrawSector(Vector3 aCenter, Vector3 aForward, float aAngle, float aRadius)
        {
            this.mAreaType = HitAreaType.SECTOR;
            this.mCenter = aCenter;
            this.mAngle = aAngle;
            this.mRadius = aRadius;
            this.mSectorFrom = Quaternion.AngleAxis(-this.mAngle / 2, Vector3.up) * aForward;
        }

        public void DrawCircle(Vector3 aCenter, float aRadius)
        {
            this.mAreaType = HitAreaType.CIRCLE;
            this.mCenter = aCenter;
            this.mRadius = aRadius;
        }
    }
}
