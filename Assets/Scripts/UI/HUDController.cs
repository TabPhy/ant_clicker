using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main HUD — displays ant count, APS, and click button.
/// Attach to your HUD Canvas and wire up the references in the Inspector.
/// </summary>
public class HUDController : MonoBehaviour
{
    [Header("Ant Count Display")]
    public TextMeshProUGUI antCountText;
    public TextMeshProUGUI antsPerSecondText;
    public TextMeshProUGUI antsPerClickText;

    [Header("Click Button")]
    public Button clickButton;

    [Header("DNA Display")]
    public TextMeshProUGUI antDNAText;

    private void Start()
    {
        clickButton.onClick.AddListener(OnClickButton);
        CurrencyManager.Instance.OnCurrencyChanged += RefreshUI;
        RefreshUI();
    }

    private void OnDestroy()
    {
        if (CurrencyManager.Instance != null)
            CurrencyManager.Instance.OnCurrencyChanged -= RefreshUI;
    }

    private void OnClickButton()
    {
        CurrencyManager.Instance.Click();
        // TODO: spawn a little ant animation at click position
    }

    private void RefreshUI()
    {
        var cm = CurrencyManager.Instance;

        antCountText.text      = BigNumberFormatter.Format(cm.Ants) + " Ants";
        antsPerSecondText.text = BigNumberFormatter.Format(cm.AntsPerSecond) + " ants/sec";
        antsPerClickText.text  = BigNumberFormatter.Format(cm.AntsPerClick)  + " ants/click";
        antDNAText.text        = BigNumberFormatter.FormatShort(cm.AntDNA)   + " DNA";
    }
}
