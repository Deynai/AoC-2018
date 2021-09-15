using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day23
{
    public class Main : MonoBehaviour
    {
        Nanobot[] bots;
        int[] bounds;

        internal struct Box
        {
            internal Vector3Int start;
            internal Vector3Int end;

            internal Box(Vector3Int start, Vector3Int end)
            {
                this.start = start;
                this.end = end;
            }
        }

        private void Start()
        {
            Initialise();

            //Part1(); // part 1 is trivial
            //Part2();

            // Part 2 explanation / notes:
            // The idea behind my first approach to part2 was similar to a binary search
            // Pick a spot to start from (arbitrarily I decided to use the mean position of all nanobots), then move steps in each direction a certain distance away
            // e.g, starting at x,y,z, search at: (x - 1024, y, z), (x + 1024, y, z), (x - 1024, y - 1024, z), etc. counting the number of bots in range at each location
            // continue this method, keeping track of maximal bots, using the max bots location as the new starting position, until number of bots remains stable
            // move a step down and start searching in a similar way but this time with more precision, += 512, then 256, etc
            // the idea is that this would quickly find a dense region and then focus in on the local maximum with a fraction of the calculations required
            // this is not very robust and can easily get stuck in the wrong places or miss maximas entirely. After figuring out more on the problem it's clear the region is dense and not smooth
            // in fact a single location sits on the border of dozens of bots - a rough approximation algorithm clearly doesn't work for finding a needle in a haystack like this

            // Attempt 2: Another idea - evenly partitioning the region into cubes - the idea was to subdivide cubes and find the cube with the most number of nanobots in, then use that
            // as the starting point for the search in attempt 1. Rushed the code, made mistakes, and generally I don't think this idea really works either. 
            // Something more refined that calcs inRange bots and subdivides in a cleverer way is probably required. I've seen others solve this using similar methods.
            // All of the broken and terrible part2 attempts are below.

            // Something entirely new - Z3, SMT, optimisation tools
            // I had no idea about this until looking up possible approaches - the power behind it is quite ridiculous, though it's a bit annoying to use in C#
            // Using this in the future as the main part of any project should probably be done in python. I still don't fully understand how it works or how to really optimise it.
            // Set variables, set constraints, and the result gets chugged out behind the scenes. Takes somewhere around 60-90s to run. 
            // Almost certainly can be improved on in terms of efficiency - setting initial constraints, etc.
            Day23OptimisationTools.Solve(bots);

            // Check surrounding area of the correct spot - when I saw this it became apparent how doomed the initial idea was. 1,1,1 offset leaves the radius of 46 bots!

            //for(int i = -1; i <= 1; i++)
            //{
            //    for(int j = -1; j <= 1; j++)
            //    {
            //        for(int k = -1; k <= 1; k++)
            //        {
            //            Vector3Int pos = new Vector3Int(19992232 + i, 58718915 + j, 22274751 + k);
            //            print($"Offset: {i},{j},{k}, Bots in range: {CheckAllDistances(pos)}");
            //        }
            //    }
            //}
        }

        private void Initialise()
        {
            string[] input = InputHelper.ParseInputArray(23);

            bots = input.Select(line => Regex.Matches(line, "-?\\d+")).Select(matches => matches.Cast<Match>().Select(n => int.Parse(n.Value)).ToArray())
                                                                                .Select(l => new Nanobot(l[0], l[1], l[2], l[3]))
                                                                                .ToArray();

            bounds = new int[6];
            for(int i = 0; i < bots.Length; i++)
            {
                if(bots[i].pos.x < bounds[0]) { bounds[0] = bots[i].pos.x; }
                else if(bots[i].pos.x > bounds[1]) { bounds[1] = bots[i].pos.x; }
                if(bots[i].pos.y < bounds[2]) { bounds[2] = bots[i].pos.y; }
                else if(bots[i].pos.y > bounds[3]) { bounds[3] = bots[i].pos.y; }
                if(bots[i].pos.z < bounds[4]) { bounds[4] = bots[i].pos.z; }
                else if(bots[i].pos.z > bounds[5]) { bounds[5] = bots[i].pos.z; }
            }
        }

        private void Part1()
        {

            Nanobot strongestBot = bots.OrderBy(b => b.radius).Last();

            int botsInRange = 0;
            for(int i = 0; i < bots.Length; i++)
            {
                if(ManhattanDistance(bots[i].pos, strongestBot.pos) <= strongestBot.radius)
                {
                    botsInRange++;
                }
            }

            print($"{botsInRange}");
        }

        private void Part2()
        {
            Vector3Int origin = GetStartingPoint();
            print($"{origin}");
            //-----
            //int[] scales = new int[] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 1500, 2500, 4000, 6000, 8000, 10000, 12000, 20000, 30000, 35000, 5000, 7000, 9000, 12000, 15000, 20000, 25000, 40000, 10000, 50000, 90000};
            Vector3Int scanPos = origin;
            for(int i = 10; i >= 0; i--)
            {
                int scale = 1 << i; // left shift, easy powers of 2
                scanPos = ScanUntilStable(18, scale, scanPos);
            }

            print($"{scanPos}, Bots: {CheckAllDistances(scanPos)}, Manhattan Distance: {ManhattanDistance(Vector3Int.zero, scanPos)}");
        }

        private Vector3Int GetStartingPoint()
        {
            //return GetMean();
            return new Vector3Int(19520917, 57169044, 21196195);
        }

        private Vector3Int GetBoxPartitioning()
        {
            int steps = 100;
            List<Nanobot> allbots = bots.ToList();
            Box firstBox = new Box(new Vector3Int(bounds[0], bounds[2], bounds[4]), new Vector3Int(bounds[1], bounds[3], bounds[5]));

            LinkedList<(List<Nanobot>, Box, int)> boxes = new LinkedList<(List<Nanobot>, Box, int)>();
            boxes.AddFirst((allbots, firstBox, 0));

            int bp = 0;
            while(bp++ < steps)
            {
                (List<Nanobot> boxBots, Box box, int divs) boxData = boxes.First();
                boxes.RemoveFirst();

                if(boxData.divs > 6) { return GetCentreOfBox(boxData.box); }

                // boxes
                Vector3Int middle = (boxData.box.end + boxData.box.start) / 2;

                AddNextBox(boxData.box.start, middle, boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x + middle.x, boxData.box.start.y, boxData.box.start.z),
                                new Vector3Int(boxData.box.end.x, middle.y, middle.z), boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x, boxData.box.start.y + middle.y, boxData.box.start.z),
                                new Vector3Int(middle.x, boxData.box.end.y, middle.z), boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x, boxData.box.start.y, boxData.box.start.z + middle.z),
                                new Vector3Int(middle.x, middle.y, boxData.box.end.z), boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x + middle.x, boxData.box.start.y + middle.y, boxData.box.start.z),
                                new Vector3Int(boxData.box.end.x, boxData.box.end.y, middle.z), boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x, boxData.box.start.y + middle.y, boxData.box.start.z + middle.z),
                                new Vector3Int(middle.x, boxData.box.end.y, boxData.box.end.z), boxData, boxes);

                AddNextBox(new Vector3Int(boxData.box.start.x + middle.x, boxData.box.start.y, boxData.box.start.z + middle.z),
                                new Vector3Int(boxData.box.end.x, middle.y, boxData.box.end.z), boxData, boxes);

                AddNextBox(middle, boxData.box.end, boxData, boxes);
            }

            return Vector3Int.zero;
        }

        private Vector3Int GetCentreOfBox(Box box)
        {
            return (box.start + box.end) / 2;
        }

        private void AddNextBox(Vector3Int newStart, Vector3Int newEnd, (List<Nanobot> boxBots, Box box, int divs) boxData, LinkedList<(List<Nanobot>, Box, int)> boxes)
        {
            Box newBox = new Box(newStart, newEnd);
            List<Nanobot> newBots = GetBotsInBox(boxData.boxBots, newBox);
            AddBoxToBoxesLL(boxes, newBots, newBox, boxData.divs + 1);
        }

        private void AddBoxToBoxesLL(LinkedList<(List<Nanobot>, Box, int)> boxes, List<Nanobot> botList, Box box, int divs)
        {
            if(boxes.Count == 0) { boxes.AddFirst((botList, box, divs)); return; }

            var currNode = boxes.First;
            for(int i = 0; i < boxes.Count; i++)
            {
                if(botList.Count > currNode.Value.Item1.Count)
                {
                    boxes.AddBefore(currNode, (botList, box, divs));
                    return;
                }

                currNode = currNode.Next;
            }

            boxes.AddLast((botList, box, divs));
        }

        private List<Nanobot> GetBotsInBox(List<Nanobot> parentBoxBots, Box box)
        {
            List<Nanobot> childBoxBots = new List<Nanobot>();
            for(int i = 0; i < parentBoxBots.Count; i++)
            {
                if(IsBotInBox(box, parentBoxBots[i]))
                {
                    childBoxBots.Add(parentBoxBots[i]);
                }
            }
            return childBoxBots;
        }

        private bool IsBotInBox(Box box, Nanobot bot)
        {
            if (bot.pos.x < box.start.x || bot.pos.x > box.end.x) { return false; }
            if (bot.pos.y < box.start.y || bot.pos.y > box.end.y) { return false; }
            if (bot.pos.z < box.start.z || bot.pos.z > box.end.z) { return false; }
            return true;
        }

        private Vector3Int GetMean()
        {
            Vector3Int maxPos = Vector3Int.zero;

            long x = 0;
            long y = 0;
            long z = 0;

            for (int i = 0; i < bots.Length; i++)
            {
                x += bots[i].pos.x;
                y += bots[i].pos.y;
                z += bots[i].pos.z;
            }

            x = Mathf.RoundToInt(x / (float)bots.Length);
            y = Mathf.RoundToInt(y / (float)bots.Length);
            z = Mathf.RoundToInt(z / (float)bots.Length);

            return new Vector3Int((int)x, (int)y, (int)z);
        }

        private Vector3Int ScanUntilStable(int scanSize, int scanScale, Vector3Int origin)
        {
            Vector3Int oldPos = origin;
            Vector3Int newPos = origin;
            int bp = 0;
            while (bp++ < 1000)
            {
                newPos = PerformScan(scanSize, scanScale, oldPos);
                if(newPos == oldPos) { break; }
                else
                {
                    oldPos = newPos;
                }
            }

            print($"Finished {scanScale} scan in {bp} steps. Pos: {newPos}");
            return newPos;
        }

        private Vector3Int PerformScan(int scanSize, int scanScale, Vector3Int origin)
        {
            Vector3Int maxPos = origin;
            int maxBots = CheckAllDistances(origin);
            //int maxBots = 0;

            for(int i = -scanSize; i < scanSize+1; i++)
            {
                for(int j = -scanSize; j < scanSize+1; j++)
                {
                    for(int k = -scanSize; k < scanSize+1; k++)
                    {
                        if(i == 0 && j == 0 && k == 0) { continue; }
                        Vector3Int pos = origin + new Vector3Int(i, j, k) * scanScale;
                        int numberOfBots = CheckAllDistances(pos);
                        if (numberOfBots > maxBots)
                        {
                            maxBots = numberOfBots;
                            maxPos = pos;
                        }
                    }
                }
            }

            print($"Bots found: {maxBots}");
            return maxPos;
        }

        private int CheckAllDistances(Vector3Int pos)
        {
            int numberOfBots = 0;
            for(int i = 0; i < bots.Length; i++)
            {
                if(ManhattanDistance(bots[i].pos, pos) <= bots[i].radius)
                {
                    numberOfBots++;
                }
            }
            return numberOfBots;
        }

        private int ManhattanDistance(Vector3Int pos1, Vector3Int pos2)
        {
            return Mathf.Abs(pos1.x - pos2.x) + Mathf.Abs(pos1.y - pos2.y) + Mathf.Abs(pos1.z - pos2.z);
        }

    }
}
