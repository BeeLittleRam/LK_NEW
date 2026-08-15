using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.GameplayTargetingGameObject)]
	[ActionDescription("Finds the closest GameObject.")]
	[HelpURL("actions/gameobject-actions/query/game-object-find-closest/")]
	public sealed class GameObjectFindClosest : BaseAction
	{
		[Tooltip("The GameObject to measure from.")]
		[SerializeField, OwnerDefaultValue]
		private GameObjectVar _gameObject;
		
		[Tooltip("Exclude GameObjects further than this distance.")]
		[SerializeField, DefaultValue(1000f)]
		private FloatVar _maxDistance;

		[Tooltip("Exclude children from the search.")]
		[SerializeField]
		private BoolVar _excludeChildren;
		
		[Tooltip("Include inactive GameObjects in the search.")]
		[SerializeField]
		private BoolVar _includeInactive;
		
		[Tooltip("Store the result in GameObject variable (null if none found).")]
		[SerializeField]
		[WriteOnly]
		private GameObjectRef _result;
		
		public override bool CanExecute() => 
			CheckParameters(_gameObject, _maxDistance, _excludeChildren, _includeInactive, _result);

		public override void Execute()
		{
			var myTransform = _gameObject.Value.transform;
			GameObject closestGameObject = null;
			var maxDistance = _maxDistance.Value;
			var closestDistance = maxDistance * maxDistance;
			
			var include = _includeInactive.Value 
				? FindObjectsInactive.Include 
				: FindObjectsInactive.Exclude;
			var allObjects = Internal.CompatibilityShims.FindObjectsByTypeShim<GameObject>(include);
			foreach (var go in allObjects) 
			{
				if (go == _gameObject.Value) continue;
				if (_excludeChildren.Value && go.transform.IsChildOf(myTransform)) continue;

				var distance = (go.transform.position - myTransform.position).sqrMagnitude;
				if (!(distance < closestDistance)) continue;
				
				closestGameObject = go;
				closestDistance = distance;
			}
			
			_result.Value = closestGameObject;
		}
		
		public override string GetSummary() => "Find closest object to {_gameObject} -> {_result}";
	}
}
