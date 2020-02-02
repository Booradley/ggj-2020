using System;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class Destroyable : MonoBehaviour
    {
        public static event Action<Destroyable> OnOutOfBounds;
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Bounds"))
            {
                OnOutOfBounds?.Invoke(this);
                Destroy(gameObject);
            }
        }
    }
}