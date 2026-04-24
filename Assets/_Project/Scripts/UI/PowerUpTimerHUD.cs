using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays one row per active power-up: name, remaining time, and a
/// fill bar that depletes. Driven by the PowerUpManager via
/// ServiceLocator (polled once per frame for fill values), and by the
/// EventBus for show/hide on activation/expiration.
/// </summary>
public class PowerUpTimerHUD : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public PowerUpData data;
        public GameObject root;
        public TextMeshProUGUI label;
        public Image fillBar;
    }

    [SerializeField] private List<Slot> slots;

    private PowerUpManager manager;

    private void Start()
    {
        manager = ServiceLocator.Get<PowerUpManager>();
        foreach (var slot in slots)
            if (slot.root != null) slot.root.SetActive(false);
    }

    private void Update()
    {
        if (manager == null) return;

        foreach (var slot in slots)
        {
            if (slot.data == null || slot.root == null) continue;

            var entry = manager.GetActive(slot.data);
            if (entry != null)
            {
                slot.root.SetActive(true);
                if (slot.label != null)
                    slot.label.text = $"{slot.data.displayName} {entry.timeRemaining:0.0}s";
                if (slot.fillBar != null)
                    slot.fillBar.fillAmount = entry.timeRemaining / entry.totalDuration;
            }
            else
            {
                slot.root.SetActive(false);
            }
        }
    }
}