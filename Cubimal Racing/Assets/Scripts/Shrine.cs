using System;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class Shrine : MonoBehaviour
    {
        public static event Action OnSacrifice;
        
        [SerializeField]
        private ParticleSystem _particles;
        
        private void OnTriggerEnter(Collider other)
        {
            Destroyable destroyable = other.gameObject.GetComponentInParent<Destroyable>();
            if (destroyable != null)
            {
                _particles.Emit(50);
                Destroy(other.gameObject);
                
                OnSacrifice?.Invoke();
            }
        }
    }
}