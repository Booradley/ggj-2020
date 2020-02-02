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
                
                _positionSequence = StartCoroutine(PositionCubimalSequence());
            }
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
            
            _cubimal.OnPickedUp -= HandleCubimalPickedUp;
            _cubimal = null;
            
            OnCubimalRemoved?.Invoke();
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
            _state = RaceTrackState.Ready;

            if (_cubimal != null)
            {
                _cubimal.FinishRace(didWin);
                _cubimal.OnPickedUp -= HandleCubimalPickedUp;
                _cubimal = null;
            }
        }
    }
}