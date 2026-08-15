
using JetBrains.Annotations;
using HutongGames.Reflection;
using UnityEngine;
using UnityEngine.Serialization;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.GameObject)]
	[ConvertibleGroup("GetComponent")]
	[ActionDescription("Gets a reference to a component of type T on the specified GameObject, or any parent of the GameObject.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/GameObject.GetComponentInParent.html")]
	public sealed class GameObjectGetComponentInParent : BaseAction
	{
		
		[Tooltip("The GameObject.")]
		[SerializeField]
		private GameObjectVar _gameObject;
		
		[FormerlySerializedAs("_type")]
		[Tooltip("The type of component to search for.")]
		[SerializeField]
		private TypeReference _componentType;
		
		[Tooltip("Whether to include inactive parent GameObjects in the search.")]
		[SerializeField]
		private BoolVar _includeInactive;
		
		[Tooltip("Store the result in Component variable.")]
		[SerializeField]
		[WriteOnly]
		private ComponentRef _result;
		
		public override bool CanExecute()
		{
			return CheckParameter(_componentType) && CheckParameters(_gameObject, _includeInactive, _result);
		}
		
		public override void Execute()
		{
			_result.Value = _gameObject.Value.GetComponentInParent(_componentType.Type, _includeInactive.Value);
		}
		
		public override string GetSummary() =>
			"Get {_componentType} component in {_gameObject} parent -> {_result}" + 
			(_includeInactive.Value ? " (including inactive)" : "");
	}
}
