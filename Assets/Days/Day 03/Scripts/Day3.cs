using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class Day3 : MonoBehaviour
{
    public GameObject fabricObject;
    public int[,] fabric;
    public Claim[] input;

    public struct Claim
    {
        public int x; // x pos
        public int y; // y pos
        public int w; // width of square
        public int h; // height of square
        public int id; // id

        public Claim(int x, int y, int w, int h, int id)
        {
            this.x = x;
            this.y = y;
            this.w = w;
            this.h = h;
            this.id = id;
        }
    }

    private Claim ParseClaim(string str)
    {
        MatchCollection matches = Regex.Matches(str, "\\d+");
        Claim claim = new Claim(int.Parse(matches[1].Value), int.Parse(matches[2].Value), int.Parse(matches[3].Value), int.Parse(matches[4].Value), int.Parse(matches[0].Value));
        return claim;
    }

    private IEnumerator Solution()
    {
        yield return Part1();
        Part2();
    }

    private IEnumerator Part1()
    {
        Regex digit = new Regex("\\d+");
        input = InputHelper.ParseInputArray(3).Select(str => ParseClaim(str)).ToArray();
        bool debug = false;

        // find max array dimensions
        int max_x = input.Select(claim => claim.x + claim.w).Max();
        int max_y = input.Select(claim => claim.y + claim.h).Max();

        // create the array
        fabric = new int[max_x + 1, max_y + 1];
        if (debug) { print($"Created array with size {max_x + 1},{max_y + 1}"); };

        Texture2D texture = new Texture2D(max_x + 1, max_y + 1);
        fabricObject.GetComponent<Renderer>().material.mainTexture = texture;

        for(int i = 0; i < max_x + 1; i++)
        {
            for(int j = 0; j < max_y + 1; j++)
            {
                Color col = new Color(0.8f, 0.1f, 0.1f);
                texture.SetPixel(i, j, col);
            }
        }
        texture.Apply();
        yield return null;

        // for each entry, add one to each tile used
        foreach (Claim claim in input)
        {
            if (debug) { print($"Claim: {claim.x},{claim.y} : {claim.w},{claim.h}"); };
            for(int i = claim.x + 1; i < claim.x + claim.w + 1; i++)
            {
                for(int j = claim.y + 1; j < claim.y + claim.h + 1; j++)
                {
                    if(fabric[i,j] == 0)
                    {
                        texture.SetPixel(i, j, Color.white);
                    }
                    else if(fabric[i,j] == 1)
                    {
                        texture.SetPixel(i, j, Color.black);
                    }
                    fabric[i,j]++;
                }
            }
            texture.Apply();
            yield return null;
        }

        // loop through whole array and count squares with more than 1 claim
        long countDisputedClaims = 0;
        for (int i = 0; i < fabric.GetLength(1); i++)
        {
            for(int j = 0; j < fabric.GetLength(0); j++)
            {
                if(fabric[i,j] > 1)
                {
                    countDisputedClaims++;
                }
            }
        }

        print(countDisputedClaims);
        yield break;
    }

    private void Part2()
    {
        foreach(Claim claim in input)
        {
            if (CheckIntact(claim))
            {
                Texture2D texture = (Texture2D) fabricObject.GetComponent<Renderer>().material.mainTexture;
                for (int i = claim.x + 1; i < claim.x + claim.w + 1; i++)
                {
                    for (int j = claim.y + 1; j < claim.y + claim.h + 1; j++)
                    {
                        texture.SetPixel(i, j, Color.green);
                    }
                }
                texture.Apply();
                print($"Intact claim with id: {claim.id}");
                break;
            }
        }
    }

    private bool CheckIntact(Claim claim)
    {
        for (int i = claim.x + 1; i < claim.x + claim.w + 1; i++)
        {
            for (int j = claim.y + 1; j < claim.y + claim.h + 1; j++)
            {
                if (fabric[i, j] != 1)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void Start()
    {
        StartCoroutine(Solution());
    }
}
