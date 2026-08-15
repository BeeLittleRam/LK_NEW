
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.GameObject)]
	[ActionDescription("Returns one active GameObject tagged tag. Returns null if no GameObject was found.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/GameObject.FindWithTag.html")]
	public sealed class GameObjectFindWithTag : BaseAction
	{
		
		[Tooltip("The tag to search for.")]
		[SerializeField]
		private StringVar _tag;
		
		[Tooltip("Store the result in GameObject variable.")]
		[SerializeField]
		[WriteOnly]
		private GameObjectRef _result;
		
		public override bool CanExecute()
		{
			return CheckParameters(_tag, _result);
		}
		
		public override void Execute()
		{
			_result.Value = GameObject.FindWithTag(_tag.Value);
		}
		
		public override string GetSummary()
		{
			return "Find GameObject with {_tag} tag -> {_result}";
		}
	}
}
