
namespace TFramework
{
    public class TFSMStateBase
    {
        public TFSM FSM = null;

        protected int mState = 0;

        //Ϊ������ILRuntime Adapter��д�Ŀչ��캯��,д�߼�����ʹ��
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

