using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class CubimalBoxSpawner : MonoBehaviour
    {
        private const float RESPAWN_DISTANCE = 0.5f;
        
        [SerializeField]
        private GameObject _cubimalBoxPrefab;

        private CubimalBox _cubimalBox;

        private void Awake()
        {
            SpawnCubimalBox();
        }

        private void SpawnCubimalBox()
        {
            _cubimalBox = Instantiate(_cubimalBoxPrefab, transform.position, transform.rotation).GetComponent<CubimalBox>();
            _cubimalBox.Spawn();
        }

        private void Update()
        {
            if (Vector3.Distance(transform.position, _cubimalBox.transform.position) >= RESPAWN_DISTANCE)
            {
                SpawnCubimalBox();
            }
        }
    }
}