using Core.Environment;
using UnityEngine;

public class SpawnerСhest : MonoBehaviour
{
    [SerializeField] private Chest _chest;
    [SerializeField] private Transform _player;
    [SerializeField] private Transform _parent;
    [SerializeField] private SpawnObjectOfExperience _spawnObjectOfExperience;
    [SerializeField] private SpawnerHealthBox _spawnerHealthBox;

    private void Start()
    {
        SpawnChest();
    }

    public void SpawnChest()
    {
        var obstacle = Instantiate(_chest,
            RandomPositionFinder.FindRandomPosition(transform, _player, -41, 15, -17, 17).position,
            Quaternion.Euler(0, Random.Range(0, 360), 0), _parent);
        obstacle.Initialize(_spawnObjectOfExperience, this, _spawnerHealthBox);
    }

    public void ChangePositionChest(Chest chest)
    {
        chest.transform.position =
            RandomPositionFinder.FindRandomPosition(transform, _player, -41, 15, -17, 17).position;
    }
}
