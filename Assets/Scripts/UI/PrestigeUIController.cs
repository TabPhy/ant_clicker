using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls the "Fly to the Moon" prestige button and confirmation panel.
/// On confirm, hands off to RocketLaunchSequencer for the full animation,
/// which calls PrestigeManager.DoPrestige() at the right moment.
/// </summary>
public class PrestigeUIController : MonoBehaviour
{
    [Header("Prestige Button")]
    public Button          prestigeButton;
    public TextMeshProUGUI prestigeButtonLabel;

    [Header("Confirmation Panel")]
    public GameObject      confirmPanel;
    public TextMeshProUGUI pendingDNAText;
    public TextMeshProUGUI prestigeCountText;
    public Button          confirmButton;
    public Button          cancelButton;

    private void Start()
    {
        prestigeButton.onClick.AddListener(OnPrestigeClicked);
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        confirmPanel.SetActive(false);

        CurrencyManager.Instance.OnCurrencyChanged += RefreshPrestigeButton;
        RefreshPrestigeButton();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= RefreshPrestigeButton;
    }

    private void RefreshPrestigeButton()
    {
        bool   can        = PrestigeManager.Instance.CanPrestige();
        double pendingDNA = PrestigeManager.Instance.CalculatePendingDNA();

        prestigeButton.interactable = can;
        prestigeButtonLabel.text    = can
            ? $"🚀 Fly to the Moon!\n(+{BigNumberFormatter.FormatShort(pendingDNA)} DNA)"
            : "🚀 Fly to the Moon\n(locked)";
    }

    private void OnPrestigeClicked()
    {
        double pendingDNA      = PrestigeManager.Instance.CalculatePendingDNA();
        pendingDNAText.text    = $"You will earn +{BigNumberFormatter.FormatShort(pendingDNA)} Ant DNA";
        prestigeCountText.text = $"Prestige #{PrestigeManager.Instance.PrestigeCount + 1}";
        confirmPanel.SetActive(true);
    }

    private void OnConfirm()
    {
        confirmPanel.SetActive(false);

        // Hand off to the cinematic launch sequence — it calls DoPrestige() internally
        if (RocketLaunchSequencer.Instance != null)
            RocketLaunchSequencer.Instance.LaunchAndPrestige();
        else
            PrestigeManager.Instance.DoPrestige();   // fallback if no sequencer in scene

        RefreshPrestigeButton();
    }

    private void OnCancel()
    {
        confirmPanel.SetActive(false);
    }
}
