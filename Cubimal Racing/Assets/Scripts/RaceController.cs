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
            Debug.Log("CUBIMAL ADDED");
            
            _raceTracksReady = 0;
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                if (raceTrack.IsOccupied())
                {
                    _raceTracksReady++;
                }
            }
            
            if (_state == RaceState.Ready && _raceTracksReady == _raceTracks.Length)
            {
                StartRace();
            }
        }

        private void HandleCubimalRemoved()
        {
            Debug.Log("CUBIMAL REMOVED");

            _raceTracksReady = 0;
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                if (raceTrack.IsOccupied())
                {
                    _raceTracksReady++;
                }
            }

            if (_raceTracksReady == 0)
            {
                Debug.Log("FINISH RACE");
                
                _state = RaceState.Ready;
            }
        }

        private void StartRace()
        {
            Debug.Log("START RACE");

            _state = RaceState.Racing;
            
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                raceTrack.StartRace();
            }
        }

        private void FinishRace(RaceTrack winningTrack)
        {
            Debug.Log("WIN RACE");
            
            _state = RaceState.Ready;
            
            foreach (RaceTrack raceTrack in _raceTracks)
            {
                raceTrack.RaceFinished(raceTrack == winningTrack);
            }
            
            if (_raceTracksReady == _raceTracks.Length)
            {
                StartRace();
            }
            
            OnRaceFinished?.Invoke();
        }
    }
}