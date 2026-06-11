using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Shows a small toast notification when a milestone is completed.
/// Toasts queue up so they never overlap.
///
/// Setup:
///   - Create a panel (e.g. 400x90px) anchored bottom-centre.
///   - Add: icon (Image), titleText (TMP), rewardText (TMP), background Image.
///   - Attach this script and assign the references.
///   - The panel starts inactive; this script activates/deactivates it.
/// </summary>
public class MilestoneToastController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject       toastPanel;
    public Image            toastIcon;
    public TextMeshProUGUI  toastTitleText;
    public TextMeshProUGUI  toastRewardText;

    [Header("Timing")]
    public float displayDuration = 3.5f;
    public float fadeDuration    = 0.4f;

    private Queue<Milestone> _queue     = new Queue<Milestone>();
    private bool             _showing   = false;
    private CanvasGroup      _canvasGroup;

    private void Start()
    {
        _canvasGroup = toastPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = toastPanel.AddComponent<CanvasGroup>();

        toastPanel.SetActive(false);
        MilestoneManager.Instance.OnMilestoneCompleted += EnqueueToast;
    }

    private void OnDestroy()
    {
        if (MilestoneManager.Instance != null)
            MilestoneManager.Instance.OnMilestoneCompleted -= EnqueueToast;
    }

    private void EnqueueToast(Milestone m)
    {
        if (!m.Data.showPopup) return;
        _queue.Enqueue(m);
        if (!_showing) StartCoroutine(ShowNextToast());
    }

    private IEnumerator ShowNextToast()
    {
        while (_queue.Count > 0)
        {
            _showing = true;
            Milestone m = _queue.Dequeue();

            // Populate
            toastTitleText.text  = $"🏆 {m.Data.title}";
            toastRewardText.text = BuildRewardString(m.Data);
            if (m.Data.icon != null) toastIcon.sprite = m.Data.icon;

            // Fade in
            toastPanel.SetActive(true);
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

            // Hold
            yield return new WaitForSeconds(displayDuration);

            // Fade out
            yield return StartCoroutine(Fade(1f, 0f, fadeDuration));
            toastPanel.SetActive(false);
        }
        _showing = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        float elapsed = 0f;
        _canvasGroup.alpha = from;
        while (elapsed < duration)
        {
            elapsed            += Time.deltaTime;
            _canvasGroup.alpha  = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }

    private string BuildRewardString(MilestoneData data)
    {
        return data.rewardType switch
        {
            MilestoneRewardType.ClickMultiplier  => $"Click power ×{data.rewardValue:F1}",
            MilestoneRewardType.APSMultiplier    => $"All production ×{data.rewardValue:F1}",
            MilestoneRewardType.FlatClickBonus   => $"+{BigNumberFormatter.FormatShort(data.rewardValue)} ants/click",
            MilestoneRewardType.FlatAPSBonus     => $"+{BigNumberFormatter.FormatShort(data.rewardValue)} ants/sec",
            MilestoneRewardType.AntDNABonus      => $"+{BigNumberFormatter.FormatShort(data.rewardValue)} Ant DNA",
            MilestoneRewardType.UnlockBuilding   => "New building unlocked!",
            _                                    => data.flavourText,
        };
    }
}
