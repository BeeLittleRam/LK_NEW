
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
    [ActionDescription("Get the next item in a List each time this action is called. " +
                       "This lets you quickly loop through all the items of an array to perform actions on them.")]
    public class ListGetNextItem : BaseAction
    {
        [BaseType(typeof(IList))]
        [Tooltip("The list variable.")]
        [SerializeReference] public IListVariableRef List;

        [OptionalField]
        [Tooltip("The index of the current item. This increases by one each time the action runs. " +
                 "Set this to 0 to start from the beginning of the list. " +
                 "If you don't provide a variable, the action will use an internal counter.")]
        public IntegerRef CurrentIndex;
        
        [MatchType(nameof(List))]
        [ConvertibleName("Item")]
        [Tooltip("Store the next item in a variable.")]
        [WriteOnly, SerializeReference] 
        public IVariableRef GetNextItem;

        [OptionalField]
        [Tooltip("Event sent after getting the next item.")]
        public EventRef LoopEvent;

        [OptionalField]
        [Tooltip("Event sent when there are no more items. Otherwise loop back to the first item.")]
        public EventRef FinishedEvent;

        [NonSerialized]
        private int _nextItemIndex;
        
        public override bool CanExecute() => CheckParameters(List, GetNextItem);

        /// <summary>
        /// Next item index using either a supplied index or an internal counter.
        /// </summary>
        private int NextItemIndex
        {
            get => CurrentIndex.IsNone ? _nextItemIndex : CurrentIndex.Value;
            set
            {
                if (CurrentIndex.IsNone)
                {
                    _nextItemIndex = value;
                }
                else
                {
                    CurrentIndex.Value = value;
                }
            }
        }

        public override void Execute()
        {
            if (List.ListVariable == null || List.ListVariable.Count == 0)
            {
                if (FinishedEvent.IsSet)
                {
                    SendEvent(FinishedEvent);
                }
                return;
            }
            
            if (NextItemIndex < 0)
            {
                NextItemIndex = 0;
            }
            
            if (NextItemIndex >= List.ListVariable.Count)
            {
                NextItemIndex = 0;

                if (FinishedEvent.IsSet)
                {
                    SendEvent(FinishedEvent);
                    return;
                }
            }
            
            GetNextItem.SetValue(List.ListVariable[NextItemIndex]);
            NextItemIndex++;
            
            SendEvent(LoopEvent);
        }

        public override string GetSummary() => 
            "Get next item from {List} -> {GetNextItem}"
            + (FinishedEvent.IsSet ? " Finished {FinishedEvent}" : "");
    }
}
