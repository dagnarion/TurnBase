using System.Collections.Generic;
using UnityEngine;

public static class SpellCaster
{
    private class CastContext
    {
        public Combatant Caster;
        public Combatant PrimaryTarget;
        public List<Combatant> AllAllies;
        public List<Combatant> AllEnemies;
        
        public Dictionary<string, float> EffectResults = new Dictionary<string, float>();
    }
    
    public static bool CanCast(SpellDefinitionSO spell, Combatant caster)
    {
        if (spell == null || caster == null || caster.CurrentHP <= 0) return false;
        return caster.CurrentMP >= spell.ManaCost;
    }

    public static bool CastSpell(
        SpellDefinitionSO spell, 
        Combatant caster, 
        Combatant primaryTarget, 
        List<Combatant> allAllies = null, 
        List<Combatant> allEnemies = null)
    {
        if (!CanCast(spell, caster))
        {
            Debug.LogWarning($"[SpellCaster] {caster?.Name} không đủ MP hoặc không hợp lệ để dùng {spell?.DisplayName}");
            return false;
        }
        if (!caster.CanUseCard(spell.ManaCost))
        {
            return false;
        }
        
        CastContext context = new CastContext
        {
            Caster = caster,
            PrimaryTarget = primaryTarget,
            AllAllies = allAllies ?? new List<Combatant>(),
            AllEnemies = allEnemies ?? new List<Combatant>()
        };
        
        foreach (SpellEffectData effectData in spell.Effects)
        {
            ExecuteEffect(spell, effectData, context);
        }

        return true;
    }

    private static void ExecuteEffect(SpellDefinitionSO spell, SpellEffectData effectData, CastContext context)
    {
        List<Combatant> targets = ResolveTargets(effectData.targetType, context);
        if (targets.Count == 0) return;
        
        float value = CalculateEffectValue(effectData.scaling, spell.Element, context);
        float totalAppliedValue = 0f;

        foreach (Combatant target in targets)
        {
            if (target == null || target.CurrentHP <= 0) continue;

            switch (effectData.kind)
            {
                case SpellEffectKind.DealDamage:
                    DamageInfo dmgInfo = new DamageInfo(
                        source: context.Caster,
                        type: spell.Element,
                        amount: value,
                        isCrit: false,
                        ignoreAmor: false
                    );
                    target.TakeDamage(dmgInfo);
                    totalAppliedValue += value;
                    break;

                case SpellEffectKind.Heal:
                    target.Heal(value);
                    totalAppliedValue += value;
                    break;

                case SpellEffectKind.GainAmor:
                    target.AddShield(value);
                    totalAppliedValue += value;
                    break;

                case SpellEffectKind.RecoverMp:
                    target.RestoreMP(value);
                    totalAppliedValue += value;
                    break;

                case SpellEffectKind.ApplyEffect:
                    Debug.Log($"[SpellCaster] Apply {effectData.statusEffect} lên {target.Name}");
                    break;

                case SpellEffectKind.RemoveEffect:
                    Debug.Log($"[SpellCaster] Remove {effectData.statusEffect} khỏi {target.Name}");
                    break;
            }
        }
        
        if (!string.IsNullOrEmpty(effectData.id))
        {
            context.EffectResults[effectData.id] = totalAppliedValue;
        }
    }

    private static float CalculateEffectValue(SpellScalingData scaling, ElementType element, CastContext context)
    {
        if (scaling == null) return 0f;

        if (scaling.scalingType == SpellScalingType.Derived)
        {
            if (!string.IsNullOrEmpty(scaling.sourceEffectRef) && 
                context.EffectResults.TryGetValue(scaling.sourceEffectRef, out float sourceVal))
            {
                return sourceVal * scaling.derivedPercentage;
            }
            return 0f;
        }
        else
        {
            float total = scaling.basePower;
            
            if (context.Caster != null && element != ElementType.None)
            {
                total += context.Caster.GetElementDamage(element);
            }

            return total;
        }
    }

    private static List<Combatant> ResolveTargets(SpellTargetType targetType, CastContext context)
    {
        List<Combatant> targets = new List<Combatant>();

        switch (targetType)
        {
            case SpellTargetType.Self:
                if (context.Caster != null && context.Caster.CurrentHP > 0)
                    targets.Add(context.Caster);
                break;

            case SpellTargetType.SelectedEnemy:
            case SpellTargetType.SelectedAlly:
                if (context.PrimaryTarget != null && context.PrimaryTarget.CurrentHP > 0)
                    targets.Add(context.PrimaryTarget);
                break;

            case SpellTargetType.AllEnemies:
                // TODO: Sau này có CombatantManager/BattleManager quản lý danh sách sống (AliveEnemies) 
                // thì lấy trực tiếp từ Manager để tối ưu hiệu năng và tránh cấp phát GC do FindAll()
                targets.AddRange(context.AllEnemies.FindAll(c => c != null && c.CurrentHP > 0));
                break;

            case SpellTargetType.AllAllies:
                // TODO: Sau này có CombatantManager/BattleManager quản lý danh sách sống (AliveAllies)
                // thì lấy trực tiếp từ Manager để tối ưu hiệu năng và tránh cấp phát GC do FindAll()
                targets.AddRange(context.AllAllies.FindAll(c => c != null && c.CurrentHP > 0));
                break;
        }

        return targets;
    }
}
