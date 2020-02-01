using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace EmeraldActivities.CubimalRacing
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class Cubimal : MonoBehaviour
    {
        private const float SETTLE_TIME = 3f;
        private const float MAX_DISTANCE = 10f;
        private const float MIN_SPEED = 0.5f;
        private const float MAX_SPEED = 1.5f;
        private const float DESTINATION_REACHED_DISTANCE = 0.1f;
        
        private static readonly int IsRacing = Animator.StringToHash("IsRacing");
        
        public enum CubimalState
        {
            Held,
            Airborne,
            Settled,
            Racing
        }
        
        [SerializeField]
        private Key _key;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private CubimalState _state = CubimalState.Settled;
        private bool _isSettling = false;
        private float _currentSettleTime = 0f;
        private Vector3 _destination;
        private Coroutine _stopUnwindingSequence;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
        }
        
        private void OnAttachedToHand(Hand hand)
        {
            if (_stopUnwindingSequence != null)
            {
                StopCoroutine(_stopUnwindingSequence);
                _stopUnwindingSequence = null;
            }
            
            _state = CubimalState.Held;
            _isSettling = false;
            
            StopRacing();
        }

        private void OnDetachedFromHand(Hand hand)
        {
            _state = CubimalState.Airborne;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_state == CubimalState.Airborne && other.gameObject.CompareTag("Ground"))
            {
                _currentSettleTime = 0f;
                _isSettling = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (_state == CubimalState.Airborne && other.gameObject.CompareTag("Ground"))
            {
                _currentSettleTime = 0f;
                _isSettling = false;
            }
        }

        private void Update()
        {
            if (_state == CubimalState.Racing)
            {
                if (Vector3.Distance(transform.position, _destination) <= DESTINATION_REACHED_DISTANCE)
                {
                    StopRacing();
                }
            }
            else if (_state == CubimalState.Airborne)
            {
                if (_isSettling)
                {
                    _currentSettleTime += Time.deltaTime;
                    if (_currentSettleTime >= SETTLE_TIME)
                    {
                        _state = CubimalState.Settled;
                        _isSettling = false;

                        if (CanRace())
                        {
                            StartRacing();
                        }
                    }
                }
                else
                {
                    _currentSettleTime = 0f;
                }
            }
        }

        private bool CanRace()
        {
            return _key.State == Key.KeyState.Windable && _key.WindAmount > 0f;
        }

        private void StartRacing()
        {
            _state = CubimalState.Racing;

            if (IsStanding())
            {
                _destination = transform.position + transform.forward * (_key.NormalisedWindAmount * MAX_DISTANCE);
            
                _navMeshAgent.enabled = true;
                _navMeshAgent.speed = Random.Range(MIN_SPEED, MAX_SPEED);
                _navMeshAgent.SetDestination(_destination);
            }
            else
            {
                _stopUnwindingSequence = StartCoroutine(StopUnwinding());
            }

            _animator.SetBool(IsRacing, true);
            _key.StartUnwinding();
        }

        private IEnumerator StopUnwinding()
        {
            yield return new WaitForSeconds(_key.UnwindSeconds);
            
            _animator.SetBool(IsRacing, false);
            _key.StopUnwinding();
            
            _stopUnwindingSequence = null;
        }

        private void StopRacing()
        {
            _navMeshAgent.enabled = false;
            _animator.SetBool(IsRacing, false);
            
            _key.StopUnwinding();
            _destination = Vector3.zero;
        }

        private bool IsStanding()
        {
            return Vector3.Dot(transform.up, Vector3.up) > 0.95f;
        }

        private void OnDrawGizmos()
        {
            if (_destination != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, _destination);
                Gizmos.DrawCube(_destination, Vector3.one * 0.1f);
            }
        }
    }
}