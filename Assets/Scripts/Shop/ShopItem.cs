using System;

/// <summary>
/// Runtime wrapper around ShopItemData — tracks purchase state.
/// </summary>
public class ShopItem
{
    public ShopItemData Data      { get; private set; }
    public bool         Purchased { get; private set; }
    public bool         Unlocked  => Data.unlockAtTotalAnts <= 0 ||
                                     CurrencyManager.Instance.TotalAntsEver >= Data.unlockAtTotalAnts;

    public event Action<ShopItem> OnPurchased;

    public ShopItem(ShopItemData data, bool alreadyPurchased = false)
    {
        Data      = data;
        Purchased = alreadyPurchased;
    }

    public bool TryPurchase()
    {
        if (Purchased)           return false;
        if (!Unlocked)           return false;
        if (!CurrencyManager.Instance.SpendAnts(Data.antCost)) return false;

        Purchased = true;
        OnPurchased?.Invoke(this);
        return true;
    }

    /// <summary>Force-set purchased (used when loading save data).</summary>
    public void MarkPurchased() => Purchased = true;
}
