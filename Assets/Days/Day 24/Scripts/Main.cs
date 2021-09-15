using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Day24
{
    public class Main : MonoBehaviour
    {
        private List<ArmyGroup> _armyGroups;
        private List<ArmyGroup> _immuneSystem;
        private List<ArmyGroup> _infection;

        private void Start()
        {
            Initialise();

            Part1();
            Part2();
        }

        private void Initialise()
        {
            string[] input = InputHelper.ParseInputString(24).Replace("\r", "").Split(new string[] { "\n\n" }, System.StringSplitOptions.None);

            _armyGroups = new List<ArmyGroup>();

            _immuneSystem = input[0].Split('\n').Skip(1).Select(line => new ArmyGroup(Faction.Immune, line)).ToList();
            _infection = input[1].Split('\n').Skip(1).Select(line => new ArmyGroup(Faction.Infection, line)).ToList();

            _armyGroups.AddRange(_immuneSystem);
            _armyGroups.AddRange(_infection);
        }

        private void Part1()
        {
            Debug.Log($"Part 1:");
            List<ArmyGroup> immuneSystemCopy = CopyArmy(_immuneSystem);
            List<ArmyGroup> infectionCopy = CopyArmy(_infection);
            List<ArmyGroup> armyCopy = new List<ArmyGroup>();
            armyCopy.AddRange(immuneSystemCopy);
            armyCopy.AddRange(infectionCopy);

            BattleManager bm = new BattleManager(armyCopy, immuneSystemCopy, infectionCopy);

            //bm.PrintCurrentState();
            bm.Battle();
            bm.PrintCurrentState();

            print($"{bm.ResultString}");
        }

        private void Part2()
        {
            Debug.Log($"Part 2:");

            BattleManager bm = null;

            for(int boost = 0; boost < 1000; boost++)
            {
                List<ArmyGroup> boostedImmuneSystem = CopyArmy(_immuneSystem);
                List<ArmyGroup> infectionCopy = CopyArmy(_infection);
                List<ArmyGroup> armyCopy = new List<ArmyGroup>();
                foreach (ArmyGroup ag in boostedImmuneSystem) { ag.ApplyBoost(boost); }
                armyCopy.AddRange(boostedImmuneSystem);
                armyCopy.AddRange(infectionCopy);

                bm = new BattleManager(armyCopy, boostedImmuneSystem, infectionCopy);
                bm.Battle();
                if (bm.ResultInt.Equals(0))
                {
                    Debug.Log($"Battle ended without a winner at boost: {boost}");
                    bm.PrintCurrentState();
                }
                if (bm.ResultInt.Equals(2))
                {
                    Debug.Log($"Battle ended with Immune System victory. Boost required: {boost}");
                    bm.PrintCurrentState();
                    break;
                }
            }
        }

        private List<ArmyGroup> CopyArmy(List<ArmyGroup> army)
        {
            List<ArmyGroup> agCopy = new List<ArmyGroup>();
            foreach(ArmyGroup ag in army) { agCopy.Add(new ArmyGroup(ag)); }
            return agCopy;
        }
    }
}
