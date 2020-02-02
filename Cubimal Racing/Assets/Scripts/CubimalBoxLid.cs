using System;
using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.CubimalRacing
{
    public class CubimalBoxLid : MonoBehaviour
    {
        public Action OnLidHovered;
        public Action OnLidUnhovered;
        public Action OnLidGrabbed;
        
        private Interactable _interactable;
        private Hand _grabbingHand;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }

        private void OnHandHoverBegin(Hand hand)
        {
            OnLidHovered?.Invoke();
        }

        private void OnHandHoverEnd(Hand hand)
        {
            OnLidUnhovered?.Invoke();
        }
        
        private void HandHoverUpdate(Hand hand)
        {
            GrabTypes startingGrabType = hand.GetGrabStarting();

            if (_interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                _interactable.enabled = false;
                OnLidGrabbed?.Invoke();
            }
        }
    }
}