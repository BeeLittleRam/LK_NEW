
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ActionDescription("Get the z velocity of the rigidbody.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html")]
	public sealed class RigidbodyGetVelocityZ : BaseAction
	{
		
		[Tooltip("The Rigidbody")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Get Rigidbody Velocity in Z")]
		[SerializeField, WriteOnly]
		private FloatRef _getVelocityZ;
		
		public override bool CanExecute() => CheckParameters(_rigidbody, _getVelocityZ);

		public override void Execute()
		{
#if UNITY_6000_0_OR_NEWER
			var velocity = _rigidbody.Value.linearVelocity;
#else
			var velocity = _rigidbody.Value.velocity;
#endif
			_getVelocityZ.Value = velocity.z;
		}
		
		public override string GetSummary()
		{
			return "Get {_rigidbody} velocity Z -> {_getVelocityZ}";
		}
	}
}

