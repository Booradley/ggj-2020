using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class SFX : MonoBehaviour
    {
        [SerializeField] 
        private AudioClip _openCubimalBoxSFX;

        private AudioSource _audioSource;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            CubimalBox.OnOpened += PlayOpenCubimalBox;
        }

        private void PlayOpenCubimalBox()
        {
            _audioSource.PlayOneShot(_openCubimalBoxSFX);
        }
    }
}