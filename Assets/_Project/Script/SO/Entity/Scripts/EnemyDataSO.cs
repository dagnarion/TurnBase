using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "Entity/EnemyData")]
public class EnemyDataSO : ScriptableObject
{
 [field:SerializeField] public float MaxHP { get; private set; }
 [field:SerializeField] public float Amor { get; private set; }
 [field:SerializeField] public List<Resitance> restances { get; private set; }
 [field:SerializeField] public List<Potency> Potencies { get; private set; }
}

[Serializable]
public struct Resitance
{
 [field:SerializeField]  public DamageType ResitanceType { get; private set; }
 [field:SerializeField]  public int BaseResitance { get; private set; }

   public Resitance(DamageType resitanceType,int baseResitance)
   {
       this.ResitanceType = resitanceType;
       this.BaseResitance = baseResitance;
   }
}

[Serializable]
public struct Potency
{
    [field:SerializeField] public DamageType DamageType { get; private set; }
    [field:SerializeField] public float Amount { get; private set; }

    public Potency(DamageType damageType,float amount)
    {
        this.DamageType = damageType;
        this.Amount = amount;
    }
}

