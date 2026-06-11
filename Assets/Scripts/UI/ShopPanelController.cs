using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Populates the shop panel with item cards, grouped by category.
/// Also handles category filter tabs.
///
/// Setup:
///   - ScrollView with VerticalLayoutGroup content.
///   - One ShopItemCard prefab.
///   - Three tab buttons: All / QoL / Production / Fun.
///   - openButton / closeButton.
///   - purchasedCountText (e.g. "7 / 30 items owned").
/// </summary>
public class ShopPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject      panelRoot;
    public Button          openButton;
    public Button          closeButton;
    public TextMeshProUGUI purchasedCountText;

    [Header("Cards")]
    public GameObject shopItemCardPrefab;
    public Transform  contentParent;

    [Header("Category Filter Tabs")]
    public Button tabAll;
    public Button tabQoL;
    public Button tabProduction;
    public Button tabFun;

    private List<ShopItemCard> _cards      = new List<ShopItemCard>();
    private ShopCategory?      _activeFilter = null;   // null = show all
    private bool               _initialised = false;

    private void Start()
    {
        panelRoot.SetActive(false);
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);

        tabAll.onClick.AddListener(()        => SetFilter(null));
        tabQoL.onClick.AddListener(()        => SetFilter(ShopCategory.QualityOfLife));
        tabProduction.onClick.AddListener(() => SetFilter(ShopCategory.ProductionBoost));
        tabFun.onClick.AddListener(()        => SetFilter(ShopCategory.FunAndCosmetic));

        ShopManager.Instance.OnItemPurchased += _ => RefreshHeader();
    }

    private void OpenPanel()
    {
        panelRoot.SetActive(true);
        if (!_initialised) BuildCards();
        else RefreshCards();
    }

    private void ClosePanel() => panelRoot.SetActive(false);

    private void BuildCards()
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);
        _cards.Clear();

        var items = ShopManager.Instance.Items;
        for (int i = 0; i < items.Count; i++)
        {
            var go   = Instantiate(shopItemCardPrefab, contentParent);
            var card = go.GetComponent<ShopItemCard>();
            card.Populate(items[i], i);
            _cards.Add(card);
        }

        ApplyFilter();
        RefreshHeader();
        _initialised = true;
    }

    private void RefreshCards()
    {
        var items = ShopManager.Instance.Items;
        for (int i = 0; i < _cards.Count && i < items.Count; i++)
            _cards[i].Populate(items[i], i);
        ApplyFilter();
        RefreshHeader();
    }

    private void SetFilter(ShopCategory? cat)
    {
        _activeFilter = cat;
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var items = ShopManager.Instance.Items;
        for (int i = 0; i < _cards.Count && i < items.Count; i++)
        {
            bool show = _activeFilter == null || items[i].Data.category == _activeFilter;
            _cards[i].gameObject.SetActive(show);
        }
    }

    private void RefreshHeader()
    {
        int purchased = ShopManager.Instance.PurchasedCount();
        int total     = ShopManager.Instance.Items.Count;
        purchasedCountText.text = $"Shop  {purchased}/{total} owned";
    }
}
