using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private CombatantChannel onTurnChangedChannel;
    [SerializeField] private CombatPhaseChannel onPhaseChangedChannel;
    [SerializeField] private CombatantChannel onResolvePhaseChannel;

    [Header("UI Reference")]
    [SerializeField] private Button endTurnButton;
    [SerializeField] private CanvasGroup handCardGroup;

    private TurnController _turnController;

    private void Awake()
    {
        _turnController = new TurnController(
            onTurnChangedChannel, 
            onPhaseChangedChannel, 
            onResolvePhaseChannel
        );
    }

    private void OnEnable()
    {
        if (onTurnChangedChannel != null)
            onTurnChangedChannel.onEventRaised.AddListener(HandleTurnChanged);
    }

    private void OnDisable()
    {
        if (onTurnChangedChannel != null)
            onTurnChangedChannel.onEventRaised.RemoveListener(HandleTurnChanged);
    }

    private void Start()
    {
        _turnController.StartCombat(CombatantType.Player);
    }

    private void HandleTurnChanged(CombatantType actor)
    {
        if (actor == CombatantType.Player)
        {
            Debug.Log("<color=green><b>[BATTLE]</b> TỚI LƯỢT PLAYER!</color>");
            SetPlayerInteractable(true);
        }
        else if (actor == CombatantType.Enemy)
        {
            Debug.Log("<color=red><b>[BATTLE]</b> TỚI LƯỢT ENEMY!</color>");
            SetPlayerInteractable(false);
            StartCoroutine(EnemyTurnRoutine());
        }
    }

    private IEnumerator EnemyTurnRoutine()
    {
        yield return new WaitForSeconds(1.5f);

        Debug.Log("<color=red>[ENEMY]</color> Enemy đã ra chiêu xong! Nhường lượt.");

        _turnController.EndActionPhase();
    }

    private void SetPlayerInteractable(bool interactable)
    {
        if (endTurnButton != null)
            endTurnButton.interactable = interactable;

        if (handCardGroup != null)
        {
            handCardGroup.interactable = interactable;
            handCardGroup.blocksRaycasts = interactable;
        }
    }

    public void OnPlayerEndAction()
    {
        if (_turnController != null && _turnController.CurrentActor == CombatantType.Player)
        {
            _turnController.EndActionPhase();
        }
    }
}