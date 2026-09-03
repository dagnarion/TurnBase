using System;
using UnityEngine;
using UnityEngine.UI;

public class CombatantStatusUI : MonoBehaviour
{
    [SerializeField] private Combatant combatant;
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider manaBar;

    private void OnEnable()
    {
        combatant.OnHealthChange += HealthUpdate;
        combatant.OnManaChange += ManaUpdate;
    }

    private void OnDisable()
    {
        combatant.OnHealthChange -= HealthUpdate;
        combatant.OnManaChange -= ManaUpdate;
    }

    private void HealthUpdate(float amount)
    {
        if (healthBar == null)  return;
        healthBar.value = amount;
    }

    private void ManaUpdate(float amount)
    {
        if(manaBar == null) return;
        manaBar.value = amount;
    }
}