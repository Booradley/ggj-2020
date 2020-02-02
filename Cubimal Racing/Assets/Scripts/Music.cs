using System.Collections;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class Music : MonoBehaviour
    {
        private const float VOLUME = 0.1f;
        private const float VOLUME_FADED = 0.025f;
        
        [SerializeField] 
        private AudioClip[] _musicTracks;

        private AudioSource _audioSource;
        private Coroutine _fadeSequence;
        
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

        public void FadeOut(float seconds)
        {
            if (_fadeSequence != null)
            {
                StopCoroutine(_fadeSequence);
            }
            
            _fadeSequence = StartCoroutine(FadeOutSequence(seconds));
        }

        private IEnumerator FadeOutSequence(float seconds)
        {
            _audioSource.volume = VOLUME_FADED;

            yield return new WaitForSeconds(seconds);

            _audioSource.volume = VOLUME;
        }
    }
}