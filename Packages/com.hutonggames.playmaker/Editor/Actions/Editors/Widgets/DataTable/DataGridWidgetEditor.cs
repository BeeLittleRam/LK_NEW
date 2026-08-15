using System.Collections.Generic;
using HutongGames.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace HutongGames.PlayMaker.UI.Editor
{
    /// <summary>
    /// UI Toolkit inspector for DataGridWidget.
    /// - Shows/hides paging controls when UsePaging is off.
    /// - Shows the correct Page Text field based on TextComponentKind.
    /// - Shows SlotIndex field picker when FillMode is BySlotIndexField.
    /// - Shows an inline help box when no valid int fields exist.
    /// </summary>
    [CustomEditor(typeof(DataGridWidget))]
    public sealed class DataGridWidgetEditor : UnityEditor.Editor
    {
        // Cache schema options per inspector instance
        private readonly List<SchemaOption> _intFieldOptions = new();

        private struct SchemaOption
        {
            public string Label;
            public SerializableGuid Guid;

            public SchemaOption(string label, SerializableGuid guid)
            {
                Label = label;
                Guid = guid;
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            root.style.minWidth = 0;

            // ── Properties ────────────────────────────────────────────────────────────
            var tableProp = serializedObject.FindProperty("_table");

            var contentProp = serializedObject.FindProperty("_content");
            var cellPrefabProp = serializedObject.FindProperty("_cellPrefab");
            var scrollRectProp = serializedObject.FindProperty("_scrollRect");

            var columnsProp = serializedObject.FindProperty("_columns");
            var slotCountProp = serializedObject.FindProperty("_slotCount");
            var showEmptySlotsProp = serializedObject.FindProperty("_showEmptySlots");
            var fillModeProp = serializedObject.FindProperty("_fillMode");
            var slotIndexGuidProp = serializedObject.FindProperty("_slotIndexFieldGuid");

            var rebuildOnEnableProp = serializedObject.FindProperty("_rebuildOnEnable");
            var rebuildOnChangedProp = serializedObject.FindProperty("_rebuildOnChanged");

            var usePagingProp = serializedObject.FindProperty("_usePaging");
            var slotsPerPageProp = serializedObject.FindProperty("_slotsPerPage");
            var pageIndexProp = serializedObject.FindProperty("_pageIndex");

            var prevBtnProp = serializedObject.FindProperty("_prevPageButton");
            var nextBtnProp = serializedObject.FindProperty("_nextPageButton");
            var pageTextKindProp = serializedObject.FindProperty("_pageTextKind");
            var pageTmpTextProp = serializedObject.FindProperty("_pageTmpText");
            var pageUguiTextProp = serializedObject.FindProperty("_pageUguiText");
            var pageFormatProp = serializedObject.FindProperty("_pageTextFormat");
            var updatePagingUiProp = serializedObject.FindProperty("_updatePagingUI");
            var disableEndsProp = serializedObject.FindProperty("_disablePagingButtonsAtEnds");
            var resetScrollOnPageChangeProp = serializedObject.FindProperty("_resetScrollOnPageChange");

            // ── Table ────────────────────────────────────────────────────────────────
            root.Add(new PropertyField(tableProp));

            // ── UI ───────────────────────────────────────────────────────────────────
            root.AddSpacer(6);
            root.Add(Header("UI"));

            root.Add(new PropertyField(contentProp));
            root.Add(new PropertyField(cellPrefabProp));
            root.Add(new PropertyField(scrollRectProp));

            // ── Grid ─────────────────────────────────────────────────────────────────
            root.AddSpacer(6);
            root.Add(Header("Grid"));

            root.Add(new PropertyField(columnsProp));
            root.Add(new PropertyField(slotCountProp));
            root.Add(new PropertyField(showEmptySlotsProp));

            var fillModeField = new PropertyField(fillModeProp);
            root.Add(fillModeField);

            // SlotIndex picker region (shown only when FillMode == BySlotIndexField)
            var slotIndexRegion = new VisualElement();
            slotIndexRegion.style.minWidth = 0;
            slotIndexRegion.style.marginLeft = 12;
            root.Add(slotIndexRegion);

            // Help box when schema isn't ready or has no int fields
            var slotIndexHelp = new HelpBox(
                "This Fill Mode requires an integer field in the Data Definition to store slot positions.\n" +
                "Add an Int field (commonly named \"SlotIndex\") and then select it here.",
                HelpBoxMessageType.Info);
            slotIndexHelp.style.marginTop = 4;

            // The actual picker row
            var slotIndexRow = new VisualElement
            {
                style =
                {
                    flexDirection = FlexDirection.Row,
                    alignItems = Align.Center,
                    minWidth = 0,
                    marginTop = 2
                }
            };

            var slotIndexLabel = new Label("Slot Index Field")
            {
                style =
                {
                    minWidth = 0,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    marginRight = 6
                }
            };

            var slotIndexPopup = new PopupField<string>(new List<string> { "<None>" }, 0)
            {
                style =
                {
                    flexGrow = 1,
                    minWidth = 0
                }
            };

            slotIndexRow.Add(slotIndexLabel);
            slotIndexRow.Add(slotIndexPopup);

            slotIndexRegion.Add(slotIndexRow);
            slotIndexRegion.Add(slotIndexHelp);

            // When popup changes, write GUID into SerializableGuid property
            slotIndexPopup.RegisterValueChangedCallback(evt =>
            {
                if (_intFieldOptions.Count == 0)
                    return;

                int idx = slotIndexPopup.index;
                if (idx < 0 || idx >= _intFieldOptions.Count)
                    return;

                var g = _intFieldOptions[idx].Guid;

                serializedObject.Update();
                SetGuidProperty(slotIndexGuidProp, g);
                serializedObject.ApplyModifiedProperties();
            });

            // ── Lifecycle ────────────────────────────────────────────────────────────
            root.AddSpacer(6);
            root.AddHeader("Lifecycle");

            root.Add(new PropertyField(rebuildOnEnableProp));
            root.Add(new PropertyField(rebuildOnChangedProp));

            // ── Paging ───────────────────────────────────────────────────────────────
            root.AddSpacer(6);
            root.Add(Header("Paging"));

            var usePagingField = new PropertyField(usePagingProp);
            root.Add(usePagingField);

            var pagingGroup = new VisualElement
            {
                style =
                {
                    marginLeft = 12,
                    minWidth = 0
                }
            };
            root.Add(pagingGroup);

            pagingGroup.Add(new PropertyField(slotsPerPageProp));
            pagingGroup.Add(new PropertyField(pageIndexProp));

            pagingGroup.AddSpacer(4);
            pagingGroup.AddHeader("Paging UI (optional)", small: true);

            pagingGroup.Add(new PropertyField(prevBtnProp));
            pagingGroup.Add(new PropertyField(nextBtnProp));

            var pageTextKindField = new PropertyField(pageTextKindProp);
            pagingGroup.Add(pageTextKindField);

            var tmpField = new PropertyField(pageTmpTextProp);
            var uguiField = new PropertyField(pageUguiTextProp);

            pagingGroup.Add(tmpField);
            pagingGroup.Add(uguiField);

            pagingGroup.Add(new PropertyField(pageFormatProp));
            pagingGroup.Add(new PropertyField(updatePagingUiProp));
            pagingGroup.Add(new PropertyField(disableEndsProp));
            pagingGroup.Add(new PropertyField(resetScrollOnPageChangeProp));

            // ── Visibility + binding refresh ─────────────────────────────────────────
            void RefreshSlotIndexPicker()
            {
                serializedObject.Update();

                // Show slot index UI only when FillMode == BySlotIndexField
                var fillMode = (DataGridWidget.FillMode)fillModeProp.enumValueIndex;
                bool wantsSlotIndex = fillMode == DataGridWidget.FillMode.BySlotIndexField;

                slotIndexRegion.style.display = wantsSlotIndex ? DisplayStyle.Flex : DisplayStyle.None;

                if (!wantsSlotIndex)
                    return;

                // Resolve definition from DataTableReference (asset/component fields).
                var def = ResolveDefinitionFromTableReference(tableProp);

                _intFieldOptions.Clear();
                BuildIntFieldOptions(def, _intFieldOptions);

                // If no definition or no options, show help and a disabled popup
                bool hasOptions = _intFieldOptions.Count > 1; // includes <None>

                slotIndexHelp.style.display = hasOptions ? DisplayStyle.None : DisplayStyle.Flex;
                slotIndexPopup.SetEnabled(hasOptions);

                // Update popup choices
                var labels = new List<string>(_intFieldOptions.Count);
                for (int i = 0; i < _intFieldOptions.Count; i++)
                    labels.Add(_intFieldOptions[i].Label);

                // Keep current selection by GUID where possible
                var currentGuid = GetGuidProperty(slotIndexGuidProp);
                int selected = 0;
                for (int i = 0; i < _intFieldOptions.Count; i++)
                {
                    if (_intFieldOptions[i].Guid == currentGuid)
                    {
                        selected = i;
                        break;
                    }
                }

                slotIndexPopup.choices = labels;
                slotIndexPopup.index = Mathf.Clamp(selected, 0, labels.Count - 1);
            }

            void RefreshPagingVisibility()
            {
                serializedObject.Update();

                bool usePaging = usePagingProp.boolValue;
                pagingGroup.style.display = usePaging ? DisplayStyle.Flex : DisplayStyle.None;

                if (!usePaging)
                    return;

                var kind = (DataGridWidget.TextComponentKind)pageTextKindProp.enumValueIndex;
                tmpField.style.display = kind == DataGridWidget.TextComponentKind.TmpText ? DisplayStyle.Flex : DisplayStyle.None;
                uguiField.style.display = kind == DataGridWidget.TextComponentKind.UguiText ? DisplayStyle.Flex : DisplayStyle.None;
            }

            // Track changes without rebuilding inspector
            root.TrackPropertyValue(usePagingProp, _ => RefreshPagingVisibility());
            root.TrackPropertyValue(pageTextKindProp, _ => RefreshPagingVisibility());

            root.TrackPropertyValue(fillModeProp, _ => RefreshSlotIndexPicker());
            root.TrackPropertyValue(tableProp, _ => RefreshSlotIndexPicker());

            RefreshSlotIndexPicker();
            RefreshPagingVisibility();

            return root;
        }

        private static VisualElement Header(string text, bool small = false)
        {
            return new Label(text)
            {
                style =
                {
                    unityFontStyleAndWeight = FontStyle.Bold,
                    marginLeft = 4,
                    marginTop = small ? 4 : 6,
                    marginBottom = 2,
                    opacity = small ? 0.85f : 1f,
                    minWidth = 0
                }
            };
        }

        private static void BuildIntFieldOptions(DataDefinition def, List<SchemaOption> options)
        {
            options.Clear();

            // Always include None
            options.Add(new SchemaOption("<None>", SerializableGuid.None));

            if (def == null)
                return;

#if UNITY_EDITOR
            var vars = def.Variables.GetVariablesInEditorOrder();
#else
            var vars = def.Variables.GetVariables();
#endif

            foreach (var v in vars)
            {
                if (v is not BaseVariable bv) continue;
                if (bv.DataType != typeof(int)) continue;

                var label = !string.IsNullOrEmpty(bv.ShortName) ? bv.ShortName : bv.Name;
                if (string.IsNullOrEmpty(label))
                    label = "(Int)";

                options.Add(new SchemaOption(label, bv.Guid));
            }
        }

        private static DataDefinition ResolveDefinitionFromTableReference(SerializedProperty tableRefProp)
        {
            if (tableRefProp == null) return null;

            var assetProp = tableRefProp.FindPropertyRelative("_tableAsset");
            if (assetProp != null && assetProp.objectReferenceValue is DataTableAsset asset)
                return asset.DataDefinition;

            var compProp = tableRefProp.FindPropertyRelative("_tableComponent");
            if (compProp != null && compProp.objectReferenceValue is DataTableComponent comp)
                return comp.DataDefinition;

            return null;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // SerializableGuid SerializedProperty helpers
        // Assumes SerializableGuid serializes two ulongs named _guidA/_guidB (or GuidA/GuidB).
        // ─────────────────────────────────────────────────────────────────────────────

        private static SerializableGuid GetGuidProperty(SerializedProperty guidProp)
        {
            if (guidProp == null) return SerializableGuid.None;

            var aProp = guidProp.FindPropertyRelative("_guidA") ?? guidProp.FindPropertyRelative("GuidA") ?? guidProp.FindPropertyRelative("guidA");
            var bProp = guidProp.FindPropertyRelative("_guidB") ?? guidProp.FindPropertyRelative("GuidB") ?? guidProp.FindPropertyRelative("guidB");

            if (aProp == null || bProp == null)
                return SerializableGuid.None;

            return new SerializableGuid((ulong)aProp.longValue, (ulong)bProp.longValue);
        }

        private static void SetGuidProperty(SerializedProperty guidProp, SerializableGuid guid)
        {
            if (guidProp == null) return;

            var (a, b) = guid.ToParts();

            var aProp = guidProp.FindPropertyRelative("_guidA") ?? guidProp.FindPropertyRelative("GuidA") ?? guidProp.FindPropertyRelative("guidA");
            var bProp = guidProp.FindPropertyRelative("_guidB") ?? guidProp.FindPropertyRelative("GuidB") ?? guidProp.FindPropertyRelative("guidB");

            if (aProp == null || bProp == null)
                return;

            aProp.longValue = unchecked((long)a);
            bProp.longValue = unchecked((long)b);
        }
    }
}

