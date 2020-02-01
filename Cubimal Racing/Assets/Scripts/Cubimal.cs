using System;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR.InteractionSystem;
using Random = UnityEngine.Random;

namespace EmeraldActivities.CubimalRacing
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class Cubimal : MonoBehaviour
    {
        private const float SETTLE_TIME = 3f;
        private const float MAX_DISTANCE = 5f;
        private const float MIN_SPEED = 1f;
        private const float MAX_SPEED = 2f;
        
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
        private CubimalState _state = CubimalState.Settled;
        private bool _isSettling = false;
        private float _currentSettleTime = 0f;
        private Vector3 _destination;
        
        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }
        
        private void OnAttachedToHand(Hand hand)
        {
            Debug.Log("Held");
            _state = CubimalState.Held;
            StopRacing();
        }

        private void OnDetachedFromHand(Hand hand)
        {
            Debug.Log("Flying");
            _state = CubimalState.Airborne;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (_state == CubimalState.Airborne && other.gameObject.CompareTag("Ground"))
            {
                Debug.Log("Settling");
                _isSettling = true;
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (_state == CubimalState.Airborne && other.gameObject.CompareTag("Ground"))
            {
                Debug.Log("Stop Settling");
                _isSettling = false;
            }
        }

        private void Update()
        {
            if (_state == CubimalState.Racing)
            {
                
            }
            else
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
            Debug.Log("Racing");
            _state = CubimalState.Racing;

            float speed = Random.Range(MIN_SPEED, MAX_SPEED);

            if (IsStanding())
            {
                _destination = transform.position + transform.forward * (_key.NormalisedWindAmount * MAX_DISTANCE);
            
                _navMeshAgent.enabled = true;
                _navMeshAgent.speed = speed;
                _navMeshAgent.SetDestination(_destination);
            }
            
            _key.StartUnwinding(speed / MAX_SPEED);
        }

        private void StopRacing()
        {
            Debug.Log("Stop Racing");
            _navMeshAgent.enabled = false;
            _key.StopUnwinding();
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