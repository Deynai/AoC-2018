using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day15GameController : MonoBehaviour
{
    public Day15RoomConstructor roomConstructor;
    public Day15UnitController unitController;

    private Day15Grid grid;
    private Day15Pathfinding pathfinding;

    private void SetupInitialState()
    {
        grid = roomConstructor.BuildRoom();
        unitController.BuildUnits(grid);
        pathfinding = new Day15Pathfinding(grid);
    }

    private IEnumerator Solution()
    {
        yield return Single();
        yield return new WaitForSeconds(5.0f);
        yield return Part2();
    }

    private IEnumerator Part2()
    {
        bool foundElfVictory = false;

        int lowAttack = 3;
        int elfAttack = 3;
        int highAttack = 200;

        while (!foundElfVictory)
        {
            unitController.ClearAllUnits();
            unitController.BuildUnits(grid);

            // set all elves to a different attack level
            elfAttack = GetNextElfAttack(lowAttack, highAttack);

            print($"Elf attack: {elfAttack}");
            foreach (Day15Unit elf in unitController.elves)
            {
                elf.attack = elfAttack;
            }
            yield return Single();

            // binary search on elf attack to find minimal point at which no elves die
            // low end determines failed run, high end determines success, when they are 1 apart we have found it
            if(unitController.elves.Where(e => e.isDead).Count() == 0)
            {
                //print($"Elf Flawless Victory with attack of: {elfAttack}");
                //foundElfVictory = true;
                highAttack = elfAttack;
            }
            else
            {
                lowAttack = elfAttack;
            }

            if(highAttack-lowAttack == 1)
            {
                print($"Flawless Elf Victory with an attack of {highAttack}");
                foundElfVictory = true;
            }
        }

        yield break;
    }

    private int GetNextElfAttack(int low, int high)
    {
        return (low + high) / 2;
    }

    private IEnumerator Single()
    {
        int roundsCount = 0;
        bool combatFinished = false;
        while(!combatFinished)
        {
            foreach (Day15Unit unit in unitController.units.OrderBy(unit => unit.Order))
            {
                // If this unit has been killed by an earlier unit, skip
                if (unit.isDead) { continue; }
                if (unitController.units.Where(u => !u.isDead && (u.faction != unit.faction)).Count().Equals(0))
                {
                    combatFinished = true;
                    break;
                }

                // Movement Phase
                Vector2Int move = pathfinding.BFSMoveToNearest(unit.pos);
                if(unitController.MoveUnit(unit, move)){
                    //yield return null;
                }

                // Attack Phase
                if (unitController.AttackAdjacentUnit(unit))
                {
                    //yield return null;
                }

                //yield return null;
            }
            roundsCount++;
            //yield return null;
            yield return new WaitForSeconds(0.1f);
        }

        long result = unitController.units.Select(u => u.health).Aggregate((health, sum) => sum + health);
        result = result * (roundsCount-1);
        print($"Result after {roundsCount} rounds: {result}");
        yield break;
    }

    private void Start()
    {
        SetupInitialState();

        StartCoroutine(Solution());
    }
}
