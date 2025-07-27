using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionTargetGrid : MonoBehaviour {
    [Header("UI Components")]
    [SerializeField] private TMP_Text actionTypeText;
    [SerializeField] private GameObject targetButtonPrefab;
    [SerializeField] private Transform targetButtonContainer;
    [SerializeField] private GridLayoutGroup gridLayout;

    private ActionType actionType;
    private List<CharacterTargetButton> targetButtons = new List<CharacterTargetButton>();
    private Action<ActionType, BaseCharacter> onTargetSelected;

    private void Awake() {
        if (gridLayout == null)
            gridLayout = GetComponentInChildren<GridLayoutGroup>();
    }

    public void Setup(ActionType actionType, Action<ActionType, BaseCharacter> onTargetSelected) {
        this.actionType = actionType;
        this.onTargetSelected = onTargetSelected;

        if (actionTypeText != null)
            actionTypeText.text = actionType.GetDisplayName();

        ClearTargets();
    }

    public void UpdateTargets(List<BaseCharacter> availableTargets) {
        ClearTargets();

        if (availableTargets == null || availableTargets.Count == 0) {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        foreach (var target in availableTargets)
            CreateTargetButton(target);
    }

    private void CreateTargetButton(BaseCharacter target) {
        if (targetButtonPrefab == null || targetButtonContainer == null) return;

        GameObject buttonObj = Instantiate(targetButtonPrefab, targetButtonContainer);
        var targetButton = buttonObj.GetComponent<CharacterTargetButton>();

        if (targetButton != null) {
            targetButton.Setup(target, actionType, onTargetSelected);
            targetButtons.Add(targetButton);
        }
    }

    private void ClearTargets() {
        foreach (var button in targetButtons)
            if (button != null)
                Destroy(button.gameObject);

        targetButtons.Clear();
    }
}