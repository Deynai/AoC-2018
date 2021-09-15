using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace Day24
{
    public class ArmyGroup
    {
        private Faction _faction;
        private int _units;
        private int _maxUnitHp;
        private int _attackDamage;
        private string _attackType;
        private int _initiative;
        
        private HashSet<string> _immunity;
        private HashSet<string> _weakness;

        private bool _targetedThisTurn = false;

        public Faction faction => _faction;
        public bool IsHostile(ArmyGroup other) => other._faction == _faction;
        public int Units => _units;
        public bool IsDead => _units <= 0;
        public int EffectivePower => _units * _attackDamage;
        public DamagePacket Damage => new DamagePacket(_units * _attackDamage, _attackType);
        public int Initiative => _initiative;

        public bool IsValidTarget => !_targetedThisTurn;
        public void SetTarget() => _targetedThisTurn = true;
        public void ClearTarget() => _targetedThisTurn = false;

        public int PredictDamage(DamagePacket packet)
        {
            int damage = packet.damage;

            if (_immunity.Contains(packet.type))
            {
                damage = 0;
            }
            if (_weakness.Contains(packet.type))
            {
                damage *= 2;
            }

            return damage;
        }
        public void DealDamage(ArmyGroup target)
        {
            target.TakeDamage(this.Damage);
        }
        private void TakeDamage(DamagePacket packet)
        {
            int unitDeaths = PredictDamage(packet) / _maxUnitHp;
            _units -= unitDeaths;
            if(_units < 0) { _units = 0; }
        }

        public void ApplyBoost(int boost)
        {
            _attackDamage += boost;
        }

        public override string ToString() => $"Units: {_units}, Health: {_maxUnitHp}, Damage: {Damage}, Initiative: {Initiative}";

        public ArmyGroup(Faction faction, int startUnits, int maxUnitHp, IEnumerable<string> immunity, IEnumerable<string> weakness, int attackDamage, string attackType, int initiative)
        {
            _faction = faction;
            _units = startUnits;
            _maxUnitHp = maxUnitHp;
            _attackDamage = attackDamage;
            _attackType = attackType;
            _initiative = initiative;
            _immunity = new HashSet<string>();
            _weakness = new HashSet<string>();
            foreach(string s in immunity) { _immunity.Add(s); }
            foreach(string s in weakness) { _weakness.Add(s); }
        }
        public ArmyGroup(Faction faction, string line)
        {
            _faction = faction;

            int[] nums = Regex.Matches(line, "\\d+").Cast<Match>().Select(n => int.Parse(n.Value)).ToArray();
            Debug.Assert(nums.Length == 4);

            _units = nums[0];
            _maxUnitHp = nums[1];
            _attackDamage = nums[2];
            _initiative = nums[3];

            _attackType = Regex.Match(line, "\\w+(?= damage)").Value;
            Debug.Assert(!_attackType.Contains(" "));

            _immunity = new HashSet<string>();
            _weakness = new HashSet<string>();
            IEnumerable<string> immunity = Regex.Match(line, "(?<=immune to ).+?(?=;|\\))").Value.Split(' ').Select(s => s.Replace(",", "").Trim());
            IEnumerable<string> weakness = Regex.Match(line, "(?<=weak to ).+?(?=;|\\))").Value.Split(' ').Select(s => s.Replace(",", "").Trim());
            foreach (string s in immunity) { if (s.Length > 0) { _immunity.Add(s); } }
            foreach (string s in weakness) { if (s.Length > 0) { _weakness.Add(s); } }
        }

        public ArmyGroup(ArmyGroup copy)
        {
            _faction = copy._faction;
            _units = copy._units;
            _maxUnitHp = copy._maxUnitHp;
            _attackDamage = copy._attackDamage;
            _attackType = copy._attackType;
            _initiative = copy._initiative;
            _immunity = new HashSet<string>();
            _weakness = new HashSet<string>();
            foreach (string s in copy._immunity) { _immunity.Add(s); }
            foreach (string s in copy._weakness) { _weakness.Add(s); }
        }
    }
}
