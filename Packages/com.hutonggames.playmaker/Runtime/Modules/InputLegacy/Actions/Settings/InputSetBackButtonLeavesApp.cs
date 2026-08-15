
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
	public sealed class InputSetBackButtonLeavesApp : BaseAction
	{
		
		[Tooltip("Set Input Back Button Leaves App")]
		[SerializeField]
		private BoolVar _setBackButtonLeavesApp;
		
		public override bool CanExecute() => CheckParameters(_setBackButtonLeavesApp);

		public override void Execute() => Input.backButtonLeavesApp = _setBackButtonLeavesApp.Value;

		public override string GetSummary() => "Set InputBackButtonLeavesApp to {_setBackButtonLeavesApp}";
	}
}
