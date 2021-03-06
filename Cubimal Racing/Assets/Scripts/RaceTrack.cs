using System;
using System.Collections;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class RaceTrack : MonoBehaviour
    {
        private const float POSITION_SECONDS = 0.5f;
        private const float WIND_SECONDS = 1.0f;

        public enum RaceTrackState
        {
            Ready,
            Set,
            Racing
        }

        public Action OnCubimalAdded;
        public Action OnCubimalRemoved;
        
        [SerializeField] 
        private Transform _destination;

        private Cubimal _cubimal;
        public Cubimal Cubimal => _cubimal;

        private RaceTrackState _state = RaceTrackState.Ready;
        private Coroutine _positionSequence;

        private void OnTriggerEnter(Collider other)
        {
            if (_cubimal != null || _state != RaceTrackState.Ready)
                return;
            
            Cubimal cubimal = other.gameObject.GetComponentInParent<Cubimal>();
            if (cubimal != null && cubimal.CanRace())
            {
                _cubimal = cubimal;
                _cubimal.OnPickedUp += HandleCubimalPickedUp;
                _cubimal.OnStopped += HandleCubimalStopped;
                
                _positionSequence = StartCoroutine(PositionCubimalSequence());
            }
        }

        private void RemoveCubimal()
        {
            _state = RaceTrackState.Ready;
            
            _cubimal.OnPickedUp -= HandleCubimalPickedUp;
            _cubimal.OnStopped -= HandleCubimalStopped;
            _cubimal = null;
            
            OnCubimalRemoved?.Invoke();
        }

        private void HandleCubimalStopped()
        {
            RemoveCubimal();
        }

        private IEnumerator PositionCubimalSequence()
        {
            _cubimal.PositionForRace();
            
            float seconds = 0f;
            while (seconds < POSITION_SECONDS)
            {
                float ratio = seconds / POSITION_SECONDS;
                _cubimal.transform.position = Vector3.Lerp(_cubimal.transform.position, transform.position, ratio);
                _cubimal.transform.rotation = Quaternion.Slerp(_cubimal.transform.rotation, transform.rotation, ratio);

                seconds += Time.deltaTime;
                
                yield return null;
            }

            if (_cubimal != null)
            {
                if (!_cubimal.IsWound)
                {
                    yield return _cubimal.AutoWind(WIND_SECONDS);
                }
            }

            _state = RaceTrackState.Set;
            
            if (_cubimal != null)
                OnCubimalAdded?.Invoke();

            _positionSequence = null;
        }

        private void HandleCubimalPickedUp()
        {
            if (_positionSequence != null)
            {
                StopCoroutine(_positionSequence);
                _positionSequence = null;
            }

            RemoveCubimal();
        }

        public bool IsOccupied()
        {
            return (_state == RaceTrackState.Set || _state == RaceTrackState.Racing) && _cubimal != null;
        }

        public void StartRace()
        {
            _state = RaceTrackState.Racing;
            
            if (_cubimal != null)
            {
                _cubimal.StartRace(_destination.position);
            }
        }

        public void RaceFinished(bool didWin)
        {
            if (_cubimal != null)
            {
                // This removes the cubimal via events
                _cubimal.FinishRace(didWin);
            }
        }
    }
}