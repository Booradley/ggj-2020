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
        private static readonly int IsTeasingBool = Animator.StringToHash("IsTeasing");

        public static Action OnOpened;

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
        private ParticleSystem _particleSystem;
        private Hand _hand;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            _particleSystem = GetComponentInChildren<ParticleSystem>();

            _lid.OnLidHovered += HandleLidHovered;
            _lid.OnLidUnhovered += HandleLidUnhovered;
            _lid.OnLidGrabbed += HandleLidGrabbed;
        }

        private void OnAttachedToHand(Hand hand)
        {
            _hand = hand;
        }

        private void OnDetachedFromHand(Hand hand)
        {
            _hand = null;
        }

        private void HandleLidHovered()
        {
            if (_state == CubimalBoxState.Closed)
                _animator.SetBool(IsTeasingBool, true);
        }

        private void HandleLidUnhovered()
        {
            if (_state == CubimalBoxState.Closed)
                _animator.SetBool(IsTeasingBool, false);
        }

        private void HandleLidGrabbed()
        {
            if (_state == CubimalBoxState.Closed)
                StartCoroutine(OpenBoxSequence());
        }

        public void Spawn()
        {
            _rigidbody.isKinematic = true;
            _animator.SetTrigger(SpawnTrigger);
        }

        private IEnumerator OpenBoxSequence()
        {
            OnOpened?.Invoke();
            
            _state = CubimalBoxState.Opening;

            _rigidbody.isKinematic = true;
            _animator.SetTrigger(OpenBool);
            
            Hand hand = _hand;
            
            yield return new WaitForSeconds(0.25f);

            GrabTypes? currentGrabTypes = hand.currentAttachedObjectInfo?.grabbedWithType;
            Hand.AttachmentFlags? currentAttachmentFlags = hand.currentAttachedObjectInfo?.attachmentFlags;
            
            Cubimal cubimal = Instantiate(_cubimalPrefabs[Random.Range(0, _cubimalPrefabs.Length)], transform.position, transform.rotation).GetComponent<Cubimal>();
            cubimal.Spawn();

            _particleSystem.transform.SetParent(null, true);
            _particleSystem.Play();
            _particleSystem = null;
            
            hand.DetachObject(gameObject, true);
            hand.AttachObject(cubimal.gameObject, currentGrabTypes.GetValueOrDefault(), currentAttachmentFlags.GetValueOrDefault());

            yield return new WaitForSeconds(0.25f);

            Destroy(gameObject);
        }
    }
}