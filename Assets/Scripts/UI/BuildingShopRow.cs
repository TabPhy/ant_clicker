using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One row in the building shop.
/// Attach to your BuildingRow prefab and wire up references.
/// Set buildingIndex in the Inspector (0 = first building, 1 = second, etc.)
/// </summary>
public class BuildingShopRow : MonoBehaviour
{
    [Header("Building Index (set in Inspector)")]
    public int buildingIndex = 0;

    [Header("UI References")]
    public Image            buildingIcon;
    public TextMeshProUGUI  buildingNameText;
    public TextMeshProUGUI  buildingDescText;
    public TextMeshProUGUI  countText;
    public TextMeshProUGUI  costText;
    public TextMeshProUGUI  apsText;
    public Button           buyButton;
    public GameObject       lockedOverlay;    // greyed-out panel shown before unlock

    private Building _building;

    private void Start()
    {
        _building = BuildingManager.Instance.Buildings[buildingIndex];

        buildingNameText.text = _building.Data.buildingName;
        buildingDescText.text = _building.Data.description;

        if (_building.Data.icon != null)
            buildingIcon.sprite = _building.Data.icon;

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
        BuildingManager.Instance.PurchaseBuilding(buildingIndex);
        Refresh();
    }

    private void Refresh()
    {
        bool unlocked = _building.Unlocked;
        lockedOverlay.SetActive(!unlocked);
        buyButton.interactable = unlocked && CurrencyManager.Instance.Ants >= _building.NextCost();

        countText.text = _building.Count.ToString();
        costText.text  = BigNumberFormatter.Format(_building.NextCost()) + " ants";
        apsText.text   = BigNumberFormatter.Format(_building.CurrentAPS) + "/s";
    }
}
