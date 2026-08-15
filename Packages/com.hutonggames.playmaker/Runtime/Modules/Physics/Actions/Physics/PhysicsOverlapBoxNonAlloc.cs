
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.PhysicsQueries)]
	[ConvertibleGroup("PhysicsOverlap")]
	[ActionDescription("Find all colliders touching or inside of the given box, and store them into the buffer.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics.OverlapBoxNonAlloc.html")]
	public sealed class PhysicsOverlapBoxNonAlloc : BaseAction
	{
		
		[Tooltip("Center of the box.")]
		[SerializeField]
		private Vector3Var _center;
		
		[Tooltip("Half of the size of the box in each dimension.")]
		[SerializeField]
		private Vector3Var _halfExtents;
		
		[Tooltip("A Layer mask defines which layers of colliders to include in the query.")]
		[SerializeField, DefaultValue("Physics.AllLayers")]
		private LayerMaskVar _layerMask;
		
		[Tooltip("The buffer to store the results in.")]
		[SerializeField]
		private ColliderListRef _results;
		
		[OptionalField]
		[Tooltip("Store the number of results in Integer variable.")]
		[SerializeField]
		[WriteOnly]
		private IntegerRef _resultCount;
		
		public override bool CanExecute()
		{
			return CheckParameters(_center, _halfExtents, _layerMask, _results);
		}
		
		public override void Execute()
		{
			_resultCount.Value = Physics.OverlapBoxNonAlloc(_center.Value, _halfExtents.Value, _results.Values, Quaternion.identity, _layerMask.Value);
		}
		
		public override string GetSummary()
		{
			return "Overlap box {_center} {_halfExtents} {_layerMask} -> {_results}";
		}
	}
}
