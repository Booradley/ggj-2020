using UnityEngine;
using Valve.VR.InteractionSystem;

namespace EmeraldActivities.CubimalRacing
{
    [RequireComponent(typeof(Interactable))]
    public class Key : MonoBehaviour
    {
        private const float MIN_WIND_AMOUNT = 0;
        private const float MAX_WIND_AMOUNT = 1080f; // 3 full rotations
        private const float UNWIND_AMOUNT_PER_SECOND = 180f;
        
        public enum KeyState
        {
            Windable,
            Winding,
            Unwinding
        }

        private float _windAmount = 0;
        public float WindAmount => _windAmount;
        public float NormalisedWindAmount => _windAmount / MAX_WIND_AMOUNT;
        public float UnwindSeconds => _windAmount / UNWIND_AMOUNT_PER_SECOND;

        private KeyState _state = KeyState.Windable;
        public KeyState State => _state;

        private Hand.AttachmentFlags _attachmentFlags = Hand.AttachmentFlags.DetachOthers
                                                        | Hand.AttachmentFlags.DetachFromOtherHand
                                                        | Hand.AttachmentFlags.TurnOnKinematic;

        private Interactable _interactable;
        private Hand _grabbingHand;
        private Quaternion _grabOffset;

        private void Awake()
        {
            _interactable = GetComponent<Interactable>();
        }

        private void HandHoverUpdate(Hand hand)
        {
            // Don't allow interaction when unwinding
            if (_state == KeyState.Unwinding)
                return;
            
            GrabTypes startingGrabType = hand.GetGrabStarting();
            bool isGrabEnding = hand.IsGrabEnding(gameObject);

            if (_interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
            {
                hand.HoverLock(_interactable);
                hand.AttachObject(gameObject, startingGrabType, _attachmentFlags);

                _grabbingHand = hand;
                _state = KeyState.Winding;
            }
            else if (isGrabEnding)
            {
                hand.DetachObject(gameObject);
                hand.HoverUnlock(_interactable);

                _grabbingHand = null;
                _state = KeyState.Windable;
                Debug.Log(_windAmount);
            }
        }

        private void Update()
        {
            if (_state == KeyState.Winding)
            {
                Vector3 angularVelocity = _grabbingHand.GetTrackedObjectAngularVelocity();
                Vector3 localAngularVelocity = transform.InverseTransformDirection(angularVelocity);
                
                localAngularVelocity.x = 0f;
                localAngularVelocity.y = 0f;
                transform.localRotation = transform.localRotation * Quaternion.Euler(localAngularVelocity);
                
                _windAmount = Mathf.Clamp(_windAmount + -localAngularVelocity.z, MIN_WIND_AMOUNT, MAX_WIND_AMOUNT);
            }
            else if (_state == KeyState.Unwinding)
            {
                _windAmount = Mathf.Clamp(_windAmount - (UNWIND_AMOUNT_PER_SECOND * Time.deltaTime), MIN_WIND_AMOUNT, MAX_WIND_AMOUNT);

                transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, -_windAmount));
            }
        }

        public void StartUnwinding()
        {
            _state = KeyState.Unwinding;
            Debug.Log(_windAmount);
        }

        public void StopUnwinding()
        {
            _state = KeyState.Windable;
            Debug.Log(_windAmount);
        }
    }
}