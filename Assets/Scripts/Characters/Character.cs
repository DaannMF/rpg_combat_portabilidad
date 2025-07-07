using System;
using UnityEngine;

public abstract class Character : MonoBehaviour {
    [Header("Character Settings")]
    [SerializeField] protected BaseCharacterStats stats;
    [SerializeField] protected CharacterType characterType;

    protected int currentHealth;
    protected GridCell currentPosition;
    protected bool isDead;
    protected bool hasMovedThisTurn;
    protected bool hasActedThisTurn;

    public BaseCharacterStats Stats => stats;
    public CharacterType CharacterType => characterType;
    public int CurrentHealth => currentHealth;
    public GridCell CurrentPosition => currentPosition;
    public bool IsDead => isDead;
    public bool HasFinishedTurn => hasMovedThisTurn && hasActedThisTurn;

    public event Action<Character> OnCharacterDeath;
    public event Action<Character, int> OnHealthChanged;

    protected virtual void Start() {
        Initialize();
    }

    protected virtual void Initialize() {
        if (stats != null) {
            currentHealth = stats.maxHealth;
            isDead = false;
            hasMovedThisTurn = false;
            hasActedThisTurn = false;
        }
    }

    public virtual void SetPosition(GridCell position) {
        currentPosition = position;
    }

    public virtual void SetStats(BaseCharacterStats newStats) {
        stats = newStats;
        if (stats != null)
            currentHealth = stats.maxHealth;
    }

    public virtual void SetCharacterType(CharacterType type) {
        characterType = type;
    }

    public virtual bool CanMoveTo(GridCell targetPosition) {
        if (hasMovedThisTurn || isDead) return false;

        int distance = currentPosition.GetChebyshevDistance(targetPosition);
        return distance <= stats.speed;
    }

    public virtual void MoveTo(GridCell targetPosition) {
        if (!CanMoveTo(targetPosition)) return;

        currentPosition = targetPosition;
        hasMovedThisTurn = true;
    }

    public virtual bool CanAttack(Character target) {
        if (hasActedThisTurn || isDead || target == null || target.IsDead) return false;

        int distance = currentPosition.GetChebyshevDistance(target.CurrentPosition);
        return stats.CanAttackAtDistance(distance);
    }

    public virtual void TakeDamage(int damage) {
        if (isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damage);
        OnHealthChanged?.Invoke(this, currentHealth);

        if (currentHealth <= 0)
            Die();
    }

    public virtual void RestoreHealth(int healing) {
        if (isDead) return;

        currentHealth = Mathf.Min(stats.maxHealth, currentHealth + healing);
        OnHealthChanged?.Invoke(this, currentHealth);
    }

    protected virtual void Die() {
        isDead = true;

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        OnCharacterDeath?.Invoke(this);
    }

    public virtual void StartTurn() {
        hasMovedThisTurn = false;
        hasActedThisTurn = false;
    }

    public virtual void EndTurn() {
        hasMovedThisTurn = true;
        hasActedThisTurn = true;
    }

    public abstract void ExecuteTurn();
}