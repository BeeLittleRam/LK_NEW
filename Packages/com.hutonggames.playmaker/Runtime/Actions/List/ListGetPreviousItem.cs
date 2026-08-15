using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
    [Serializable]
    [PublicAPI]
    [ActionCategory(Category.List)]
    [ConvertibleGroup("ListGetItem")]
    [ActionDescription("Get the previous item in a List each time this action is called.")]
    public class ListGetPreviousItem : BaseAction
    {
        [BaseType(typeof(IList))]
        [Tooltip("The list variable.")]
        [SerializeReference] public IListVariableRef List;

        [OptionalField]
        [Tooltip("The index of the current item. This decreases by one each time the action runs. " +
                 "Set this to the last valid index to start from the end of the list. " +
                 "If you don't provide a variable, the action will use an internal counter.")]
        public IntegerRef CurrentIndex;

        [Tooltip("The first index in the traversal range.")]
        public IntegerVar StartIndex;

        [Tooltip("The last index in the traversal range.")]
        public IntegerVar EndIndex;

        [Tooltip("Loop back to the end index after reaching the start index.")]
        public BoolVar Loop;
        
        [MatchType(nameof(List))]
        [ConvertibleName("Item")]
        [Tooltip("Store the previous item in a variable.")]
        [WriteOnly, SerializeReference] public IVariableRef GetPreviousItem;

        [OptionalField]
        [Tooltip("Event sent after getting the previous item.")]
        public EventRef LoopEvent;

        [OptionalField]
        [Tooltip("Event sent when the traversal reaches the start index and Loop is false.")]
        public EventRef FinishedEvent;

        [NonSerialized]
        private int _currentIndex = -1;

        public override bool CanExecute() => CheckParameters(List, StartIndex, EndIndex, Loop, GetPreviousItem);

        private int TraversalIndex
        {
            get => CurrentIndex.IsNone ? _currentIndex : CurrentIndex.Value;
            set
            {
                if (CurrentIndex.IsNone)
                {
                    _currentIndex = value;
                }
                else
                {
                    CurrentIndex.Value = value;
                }
            }
        }

        public override void Execute()
        {
            var list = List.ListVariable;
            if (list == null || list.Count == 0)
            {
                SendEvent(FinishedEvent);
                return;
            }

            var startIndex = Mathf.Clamp(StartIndex.Value, 0, list.Count - 1);
            var endIndex = Mathf.Clamp(EndIndex.Value, 0, list.Count - 1);

            if (startIndex > endIndex)
            {
                (startIndex, endIndex) = (endIndex, startIndex);
            }

            if (TraversalIndex < startIndex || TraversalIndex > endIndex)
            {
                TraversalIndex = endIndex;
            }

            GetPreviousItem.SetValue(list[TraversalIndex]);
            SendEvent(LoopEvent);

            if (TraversalIndex == startIndex)
            {
                if (Loop.Value)
                {
                    TraversalIndex = endIndex;
                }
                else
                {
                    TraversalIndex = startIndex;
                    SendEvent(FinishedEvent);
                }

                return;
            }

            TraversalIndex--;
        }

        public override string GetSummary() =>
            "Get previous item from {List} -> {GetPreviousItem}"
            + (FinishedEvent.IsSet ? " Finished {FinishedEvent}" : "");
    }
}
