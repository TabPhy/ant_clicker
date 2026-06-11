using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Populates and refreshes the full Achievements panel.
/// Spawns one AchievementCard prefab per milestone and updates them when the panel opens.
///
/// Setup:
///   - Create a ScrollView with a VerticalLayoutGroup content rect.
///   - Assign achievementCardPrefab (has AchievementCard component).
///   - Assign contentParent (the ScrollView's Content transform).
///   - Wire openButton / closeButton / panelRoot.
///   - Assign completedCountText (e.g. "12 / 40 Achievements").
/// </summary>
public class AchievementsPanelController : MonoBehaviour
{
    [Header("Panel")]
    public GameObject       panelRoot;
    public Button           openButton;
    public Button           closeButton;
    public TextMeshProUGUI  completedCountText;

    [Header("Cards")]
    public GameObject  achievementCardPrefab;
    public Transform   contentParent;

    private List<AchievementCard> _cards = new List<AchievementCard>();
    private bool _initialised = false;

    private void Start()
    {
        panelRoot.SetActive(false);
        openButton.onClick.AddListener(OpenPanel);
        closeButton.onClick.AddListener(ClosePanel);

        // Refresh header count when a new milestone fires
        MilestoneManager.Instance.OnMilestoneCompleted += _ => RefreshHeader();
    }

    private void OpenPanel()
    {
        panelRoot.SetActive(true);
        if (!_initialised) BuildCards();
        else RefreshCards();
    }

    private void ClosePanel() => panelRoot.SetActive(false);

    // ── Build once ────────────────────────────────────────────────
    private void BuildCards()
    {
        // Clear any placeholder children
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);
        _cards.Clear();

        foreach (var m in MilestoneManager.Instance.Milestones)
        {
            var go   = Instantiate(achievementCardPrefab, contentParent);
            var card = go.GetComponent<AchievementCard>();
            card.Populate(m);
            _cards.Add(card);
        }

        RefreshHeader();
        _initialised = true;
    }

    // ── Refresh open panel when state changes ─────────────────────
    private void RefreshCards()
    {
        var milestones = MilestoneManager.Instance.Milestones;
        for (int i = 0; i < _cards.Count && i < milestones.Count; i++)
            _cards[i].Populate(milestones[i]);
        RefreshHeader();
    }

    private void RefreshHeader()
    {
        int completed = MilestoneManager.Instance.CompletedCount();
        int total     = MilestoneManager.Instance.Milestones.Count;
        completedCountText.text = $"Achievements: {completed} / {total}";
    }
}
