
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Physics2DQueries)]
	[ActionDescription("Casts a line against Colliders in the Scene.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics2D.Linecast.html")]
	public sealed class Physics2DLinecast : BaseAction
	{
		
		[Tooltip("The start point of the line in world space.")]
		[SerializeField]
		private Vector2Var _start;
		
		[Tooltip("The end point of the line in world space.")]
		[SerializeField]
		private Vector2Var _end;
		
		[ActionHeader("Filter")]
		
		[Tooltip("Filter to check objects only on specific layers.")]
		[SerializeField]
		[DefaultValue("~Physics2DDefaultRaycastLayers")]
		private LayerMaskVar _layerMask;

		[Tooltip("Only include objects with a Z coordinate (depth) greater than or equal to this value.")]
		[SerializeField]
		[DefaultValue("~FloatNegativeInfinity")]
		private FloatVar _minDepth;
		
		[Tooltip("Only include objects with a Z coordinate (depth) less than or equal to this value.")]
		[SerializeField]
		[DefaultValue("~FloatPositiveInfinity")]
		private FloatVar _maxDepth;
		
		[ActionHeader("Results")]
		
		[WriteOnly]
		[Tooltip("The GameObject hit by the linecast.")]
		[SerializeField, OptionalField]
		private GameObjectRef _gameObjectHit;
		
		[Tooltip("Store the result in RaycastHit2D List variable.")]
		[SerializeField]
		[WriteOnly, OptionalField]
		private RaycastHit2DRef _result;
		
		public override bool CanExecute()
		{
			return CheckParameters(_start, _end, _layerMask, _minDepth, _maxDepth);
		}

		public override string ErrorCheck()
		{
			return _result.IsAssigned || _gameObjectHit.IsAssigned
				? string.Empty
				: "Specify at least one output: RaycastHit2D or GameObject.";
		}
		
		public override void Execute()
		{
			var result = Physics2D.Linecast(_start.Value, _end.Value, _layerMask.Value, _minDepth.Value, _maxDepth.Value);

			if (_result.IsAssigned)
			{
				_result.Value = result;
			}

			if (_gameObjectHit.IsAssigned)
			{
				_gameObjectHit.Value = result.collider ? result.collider.gameObject : null;
			}
		}
		
		public override string GetSummary()
		{
			return "Linecast {_start} to {_end} ({_layerMask}) {_result:output} {_gameObjectHit:output}";
		}
	}
}
