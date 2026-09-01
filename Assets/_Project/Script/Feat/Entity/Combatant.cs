using System;
using NaughtyAttributes;
using UnityEngine;

public class Combatant : MonoBehaviour
{
   [field: SerializeField] public String name { get; private set; }
   [SerializeField] private float MaxHp;
   [SerializeField] private float MaxMp;
   [SerializeField] private Resistance[] Resitances;
   [SerializeField] private ElementType[] ElementDamages;
   public bool IsPlayed { get; private set; }
   [field:ProgressBar("CurrentHP","MaxHp",EColor.Red),SerializeField]
    public float CurrentHP { get; private set; }
   [field:ProgressBar("CurrentMP","MaxMp",EColor.Blue),SerializeField]
   public float CurrentMP { get; private set; }

   private void Start()
   {
      CurrentHP = MaxHp;
      CurrentMP = MaxMp;
   }

   public void TakeDamage()
   {
      
   }

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