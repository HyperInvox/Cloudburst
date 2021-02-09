using R2API;
using RoR2;
using UnityEngine;

public abstract class BuffBuilder
{
    public BuffDef buffDef;
    public CustomBuff customBuff;
    public BuffIndex buffIndex;

    public abstract string IconPath { get; }
    public abstract bool CanStack { get; }
    public abstract bool IsDebuff { get; }
    public abstract Color BuffColor { get; }

    public virtual void Init() {
        CreateBuff();
        Hook();
    }

    public virtual void CreateBuff() {
        buffDef = new BuffDef {
            iconPath = IconPath,
            buffColor = BuffColor,
            canStack = CanStack,
            isDebuff = IsDebuff
        };
        customBuff = new CustomBuff(buffDef);
        buffIndex = BuffAPI.Add(customBuff);
    }

    public virtual void Hook() {

    }

    public int GetCount(CharacterBody body) {
        if (!body) return 0;

        return body.GetBuffCount(buffIndex);
    }
}