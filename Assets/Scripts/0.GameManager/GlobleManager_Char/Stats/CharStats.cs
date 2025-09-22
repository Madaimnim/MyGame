using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public abstract class CharStats<TSkill> where TSkill :SkillBase
{
    public int Id;
    public int Level;
    public string Name;
    public int MaxHp;
    public int AttackPower;
    public float MoveSpeed;
    public GameObject CharPrefab;
    public Sprite SpriteIcon;
    public int SkillSlotCount;

    [System.NonSerialized] public SkillSlotRuntime<TSkill>[] SkillSlots;
    public IDamageable owner { get; protected set; }

    public virtual void InitializeOwner(IDamageable damageable) {
        owner = damageable;
    }

    public void InitializeSkillSlots(int slotCount) {                   //提供一個方法初始化技能槽數量
        SkillSlots = new SkillSlotRuntime<TSkill>[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            SkillSlots[i] = new SkillSlotRuntime<TSkill>(i);
        }
    }
}
