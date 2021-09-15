using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Day22
{
    public static class Day22Tile
    {
        public enum Tile
        {
            rocky,
            wet,
            narrow
        }

        public struct Square
        {
            int x;
            int y;
            Tile tile;

            public Square(int _x, int _y, int _tile)
            {
                x = _x;
                y = _y;
                tile = (Tile)_tile;
            }

            public Square(int _x, int _y, Tile _tile)
            {
                x = _x;
                y = _y;
                tile = _tile;
            }
       
        }
    }
}
