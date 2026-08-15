
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Rigidbody)]
	[ActionDescription("Get the X velocity of the rigidbody.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Rigidbody-linearVelocity.html")]
	public sealed class RigidbodyGetVelocityX : BaseAction
	{
		
		[Tooltip("The Rigidbody")]
		[SerializeField]
		private RigidbodyVar _rigidbody;
		
		[Tooltip("Get Rigidbody Velocity in X")]
		[SerializeField, WriteOnly]
		private FloatRef _getVelocityX;
		
		public override bool CanExecute() => CheckParameters(_rigidbody, _getVelocityX);

		public override void Execute()
		{
#if UNITY_6000_0_OR_NEWER
			var velocity = _rigidbody.Value.linearVelocity;
#else
			var velocity = _rigidbody.Value.velocity;
#endif
			_getVelocityX.Value = velocity.x;
		}
		
		public override string GetSummary()
		{
			return "Get {_rigidbody} velocity X -> {_getVelocityX}";
		}
	}
}

