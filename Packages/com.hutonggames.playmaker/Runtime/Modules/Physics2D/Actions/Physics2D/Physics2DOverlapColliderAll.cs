
using JetBrains.Annotations;
using UnityEngine;


namespace HutongGames.PlayMaker.Actions
{
	
	
	[System.Serializable]
	[PublicAPI]
	[ActionCategory(Category.Physics2DQueries)]
	[ActionDescription("Gets a list of all Colliders that overlap the given Collider.")]
	[HelpURL("https://docs.unity3d.com/ScriptReference/Physics2D.OverlapCollider.html")]
	public sealed class Physics2DOverlapColliderAll : BaseAction
	{
		
		[Tooltip("Collider.")]
		[SerializeField]
		private Collider2DVar _collider;
		
		[Tooltip("The contact filter used to filter the results differently, such as by layer mask," +
			" Z depth.  Note that normal angle is not used for overlap testing.")]
		[SerializeField]
		private ContactFilter2DVar _contactFilter;
		
		[Tooltip("The list to receive results.  The size of the array determines the maximum numbe" +
			"r of results that can be returned.")]
		[SerializeField]
		private Collider2DListRef _results;
		
		[OptionalField]
		[Tooltip("Store the number of results in Integer variable.")]
		[SerializeField]
		[WriteOnly]
		private IntegerRef _resultCount;
		
		public override bool CanExecute() => CheckParameters(_collider, _contactFilter, _results);

		public override void Execute()
		{
			var resultCount = Physics2D.OverlapCollider(_collider.Value, _contactFilter.Value, _results.Values);
			if (_resultCount.IsAssigned)
			{
				_resultCount.Value = resultCount;
			}
		}
		
		public override string GetSummary() => 
			"Physics2D Overlap Collider: {_collider} {_contactFilter} -> {_results} -> {_resultCount}";
	}
}
