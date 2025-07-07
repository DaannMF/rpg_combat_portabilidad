using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatsDisplay : MonoBehaviour {
    [Header("UI Components")]
    [SerializeField] private Image characterSprite;
    [SerializeField] private Outline activeOutline;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text statusText;

    private Character character;
    private int maxHealth;

    private void Awake() {
        FindUIComponents();
    }

    private void FindUIComponents() {
        if (characterSprite == null)
            characterSprite = transform.Find("CharacterSprite")?.GetComponent<Image>();

        if (activeOutline == null)
            activeOutline = GetComponent<Outline>();

        if (characterNameText == null)
            characterNameText = transform.Find("CharacterName")?.GetComponent<TMP_Text>();

        if (healthText == null)
            healthText = transform.Find("HealthText")?.GetComponent<TMP_Text>();

        if (statusText == null)
            statusText = transform.Find("StatusText")?.GetComponent<TMP_Text>();
    }

    public void Initialize(Character character) {
        this.character = character;
        this.maxHealth = character.Stats.maxHealth;

        SetupCharacterDisplay();
        SetActiveIndicator(false);

        UpdateDisplay();
    }

    private void SetupCharacterDisplay() {
        if (character == null) return;

        if (characterNameText != null) {
            string displayName = GetFormattedCharacterName();
            characterNameText.text = displayName;
        }

        if (characterSprite != null && character.Stats.characterSprite != null)
            characterSprite.sprite = character.Stats.characterSprite;
    }

    private string GetFormattedCharacterName() {
        string baseName = !string.IsNullOrEmpty(character.Stats.characterName)
            ? character.Stats.characterName
            : character.CharacterType.ToString();

        if (character.CharacterType.IsPlayer()) {
            string playerNumber = GetPlayerNumber(character.CharacterType);
            return $"{baseName} (Player {playerNumber})";
        }

        return baseName;
    }

    private string GetPlayerNumber(CharacterType characterType) {
        return characterType switch {
            CharacterType.Player1 => "1",
            CharacterType.Player2 => "2",
            CharacterType.Player3 => "3",
            _ => "?",
        };
    }

    public void UpdateHealth(int newHealth) {
        if (healthText != null) {
            healthText.text = $"{newHealth}/{maxHealth}";
        }
    }

    public void UpdateDeathStatus(bool isDead) {
        if (statusText != null) {
            statusText.text = isDead ? "DEAD" : "ALIVE";
            statusText.color = isDead ? Color.red : Color.green;
        }

        if (characterSprite != null) {
            Color spriteColor = characterSprite.color;
            spriteColor.a = isDead ? 0.5f : 1f;
            characterSprite.color = spriteColor;
        }
    }

    public void SetActiveIndicator(bool active) {
        if (activeOutline != null) {
            activeOutline.enabled = active;
        }
    }

    private void UpdateDisplay() {
        if (character == null) return;

        UpdateHealth(character.CurrentHealth);
        UpdateDeathStatus(character.IsDead);
    }
}