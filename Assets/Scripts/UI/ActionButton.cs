using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionButton : MonoBehaviour {
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text buttonText;

    private void Awake() {
        if (button == null)
            button = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>();
    }

    public void Setup(PlayerAction action, System.Action<PlayerAction> onClickCallback) {
        if (buttonText != null) {
            string displayText = action.displayName;
            if (action.target != null) {
                string targetName = GetTargetDisplayName(action.target);
                displayText += $" ({targetName})";
            }
            buttonText.text = displayText;
        }

        if (button != null) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => onClickCallback?.Invoke(action));
            button.interactable = action.isAvailable;
        }
    }

    private string GetTargetDisplayName(Character target) {
        if (target == null) return "Unknown";

        if (target.Stats != null && !string.IsNullOrEmpty(target.Stats.characterName)) {
            return target.Stats.characterName;
        }

        return $"{target.CharacterType} at ({target.CurrentPosition.x},{target.CurrentPosition.y})";
    }
}