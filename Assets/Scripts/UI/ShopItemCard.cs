using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One card in the shop panel.
/// Assign to a prefab; ShopPanelController spawns one per ShopItem.
/// </summary>
public class ShopItemCard : MonoBehaviour
{
    [Header("UI References")]
    public Image            itemIcon;
    public TextMeshProUGUI  nameText;
    public TextMeshProUGUI  descriptionText;
    public TextMeshProUGUI  costText;
    public TextMeshProUGUI  categoryBadge;
    public Button           buyButton;
    public TextMeshProUGUI  buyButtonLabel;
    public GameObject       purchasedOverlay;   // tick/banner shown once bought
    public CanvasGroup      lockedGroup;        // dims card before unlock threshold

    private ShopItem _item;
    private int      _index;

    public void Populate(ShopItem item, int index)
    {
        _item  = item;
        _index = index;

        if (item.Data.icon != null) itemIcon.sprite = item.Data.icon;

        nameText.text        = item.Data.itemName;
        descriptionText.text = item.Data.description;
        costText.text        = BigNumberFormatter.Format(item.Data.antCost) + " ants";
        categoryBadge.text   = CategoryEmoji(item.Data.category) + " " + item.Data.category.ToString();

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuy);

        CurrencyManager.Instance.OnCurrencyChanged += Refresh;
        Refresh();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= Refresh;
    }

    private void OnBuy()
    {
        ShopManager.Instance.PurchaseItem(_index);
        Refresh();
    }

    private void Refresh()
    {
        bool purchased = _item.Purchased;
        bool unlocked  = _item.Unlocked;
        bool canAfford = CurrencyManager.Instance.Ants >= _item.Data.antCost;

        purchasedOverlay.SetActive(purchased);
        buyButton.gameObject.SetActive(!purchased);

        lockedGroup.alpha          = unlocked ? 1f : 0.4f;
        lockedGroup.interactable   = unlocked && !purchased;
        lockedGroup.blocksRaycasts = unlocked && !purchased;

        if (!purchased)
        {
            buyButton.interactable = unlocked && canAfford;
            buyButtonLabel.text    = purchased  ? "✓ Owned"  :
                                     !unlocked  ? "🔒 Locked" :
                                     !canAfford ? "🐜 Too costly" : "Buy";
        }
    }

    private string CategoryEmoji(ShopCategory cat) => cat switch
    {
        ShopCategory.QualityOfLife  => "⚙️",
        ShopCategory.ProductionBoost => "⚡",
        ShopCategory.FunAndCosmetic => "🎨",
        _ => ""
    };
}
