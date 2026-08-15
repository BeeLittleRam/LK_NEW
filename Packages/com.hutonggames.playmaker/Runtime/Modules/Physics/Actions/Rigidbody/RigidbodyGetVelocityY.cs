
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ActionDescription("Get the Y velocity of the rigidbody.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html")]
	public sealed class RigidbodyGetVelocityY : BaseAction
	{
		
		[Tooltip("The Rigidbody")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Get Rigidbody Velocity in Y")]
		[SerializeField, WriteOnly]
		private FloatRef _getVelocityY;
		
		public override bool CanExecute() => CheckParameters(_rigidbody, _getVelocityY);

		public override void Execute()
		{
#if UNITY_6000_0_OR_NEWER
			var velocity = _rigidbody.Value.linearVelocity;
#else
			var velocity = _rigidbody.Value.velocity;
#endif
			_getVelocityY.Value = velocity.y;
		}
		
		public override string GetSummary()
		{
			return "Get {_rigidbody} velocity Y -> {_getVelocityY}";
		}
	}
}

