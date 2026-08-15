
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Physics2DQueries)]
	[ActionDescription("Get a list of all Colliders that fall within a circular area.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics2D.OverlapCircle.html")]
	public sealed class Physics2DOverlapCircleAll : BaseAction
	{
		
		[Tooltip("The center of the circle.")]
		[SerializeField]
		private Vector2Var _point;
		
		[Tooltip("The radius of the circle.")]
		[SerializeField]
		private FloatVar _radius;
		
		[Tooltip("The contact filter used to filter the results differently, such as by layer mask," +
		         " Z depth.  Note that normal angle is not used for overlap testing.")]
		[SerializeField]
		private ContactFilter2DVar _contactFilter;
		
		[Tooltip("List to receive results.")]
		[SerializeField]
		private Collider2DListRef _results;
		
		[OptionalField]
		[Tooltip("Store the number of results in Integer variable.")]
		[SerializeField]
		[WriteOnly]
		private IntegerRef _resultCount;
		
		public override bool CanExecute() => 
			CheckParameters(_point, _radius, _results, _contactFilter);

		public override void Execute()
		{
			var resultCount = Physics2D.OverlapCircle(
				_point.Value, _radius.Value,_contactFilter.Value, _results.Values );
			if (_resultCount.IsAssigned) _resultCount.Value = resultCount;
		}


		public override string GetSummary() => 
			"Physics2D Overlap Circle Non Alloc: {_point} {_radius} -> {_results} -> {_resultCount}";
	}
}
