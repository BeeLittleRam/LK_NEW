using System;
using UnityEngine;

namespace HutongGames.PlayMaker
{
    // NOTE: Inherits ObjectPredicates
    // So we only need to define GameObject specific predicates
    
    [Serializable, DisplayName("Has Component")]
    public class GameObjectHasComponentPredicate : BasePredicate<GameObject>
    {
        public TypeVar Parameter;
        
        protected override bool DoEvaluate(GameObject go) => go != null && go.HasComponent(Parameter.Value);
        
        public override string ToString() => $"Has Component {Parameter}";
    }
    
    [Serializable, DisplayName("Has Tag")]
    public class GameObjectHasTagPredicate : BasePredicate<GameObject>
    {
        public StringVar Parameter;
        
        protected override bool DoEvaluate(GameObject go) => go != null && !string.IsNullOrEmpty(Parameter.Value) && go.CompareTag(Parameter.Value);

        public override string ToString() => $"Has Tag {Parameter}";
    }
}
