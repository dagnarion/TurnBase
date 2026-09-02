public class TurnController
{
    // State của trận đấu
    public CombatantType CurrentActor { get; private set; }
    public CombatPhase CurrentPhase { get; private set; }
    public int TurnCount { get; private set; }
    public bool IsInCombat { get; private set; }
    
    private readonly EventChannel<CombatantType> _onTurnChangedChannel;
    private readonly EventChannel<CombatPhase> _onPhaseChangedChannel;
    private readonly EventChannel<CombatantType> _onResolvePhaseChannel;

    public TurnController(
        EventChannel<CombatantType> onTurnChangedChannel,
        EventChannel<CombatPhase> onPhaseChangedChannel,
        EventChannel<CombatantType> onResolvePhaseChannel = null)
    {
        _onTurnChangedChannel = onTurnChangedChannel;
        _onPhaseChangedChannel = onPhaseChangedChannel;
        _onResolvePhaseChannel = onResolvePhaseChannel;
    }
    
    public void StartCombat(CombatantType startingActor = CombatantType.Player)
    {
        if (IsInCombat) return;

        IsInCombat = true;
        TurnCount = 0;
        StartNewTurn(startingActor);
    }

    public void EndActionPhase()
    {
        if (!IsInCombat || CurrentPhase != CombatPhase.Action) return;

        CombatantType nextActor = (CurrentActor == CombatantType.Player) 
            ? CombatantType.Enemy 
            : CombatantType.Player;

        StartNewTurn(nextActor);
    }

    public void EndCombat()
    {
        IsInCombat = false;
    }

    private void StartNewTurn(CombatantType actor)
    {
        CurrentActor = actor;
        TurnCount++;

        _onTurnChangedChannel?.EventRaised(CurrentActor);

        EnterResolvePhase();
    }

    private void EnterResolvePhase()
    {
        CurrentPhase = CombatPhase.Resolve;
        _onPhaseChangedChannel?.EventRaised(CurrentPhase);

        _onResolvePhaseChannel?.EventRaised(CurrentActor);
        EnterActionPhase();
    }

    private void EnterActionPhase()
    {
        CurrentPhase = CombatPhase.Action;
        _onPhaseChangedChannel?.EventRaised(CurrentPhase);

    }
}