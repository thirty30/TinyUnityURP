using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework.TGUI
{
    public class Billboard : MonoBehaviour
    {
        public enum BillBoardType
        {
            Free,
            LockYAxis,
        }

        public BillBoardType Type;
        public Camera TargetCamera;

        private void Update()
        {
            if (this.TargetCamera == null) { return; }

            if (this.Type == BillBoardType.Free)
            {
                this.gameObject.transform.LookAt(this.TargetCamera.transform, this.TargetCamera.transform.up);
            }
            else if (this.Type == BillBoardType.LockYAxis)
            {
                this.gameObject.transform.LookAt(this.TargetCamera.transform, Vector3.up);
            }
        }
    }
}
