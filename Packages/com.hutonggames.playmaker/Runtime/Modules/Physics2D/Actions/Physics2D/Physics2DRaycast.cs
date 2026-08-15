
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Physics2DQueries)]
	[ActionDescription("Casts a ray against Colliders in the Scene, returning the first Collider that contacts with it.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics2D.Raycast.html")]
	public sealed class Physics2DRaycast : BaseAction
	{
		
		[Tooltip("The point in 2D space where the ray originates.")]
		[SerializeField]
		private Vector2Var _origin;
		
		[Tooltip("A vector representing the direction of the ray.")]
		[SerializeField]
		private Vector2Var _direction;
		
		[Tooltip("The maximum distance over which to cast the ray.")]
		[SerializeField]
		[DefaultValue("~MathfInfinity")]
		private FloatVar _maxDistance;
		
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
		[Tooltip("The GameObject hit by the raycast.")]
		[SerializeField, OptionalField]
		private GameObjectRef _gameObjectHit;
		
		[WriteOnly]
		[Tooltip("Store the results in RaycastHit2D variable.")]
		[SerializeField, OptionalField]
		private RaycastHit2DRef _result;
		
		public override bool CanExecute() => 
			CheckParameters(_origin, _direction, _maxDistance, _layerMask, _minDepth, _maxDepth);

		public override string ErrorCheck()
		{
			return _result.IsAssigned || _gameObjectHit.IsAssigned
				? string.Empty
				: "Specify at least one output: RaycastHit2D or GameObject.";
		}

		public override void Execute()
		{
			var result = Physics2D.Raycast(
				_origin.Value, _direction.Value, _maxDistance.Value, _layerMask.Value, _minDepth.Value, _maxDepth.Value);

			if (_result.IsAssigned)
			{
				_result.Value = result;
			}

			if (_gameObjectHit.IsAssigned)
			{
				_gameObjectHit.Value = result.collider ? result.collider.gameObject : null;
			}
		}
		
		public override string GetSummary() => "Physics2D Raycast from {_origin} direction {_direction} -> {_result} {_gameObjectHit:output}";
	}
}
