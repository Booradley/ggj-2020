using UnityEngine;

namespace EmeraldActivities.CubimalRacing
{
    public class CubimalBoxSpawner : MonoBehaviour
    {
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
    }
}