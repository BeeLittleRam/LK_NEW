using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HutongGames.PlayMaker.UI
{
    /// <summary>
    /// Displays a fixed-slot grid view of a DataTable using a repeated cell prefab.
    /// Slots are stable positions; rows can be mapped sequentially or by an integer SlotIndex field.
    /// </summary>
    [AddComponentMenu("PlayMaker/Widgets/Data Grid Widget")]
    public sealed class DataGridWidget : MonoBehaviour
    {
        [SerializeField, Tooltip(
            "Select the Data Table to display. You can reference a DataTableAsset (project asset) " +
            "or a DataTableComponent (scene object).")]
        private DataTableReference _table;

        [SerializeField, Tooltip(
            "Parent RectTransform that will contain the generated cell instances. " +
            "Typically this is the Content object of a ScrollRect with a GridLayoutGroup.")]
        private RectTransform _content;

        [SerializeField, Tooltip(
            "Prefab used to create each visible slot. The prefab must contain a DataRowView component " +
            "on the root or in its children.")]
        private GameObject _cellPrefab;

        [SerializeField, OptionalField, Tooltip(
            "Optional ScrollRect used to reset the scroll position when rebuilding or changing pages.")]
        private ScrollRect _scrollRect;

        [Header("Grid")]

        [SerializeField, Min(1), Tooltip(
            "Number of columns in the grid. Used for navigation and slot layout.")]
        private int _columns = 4;

        [SerializeField, Min(0), Tooltip(
            "Total number of slots in the grid. If 0, the widget derives slot count from paging " +
            "or (if paging is off) from the current table row count.")]
        private int _slotCount = 16;

        [SerializeField, Tooltip(
            "If enabled, the widget will build slots even when there is no row assigned, " +
            "binding an empty state to the cell prefab.")]
        private bool _showEmptySlots = true;

        public enum FillMode
        {
            Sequential,
            BySlotIndexField
        }

        [SerializeField, Tooltip(
            "How rows are assigned to grid slots.\n\n" +
            "Sequential: rows fill slots from 0 upward.\n" +
            "By Slot Index Field: each row provides an integer SlotIndex that chooses its slot.")]
        private FillMode _fillMode = FillMode.Sequential;

        [SerializeField, Tooltip(
            "Field GUID (in the assigned DataDefinition) used as the SlotIndex when Fill Mode is By Slot Index Field.\n\n" +
            "This is chosen in the custom inspector from integer fields in the DataDefinition.")]
        private SerializableGuid _slotIndexFieldGuid = SerializableGuid.None;

        [SerializeField, Tooltip("Rebuild the visible slots automatically when this component becomes enabled.")]
        private bool _rebuildOnEnable = true;

        [SerializeField, Tooltip(
            "Automatically rebuild when the DataTable structure changes (rows added, removed, sorted, or cleared). " +
            "Disable this if you prefer to control rebuilding manually.")]
        private bool _rebuildOnChanged = true;

        [SerializeField, Tooltip("Enable paging to show a subset of slots at a time.")]
        private bool _usePaging;

        [SerializeField, Min(1), Tooltip("Number of slots per page when paging is enabled.")]
        private int _slotsPerPage = 16;

        [SerializeField, Tooltip("Zero-based index of the currently displayed page.")]
        private int _pageIndex;

        [SerializeField, OptionalField, Tooltip("Optional button to go to previous page when paging is enabled.")]
        private Button _prevPageButton;

        [SerializeField, OptionalField, Tooltip("Optional button to go to next page when paging is enabled.")]
        private Button _nextPageButton;

        public enum TextComponentKind { TmpText, UguiText }

        [SerializeField, Tooltip("Select which text component type is used for Page Text.")]
        private TextComponentKind _pageTextKind = TextComponentKind.TmpText;

        [SerializeField, OptionalField, Tooltip("Optional TextMeshPro text used to display page info.")]
        private TMPro.TMP_Text _pageTmpText;

        [SerializeField, OptionalField, Tooltip("Optional legacy uGUI Text used to display page info.")]
        private Text _pageUguiText;

        [SerializeField, Tooltip(
            "Format string for the Page Text. Uses: {0} = current page (1-based), {1} = total pages.\n" +
            "Example: \"Page {0}/{1}\"")]
        private string _pageTextFormat = "Page {0}/{1}";

        [SerializeField, Tooltip("Update page text + button enabled state whenever the view changes.")]
        private bool _updatePagingUI = true;

        [SerializeField, Tooltip("Disable Prev/Next buttons when you are at the first/last page.")]
        private bool _disablePagingButtonsAtEnds = true;

        [SerializeField, Tooltip("Reset scroll position to the top when changing pages using the paging UI buttons.")]
        private bool _resetScrollOnPageChange = true;

        // ─────────────────────────────────────────────────────────────────────────────
        // Pooling / active
        // ─────────────────────────────────────────────────────────────────────────────

        [Serializable]
        private sealed class CellInstance
        {
            public GameObject Go;
            public DataItemUI View;
            public int SlotIndex;

            public void SetActive(bool active)
            {
                if (Go != null) Go.SetActive(active);
            }
        }

        private readonly List<CellInstance> _active = new();
        private readonly Stack<CellInstance> _pool = new();

        private readonly Dictionary<int, DataRow> _rowBySlot = new();

        private DataTable _resolvedTable;
        private DataTable _subscribedTable;

        private bool _rebuildQueued;

        // ─────────────────────────────────────────────────────────────────────────────
        // Unity
        // ─────────────────────────────────────────────────────────────────────────────

        private void OnEnable()
        {
            UpdateSubscription(queueRebuild: false);
            HookPagingUI(hook: true);

            if (_rebuildOnEnable)
                Rebuild(resetScroll: true);

            _rebuildQueued = false;

            if (_updatePagingUI)
                UpdatePagingUI();
        }

        private void LateUpdate()
        {
            UpdateSubscription(queueRebuild: true);

            if (_rebuildQueued)
            {
                _rebuildQueued = false;
                Rebuild(resetScroll: false);
            }
        }

        private void OnDisable()
        {
            HookPagingUI(hook: false);
            Unsubscribe();
            _rebuildQueued = false;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Public API
        // ─────────────────────────────────────────────────────────────────────────────

        public void Rebuild(bool resetScroll = false)
        {
            UpdateSubscription(queueRebuild: false);

            if (!ValidateSetup())
                return;

            _resolvedTable = _table?.ResolveData();
            if (_resolvedTable == null)
            {
                Clear();
                if (_updatePagingUI) UpdatePagingUI();
                return;
            }

            var def = _resolvedTable.DataDefinition;
            int totalSlots = GetTotalSlotCount();
            GetVisibleSlotRange(totalSlots, out int startSlot, out int visibleSlots);

            BuildSlotMap(_resolvedTable, totalSlots);

            ClearActiveToPool();

            for (int i = 0; i < visibleSlots; i++)
            {
                int slot = startSlot + i;
                if (slot < 0 || slot >= totalSlots)
                    break;

                var inst = GetOrCreateInstance();
                inst.SlotIndex = slot;

                if (_rowBySlot.TryGetValue(slot, out var row) && row != null)
                {
                    inst.View.Bind(def, row);
                }
                else
                {
                    // For fixed-slot inventories, empty slots are meaningful.
                    if (_showEmptySlots)
                        inst.View.BindEmpty(def);
                    else
                        inst.View.BindEmpty(def); // still bind empty; callers can hide visuals via targets if desired
                }

                inst.View.Apply();
                _active.Add(inst);
            }

            if (resetScroll)
                ResetScrollToTop();

            if (_updatePagingUI)
                UpdatePagingUI();
        }

        public void Refresh()
        {
            if (_active.Count == 0)
                return;

            _resolvedTable = _table?.ResolveData();
            if (_resolvedTable == null)
                return;

            var def = _resolvedTable.DataDefinition;

            int totalSlots = GetTotalSlotCount();
            BuildSlotMap(_resolvedTable, totalSlots);

            for (int i = 0; i < _active.Count; i++)
            {
                var inst = _active[i];
                if (inst == null || inst.View == null)
                    continue;

                if (_rowBySlot.TryGetValue(inst.SlotIndex, out var row) && row != null)
                    inst.View.Bind(def, row);
                else
                    inst.View.BindEmpty(def);

                inst.View.Apply();
            }
        }

        public void Clear()
        {
            _resolvedTable = null;
            _rowBySlot.Clear();
            ClearActiveToPool();
        }

        public void SetPage(int pageIndex, bool rebuild = true, bool resetScroll = true)
        {
            _pageIndex = Mathf.Max(0, pageIndex);
            if (rebuild) Rebuild(resetScroll);
            else if (_updatePagingUI) UpdatePagingUI();
        }

        public void NextPage(bool rebuild = true, bool resetScroll = true)
        {
            _pageIndex++;
            if (rebuild) Rebuild(resetScroll);
            else if (_updatePagingUI) UpdatePagingUI();
        }

        public void PreviousPage(bool rebuild = true, bool resetScroll = true)
        {
            _pageIndex = Mathf.Max(0, _pageIndex - 1);
            if (rebuild) Rebuild(resetScroll);
            else if (_updatePagingUI) UpdatePagingUI();
        }

        public void GetPageInfo(out int pageIndex, out int totalPages, out int totalSlots)
        {
            totalSlots = GetTotalSlotCount();
            int size = Mathf.Max(1, _slotsPerPage);
            totalPages = _usePaging ? Mathf.Max(1, (totalSlots + size - 1) / size) : 1;
            pageIndex = _usePaging ? Mathf.Clamp(_pageIndex, 0, totalPages - 1) : 0;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Internals
        // ─────────────────────────────────────────────────────────────────────────────

        private bool ValidateSetup()
        {
            if (_content == null) return false;
            if (_cellPrefab == null) return false;

            _columns = Mathf.Max(1, _columns);
            _slotsPerPage = Mathf.Max(1, _slotsPerPage);
            return true;
        }

        private int GetTotalSlotCount()
        {
            if (_slotCount > 0)
                return _slotCount;

            // When paging is enabled and slot count isn't set, treat the grid as one page.
            if (_usePaging)
                return Mathf.Max(1, _slotsPerPage);

            _resolvedTable = _table?.ResolveData();
            return Mathf.Max(0, _resolvedTable?.RowCount ?? 0);
        }

        private void GetVisibleSlotRange(int totalSlots, out int startSlot, out int count)
        {
            startSlot = 0;
            count = totalSlots;

            if (!_usePaging)
                return;

            int size = Mathf.Max(1, _slotsPerPage);
            int totalPages = Mathf.Max(1, (totalSlots + size - 1) / size);

            _pageIndex = Mathf.Clamp(_pageIndex, 0, totalPages - 1);

            startSlot = _pageIndex * size;
            count = Mathf.Max(0, Mathf.Min(size, totalSlots - startSlot));
        }

        private void BuildSlotMap(DataTable table, int totalSlots)
        {
            _rowBySlot.Clear();
            if (table == null) return;

            var rows = table.Rows;
            if (rows == null || rows.Count == 0) return;

            switch (_fillMode)
            {
                default:
                case FillMode.Sequential:
                {
                    int max = Mathf.Min(totalSlots, rows.Count);
                    for (int i = 0; i < max; i++)
                    {
                        var row = rows[i];
                        if (row != null)
                            _rowBySlot[i] = row;
                    }
                    return;
                }

                case FillMode.BySlotIndexField:
                {
                    if (_slotIndexFieldGuid == SerializableGuid.None)
                        return;

                    for (int r = 0; r < rows.Count; r++)
                    {
                        var row = rows[r];
                        if (row == null) continue;

                        if (!TryGetIntCell(row, _slotIndexFieldGuid, out var slot))
                            continue;

                        if (slot < 0 || slot >= totalSlots)
                            continue;

                        // First wins
                        if (!_rowBySlot.ContainsKey(slot))
                            _rowBySlot.Add(slot, row);
                    }

                    return;
                }
            }
        }

        private static bool TryGetIntCell(DataRow row, SerializableGuid fieldGuid, out int value)
        {
            value = 0;
            if (row == null || fieldGuid == SerializableGuid.None)
                return false;

            var cells = row.Cells;
            if (cells == null) return false;

            for (int i = 0; i < cells.Count; i++)
            {
                var c = cells[i];
                if (c == null || c.FieldGuid != fieldGuid) continue;

                var v = c.Value;
                return TryGetInt(v, out value);
            }

            return false;
        }

        /// <summary>
        /// Best-effort int extraction from your variable system.
        /// Replace/extend these cases to match your concrete types (IntVar/IntRef/etc).
        /// </summary>
        private static bool TryGetInt(IVariableVar v, out int value)
        {
            value = 0;
            if (v is IntegerVar iv)
            {
                value = iv.Value; 
                return true;
            }

            return false;
        }

        private void ClearActiveToPool()
        {
            for (int i = 0; i < _active.Count; i++)
                ReleaseInstance(_active[i]);

            _active.Clear();
        }

        private CellInstance GetOrCreateInstance()
        {
            if (_pool.Count > 0)
            {
                var inst = _pool.Pop();
                inst.SetActive(true);
                inst.Go.transform.SetAsLastSibling();
                return inst;
            }

            var go = Instantiate(_cellPrefab, _content, worldPositionStays: false);

            var view = go.GetComponent<DataItemUI>() ??
                       go.GetComponentInChildren<DataItemUI>(includeInactive: true);

            if (view == null)
            {
                go.SetActive(false);
                throw new InvalidOperationException(
                    $"DataGridWidget CellPrefab '{_cellPrefab.name}' does not contain a DataRowView component.");
            }

            return new CellInstance
            {
                Go = go,
                View = view,
                SlotIndex = -1
            };
        }

        private void ReleaseInstance(CellInstance inst)
        {
            if (inst == null || inst.Go == null)
                return;

            inst.SlotIndex = -1;
            inst.SetActive(false);
            inst.Go.transform.SetAsLastSibling();
            _pool.Push(inst);
        }

        private void ResetScrollToTop()
        {
            if (_scrollRect == null)
                return;

            _scrollRect.verticalNormalizedPosition = 1f;
            _scrollRect.horizontalNormalizedPosition = 0f;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Subscription
        // ─────────────────────────────────────────────────────────────────────────────

        private void UpdateSubscription(bool queueRebuild)
        {
            if (!_rebuildOnChanged)
            {
                Unsubscribe();
                return;
            }

            var current = _table?.ResolveData();

            if (ReferenceEquals(current, _subscribedTable))
                return;

            Unsubscribe();

            _subscribedTable = current;

            if (_subscribedTable != null)
                _subscribedTable.StructureChanged += OnTableStructureChanged;

            if (queueRebuild && isActiveAndEnabled)
                _rebuildQueued = true;
        }

        private void Unsubscribe()
        {
            if (_subscribedTable != null)
                _subscribedTable.StructureChanged -= OnTableStructureChanged;

            _subscribedTable = null;
        }

        private void OnTableStructureChanged()
        {
            if (!_rebuildOnChanged) return;
            if (!isActiveAndEnabled) return;

            _rebuildQueued = true;
        }

        // ─────────────────────────────────────────────────────────────────────────────
        // Paging UI
        // ─────────────────────────────────────────────────────────────────────────────

        private void HookPagingUI(bool hook)
        {
            if (_prevPageButton != null)
            {
                if (hook) _prevPageButton.onClick.AddListener(OnPrevPageClicked);
                else _prevPageButton.onClick.RemoveListener(OnPrevPageClicked);
            }

            if (_nextPageButton != null)
            {
                if (hook) _nextPageButton.onClick.AddListener(OnNextPageClicked);
                else _nextPageButton.onClick.RemoveListener(OnNextPageClicked);
            }
        }

        private void OnPrevPageClicked()
        {
            if (!_usePaging) return;
            PreviousPage(rebuild: true, resetScroll: _resetScrollOnPageChange);
        }

        private void OnNextPageClicked()
        {
            if (!_usePaging) return;
            NextPage(rebuild: true, resetScroll: _resetScrollOnPageChange);
        }

        private void UpdatePagingUI()
        {
            if (!_usePaging)
            {
                SetPageText(string.Empty);

                if (_disablePagingButtonsAtEnds)
                {
                    if (_prevPageButton != null) _prevPageButton.interactable = false;
                    if (_nextPageButton != null) _nextPageButton.interactable = false;
                }

                return;
            }

            GetPageInfo(out var pageIndex, out var totalPages, out _);

            var text = !string.IsNullOrEmpty(_pageTextFormat)
                ? string.Format(_pageTextFormat, pageIndex + 1, totalPages)
                : $"{pageIndex + 1}/{totalPages}";

            SetPageText(text);

            if (_disablePagingButtonsAtEnds)
            {
                if (_prevPageButton != null) _prevPageButton.interactable = pageIndex > 0;
                if (_nextPageButton != null) _nextPageButton.interactable = pageIndex < totalPages - 1;
            }
        }

        private void SetPageText(string text)
        {
            switch (_pageTextKind)
            {
                case TextComponentKind.TmpText:
                    if (_pageTmpText != null) _pageTmpText.text = text;
                    break;

                case TextComponentKind.UguiText:
                    if (_pageUguiText != null) _pageUguiText.text = text;
                    break;
            }
        }
    }
}
