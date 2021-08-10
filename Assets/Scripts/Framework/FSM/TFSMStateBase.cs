
namespace TFramework
{
    public class TFSMStateBase
    {
        public TFSM FSM = null;

        protected int mState = 0;

        //为了生成ILRuntime Adapter而写的空构造函数,写逻辑不能使用
        public TFSMStateBase() { }

        public TFSMStateBase(int aState)
        {
            this.mState = aState;
        }

        public int GetState()
        {
            return this.mState;
        }

        public void Update()
        {
            this.OnUpdateState();
        }

        public virtual void OnEnterState() { }
        public virtual void OnUpdateState() { }
        public virtual void OnExitState() { }
    }
}

