using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class TFSM
    {
        private TFSMStateBase mCurStateObj = null;
        private Dictionary<int, TFSMStateBase> mStateObjs = new Dictionary<int, TFSMStateBase>();

        public bool RegisterState(TFSMStateBase aStateObj)
        {
            int nState = aStateObj.GetState();
            if (this.mStateObjs.ContainsKey(nState) == true)
            {
                return false;
            }
            this.mStateObjs.Add(nState, aStateObj);
            return true;
        }

        public void Update()
        {
            if (this.mCurStateObj != null)
            {
                this.mCurStateObj.Update();
            }
        }

        public void SetState(int aState)
        {
            if (this.mCurStateObj != null && this.mCurStateObj.GetState() == aState)
            {
                return;
            }

            if (this.mCurStateObj != null)
            {
                this.mCurStateObj.OnExitState();
            }

            if (this.mStateObjs.ContainsKey(aState) == false)
            {
                Debug.LogError("Cannot find the state: " + aState.ToString());
                return;
            }

            this.mCurStateObj = this.mStateObjs[aState];
            this.mCurStateObj.OnEnterState();
        }

        public int GetState()
        {
            return this.mCurStateObj.GetState();
        }

        public TFSMStateBase GetStateObj()
        {
            return this.mCurStateObj;
        }
    }
}

