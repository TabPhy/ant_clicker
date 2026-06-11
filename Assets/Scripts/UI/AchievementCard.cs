using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// One row/card in the Achievements panel.
/// Assign to a prefab; the AchievementsPanelController spawns one per milestone.
/// </summary>
public class AchievementCard : MonoBehaviour
{
    [Header("UI References")]
    public Image            cardIcon;
    public TextMeshProUGUI  titleText;
    public TextMeshProUGUI  descriptionText;
    public TextMeshProUGUI  rewardText;
    public TextMeshProUGUI  progressText;     // e.g. "12,500 / 1,000,000"
    public Image            completedOverlay; // green tick overlay shown when done
    public CanvasGroup      lockedGroup;      // dim card when not yet visible

    public void Populate(Milestone m)
    {
        var data = m.Data;

        if (data.icon != null) cardIcon.sprite = data.icon;

        titleText.text       = data.title;
        descriptionText.text = data.description;
        rewardText.text      = BuildRewardLabel(data);

        bool show = data.visibleBeforeUnlock || m.Completed;
        lockedGroup.alpha          = show ? 1f : 0.35f;
        lockedGroup.interactable   = show;
        lockedGroup.blocksRaycasts = show;

        completedOverlay.gameObject.SetActive(m.Completed);

        // Progress text (hidden once completed)
        progressText.gameObject.SetActive(!m.Completed && show);
        if (!m.Completed && show)
            progressText.text = BuildProgressLabel(data);
    }

    private string BuildRewardLabel(MilestoneData data)
    {
        return data.rewardType switch
        {
            MilestoneRewardType.ClickMultiplier  => $"Reward: Click power ×{data.rewardValue:F1}",
            MilestoneRewardType.APSMultiplier    => $"Reward: All production ×{data.rewardValue:F1}",
            MilestoneRewardType.FlatClickBonus   => $"Reward: +{BigNumberFormatter.FormatShort(data.rewardValue)} ants/click",
            MilestoneRewardType.FlatAPSBonus     => $"Reward: +{BigNumberFormatter.FormatShort(data.rewardValue)} ants/sec",
            MilestoneRewardType.AntDNABonus      => $"Reward: +{BigNumberFormatter.FormatShort(data.rewardValue)} Ant DNA",
            MilestoneRewardType.UnlockBuilding   => "Reward: Unlocks new building",
            _                                    => data.flavourText,
        };
    }

    private string BuildProgressLabel(MilestoneData data)
    {
        // Pull live value from MilestoneManager context
        var ctx = MilestoneManager.Instance;  // rough approach — refine if needed
        return $"Goal: {BigNumberFormatter.FormatShort(data.threshold)}";
    }
}
