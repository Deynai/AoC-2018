using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Day23
{
    public class Nanobot
    {
        public Vector3Int pos;
        public int radius;

        public Nanobot(int x, int y, int z, int r)
        {
            pos = new Vector3Int(x, y, z);
            radius = r;
        }
    }
}
