using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Z3;
using UnityEngine;

namespace Day23
{
    public static class Day23OptimisationTools
    {
        // Check if a single nanobot is in range
        private static BoolExpr NanobotInRange(IntExpr[] currPos, ArithExpr[] botPos, Context ctx)
        {
            ArithExpr distance = ctx.MkAdd(MkDiff(currPos[0], botPos[0], ctx), MkDiff(currPos[1], botPos[1], ctx), MkDiff(currPos[2], botPos[2], ctx));

            return ctx.MkLe(distance, botPos[3]);
        }

        // manhattan distance from origin
        private static ArithExpr MkOriginDistance(IntExpr[] currPos, Context ctx)
        {
            ArithExpr distance = ctx.MkInt(0);
            IntExpr zero = ctx.MkInt(0);
            for (int i = 0; i < 3; i++)
            {
                distance = ctx.MkAdd(distance, MkDiff(currPos[i], zero, ctx));
            }

            return distance;
        }

        // distance between two numbers
        private static ArithExpr MkDiff(ArithExpr x, ArithExpr y, Context ctx)
        {
            return MkAbs(ctx.MkSub(x, y), ctx);
        }

        // simple abs(x)
        private static ArithExpr MkAbs(ArithExpr x, Context ctx)
        {
            return (ArithExpr)ctx.MkITE(ctx.MkLe(x, ctx.MkInt(0)), ctx.MkSub(ctx.MkInt(0), x), x);
        }


        public static void Solve(Nanobot[] bots)
        {
            using (Context ctx = new Context())
            {
                // We have vars x, y, z
                IntExpr[] posExpr = new IntExpr[3];
                posExpr[0] = ctx.MkIntConst("x");
                posExpr[1] = ctx.MkIntConst("y");
                posExpr[2] = ctx.MkIntConst("z");

                // we have an array of nanobots with vars x,y,z,r
                IntExpr[][] botExprs = new IntExpr[bots.Length][];
                for (int i = 0; i < bots.Length; i++)
                {
                    botExprs[i] = new IntExpr[4];
                    botExprs[i][0] = ctx.MkInt(bots[i].pos.x);
                    botExprs[i][1] = ctx.MkInt(bots[i].pos.y);
                    botExprs[i][2] = ctx.MkInt(bots[i].pos.z);
                    botExprs[i][3] = ctx.MkInt(bots[i].radius);
                }

                // sum = add(sum, ITE(in range))
                // basically sum += NanobotInRange(bot) ? 1 : 0
                ArithExpr sum = ctx.MkInt(0);
                IntExpr mkOne = ctx.MkInt(1);
                IntExpr mkZero = ctx.MkInt(0);
                for (int i = 0; i < bots.Length; i++)
                {
                    sum = ctx.MkAdd(sum, (ArithExpr)ctx.MkITE(NanobotInRange(posExpr, botExprs[i], ctx), mkOne, mkZero));
                }

                ArithExpr distFromOrigin = MkOriginDistance(posExpr, ctx);

                // we want to maximise the sum expression
                // and minimise the dist from origin
                Optimize sumInRange = ctx.MkOptimize();

                Optimize.Handle hMax = sumInRange.MkMaximize(sum);
                Optimize.Handle hMin = sumInRange.MkMaximize(distFromOrigin);

                Status check = sumInRange.Check();
                Debug.Log($"{check}");

                // turns out there's only one spot?
                Debug.Log($"hMax bounds: {hMax.Lower}, {hMax.Upper}");
                Debug.Log($"hMin bounds: {hMin.Lower}, {hMin.Upper}");

                Model result = sumInRange.Model;
                int distanceFromOrigin = 0;
                foreach (var ct in result.Consts)
                {
                    distanceFromOrigin += Mathf.Abs(int.Parse(ct.Value.ToString()));
                }
                Debug.Log($"Pos: ({result.ConstInterp(posExpr[0])}, {result.ConstInterp(posExpr[1])}, {result.ConstInterp(posExpr[2])})");
                Debug.Log($"Distance from Origin for final point: {distanceFromOrigin}");

                // Had an issue where the located spot was actually slihtly off, with a max of 966. Bug in the optimiser? Couldn't see how to reproduce it or what caused it. Happened on a couple of tests.

                ctx.Dispose();
            }
        }

    }
}