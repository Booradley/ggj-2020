using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class SFX : MonoBehaviour
    {
        [SerializeField] 
        private AudioClip _openCubimalBoxSFX;
        
        [SerializeField] 
        private AudioClip _winRaceSFX;
        
        [SerializeField] 
        private AudioClip _outOfBoundsSFX;

        [SerializeField] 
        private Music _music;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            CubimalBox.OnOpened += PlayOpenCubimalBox;
            RaceController.OnRaceFinished += PlayRaceFinished;
            Destroyable.OnOutOfBounds += destroyable => PlayOutOfBounds();
        }

        private void PlayOpenCubimalBox()
        {
            _music.FadeOut(_openCubimalBoxSFX.length);
            _audioSource.PlayOneShot(_openCubimalBoxSFX);
        }

        private void PlayRaceFinished()
        {
            _music.FadeOut(_winRaceSFX.length);
            _audioSource.PlayOneShot(_winRaceSFX);
        }

        private void PlayOutOfBounds()
        {
            _music.FadeOut(_outOfBoundsSFX.length);
            _audioSource.PlayOneShot(_outOfBoundsSFX);
        }
    }
}