using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttributeWeightData
{
    public AttributeType attribute;
    public float weight;
}

[Serializable]
public class SpellConditionData
{
    public SpellConditionKind kind = SpellConditionKind.None;
}

[Serializable]
public class SpellScalingData
{
    public SpellScalingType scalingType = SpellScalingType.Independent;
    
    [Header("If Independent")]
    public float basePower;
    public List<AttributeWeightData> independentWeights = new List<AttributeWeightData>();
    
    [Header("If Derived")]
    [Tooltip("Percentage to take from the source effect (e.g., 0.5 for 50%)")]
    public float derivedPercentage;
    public string sourceEffectRef;
}

[Serializable]
public class SpellEffectData
{
    public string id;
    public SpellEffectKind kind;
    public SpellTargetType targetType;
    public int targetCount = 1;
    
    [Space(10)]
    public SpellConditionData condition = new SpellConditionData();
    public SpellScalingData scaling = new SpellScalingData();

    [Header("Payload (Tùy chọn)")]
    public StatusEffects statusEffect;
}