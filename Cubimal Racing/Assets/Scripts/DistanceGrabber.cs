using UnityEngine;
using Valve.VR.Extras;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.CubimalRacing
{
    public class DistanceGrabber : MonoBehaviour
    {
        private LaserPointer _laserPointer;
        private Hand _hand;
        
        private void Awake()
        {
            _laserPointer = GetComponent<LaserPointer>();
            _hand = GetComponent<Hand>();
 
            _laserPointer.PointerIn += PointerInside;
            _laserPointer.PointerOut += PointerOutside;
        }
 
        public void PointerInside(object sender, PointerEventArgs e)
        {
            Interactable interactable = e.target.GetComponentInParent<Interactable>();
            Throwable throwable = e.target.GetComponentInParent<Throwable>();
            if (interactable != null && throwable != null)
            {
                if (_hand.hoveringInteractable == null)
                {
                    _hand.HoverLock(interactable);
                    throwable.attachmentFlags |= Hand.AttachmentFlags.SnapOnAttach;
                    throwable.onPickUp.AddListener(HandlePickedUp);

                    void HandlePickedUp()
                    {
                        throwable.attachmentFlags &= ~Hand.AttachmentFlags.SnapOnAttach;
                        throwable.onPickUp.RemoveListener(HandlePickedUp);
                    }
                }
            }
        }
 
        public void PointerOutside(object sender, PointerEventArgs e)
        {
            Interactable interactable = e.target.GetComponentInParent<Interactable>();
            Throwable throwable = e.target.GetComponentInParent<Throwable>();
            if (interactable != null && throwable != null)
            {
                if (_hand.hoveringInteractable == interactable)
                {
                    _hand.HoverUnlock(interactable);
                    throwable.attachmentFlags &= ~Hand.AttachmentFlags.SnapOnAttach;
                }
            }
        }
    }
}