using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Day9MarbleManager
{
    int playerCount;
    long[] playerScore;
    int marbleCount;
    List<int> marbles;
    int currentIndex;

    Day9Marble currentMarble;

    class Day9Marble
    {
        public int id;
        public Day9Marble next;
        public Day9Marble prev;

        public Day9Marble(int id)
        {
            this.id = id;
            next = null;
            prev = null;
        }
    }

    public long[] Scores { get { return playerScore; } }

    public Day9MarbleManager(int playerCount, int marbleCount)
    {
        this.playerCount = playerCount;
        this.marbleCount = marbleCount;
        playerScore = new long[playerCount];
        marbles = new List<int>();
        marbles.Add(0);
        currentIndex = 0;
    }

    public void RunGame()
    {
        for(int marbleNumber = 1; marbleNumber < marbleCount+1; marbleNumber++)
        {
            int currentPlayer = marbleNumber % playerCount;

            if(marbleNumber % 23 == 0)
            {
                playerScore[currentPlayer] += ClaimPointsMarble(marbleNumber);
            }
            else
            {
                InsertNextMarble(marbleNumber);
            }
        }
    }

    public void RunGameLL()
    {
        // The method with a list/insert based approach took too long for part 2
        // Linkedlist works considerably faster
        currentMarble = new Day9Marble(0);
        currentMarble.next = currentMarble;
        currentMarble.prev = currentMarble;

        for (int marbleNumber = 1; marbleNumber < marbleCount + 1; marbleNumber++)
        {
            int currentPlayer = marbleNumber % playerCount;

            if (marbleNumber % 23 == 0)
            {
                playerScore[currentPlayer] += ClaimPointsMarbleLL(marbleNumber);
            }
            else
            {
                InsertNextMarbleLL(marbleNumber);
            }
        }
    }

    private void InsertNextMarble(int marbleNumber)
    {
        int insertionIndex = ((currentIndex + 1) % marbles.Count()) + 1;
        marbles.Insert(insertionIndex, marbleNumber);
        currentIndex = insertionIndex;
    }

    private int ClaimPointsMarble(int marbleNumber)
    {
        int points = marbleNumber;
        int removeIndex = (((currentIndex - 7) % marbles.Count()) + marbles.Count()) % marbles.Count();
        points += marbles[removeIndex];
        marbles.RemoveAt(removeIndex);
        currentIndex = removeIndex % marbles.Count();
        return points;
    }

    private void InsertNextMarbleLL(int marbleNumber)
    {
        Day9Marble newMarble = new Day9Marble(marbleNumber);
        currentMarble.next.next.prev = newMarble;
        newMarble.next = currentMarble.next.next;
        newMarble.prev = currentMarble.next;
        currentMarble.next.next = newMarble;
        currentMarble = newMarble;
    }

    private int ClaimPointsMarbleLL(int marbleNumber)
    {
        int points = marbleNumber;
        Day9Marble marbleToRemove = currentMarble.prev;
        for (int i = 0; i < 6; i++)
        {
            marbleToRemove = marbleToRemove.prev;
        }
        marbleToRemove.prev.next = marbleToRemove.next;
        marbleToRemove.next.prev = marbleToRemove.prev;
        points += marbleToRemove.id;
        currentMarble = marbleToRemove.next;
        return points;
    }
}
