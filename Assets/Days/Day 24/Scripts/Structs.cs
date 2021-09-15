using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Day24
{
    public enum Faction
    {
        Immune,
        Infection
    }

    public struct DamagePacket
    {
        public int damage;
        public string type;

        public DamagePacket(int damage, string type)
        {
            this.damage = damage;
            this.type = type;
        }

        public override string ToString() => $"{damage} {type} damage";
    }
}
