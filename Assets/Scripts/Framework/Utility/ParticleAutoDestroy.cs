using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TFramework
{
    public class ParticleAutoDestroy : MonoBehaviour
    {
        private ParticleSystem[] mParticleSystems;

        void Start()
        {
            this.mParticleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        void Update()
        {
            if (this.mParticleSystems.Length <= 0)
            {
                return;
            }
            bool isAllStopped = true;
            foreach (ParticleSystem ps in this.mParticleSystems)
            {
                if (ps.isStopped == false)
                {
                    isAllStopped = false;
                    break;
                }
            }
            if (isAllStopped == true)
            {
                GameObject.Destroy(gameObject);
            }
        }
    }
}
