
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Camera)]
	[ActionDescription("Fills an array of Camera with the current cameras in the Scene, without allocating a new array.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Camera.GetAllCameras.html")]
	public sealed class CameraGetAllCameras__NonAlloc : BaseAction
	{
		
		[Tooltip("An array to be filled up with cameras currently in the Scene.")]
		[SerializeField]
		private CameraListRef _cameras;
		
		[Tooltip("Store the number of cameras found in Integer variable.")]
		[SerializeField]
		[WriteOnly]
		private IntegerRef _numCameras;
		
		public override bool CanExecute()
		{
			return CheckParameters(_cameras, _numCameras);
		}
		
		public override void Execute()
		{
			//UnityEngine.Camera.GetAllCameras(UnityEngine.Camera[]);
			_numCameras.Value = Camera.GetAllCameras(_cameras.Values);
		}
		
		public override string GetSummary()
		{
			return "Get all cameras into {_cameras} ({_numCameras})";
		}
	}
}
