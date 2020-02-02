using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.CubimalRacing
{
    public class Controller : MonoBehaviour
    {
        private void Start()
        {
            Teleport.instance.CancelTeleportHint();
        }
    }
}