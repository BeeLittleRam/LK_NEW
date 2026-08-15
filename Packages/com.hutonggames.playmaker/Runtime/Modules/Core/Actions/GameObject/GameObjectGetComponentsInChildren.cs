
using JetBrains.Annotations;
using HutongGames.Reflection;
using UnityEngine;
using UnityEngine.Serialization;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.GameObject)]
	[ActionDescription("Gets references to all components of type T on the specified GameObject, and any " +
		"child of the GameObject.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/GameObject.GetComponentsInChildren.html")]
	public sealed class GameObjectGetComponentsInChildren : BaseAction
	{
		
		[Tooltip("The GameObject.")]
		[SerializeField]
		private GameObjectVar _gameObject;
		
		[FormerlySerializedAs("_type")]
		[Tooltip("The type of component to search for.")]
		[SerializeField, BaseType(typeof(Component))]
		private TypeReference _componentType;
		
		[MatchType(nameof(_componentType))]
		[Tooltip("Store the result in Component List variable.")]
		[SerializeReference, WriteOnly]
		private IListVariableRef _result;
		
		public override bool CanExecute()
		{
			return CheckParameters(_gameObject, _componentType, _result);
		}
		
		public override void Execute()
		{
			_result.SetValue(_gameObject.Value.GetComponentsInChildren(_componentType.Type));
		}
		
		public override string GetSummary()
		{
			return "Get {_componentType} components in {_gameObject} and children -> {_result}";
		}
	}
}
