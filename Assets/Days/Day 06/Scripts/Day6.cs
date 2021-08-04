using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class Day6 : MonoBehaviour
{
    List<Point> points;

    class Point
    {
        public int id;
        public int x;
        public int y;
        public bool isFinite;
        public int area;
        public Point(int id, int x, int y)
        {
            this.id = id;
            this.x = x;
            this.y = y;
            isFinite = true;
            area = 0;
        }
    }

    private void Part1()
    {
        (int x, int y)[] inputPoints = InputHelper.ParseInputArray(6).Select(p => { MatchCollection matches = Regex.Matches(p, "\\d+");
                                                                               return (int.Parse(matches[0].Value), int.Parse(matches[1].Value));
                                                                             }).ToArray();
        points = new List<Point>();
        for(int i = 0; i < inputPoints.Length; i++)
        {
            points.Add(new Point(i, inputPoints[i].x, inputPoints[i].y));
        }

        int min_x = points.Select(p => p.x).Min();
        int max_x = points.Select(p => p.x).Max();
        int min_y = points.Select(p => p.y).Min();
        int max_y = points.Select(p => p.y).Max();

        // create an array around all points with 1 border space
        // check all border points and flag all closest points as infinite
        for (int i = min_x - 1; i < max_x + 2; i++)
        {
            FlagInfinitePoint((i, min_y - 1), points);
            FlagInfinitePoint((i, max_y + 1), points);
        }

        for (int j = min_y - 1; j < max_y + 2; j++)
        {
            FlagInfinitePoint((min_x - 1, j), points);
            FlagInfinitePoint((max_x + 1, j), points);
        }

        // go through the full array and add one to each point for each square which is closest to it
        for (int i = min_x; i < max_x + 1; i++)
        {
            for (int j = min_y; j < max_y + 1; j++)
            {
                int closest = GetClosestPoint((i, j), points);
                if(closest != -1)
                {
                    points[closest].area++;
                }
            }
        }

        // take the max of the non-infinite points
        int maxArea = points.Where(x => x.isFinite).Select(p => p.area).Max();

        print(maxArea);
    }

    private void Part2()
    {
        int min_x = points.Select(p => p.x).Min();
        int max_x = points.Select(p => p.x).Max();
        int min_y = points.Select(p => p.y).Min();
        int max_y = points.Select(p => p.y).Max();

        int validArea = 0;
        for (int i = min_x; i < max_x + 1; i++)
        {
            for (int j = min_y; j < max_y + 1; j++)
            {
                validArea += CheckAllPointsDistanceSum((i, j), points, 10000) ? 1 : 0;
            }
        }
        print(validArea);
    }

    private int Distance((int x, int y) a, (int x, int y) b)
    {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }
    private int Distance((int x, int y) a, Point b)
    {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }
    private int Distance(Point a, Point b)
    {
        return (Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y));
    }
    private void FlagInfinitePoint((int x, int y) a, List<Point> points)
    {
        int closestPoint = GetClosestPoint((a.x, a.y), points);
        if (closestPoint != -1)
        {
            points[closestPoint].isFinite = false;
        }
    }

    // return -1 if more than one closest point, else return the index of the points array
    private int GetClosestPoint((int x, int y) a, List<Point> points)
    {
        List<(int, int)> distances = new List<(int, int)>();
        for(int i = 0; i < points.Count; i++)
        {
            distances.Add((i, Distance(a, points[i])));
        }

        distances = distances.OrderBy(d => d.Item2).Take(2).ToList();
        return distances[0].Item2 == distances[1].Item2 ? -1 : distances[0].Item1;
    }

    private bool CheckAllPointsDistanceSum((int x, int y) a, List<Point> points, int range)
    {
        int distanceSum = 0;
        for (int i = 0; i < points.Count; i++)
        {
            distanceSum += Distance(a, points[i]);
        }

        return distanceSum < range;
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
