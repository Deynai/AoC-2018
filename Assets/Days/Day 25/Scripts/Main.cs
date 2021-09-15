using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day25
{
    internal class Pos
    {
        internal int x;
        internal int y;
        internal int z;
        internal int w;
        internal int constellation;
        internal bool isInConstellation;

        internal Pos(int _x, int _y, int _z, int _w)
        {
            x = _x;
            y = _y;
            z = _z;
            w = _w;
        }
        internal Pos(int[] pos)
        {
            x = pos[0];
            y = pos[1];
            z = pos[2];
            w = pos[3];
        }
    }

    public class Main : MonoBehaviour
    { 
        private void Start()
        {
            Part1();
        }

        private void Part1()
        {
            Pos[] stars = InputHelper.ParseInputArray(25).Select(line => new Pos(Regex.Matches(line, "-?\\d+").Cast<Match>().Select(n => int.Parse(n.Value)).ToArray())).ToArray();

            int cons = 0;
            for(int i = 0; i < stars.Length; i++)
            {
                if (!stars[i].isInConstellation)
                {
                    ChainAllStars(stars, stars[i], cons, i);
                    cons++;
                }
            }

            print($"Number of constellations: {cons}");
        }

        private void ChainAllStars(Pos[] stars, Pos star, int cons, int index)
        {
            Queue<Pos> chainStars = new Queue<Pos>();
            chainStars.Enqueue(star);

            while(chainStars.Count > 0)
            {
                Pos currStar = chainStars.Dequeue();
                currStar.constellation = cons;

                for (int i = 0; i < stars.Length; i++)
                {
                    if (stars[i].isInConstellation) { continue; }
                    if (Dist(currStar, stars[i]) <= 3)
                    {
                        chainStars.Enqueue(stars[i]);
                        stars[i].isInConstellation = true;
                    }
                }
            }
        }

        private int Dist(Pos a, Pos b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z) + Mathf.Abs(a.w - b.w);
        }
    }
}
