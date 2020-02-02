using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class Music : MonoBehaviour
    {
        [SerializeField] 
        private AudioClip[] _musicTracks;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            
            PlayRandomTrack();
        }

        private void PlayRandomTrack()
        {
            _audioSource.PlayOneShot(_musicTracks[Random.Range(0, _musicTracks.Length)]);
        }

        private void Update()
        {
            if (!_audioSource.isPlaying)
            {
                PlayRandomTrack();
            }
        }
    }
}