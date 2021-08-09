using UnityEngine;
using System;
using System.Collections.Generic;

namespace HotfixGameplay.Framework
{
    public class HotfixGlobalEvent
    {
        public delegate void Callback(params object[] args);
        private class EventData
        {
            public Callback mCallback;
            public bool mOnce = false;
        }
        private static Dictionary<Enum, List<EventData>> mDicGlobalEvent = new Dictionary<Enum, List<EventData>>();

        private static void AddEvent(Enum aEvent, EventData aData)
        {
            if (mDicGlobalEvent.ContainsKey(aEvent))
            {
                mDicGlobalEvent[aEvent].Add(aData);
            }
            else
            {
                List<EventData> list = new List<EventData>();
                list.Add(aData);
                mDicGlobalEvent.Add(aEvent, list);
            }
        }

        public static void Add(Enum aEvent, Callback aCallback)
        {
            EventData zed = new EventData();
            zed.mCallback += aCallback;
            zed.mOnce = false;
            AddEvent(aEvent, zed);
        }

        public static void AddOneShot(Enum aEvent, Callback aCallback)
        {
            EventData zed = new EventData();
            zed.mCallback += aCallback;
            zed.mOnce = true;
            AddEvent(aEvent, zed);
        }

        public static void RemoveEvent(Enum aEvent)
        {
            if (mDicGlobalEvent.ContainsKey(aEvent))
            {
                mDicGlobalEvent[aEvent].Clear();
                mDicGlobalEvent.Remove(aEvent);
            }
        }

        public static void RemoveEvent(Enum aEvent, Callback aCallback)
        {
            if (mDicGlobalEvent.ContainsKey(aEvent))
            {
                foreach (EventData zed in mDicGlobalEvent[aEvent])
                {
                    if (zed.mCallback.Equals(aCallback))
                    {
                        mDicGlobalEvent[aEvent].Remove(zed);
                        break;
                    }
                }
                if (mDicGlobalEvent[aEvent].Count <= 0)
                {
                    mDicGlobalEvent.Remove(aEvent);
                }
            }
        }

        public static void Dispatch(Enum aEvent, params object[] args)
        {
            if (mDicGlobalEvent.ContainsKey(aEvent))
            {
                List<EventData> list = mDicGlobalEvent[aEvent];
                List<EventData> removeList = new List<EventData>();
                for (int i = 0, max = list.Count; i < max; ++i)
                {
                    if (list[i].mOnce)
                    {
                        removeList.Add(list[i]);
                    }
                    list[i].mCallback(args);
                }
                for (int i = 0, max = removeList.Count; i < max; ++i)
                {
                    list.Remove(removeList[i]);
                }
                if (list.Count <= 0)
                {
                    mDicGlobalEvent.Remove(aEvent);
                }
            }
        }
    }
}