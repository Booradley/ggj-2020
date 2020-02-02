using System;
using System.Collections;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace EmeraldActivities.CubimalRacing
{
    public class CubimalBox : MonoBehaviour
    {
        private static readonly int OpenBool = Animator.StringToHash("Open");
        private static readonly int SpawnTrigger = Animator.StringToHash("Spawn");

        public enum CubimalBoxState
        {
            Closed,
            Opening
        }

        [SerializeField]
        private GameObject[] _cubimalPrefabs;

        [SerializeField]
        private CubimalBoxLid _lid;

        private CubimalBoxState _state = CubimalBoxState.Closed;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private Hand _hand;

        private void Start()
        {
            _lid.OnLidGrabbed += HandleLidGrabbed;
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
        }
        
        private void OnAttachedToHand(Hand hand)
        {
            _hand = hand;
        }

        private void OnDetachedFromHand(Hand hand)
        {
            _hand = null;
        }

        private void HandleLidGrabbed()
        {
            if (_state == CubimalBoxState.Closed)
                StartCoroutine(OpenBoxSequence());
        }

        public void Spawn()
        {
            
        }

        private IEnumerator OpenBoxSequence()
        {
            _state = CubimalBoxState.Opening;

            _rigidbody.isKinematic = true;
            _animator.SetTrigger(OpenBool);
            
            yield return new WaitForSeconds(0.5f);

            GrabTypes? currentGrabTypes = _hand.currentAttachedObjectInfo?.grabbedWithType;
            Hand.AttachmentFlags? currentAttachmentFlags = _hand.currentAttachedObjectInfo?.attachmentFlags;
            
            Cubimal cubimal = Instantiate(_cubimalPrefabs[Random.Range(0, _cubimalPrefabs.Length)], transform.position, transform.rotation).GetComponent<Cubimal>();

            Hand hand = _hand;
            hand.DetachObject(gameObject, true);
            hand.AttachObject(cubimal.gameObject, currentGrabTypes.GetValueOrDefault(), currentAttachmentFlags.GetValueOrDefault());

            cubimal.Spawn();

            Destroy(gameObject);
        }
    }
}