
namespace TFramework
{
    public class TFSMStateBase
    {
        protected int State;

        public TFSMStateBase(int aState)
        {
            this.State = aState;
        }

        public int GetState()
        {
            return this.State;
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

