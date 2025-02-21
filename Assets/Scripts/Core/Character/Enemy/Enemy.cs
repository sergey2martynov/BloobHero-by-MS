﻿using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Core.Character.Enemy
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private Health _health;
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private int _damage;
        [SerializeField] private int _dropeChance;
        [SerializeField] private SkinnedMeshRenderer _meshRenderer;
        [SerializeField] private EnemyMovementInput _movement;
        [SerializeField] private HealthBarFiller _healthBarFiller;
        [SerializeField] private AudioSource _deathSound;


        private SpawnerEnemies _spawnerEnemies;
        private KillCounter _killCounter;
        private ExperiencePool _experiencePool;
        private Camera _camera;
        private Color _color;
        private SpawnerBoss _spawnerBoss;
        private TimeCounter _timeCounter;

        public Health Health => _health;
        public bool IsDie { get; private set; }
        public EnemyType EnemyType => _enemyType;
        public int Damage => _damage;
        public Camera Camera => _camera;
        public SkinnedMeshRenderer MeshRenderer => _meshRenderer;

        public void Initialize(KillCounter killCounter, ExperiencePool pool, Camera camera, SpawnerBoss spawnerBoss,
            TimeCounter timeCounter, BloodSplatPool bloodSplat, int health, SpawnerEnemies spawnerEnemies)
        {
            _killCounter = killCounter;
            IsDie = false;
            _experiencePool = pool;
            _camera = camera;
            _spawnerBoss = spawnerBoss;
            _timeCounter = timeCounter;
            _health.BloodSplatPool = bloodSplat;
            _movement.ReturnSpeed();
            _spawnerEnemies = spawnerEnemies;
            if (_enemyType == EnemyType.Boss)
                _health.SetNewHealthPoint(health);
            _healthBarFiller.ReturnHealthBarValue();
            _healthBarFiller.gameObject.SetActive(false);
        }

        private void Start()
        {
            _color = _meshRenderer.material.color;
            _health.HealthIsOver += Die;
        }

        private void OnDestroy()
        {
            _health.HealthIsOver -= Die;
        }

        private void Die()
        {
            _deathSound.Play();
            IsDie = true;
            _killCounter.IncreaseCounter();

            if (_enemyType == EnemyType.Boss)
            {
                _spawnerBoss.SpawnedBosses--;
                Destroy(gameObject);

                if (_spawnerBoss.SpawnedBosses <= 0)
                {
                    _spawnerEnemies.RemoveAllEnemies();
                    
                    DOTween.Sequence().AppendInterval(2.5f).OnComplete(() =>
                    {
                        _timeCounter.UpdateWave();
                    });
                }
            }
            else
            {
                var calculatedChance = Random.Range(0, 100);

                if (calculatedChance < _dropeChance)
                {
                    SpawnObjOfExperience();
                }

                _spawnerEnemies.EnemyPools[(int) _enemyType].Release(gameObject);
                _spawnerEnemies.SpawnedEnemies.Remove(this);
            }
        }

        public void SetSpawnerEnemiesRef(SpawnerEnemies spawnerEnemies)
        {
            _spawnerEnemies = spawnerEnemies;
        }

        public void SpawnObjOfExperience()
        {
            GameObject objectOfExperience;

            objectOfExperience = _experiencePool.Get();
            objectOfExperience.transform.position = transform.position;
        }

        public void ReturnColor()
        {
            _meshRenderer.material.color = _color;
        }
    }
}