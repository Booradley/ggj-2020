using System;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class RaceController : MonoBehaviour
    {
        public static Action OnRaceFinished;
        
        public enum RaceState
        {
            Ready,
            Racing
        }
        
        [SerializeField]
        private RaceTrack[] _raceTracks;

        private RaceState _state = RaceState.Ready;
        private int _raceTracksReady = 0;

        private void Awake()
        {
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                raceTrack.OnCubimalAdded += HandleCubimalAdded;
                raceTrack.OnCubimalRemoved += HandleCubimalRemoved;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_state != RaceState.Racing)
                return;
            
            Cubimal cubimal = other.gameObject.GetComponentInParent<Cubimal>();
            if (cubimal != null)
            {
                foreach (RaceTrack raceTrack in _raceTracks)
                {
                    if (raceTrack.Cubimal == cubimal)
                    {
                        FinishRace(raceTrack);
                        return;
                    }
                }
            }
        }

        private void HandleCubimalAdded()
        {
            if (_state != RaceState.Ready)
                return;
            
            _raceTracksReady++;
            if (_raceTracksReady == _raceTracks.Length)
            {
                StartRace();
            }
        }

        private void HandleCubimalRemoved()
        {
            _raceTracksReady--;

            if (_raceTracksReady == 0 && _state == RaceState.Racing)
            {
                _state = RaceState.Ready;
                _raceTracksReady = 0;
            }
        }

        private void StartRace()
        {
            _state = RaceState.Racing;
            
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                raceTrack.StartRace();
            }
        }

        private void FinishRace(RaceTrack winningTrack)
        {
            _state = RaceState.Ready;
            _raceTracksReady = 0;
            
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                raceTrack.RaceFinished(raceTrack == winningTrack);
            }
            
            OnRaceFinished?.Invoke();
        }
    }
}