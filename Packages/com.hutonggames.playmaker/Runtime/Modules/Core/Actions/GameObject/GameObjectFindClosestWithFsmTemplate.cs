using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.GameplayTargetingGameObject)]
	[ActionDescription("Finds the closest GameObject with the given FSM Template. " +
	                   "Templates can be a useful way to add capabilities or properties to a GameObject. " +
	                   "You can then use this action to find the closest GameObject with a certain capability or property. " +
	                   "\n\nFor example, find the closest enemy with a TakeDamage template. " +
	                   "You can then target that GameObject or send a Hit global event to that component.")]
	[HelpURL("actions/gameobject-actions/query/game-object-find-closest-with-fsm-template/")]
	public sealed class GameObjectFindClosestWithFsmTemplate : BaseAction
	{
		[Tooltip("The GameObject to measure from.")]
		[SerializeField, OwnerDefaultValue]
		private GameObjectVar _gameObject;

		[Tooltip("The FSM Template to search for. This can be found in FSM Template Components.")]
		[SerializeField]
		private FsmTemplateVar _fsmTemplate;
		
		[Tooltip("Exclude GameObjects further than this distance.")]
		[SerializeField, DefaultValue(1000f)]
		private FloatVar _maxDistance;

		[Tooltip("Exclude children from the search.")]
		[SerializeField]
		private BoolVar _excludeChildren;
		
		[Tooltip("Include inactive GameObjects in the search.")]
		[SerializeField]
		private BoolVar _includeInactive;
		
		[ActionHeader("Result")]
		[Tooltip("Store the closest GameObject (or null if none found).")]
		[SerializeField, WriteOnly]
		private GameObjectRef _closest;

		[Tooltip("Store the FSM Component (or null if none found).")]
		[SerializeField, WriteOnly]
		private BaseFsmComponentRef _fsmTemplateComponent;
		
		public override bool CanExecute() => 
			CheckParameters(_gameObject, _closest, _maxDistance, _excludeChildren, _includeInactive, _fsmTemplate, _fsmTemplateComponent);

		public override void Execute()
		{
			var myTransform = _gameObject.Value.transform;
			GameObject closestGameObject = null;
			FsmTemplateComponent closestFsmTemplateComponent = null;
			var maxDistance = _maxDistance.Value;
			var closestDistance = maxDistance * maxDistance;
			
			var include = _includeInactive.Value ? FindObjectsInactive.Include : FindObjectsInactive.Exclude;
			var all = Internal.CompatibilityShims.FindObjectsByTypeShim<FsmTemplateComponent>(include);

			foreach (var fsmTemplateComponent in all)
			{
				var go = fsmTemplateComponent.gameObject;
				if (_excludeChildren.Value && go.transform.IsChildOf(myTransform)) continue;

				var distance = (go.transform.position - myTransform.position).sqrMagnitude;
				if (!(distance < closestDistance)) continue;
				
				closestGameObject = go;
				closestFsmTemplateComponent = fsmTemplateComponent;
				closestDistance = distance;
			}
			
			_closest.Value = closestGameObject;
			_fsmTemplateComponent.Value = closestFsmTemplateComponent;
		}
		
		public override string GetSummary() => "Find closest object to {_gameObject} with {_fsmTemplate} -> {_closest}";
	}
}
