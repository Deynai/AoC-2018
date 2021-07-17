using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day15UnitController : MonoBehaviour
{
    public GameObject goblin;
    public GameObject elf;

    public Day15Grid grid;
    public List<Day15Unit> goblins = new List<Day15Unit>();
    public List<Day15Unit> elves = new List<Day15Unit>();
    public List<Day15Unit> units = new List<Day15Unit>();

    private char[][] charGrid;
    private HashSet<char> unitChars = new HashSet<char> { 'E', 'G' };

    public void BuildUnits(Day15Grid _grid)
    {
        grid = _grid;
        charGrid = InputHelper.ParseInputCharArray(15);

        for (int i = 0; i < charGrid[0].Length; i++)
        {
            for (int j = 0; j < charGrid.Length; j++)
            {
                if (unitChars.Contains(charGrid[j][i]))
                {
                    Day15Unit newUnit = CreateUnit(charGrid[j][i]);
                    newUnit.Initialise(new Vector2Int(i, j));
                    grid.Grid[i, j].hasUnit = true;
                    grid.Grid[i, j].unit = newUnit;
                    //newUnit.transform.position = new Vector3(i, newUnit.transform.position.y, -j);
                }
            }
        }
    }

    private Day15Unit CreateUnit(char c)
    {
        Day15Unit unit = null;

        if (c.Equals('E'))
        {
            GameObject newElf = Instantiate(elf, transform, false);
            unit = newElf.GetComponent<Day15Elf>();
            elves.Add(unit);
        }
        else if (c.Equals('G'))
        {
            GameObject newGoblin = Instantiate(goblin, transform, false);
            unit = newGoblin.GetComponent<Day15Goblin>();
            goblins.Add(unit);
        }

        units.Add(unit);
        return unit;
    }

    public bool MoveUnit(Day15Unit unit, Vector2Int location)
    {
        if(location.Equals(unit.pos))
        {
            return false;
        }
        // validate moves?

        grid.ClearUnit(unit.pos);
        grid.SetUnit(location, unit);
        unit.pos = location;
        unit.transform.position = new Vector3(location.x, unit.transform.position.y, -location.y);
        return true;
    }

    public bool AttackAdjacentUnit(Day15Unit unit)
    {
        var adjacentHostiles = grid.Grid[unit.pos.x, unit.pos.y].adjacentTiles.Where(tile => tile.hasUnit && tile.unit.faction != unit.faction).Select(tile => tile.unit).ToList();
        var lowestHealthGroup = adjacentHostiles.GroupBy(u => u.health).OrderBy(group => group.Key).FirstOrDefault();
        if(lowestHealthGroup == null) { return false; }
        var target = lowestHealthGroup.OrderBy(u => u.Order).FirstOrDefault();

        if(target != null)
        {
            target.TakeDamage(unit.attack);
            ClearIfDead(target);
            return true;
        }
        return false;
    }

    private void ClearIfDead(Day15Unit unit)
    {
        if (unit.isDead)
        {
            grid.ClearUnit(unit.pos);
        }
    }

    public void ClearAllUnits()
    {
        foreach(Day15Unit unit in units)
        {
            grid.ClearUnit(unit.pos);
            Destroy(unit.gameObject);
        }

        units.Clear();
        elves.Clear();
        goblins.Clear();
    }
}
 