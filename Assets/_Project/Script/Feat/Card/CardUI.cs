using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class CardUI : MonoBehaviour
{
    [Header("Event Channel (Lắng nghe để hồi Cooldown)")]
    [SerializeField] private CombatantChannel onResolvePhaseChannel;

    [Header("Data")]
    [SerializeField] private SpellDefinitionSO spellData;
    [SerializeField] private Combatant owner;

    [Header("State")]
    [SerializeField] private int currentCooldown = 0;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    public SpellDefinitionSO SpellData => spellData;
    public Combatant Owner => owner;
    public int CurrentCooldown => currentCooldown;
    public bool IsOnCooldown => currentCooldown > 0;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        if (onResolvePhaseChannel != null)
            onResolvePhaseChannel.onEventRaised.AddListener(HandleResolvePhase);
    }

    private void OnDisable()
    {
        if (onResolvePhaseChannel != null)
            onResolvePhaseChannel.onEventRaised.RemoveListener(HandleResolvePhase);
    }

    private void HandleResolvePhase(CombatantType actor)
    {
        bool isOwnerTurn = (owner != null && !owner.isEnemy && actor == CombatantType.Player) ||
                           (owner != null && owner.isEnemy && actor == CombatantType.Enemy);

        if (isOwnerTurn)
        {
            if (currentCooldown > 0)
            {
                ReduceCooldown(1);
                Debug.Log($"<color=cyan>[CardUI]</color> Lá bài <b>{spellData?.DisplayName}</b> giảm hồi chiêu còn: {currentCooldown} turn.");
            }
            else
            {
                UpdateVisualState();
            }
        }
    }
    public void Setup(SpellDefinitionSO spell, Combatant cardOwner)
    {
        spellData = spell;
        owner = cardOwner;
        currentCooldown = 0;
        UpdateVisualState();
    }

    public bool CanCast()
    {
        if (IsOnCooldown) return false;
        if (spellData == null || owner == null || owner.CurrentHP <= 0) return false;
        return owner.CurrentMP >= spellData.ManaCost;
    }
    
    public void TriggerCooldown()
    {
        if (spellData != null && spellData.Cooldown > 0)
        {
            currentCooldown = spellData.Cooldown;
        }
        else
        {
            currentCooldown = 0;
        }
        UpdateVisualState();
    }

    public void ReduceCooldown(int amount = 1)
    {
        if (currentCooldown > 0)
        {
            currentCooldown = Mathf.Max(0, currentCooldown - amount);
            UpdateVisualState();
        }
    }

    public void UpdateVisualState()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (IsOnCooldown || !CanCast())
        {
            canvasGroup.alpha = 0.5f; 
        }
        else
        {
            canvasGroup.alpha = 1f; 
        }
    }
}