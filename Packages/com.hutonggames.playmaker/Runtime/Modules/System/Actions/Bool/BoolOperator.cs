using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [PublicAPI, Serializable]
    [ActionCategory(Category.Bool)]
    [ActionDescription("Performs math operations on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
    public class BoolOperator : BaseAction
    {
        public enum Operation
        {
            // ReSharper disable InconsistentNaming
            AND,
            NAND,
            OR,
            XOR
            // ReSharper restore InconsistentNaming
        }
        
        [Tooltip("The first bool.")]
        [SerializeField]
        private BoolVar _bool1;

        [Tooltip("The second bool.")]
        [SerializeField]
        private BoolVar _bool2;

        [Tooltip("The boolean operation.")]
        [SerializeField]
        private Operation _operation;
        
        [Tooltip("Store the result.")]
        [SerializeField]
        [WriteOnly]
        private BoolRef _result;

        public override bool CanExecute() => CheckParameters(_bool1, _bool2, _result);

        public override void Execute()
        {
            var v1 = _bool1.Value;
            var v2 = _bool2.Value;

            switch (_operation)
            {
                case Operation.AND:
                    _result.Value = v1 && v2;
                    break;

                case Operation.NAND:
                    _result.Value = !(v1 && v2);
                    break;

                case Operation.OR:
                    _result.Value = v1 || v2;
                    break;

                case Operation.XOR:
                    _result.Value = v1 ^ v2;
                    break;
            }
        }

        public override string GetSummary() => "{_result} = {_bool1} {_operation} {_bool2}";
    }
}