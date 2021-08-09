using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TFramework.TGUI
{
    public class TDragUtility : ScrollRect
    {
        public delegate void DragCallBack(Vector2 StartPosition, Vector2 EndPosition, Vector2 DragDir);
        public delegate void StartDragCallBack(Vector2 StartPosition, Vector2 EndPosition, Vector2 DragDir);
        public delegate void EndDragCallBack(Vector2 StartPosition, Vector2 EndPosition, Vector2 DragDir);

        private Vector2 mStartPos = Vector2.zero;
        private DragCallBack mCallBack;
        private StartDragCallBack mStartCallBack;
        private EndDragCallBack mEndCallBack;

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            this.mStartPos = eventData.position;
            this.mStartCallBack?.Invoke(this.mStartPos, this.mStartPos, Vector2.zero);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);
            Vector2 dir = (eventData.position - this.mStartPos).normalized;
            this.mCallBack?.Invoke(this.mStartPos, eventData.position, dir);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
            if (this.mEndCallBack != null)
            {
                Vector2 dir = (eventData.position - this.mStartPos).normalized;
                this.mEndCallBack(this.mStartPos, eventData.position, dir);
            }
            this.mStartPos = Vector2.zero;
        }

        public void AddDragEvent(DragCallBack aFunc)
        {
            this.mCallBack += aFunc;
        }

        public void RemoveDragEvent(DragCallBack aFunc)
        {
            this.mCallBack -= aFunc;
        }

        public void AddStartDragEvent(StartDragCallBack aFunc)
        {
            this.mStartCallBack += aFunc;
        }

        public void RemoveStartDragEvent(StartDragCallBack aFunc)
        {
            this.mStartCallBack -= aFunc;
        }

        public void AddEndDragEvent(EndDragCallBack aFunc)
        {
            this.mEndCallBack += aFunc;
        }

        public void RemoveEndDragEvent(EndDragCallBack aFunc)
        {
            this.mEndCallBack -= aFunc;
        }
    }
}

