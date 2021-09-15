using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Day22
{
    public class Day22GeoIndexer
    {
        public int caveDepth;
        public Vector2Int target;

        private Dictionary<Vector2Int, int> memoIndex;
        private Dictionary<Vector2Int, int> memoErosion;

        public Day22GeoIndexer(Vector2Int target, int depth)
        {
            memoIndex = new Dictionary<Vector2Int, int>();
            memoErosion = new Dictionary<Vector2Int, int>();
            memoIndex[target] = 0;
            memoIndex[Vector2Int.zero] = 0;
            caveDepth = depth;
            this.target = target;
        }

        public int GetIndex(int x, int y)
        {
            return GetIndex(new Vector2Int(x, y));
        }
        public int GetIndex(Vector2Int pos)
        {
            if (memoIndex.ContainsKey(pos))
            {
                return memoIndex[pos];
            }

            if(pos.y == 0)
            {
                return mIndexReturn(pos, pos.x * 16807);
            }

            if(pos.x == 0)
            {
                return mIndexReturn(pos, pos.y * 48271);
            }

            return mIndexReturn(pos, GetErosion(pos.x - 1, pos.y) * GetErosion(pos.x, pos.y - 1));

        }

        public int GetErosion(int x, int y)
        {
            return GetErosion(new Vector2Int(x, y));
        }
        public int GetErosion(Vector2Int pos)
        {
            if (memoErosion.ContainsKey(pos))
            {
                return memoErosion[pos];
            }

            return mErosionReturn(pos, (GetIndex(pos) + caveDepth) % 20183);
        }

        public int GetType(int x, int y)
        {
            return GetType(new Vector2Int(x, y));
        }
        public int GetType(Vector2Int pos)
        {
            return (GetErosion(pos) % 3 + 3) % 3;
        }

        private int mIndexReturn(Vector2Int pos, int val)
        {
            memoIndex.Add(pos, val);
            return val;
        }
        private int mErosionReturn(Vector2Int pos, int val)
        {
            memoErosion.Add(pos, val);
            return val;
        }
    }
}
