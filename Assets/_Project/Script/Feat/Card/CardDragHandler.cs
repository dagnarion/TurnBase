using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CardUI))]
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(CanvasGroup))]
public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Drag Settings")]
    [Tooltip("Tỉ lệ phóng to lá bài khi bắt đầu cầm/kéo")]
    [SerializeField] private float holdScale = 1.15f;
    [Tooltip("Ngưỡng kéo lên (Y) để kích hoạt với các chiêu Self / AllEnemies / AllAllies")]
    [SerializeField] private float playThresholdY = 150f;
    [SerializeField] private float tweenDuration = 0.2f;
    [SerializeField] private LayerMask targetLayerMask = ~0;

    private CardUI cardUI;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;

    private Vector2 originalPosition;
    private Vector3 originalScale;
    private int originalSiblingIndex;
    private bool isDragging = false;
    private bool isTargetedSpell = false;

    private void Awake()
    {
        cardUI = GetComponent<CardUI>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvas = GetComponentInParent<Canvas>();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
        originalSiblingIndex = transform.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardUI == null || cardUI.SpellData == null || cardUI.Owner == null || !cardUI.CanCast())
        {
            eventData.pointerDrag = null;
            isDragging = false;
            return;
        }

        isDragging = true;
        rectTransform.DOKill();

        SpellDefinitionSO spell = cardUI.SpellData;
        SpellTargetType primaryTargetType = GetPrimaryTargetType(spell);
        isTargetedSpell = (primaryTargetType == SpellTargetType.SelectedEnemy || primaryTargetType == SpellTargetType.SelectedAlly);

        transform.SetAsLastSibling();
        canvasGroup.blocksRaycasts = false;

        rectTransform.DOScale(originalScale * holdScale, 0.15f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging || canvas == null)
        {
            Debug.Log("CannotMove");
            return;
        }
        if (!isTargetedSpell)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        canvasGroup.blocksRaycasts = true;

        if (cardUI == null || cardUI.SpellData == null || cardUI.Owner == null)
        {
            ReturnToHand();
            return;
        }

        SpellDefinitionSO spell = cardUI.SpellData;
        SpellTargetType primaryTargetType = GetPrimaryTargetType(spell);

        bool castSuccess = false;
        Vector2 screenPos = eventData.position;

        switch (primaryTargetType)
        {
            case SpellTargetType.SelectedEnemy:
            case SpellTargetType.SelectedAlly:
                castSuccess = TryTargetCast(spell, primaryTargetType, screenPos);
                break;

            case SpellTargetType.Self:
            case SpellTargetType.AllEnemies:
            case SpellTargetType.AllAllies:
                castSuccess = TryAreaOrSelfCast(spell);
                break;
        }

        if (castSuccess)
        {
            cardUI.TriggerCooldown();
        }
        ReturnToHand();
    }
    
    private bool TryTargetCast(SpellDefinitionSO spell, SpellTargetType targetType, Vector2 screenPosition)
    {
        Camera mainCam = Camera.main;
        if (mainCam == null) return false;

        Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(screenPosition);
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, targetLayerMask);

        if (hit != null && hit.TryGetComponent<Combatant>(out var targetCombatant))
        {
            if (targetCombatant.CurrentHP <= 0) return false;
            
            if (targetType == SpellTargetType.SelectedEnemy && targetCombatant.isEnemy == cardUI.Owner.isEnemy)
            {
                Debug.LogWarning("[CardDragHandler] Mục tiêu phải là Kẻ thù!");
                return false;
            }

            if (targetType == SpellTargetType.SelectedAlly && targetCombatant.isEnemy != cardUI.Owner.isEnemy)
            {
                Debug.LogWarning("[CardDragHandler] Mục tiêu phải là Đồng minh!");
                return false;
            }

            return SpellCaster.CastSpell(spell, cardUI.Owner, targetCombatant);
        }

        return false;
    }

    private bool TryAreaOrSelfCast(SpellDefinitionSO spell)
    {
        if (rectTransform.anchoredPosition.y >= (originalPosition.y + playThresholdY))
        {
            return SpellCaster.CastSpell(spell, cardUI.Owner, null);
        }

        return false;
    }

    private SpellTargetType GetPrimaryTargetType(SpellDefinitionSO spell)
    {
        if (spell.Effects != null && spell.Effects.Count > 0)
        {
            return spell.Effects[0].targetType;
        }
        return SpellTargetType.Self;
    }

    private void ReturnToHand()
    {
        rectTransform.DOKill();
        transform.SetSiblingIndex(originalSiblingIndex);
        rectTransform.DOAnchorPos(originalPosition, tweenDuration).SetEase(Ease.OutQuad);
        rectTransform.DOScale(originalScale, tweenDuration);
        cardUI.UpdateVisualState();
    }
}
