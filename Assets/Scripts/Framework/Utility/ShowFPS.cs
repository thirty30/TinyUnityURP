using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TFramework
{
    public class ShowFPS : MonoBehaviour
    {
        public Text FPSText;
        private float UpdateInterval = 0.5F;
        private double LastInterval;
        private int Frames = 0;
        private float FPS;

        void Start()
        {
            this.LastInterval = Time.realtimeSinceStartup;
            this.Frames = 0;
        }

        private void Update()
        {
            if (this.FPSText == null)
            {
                this.FPSText.gameObject.SetActive(false);
                return;
            }
            ++this.Frames;
            float fTimeNow = Time.realtimeSinceStartup;
            if (fTimeNow > this.LastInterval + this.UpdateInterval)
            {
                this.FPS = (float)(this.Frames / (fTimeNow - this.LastInterval));
                this.Frames = 0;
                this.LastInterval = fTimeNow;

                this.FPSText.text = this.FPS.ToString("F2");
            }
        }
    }
}

