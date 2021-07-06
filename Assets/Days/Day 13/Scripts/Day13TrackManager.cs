using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Day13TrackManager : MonoBehaviour
{
    public GameObject straightTrack;
    public GameObject curveTrack;
    public GameObject intersectTrack;
    public GameObject cart;
    public GameObject cartParent;

    private char[][] input;
    private int[][] cartCheck;
    private GameObject[][] tracks;
    private List<Day13Cart> carts; // list of carts
    private Dictionary<int, Day13Cart> cartDict;
    private bool crashOccurred = false;

    private GameObject cartToFollow;
    private Vector3 cameraOffset = new Vector3(0,9,-7);

    private void BuildTracks()
    {
        cartDict = new Dictionary<int, Day13Cart>();
        input = InputHelper.ParseInputCharArrayNoTrim(13);
        tracks = new GameObject[input.Length][];
        cartCheck = new int[input.Length][];

        // place straight edges and intersections
        for (int j = 0; j < input.Length; j++)
        {
            tracks[j] = new GameObject[input[j].Length];
            cartCheck[j] = new int[input[j].Length];
            for(int i = 0; i < input[j].Length; i++)
            {
                switch (input[j][i])
                {
                    // empty space
                    case ' ': // do nothing
                        break;
                    // horizontal track
                    case '-': 
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        break;
                    // vertical track
                    case '|':
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        tracks[j][i].transform.Rotate(Vector3.up * 90);
                        break;
                    // intersection
                    case '+':
                        tracks[j][i] = Instantiate(intersectTrack, transform, false);
                        break;

                    // need to rotate this depending on which two sides are connected
                    // checking one square to the west should be sufficient, with a boundary check
                    case '\\':
                        tracks[j][i] = Instantiate(curveTrack, transform, false);
                        if(i > 0 && (input[j][i-1].Equals('-') || input[j][i - 1].Equals('+') || input[j][i - 1].Equals('<') || input[j][i - 1].Equals('>')))
                        {
                            // then it's a south-west connection
                        }
                        else
                        {
                            // then it's not
                            tracks[j][i].transform.Rotate(Vector3.up * 180);
                        }
                        break;

                    case '/':
                        tracks[j][i] = Instantiate(curveTrack, transform, false);
                        if (i > 0 && (input[j][i - 1].Equals('-') || input[j][i - 1].Equals('+') || input[j][i - 1].Equals('<') || input[j][i - 1].Equals('>')))
                        {
                            // then it's a north-west connection
                            tracks[j][i].transform.Rotate(Vector3.up * -90);
                        }
                        else
                        {
                            // then it's not
                            tracks[j][i].transform.Rotate(Vector3.up * 90);
                        }

                        break;

                    // horizontal tracks with a cart on
                    case '>':
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        AddCart(new Vector3(i, 0, j), 1);
                        cartCheck[j][i] = carts.Count();
                        break;

                    case '<':
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        AddCart(new Vector3(i, 0, j), 3);
                        cartCheck[j][i] = carts.Count();
                        break;

                    // vertical tracks with a cart on
                    case 'v':
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        tracks[j][i].transform.Rotate(Vector3.up * 90);
                        AddCart(new Vector3(i, 0, j), 0);
                        cartCheck[j][i] = carts.Count();
                        break;

                    case '^':
                        tracks[j][i] = Instantiate(straightTrack, transform, false);
                        tracks[j][i].transform.Rotate(Vector3.up * 90);
                        AddCart(new Vector3(i, 0, j), 2);
                        cartCheck[j][i] = carts.Count();
                        break;

                    default:
                        break;
                }

                if (tracks[j][i] != null)
                {
                    tracks[j][i].transform.position = new Vector3(i, 0, j);
                }
            }
        }
    }

    private void AddCart(Vector3 pos, int facing)
    {
        GameObject newCart = Instantiate(cart, cartParent.transform, false);
        newCart.GetComponent<Day13Cart>().Initialise(pos, facing);
        carts.Add(newCart.GetComponent<Day13Cart>());
        cartDict.Add(carts.Count(), newCart.GetComponent<Day13Cart>());
    }

    private IEnumerator Part1()
    {
        cartToFollow = carts[0].gameObject;
        int bp = 0;
        while (!crashOccurred)
        {
            yield return RunCarts();
            if(bp++ > 3000) { Debug.Log($"Hit bp, no collision after 3000 ticks"); break; }
            yield return null;
        }

        yield return Part2();
        yield break;
    }

    private IEnumerator Part2()
    {
        cartToFollow = carts[0].gameObject;

        while (carts.Count > 1)
        {
            yield return RunCarts();
            if (crashOccurred)
            {
                foreach(Day13Cart cart in carts)
                {
                    if (cart.hasCrashed)
                    {
                        Destroy(cart.gameObject);
                    }
                }
                carts = carts.Where(c => !c.hasCrashed).ToList();
                cartToFollow = carts[0].gameObject;
            }
        }

        Debug.Log($"Final cart position: {carts[0].transform.position}");
        yield break;
    }

    private IEnumerator RunCarts()
    {
        crashOccurred = false;

        foreach(Day13Cart cart in carts.OrderBy(c => c.moveOrder))
        {
            if (cart.hasCrashed) { continue; }

            (int x, int y) currPos = cart.pos;
            cart.MoveForward();
            (int x, int y) pos = cart.pos;

            if (cartCheck[pos.y][pos.x] > 0)
            {
                int cart1 = cartCheck[currPos.y][currPos.x];
                int cart2 = cartCheck[pos.y][pos.x];
                cartCheck[currPos.y][currPos.x] = 0;
                cartCheck[pos.y][pos.x] = 0;
                cartDict[cart1].Crash();
                cartDict[cart2].Crash();
            }
            else
            {
                cartCheck[pos.y][pos.x] = cartCheck[currPos.y][currPos.x];
                cartCheck[currPos.y][currPos.x] = 0;
            }

            if (input[pos.y][pos.x].Equals('\\'))
            {
                cart.TurnA();
            }
            else if (input[pos.y][pos.x].Equals('/'))
            {
                cart.TurnB();
            }
            else if (input[pos.y][pos.x].Equals('+'))
            {
                cart.IntersectionTurn();
            }

            if (cart.hasCrashed)
            {
                Debug.Log($"Cart crashed at location: {pos.x},{pos.y}");
                crashOccurred = true;
            }
        }

        yield break;
    }

    void Start()
    {
        carts = new List<Day13Cart>();
        Camera.main.transform.eulerAngles = new Vector3(50, 0, 0);
        Camera.main.transform.position = cameraOffset;
        BuildTracks();

        StartCoroutine(Part2());
    }

    private void Update()
    {
        if (cartToFollow != null)
        {
            Vector3 direction = cartToFollow.transform.position + cameraOffset - Camera.main.transform.position;
            Camera.main.transform.position += direction * Time.deltaTime * 15f;
        }
    }
}
