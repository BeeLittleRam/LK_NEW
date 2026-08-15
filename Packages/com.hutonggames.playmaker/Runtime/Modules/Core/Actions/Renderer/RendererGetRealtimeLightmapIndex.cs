
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Renderer)]
	[ActionDescription("The index of the real-time lightmap applied to this renderer.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Renderer-realtimeLightmapIndex.html")]
	public sealed class RendererGetRealtimeLightmapIndex : BaseAction
	{
		
		[Tooltip("The Renderer")]
		[SerializeField]
		private RendererVar _renderer;
		
		[Tooltip("Get Renderer Realtime Lightmap Index")]
		[SerializeField]
		[WriteOnly]
		private IntegerRef _getRealtimeLightmapIndex;
		
		public override bool CanExecute()
		{
			return CheckParameters(_renderer, _getRealtimeLightmapIndex);
		}
		
		public override void Execute()
		{
			_getRealtimeLightmapIndex.Value = _renderer.Value.realtimeLightmapIndex;
		}
		
		public override string GetSummary()
		{
			return "Get {_renderer} realtimeLightmapIndex -> {_getRealtimeLightmapIndex}";
		}
	}
}
