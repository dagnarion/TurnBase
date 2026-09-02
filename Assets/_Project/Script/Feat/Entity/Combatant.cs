using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Combatant : MonoBehaviour
{
   [field: SerializeField] public String Name { get; private set; }
   [SerializeField] private float MaxHp;
   [SerializeField] private float MaxMp;
   [SerializeField] private float MaxShield;
   [SerializeField] private Resistance[] Resitances;
   [SerializeField] private ElementDamage[] ElementDamages;
   private Dictionary<ElementType, float> elementRes = new Dictionary<ElementType, float>();
   private Dictionary<ElementType, float> elementDamage = new Dictionary<ElementType, float>();
   [field: SerializeField] public bool isEnemy { get; private set; } = false;
   public bool IsPlayed { get; private set; }
   [field:ProgressBar("CurrentHP","MaxHp",EColor.Red),SerializeField]
    public float CurrentHP { get; private set; }
   [field:ProgressBar("CurrentMP","MaxMp",EColor.Blue),SerializeField]
   public float CurrentMP { get; private set; }   
   [field:ProgressBar("CurrentShield","MaxShield",EColor.Gray),SerializeField]
   public float CurrentShield { get; private set; }

   private void Start()
   {
      CurrentHP = MaxHp;
      CurrentMP = MaxMp;
      foreach (var it in Resitances)
      {
         elementRes.Add(it.ResType,it.Amount);
      }      
      
      foreach (var it in ElementDamages)
      {
         elementDamage.Add(it.ElementType,it.Amount);
      }
   }

   public bool CanUseCard(float amount)
   {
      if (CurrentMP - amount < 0) return false;
      CurrentMP -= amount;
      return true;
   }

   public void TakeDamage(DamageInfo info)
   {
      if (CurrentHP <= 0) return;
      float damage = info.Amount;
      if (!info.IgnoreAmor)
      {
         float res = GetResistance(info.Type);
         damage = info.Amount * 90f / Mathf.Max(1f, res + 90f);
      }

      if (CurrentShield > 0)
      {
         if (CurrentShield >= damage)
         {
            CurrentShield -= damage;
            damage = 0f;
         }
         else
         {
            damage -= CurrentShield;
            CurrentShield = 0f;
         }
      }

      CurrentHP -= damage;
      if (CurrentHP <= 0)
      {
         CurrentHP = 0;
         Destroy(this.gameObject);
      }
   }

   public float GetResistance(ElementType type)
   {
      return elementRes.TryGetValue(type, out float val) ? val : 0f;
   }

   public float GetElementDamage(ElementType type)
   {
      return elementDamage.TryGetValue(type, out float val) ? val : 0f;
   }

   public void AddShield(float amount) => CurrentShield += amount;
   public void Heal(float amount) => CurrentHP = Mathf.Clamp(CurrentHP + amount, 0, MaxHp);
   public void RestoreMP(float amount) => CurrentMP = Mathf.Clamp(CurrentMP + amount, 0, MaxMp);
   
}
[Serializable]
public class Resistance
{
   [field:SerializeField] public ElementType ResType { get; private set; }
   [field:SerializeField] public float Amount { get; private set; }
}

[Serializable]
public class ElementDamage
{
   [field:SerializeField] public ElementType ElementType { get; private set; }
   [field:SerializeField] public float Amount { get; private set; }
}

public struct DamageInfo
{
   public ElementType Type { get; private set; }
   public Combatant Source { get; private set; }
   public float Amount { get; private set; }
   public bool IsCrit { get; private set; }
   public bool IgnoreAmor { get; private set; }

   public DamageInfo(Combatant source, ElementType type, float amount, bool isCrit,bool ignoreAmor)
   {
      this.Source = source;
      this.Type = type;
      this.Amount = amount;
      this.IsCrit = isCrit;
      this.IgnoreAmor = ignoreAmor;
   }
}