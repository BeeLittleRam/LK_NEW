
using JetBrains.Annotations;
using System;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.PhysicsQueries)]
	[ActionDescription("Performs a linecast between start and end. Optionally stores whether anything was hit and information about the hit.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics.Linecast.html")]
	public sealed class PhysicsLinecast : BaseAction
	{
		
		[Tooltip("Start point.")]
		[SerializeField]
		private Vector3Var _start;
		
		[Tooltip("End point.")]
		[SerializeField]
		private Vector3Var _end;
		
		[Tooltip("A Layer mask that is used to selectively ignore colliders when casting a ray.")]
		[SerializeField]
		[DefaultValue("Physics.DefaultRaycastLayers")]
		private LayerMaskVar _layerMask;
		
		[Tooltip("Specifies whether this query should hit Triggers.")]
		[DefaultValue(QueryTriggerInteraction.UseGlobal)]
		[SerializeField]
		private QueryTriggerInteraction _hitTriggers;
		
		[Tooltip("Store the result in Bool variable.")]
		[SerializeField]
		[OptionalField]
		[WriteOnly]
		private BoolRef _result;

		[Tooltip("Store hit information from the linecast.")]
		[SerializeField]
		[OptionalField]
		[WriteOnly]
		private RaycastHitRef _hitInfo;
		
		public override bool CanExecute()
		{
			return CheckParameters(_start, _end, _layerMask) &&
			       (_result.HasValue() || _hitInfo.HasValue());
		}
		
		public override void Execute()
		{
			var didHit = Physics.Linecast(_start.Value, _end.Value, out var hitInfo, _layerMask.Value, _hitTriggers);
			
			if (_result.HasValue())
			{
				_result.Value = didHit;
			}
			
			if (_hitInfo.HasValue())
			{
				_hitInfo.Value = hitInfo;
			}
		}
		
		public override string GetSummary()
		{
			return "Linecast from {_start} to {_end} using {_layerMask} {_result:output} {_hitInfo:output}";
		}
	}
}
