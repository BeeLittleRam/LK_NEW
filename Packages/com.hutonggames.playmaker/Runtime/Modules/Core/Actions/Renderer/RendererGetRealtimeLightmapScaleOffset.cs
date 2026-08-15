
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Renderer)]
	[ActionDescription("The UV scale & offset used for a real-time lightmap.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Renderer-realtimeLightmapScaleOffset.htm" +
		"l")]
	public sealed class RendererGetRealtimeLightmapScaleOffset : BaseAction
	{
		
		[Tooltip("The Renderer")]
		[SerializeField]
		private RendererVar _renderer;
		
		[Tooltip("Get Renderer Realtime Lightmap Scale Offset")]
		[SerializeField]
		[WriteOnly]
		private Vector4Ref _getRealtimeLightmapScaleOffset;
		
		public override bool CanExecute()
		{
			return CheckParameters(_renderer, _getRealtimeLightmapScaleOffset);
		}
		
		public override void Execute()
		{
			_getRealtimeLightmapScaleOffset.Value = _renderer.Value.realtimeLightmapScaleOffset;
		}
		
		public override string GetSummary()
		{
			return "Get {_renderer} realtimeLightmapScaleOffset -> {_getRealtimeLightmapScaleOffset}";
		}
	}
}
