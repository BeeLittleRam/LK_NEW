using System;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable, PublicAPI]
    [ActionCategory(Category.GameplayTargetingGameObject)]
    [ConvertibleGroup("GameObjectData")]
    [ActionDescription("Finds the closest GameObject with a matching Data Component (DataDefinition).")]
    [HelpURL("actions/gameobject-actions/query/game-object-find-closest-with-data/")]
    public sealed class GameObjectFindClosestWithData : BaseAction
    {
        [Tooltip("The GameObject to measure from.")]
        public GameObjectVar GameObject;

        [RequiredField]
        [Tooltip("The DataDefinition to search for. Used to find matching Data Components.")]
        public DataDefinition DataDefinition;

        [DefaultValue(1000f)]
        [Tooltip("Exclude GameObjects further than this distance.")]
        public FloatVar MaxDistance;

        [Tooltip("Exclude children from the search.")]
        public BoolVar ExcludeChildren;

        [Tooltip("Include inactive GameObjects in the search.")]
        public BoolVar IncludeInactive;

        [ActionHeader("Result")]

        [WriteOnly]
        [Tooltip("Store the closest GameObject (or null if none found).")]
        public GameObjectRef Closest;

        [OptionalField, WriteOnly]
        [Tooltip("Store the matching Data Component (or null if none found).")]
        public DataRecordComponentRef Component;

        public override void Execute()
        {
            var from = GameObject.Value;
            if (from == null)
            {
                if (Closest.IsAssigned) Closest.Value = null;
                if (Component.IsAssigned) Component.Value = null;
                return;
            }

            var def = DataDefinition;
            if (def == null)
            {
                if (Closest.IsAssigned) Closest.Value = null;
                if (Component.IsAssigned) Component.Value = null;
                return;
            }

            var myTransform = from.transform;
            var maxDistance = MaxDistance.Value;
            var closestDistance = maxDistance * maxDistance;

            GameObject closestGameObject = null;
            DataRecordComponent closestComponent = null;

            var include = IncludeInactive.Value ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
            var all = Internal.CompatibilityShims.FindObjectsByTypeShim<DataRecordComponent>(include);

            foreach (var dataComponent in all)
            {
                if (dataComponent == null) continue;

                var record = dataComponent.Data;
                if (record == null || record.DataDefinition != def) continue;

                var go = dataComponent.gameObject;
                if (ExcludeChildren.Value && go.transform.IsChildOf(myTransform)) continue;

                var distance = (go.transform.position - myTransform.position).sqrMagnitude;
                if (!(distance < closestDistance)) continue;

                closestGameObject = go;
                closestComponent = dataComponent;
                closestDistance = distance;
            }

            if (Closest.IsAssigned) Closest.Value = closestGameObject;
            if (Component.IsAssigned) Component.Value = closestComponent;
        }

        public override string GetSummary()
        {
            return "Find closest object to {GameObject} with {DataDefinition} -> {Closest}";
        }
    }
}
