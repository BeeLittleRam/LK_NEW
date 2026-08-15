using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [System.Serializable]
    [PublicAPI]
    [ActionCategory(Category.LogicValue)]
    [ActionDescription("Check if a variable is equal to a given value.")]
    public class CheckVariableEquals : BaseTrueFalseAction
    {
        [SerializeReference]
        [BaseType(typeof(object))]
        [Tooltip("The Variable to check.")]
        public AnyVariableRef Variable;
        
        [SerializeReference]
        [MatchType(nameof(Variable)), CanBeNullOrEmpty]
        [Tooltip("Check if the Variable is equal to this value.")]
        public IVariableVar Value;

        public override bool CanExecute() => !Variable.IsNone;

        protected override bool Test()
        {
            var value1 = Value.GetValue();
            var value2 = Variable.GetValue();
            if (value1 == null && value2 == null) return true;
            return value1 != null && value1.Equals(value2);
        }

        protected override string TrueSummary => "{Variable} == {Value}";
        protected override string FalseSummary => "{Variable} != {Value}";
    }
}