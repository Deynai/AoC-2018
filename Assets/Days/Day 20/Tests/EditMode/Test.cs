using NUnit.Framework;
using UnityEngine;

public class Test
{
    [Test]
    public void Test1()
    {
        int longestRoute = ParseRegexBackwardsFurthest("^WNE$");
        Assert.AreEqual(longestRoute, 3);
    }

    [Test]
    public void Test2()
    {
        int longestRoute = ParseRegexBackwardsFurthest("^ENWWW(NEEE|SSE(EE|N))$");
        Assert.AreEqual(longestRoute, 10);
    }

    [Test]
    public void Test3()
    {
        int longestRoute = ParseRegexBackwardsFurthest("^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$");
        Assert.AreEqual(longestRoute, 18);
    }

    [Test]
    public void Test4()
    {
        int longestRoute = ParseRegexBackwardsFurthest("^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$");
        Assert.AreEqual(longestRoute, 23);
    }

    [Test]
    public void Test5()
    {
        int longestRoute = ParseRegexBackwardsFurthest("^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$");
        Assert.AreEqual(longestRoute, 31);
    }
    
    private int ParseRegexBackwardsFurthest(string input)
    {
        Day20RegexParser parser = new Day20RegexParser(input);
        parser.ParseRegex();

        int[] bounds = parser.GetPaddedBounds();
        int[,] map = Day20MapBuilder.GenerateMap(parser.GetRooms(), bounds);
        return Day20BFS.FindFurthestRoom(map, bounds);
    }
}
