using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterTargetButton : MonoBehaviour {
    [Header("UI Components")]
    [SerializeField] private Button button;
    [SerializeField] private Image characterImage;

    private Character targetCharacter;
    private ActionType actionType;
    private Action<ActionType, Character> onClickCallback;

    private void Awake() {
        if (button == null)
            button = GetComponent<Button>();

        if (characterImage == null)
            characterImage = GetComponent<Image>();
    }

    public void Setup(Character target, ActionType actionType, Action<ActionType, Character> onClickCallback) {
        this.targetCharacter = target;
        this.actionType = actionType;
        this.onClickCallback = onClickCallback;

        if (characterImage != null && target != null && target.Stats != null && target.Stats.characterSprite != null) {
            characterImage.sprite = target.Stats.characterSprite;
            characterImage.color = Color.white;
        };

        if (button != null) {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked() {
        if (targetCharacter != null)
            onClickCallback?.Invoke(actionType, targetCharacter);
    }
}