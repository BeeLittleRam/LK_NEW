using HutongGames.Reflection;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.GameObject)]
    [ConvertibleGroup("GetComponent")]
    [ActionDescription("Get a Component on a GameObject, or any child of the GameObject.")]
    [HelpURL("https://docs.unity3d.com/ScriptReference/GameObject.GetComponentInChildren.html")]
    public class GameObjectGetComponentInChildren : BaseAction
    {
        [Tooltip("The GameObject.")]
        public GameObjectVar GameObject;

        [Tooltip("The type of Component to get. E.g., 'Rigidbody' or 'Renderer'")]
        [BaseType(typeof(Component))]
        public TypeReference ComponentType;
        
        [Tooltip("Should the search include inactive GameObjects?")]
        public BoolVar IncludeInactive;
        
        [SerializeReference]
        [WriteOnly, MatchType(nameof(ComponentType))]
        [Tooltip("Store the result in a Component variable")]
        public IVariableRef StoreResult;
        
        public override void Execute()
        {
            if (!RuntimeCheck(GameObject)) return;
            StoreResult?.SetValue(GameObject.Value.GetComponentInChildren(ComponentType.Type, IncludeInactive.Value));
        }
        
        public override string GetSummary() => "Get {ComponentType} component on {GameObject} or children -> {StoreResult}" 
                                               + (IncludeInactive.Value ? " (including inactive)" : "");
    }
}
