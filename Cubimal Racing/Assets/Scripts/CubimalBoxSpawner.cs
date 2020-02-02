using System.Collections;
using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class CubimalBoxSpawner : MonoBehaviour
    {
        private const float RESPAWN_DISTANCE = 0.5f;
        private const float RESPAWN_DELAY_SECONDS = 0.5f;
        
        [SerializeField]
        private GameObject _cubimalBoxPrefab;

        private bool _isSpawning = false;
        private CubimalBox _cubimalBox;

        private void Awake()
        {
            SpawnCubimalBox();
        }

        private void SpawnCubimalBox()
        {
            if (!_isSpawning)
            {
                _isSpawning = true;

                StartCoroutine(SpawnBoxSequence());
            }
        }

        private IEnumerator SpawnBoxSequence()
        {
            yield return new WaitForSeconds(RESPAWN_DELAY_SECONDS);
            
            _cubimalBox = Instantiate(_cubimalBoxPrefab, transform.position, transform.rotation).GetComponent<CubimalBox>();
            _cubimalBox.Spawn();

            _isSpawning = false;
        }

        private void Update()
        {
            if (_cubimalBox != null)
            {
                if (Vector3.Distance(transform.position, _cubimalBox.transform.position) >= RESPAWN_DISTANCE)
                {
                    SpawnCubimalBox();
                }
            }
            else
            {
                SpawnCubimalBox();
            }
        }
    }
}