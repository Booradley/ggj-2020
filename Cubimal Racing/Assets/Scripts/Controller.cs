using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.CubimalRacing
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem _outOfBoundsParticles;
        
        private void Start()
        {
            Teleport.instance.CancelTeleportHint();
            
            Destroyable.OnOutOfBounds += HandleObjectOutOfBounds;
        }

        private void HandleObjectOutOfBounds(Destroyable destroyable)
        {
            _outOfBoundsParticles.transform.position = destroyable.transform.position;
            _outOfBoundsParticles.Emit(100);
        }
    }
}