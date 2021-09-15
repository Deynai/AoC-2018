using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Day24
{
    public class BattleManager
    {
        private List<(ArmyGroup attacker, ArmyGroup defender)> battlePlan;

        private List<ArmyGroup> _armyGroups;
        private List<ArmyGroup> _immuneSystem;
        private List<ArmyGroup> _infection;

        private bool _isBattleOver = false;
        private int _battleResult = 0;
        private string[] _resultStrings = { "Ongoing", "Infection wins", "Immune System wins" };

        public bool IsBattleOver => _isBattleOver;
        public string ResultString => _resultStrings[_battleResult];
        public int ResultInt => _battleResult;

        public BattleManager(List<ArmyGroup> armyGroups, List<ArmyGroup> immuneSystem, List<ArmyGroup> infection)
        {
            _armyGroups = armyGroups;
            _immuneSystem = immuneSystem;
            _infection = infection;

            battlePlan = new List<(ArmyGroup attacker, ArmyGroup defender)>();
        }

        public void Battle()
        {
            int bp = 10000;
            while (!_isBattleOver)
            {
                if(bp-- < 0) { Debug.Log($"Battle breakpoint hit"); return; }

                ClearAllTargets();
                TargetSelectionPhase();
                AttackingPhase();
                CheckWinner();
            }
        }

        private void TargetSelectionPhase()
        {
            foreach (ArmyGroup ag in _armyGroups.OrderBy(ag => -ag.EffectivePower).ThenBy(ag => -ag.Initiative))
            {
                if (ag.IsDead) { continue; }
                if (ag.faction == Faction.Immune) { SelectTarget(ag, _infection); }
                else                              { SelectTarget(ag, _immuneSystem); }
            }
        }

        private void SelectTarget(ArmyGroup ag, List<ArmyGroup> enemies)
        {
            if(enemies.Where(e => e.IsValidTarget && !e.IsDead).Count() == 0) { return; }

            //ArmyGroup target = enemies.Where(e => e.IsValidTarget && !e.IsDead)
            //                          .GroupBy(e => e.PredictDamage(ag.Damage))
            //                          .OrderBy(dam => -dam.Key)
            //                          .First()
            //                          .GroupBy(e => e.EffectivePower)
            //                          .OrderBy(ep => -ep.Key)
            //                          .First()
            //                          .OrderBy(e => -e.Initiative)
            //                          .First();

            ArmyGroup target = enemies.Where(e => e.IsValidTarget && !e.IsDead)
                                      .OrderBy(e => -e.PredictDamage(ag.Damage))
                                      .ThenBy(e => -e.EffectivePower)
                                      .ThenBy(e => -e.Initiative)
                                      .First();

            if(target.PredictDamage(ag.Damage) > 0)
            {
                target.SetTarget();
                battlePlan.Add((ag, target));
            }
        }

        private void AttackingPhase()
        {
            foreach((ArmyGroup attacker, ArmyGroup defender) in battlePlan.OrderBy(f => -f.attacker.Initiative))
            {
                if (!attacker.IsDead) { attacker.DealDamage(defender); }
            }
        }

        private void ClearAllTargets()
        {
            battlePlan.Clear();
            foreach(ArmyGroup ag in _armyGroups)
            {
                ag.ClearTarget();
            }
        }

        private void CheckWinner()
        {
            int immuneAlive = _immuneSystem.Where(ag => !ag.IsDead).Count();
            int infectedAlive = _infection.Where(ag => !ag.IsDead).Count();

            if(immuneAlive == 0 && infectedAlive > 0) { _battleResult = 1; _isBattleOver = true; }
            if(infectedAlive == 0 && immuneAlive > 0) { _battleResult = 2; _isBattleOver = true; }
        }

        public void PrintCurrentState()
        {
            Debug.Log($"Immune System: {_immuneSystem.Select(ag => ag.Units).Aggregate((a, s) => a + s)}");
            foreach(ArmyGroup ag in _immuneSystem)
            {
                if (!ag.IsDead)
                {
                    Debug.Log(ag.ToString());
                }
            }

            Debug.Log($"Infection: {_infection.Select(ag => ag.Units).Aggregate((a, s) => a + s)}");
            foreach (ArmyGroup ag in _infection)
            {
                if (!ag.IsDead)
                {
                    Debug.Log(ag.ToString());
                }
            }
        }
    }
}
