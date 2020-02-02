using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace EmeraldActivities.CubimalRacing
{
    public class Cubimal : MonoBehaviour
    {
        private static readonly int IsRacingBool = Animator.StringToHash("IsRacing");
        private static readonly int SpawnTrigger = Animator.StringToHash("Spawn");
        private static readonly int WinRaceTrigger = Animator.StringToHash("WinRace");
        
        private const float SETTLE_TIME = 1.5f;
        private const float MAX_DISTANCE = 5f;
        private const float MIN_SPEED = 0.5f;
        private const float MAX_SPEED = 1.0f;
        private const float DESTINATION_REACHED_DISTANCE = 0.25f;
        
        public enum CubimalState
        {
            Held,
            Airborne,
            Settled,
            Moving,
            Racing
        }

        public event Action OnPickedUp;
        public event Action OnStopped;

        public bool IsWound => _key.WindAmount > 0f;
        
        [SerializeField]
        private Key _key;

        [SerializeField] 
        private GameObject _crown;

        private NavMeshAgent _navMeshAgent;
        private Animator _animator;
        private Rigidbody _rigidbody;
        private CubimalState _state = CubimalState.Settled;
        private bool _isSettling = false;
        private float _currentSettleTime = 0f;
        private Vector3 _destination;
        private Coroutine _stopUnwindingSequence;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody>();
            
            _crown.SetActive(false);

            _key.OnUnwound += HandleKeyUnwound;
        }

        private void OnAttachedToHand(Hand hand)
        {
            if (_stopUnwindingSequence != null)
            {
                StopCoroutine(_stopUnwindingSequence);
                _stopUnwindingSequence = null;
            }
            
            _state = CubimalState.Held;
            _currentSettleTime = 0f;
            _isSettling = false;
            
            StopMoving();
            
            OnPickedUp?.Invoke();
        }

        private void OnDetachedFromHand(Hand hand)
        {
            _state = CubimalState.Airborne;
            _currentSettleTime = 0f;
            _isSettling = false;
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
            if (_state == CubimalState.Moving)
            {
                if (Vector3.Distance(transform.position, _destination) <= DESTINATION_REACHED_DISTANCE || !_navMeshAgent.hasPath)
                {
                    StopMoving();
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

                        if (CanMove())
                        {
                            StartMoving();
                        }
                    }
                }
            }
        }

        private void HandleKeyUnwound()
        {
            StopMoving();
        }

        public void Spawn(bool isHeld)
        {
            _animator.SetTrigger(SpawnTrigger);

            if (isHeld)
            {
                _state = CubimalState.Airborne;
            }
        }

        public bool CanRace()
        {
            return _state == CubimalState.Airborne || _state == CubimalState.Settled;
        }

        public void PositionForRace()
        {
            _state = CubimalState.Racing;
            _rigidbody.isKinematic = true;
        }

        public void StartRace(Vector3 destination)
        {
            if (CanMove())
            {
                StartMoving(destination);
            }
            else
            {
                _state = CubimalState.Settled;
                _rigidbody.isKinematic = false;
            }
        }

        public void FinishRace(bool didWin)
        {
            if (didWin)
            {
                StopMoving();
                _animator.SetTrigger(WinRaceTrigger);
                _crown.SetActive(true);
            }
        }

        public bool CanMove()
        {
            return (_state == CubimalState.Settled || _state == CubimalState.Racing) && _key.State == Key.KeyState.Windable && _key.WindAmount > 0f;
        }

        public void StartMoving(Vector3 overrideDestination = default)
        {
            _state = CubimalState.Moving;

            if (IsStanding())
            {
                if (overrideDestination != default)
                {
                    _destination = overrideDestination;
                }
                else
                {
                    _destination = transform.position + transform.forward * (_key.NormalisedWindAmount * MAX_DISTANCE);
                }
            
                _navMeshAgent.enabled = true;
                _navMeshAgent.speed = Random.Range(MIN_SPEED, MAX_SPEED);
                _navMeshAgent.SetDestination(_destination);
            }
            else
            {
                _stopUnwindingSequence = StartCoroutine(StopUnwinding());
            }

            _animator.SetBool(IsRacingBool, true);
            _key.StartUnwinding();
        }

        public IEnumerator AutoWind(float windSeconds)
        {
            yield return _key.AutoWind(windSeconds);
        }

        private IEnumerator StopUnwinding()
        {
            yield return new WaitForSeconds(_key.UnwindSeconds);
            
            _animator.SetBool(IsRacingBool, false);
            _key.StopUnwinding();
            
            _stopUnwindingSequence = null;
        }

        private void StopMoving()
        {
            _state = CubimalState.Settled;
            _isSettling = false;
            
            _navMeshAgent.enabled = false;
            _animator.SetBool(IsRacingBool, false);
            
            _key.StopUnwinding();
            _destination = Vector3.zero;
            
            OnStopped?.Invoke();
        }

        private bool IsStanding()
        {
            return Vector3.Dot(transform.up, Vector3.up) > 0.75f;
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