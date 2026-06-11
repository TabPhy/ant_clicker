using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// One purchasable shop item (ScriptableObject).
/// Items are bought ONCE (not stacked like buildings).
/// Create via right-click → AntClicker → Shop Item.
/// </summary>
[CreateAssetMenu(fileName = "NewShopItem", menuName = "AntClicker/Shop Item")]
public class ShopItemData : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique ID used in save data — never change after shipping.")]
    public string id          = "item_unique_id";
    public string itemName    = "Item Name";
    public string description = "What this does.";
    public string flavourText = "\"A quote about ants.\"";
    public Sprite icon;

    [Header("Shop Category")]
    public ShopCategory category = ShopCategory.QualityOfLife;

    [Header("Cost")]
    public double antCost = 1000;   // paid in ants

    [Header("Unlock Condition")]
    [Tooltip("Only show this item once player has earned this many total ants ever")]
    public double unlockAtTotalAnts = 0;

    [Header("Effects")]
    [Tooltip("All effects applied when purchased (can stack multiple)")]
    public List<ShopItemEffectEntry> effects = new List<ShopItemEffectEntry>();
}

/// <summary>One effect entry — pairs an effect type with a numeric value.</summary>
[System.Serializable]
public class ShopItemEffectEntry
{
    public ShopItemEffect effect;
    [Tooltip("Multiplier: 1.5 = +50%. Flat value: 10 = +10 units. Bool toggle: 1 = on.")]
    public double value = 1;
}

public enum ShopCategory
{
    QualityOfLife,
    ProductionBoost,
    FunAndCosmetic,
}
