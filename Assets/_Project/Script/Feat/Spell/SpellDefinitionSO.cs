using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellDefinition", menuName = "TurnBase/Spell Definition")]
public class SpellDefinitionSO : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private string spellId;
    [SerializeField] private string displayName;
    [SerializeField] private Ranks rank = Ranks.Rank_I;
    [SerializeField] private ElementType element;
    
    [Header("Costs & Requirements")]
    [SerializeField] private int manaCost;
    [SerializeField] private int cooldown;
    [SerializeField] private int minWisdomToImprint;

    [Header("Execution Pipeline")]
    [Tooltip("Các effect sẽ chạy tuần tự từ trên xuống dưới")]
    [SerializeField] private List<SpellEffectData> effects = new List<SpellEffectData>();
    
    public String  SpellId => spellId;
    public String DisplayName => displayName;
    public Ranks Rank => rank;
    public ElementType Element => element;
    public int ManaCost => manaCost;
    public int Cooldown => cooldown;
    public int MinWisdomToImprint => minWisdomToImprint;
    public List<SpellEffectData> Effects => effects;
}