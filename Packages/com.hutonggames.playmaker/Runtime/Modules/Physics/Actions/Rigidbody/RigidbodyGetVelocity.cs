
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ActionDescription("Get the velocity vector of the rigidbody.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html")]
	public sealed class RigidbodyGetVelocity : BaseAction
	{
		
		[Tooltip("The Rigidbody")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Get Rigidbody Velocity")]
		[SerializeField]
		[WriteOnly]
		private Vector3Ref _getVelocity;
		
		public override bool CanExecute()
		{
			return CheckParameters(_rigidbody, _getVelocity);
		}
		
		public override void Execute()
		{
#if UNITY_6000_0_OR_NEWER
			_getVelocity.Value = _rigidbody.Value.linearVelocity;
#else
			_getVelocity.Value = _rigidbody.Value.velocity;
#endif
		}
		
		public override string GetSummary()
		{
			return "Get {_rigidbody} velocity -> {_getVelocity}";
		}
	}
}

