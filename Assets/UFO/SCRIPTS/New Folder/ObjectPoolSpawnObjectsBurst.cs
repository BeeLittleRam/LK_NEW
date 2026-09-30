using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [ActionCategory(Category.Pool)]
    [ConvertibleGroup(ConvertibleGroup.Instantiate)]
    [ActionDescription("Spawn multiple prefabs from a pool over a given burst duration, with a random Y rotation offset.")]
    public class ObjectPoolSpawnObjectsBurst : BaseWaitAction
    {
        [Tooltip("The prefab to spawn.")]
        [SerializeField]
        private GameObjectVar _prefab;

        [Tooltip("The total number of instances to spawn.")]
        [SerializeField]
        private IntegerVar _count;

        [Tooltip("The position where the objects will spawn.")]
        [SerializeField]
        private Vector3Var _startPosition;

        [Tooltip("The base rotation of the spawned objects.")]
        [SerializeField]
        private QuaternionVar _startRotation;

        [Tooltip("The total amount of time, in seconds, for the entire burst.")]
        [SerializeField]
        private FloatVar _burstDuration;

        [Tooltip("Random Y rotation range in degrees. Example: 5 = random Y rotation between -5 and +5 degrees.")]
        [SerializeField]
        private FloatVar _randomYRotation;

        [OptionalField]
        [Tooltip("Store the created GameObjects in a list.")]
        [SerializeField, WriteOnly]
        private GameObjectListRef _spawnedObjects;

        [OptionalField]
        [Tooltip("Set the parent of the created Objects.")]
        [SerializeField]
        public GameObjectVar _setParent;

        private int _spawnedCount;
        private float _spawnTimer;
        private float _spawnInterval;

        private List<GameObject> _spawnedObjectsList;

        public override bool CanExecute()
        {
            return CheckParameters(
                _prefab,
                _count,
                _startPosition,
                _startRotation,
                _burstDuration,
                _randomYRotation
            );
        }

        public override void OnStart()
        {
            _spawnedCount = 0;
            _spawnTimer = 0f;

            _spawnedObjectsList = new List<GameObject>(
                Mathf.Max(0, _count.Value)
            );

            // Nothing to spawn.
            if (_count.Value <= 0)
            {
                Finish();
                return;
            }

            // Only one object.
            if (_count.Value == 1)
            {
                SpawnObject();
                Finish();
                return;
            }

            // Spawn all objects immediately if duration is 0.
            if (_burstDuration.Value <= 0f)
            {
                while (_spawnedCount < _count.Value)
                {
                    SpawnObject();
                }

                Finish();
                return;
            }

            // First object spawns immediately.
            SpawnObject();

            // Spread the remaining objects across the burst duration.
            //
            // Example:
            // Count = 10
            // Duration = 0.5 seconds
            //
            // Object 1 = 0.00
            // Object 2 = 0.055
            // Object 3 = 0.111
            // ...
            // Object 10 = 0.50
            _spawnInterval = _burstDuration.Value / (_count.Value - 1);
        }

        public override void Execute()
        {
            if (_spawnedCount >= _count.Value)
            {
                Finish();
                return;
            }

            _spawnTimer += Time.deltaTime;

            while (_spawnTimer >= _spawnInterval &&
                   _spawnedCount < _count.Value)
            {
                _spawnTimer -= _spawnInterval;

                SpawnObject();
            }

            // Update progress for PlayMaker.
            if (_burstDuration.Value > 0f)
            {
                Progress = Mathf.Clamp01(
                    (_spawnedCount - 1) * _spawnInterval / _burstDuration.Value
                );
            }

            if (_spawnedCount >= _count.Value)
            {
                Progress = 1f;
                Finish();
            }
        }

        private void SpawnObject()
        {
            if (_spawnedCount >= _count.Value)
                return;

            // Random Y rotation.
            float randomY = UnityEngine.Random.Range(
                -_randomYRotation.Value,
                _randomYRotation.Value
            );

            Quaternion spawnRotation =
                _startRotation.Value *
                Quaternion.Euler(0f, randomY, 0f);

            GameObject spawnedObject =
                ObjectPoolManager.SpawnObject(
                    _prefab.Value,
                    _startPosition.Value,
                    spawnRotation
                );

            if (spawnedObject == null)
                return;

            spawnedObject.transform.SetPositionAndRotation(
                _startPosition.Value,
                spawnRotation
            );

            if (_setParent.HasValue())
            {
                spawnedObject.transform.SetParent(
                    _setParent.Value.transform
                );
            }

            spawnedObject.SetActive(true);

            _spawnedObjectsList.Add(spawnedObject);

            _spawnedCount++;

            if (_spawnedObjects.IsAssigned)
            {
                _spawnedObjects.Value = _spawnedObjectsList;
            }
        }

        public override void OnStop()
        {
            _spawnedObjectsList = null;
        }

        public override string GetSummary()
        {
            return "Burst spawn {_count} {_prefab} over {_burstDuration}s " +
                   "at {_startPosition}, Y ±{_randomYRotation}° " +
                   "{_spawnedObjects:output}";
        }
    }
}