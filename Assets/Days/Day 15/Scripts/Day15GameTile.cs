using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day15GameTile
{
    public Day15RoomConstructor roomConstructor;

    public GameObject gameTile;
    public char tile;
    public bool walkableTile;
    public bool hasUnit;
    public Day15Unit unit;
    public Vector2Int pos;
    public Day15GameTile[] adjacentTiles = new Day15GameTile[4];
    public bool Walkable { get { return (walkableTile && !hasUnit); } }
    public int Order { get { return (pos.x + 1000 * pos.y); } }

    public Day15GameTile(char tile, Vector2Int pos, Day15RoomConstructor roomConstructor)
    {
        this.roomConstructor = roomConstructor;
        this.tile = tile;
        walkableTile = !tile.Equals('#');
        this.pos = pos;
        gameTile = roomConstructor.CreatePrefab(tile);
        gameTile.transform.position = new Vector3(pos.x, gameTile.transform.position.y, -pos.y);
        this.hasUnit = false;
    }
}
