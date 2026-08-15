
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.InputSettings)]
	[ActionDescription("Should Back button quit the application? Only usable on Android, Windows Phone or" +
		" Windows Tablets.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Input-backButtonLeavesApp.html")]
	public sealed class InputGetBackButtonLeavesApp : BaseAction
	{
		
		[Tooltip("Get Input Back Button Leaves App")]
		[SerializeField]
		[WriteOnly]
		private BoolRef _getBackButtonLeavesApp;
		
		public override bool CanExecute() => CheckParameters(_getBackButtonLeavesApp);

		public override void Execute() => _getBackButtonLeavesApp.Value = Input.backButtonLeavesApp;

		public override string GetSummary() => "Get BackButtonLeavesApp -> {_getBackButtonLeavesApp} ";
	}
}
