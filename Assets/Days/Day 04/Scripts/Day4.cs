using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TMPro;

public class Day4 : MonoBehaviour
{
    public GameObject dayEntry;
    public GameObject contentParent;
    private List<GameObject> dayEntries = new List<GameObject>();

    private string[] input;
    private DateTime startOfArrayTime;
    private int[] minuteSchedule;
    private List<int[]> minutesByDay;

    private void ParseInput()
    {
        input = InputHelper.ParseInputArray(4).OrderBy(p => p).ToArray();
    }

    private void ConstructTimetable()
    {
        // get first day and last day
        startOfArrayTime = DateTime.Parse(input[0].Substring(1,10));
        //DateTime startEventTime = DateTime.Parse(input[0].Substring(1, 16));
        DateTime endDay = DateTime.Parse(input[input.Length-1].Substring(1, 16));

        // construct array of 1440 * number of days
        minuteSchedule = new int[(int) Math.Ceiling((endDay - startOfArrayTime).TotalDays + 1) * 1440];

        // foreach switch: new guard, wakes up, falls asleep
        int guardNumber = 0;
        DateTime lastEvent = startOfArrayTime;
        foreach(string line in input)
        {
            char eventCase = line[19]; // first letter of text, either 'w', 'f', or 'G'
            DateTime eventTime = DateTime.Parse(line.Substring(1, 16));

            if (eventCase == 'G')
            {
                // set all entries between last event and now to guardNumber
                //SetInterval(GetTimeIndex(lastEvent), GetTimeIndex(eventTime), guardNumber);

                MatchCollection matches = Regex.Matches(line, "\\d+");
                guardNumber = int.Parse(matches[matches.Count - 1].Value);
            }
            else if(eventCase == 'f')
            {
                // set all entries between last event and now to 0, but they are 0 already
            }
            else
            {
                SetInterval(GetTimeIndex(lastEvent), GetTimeIndex(eventTime), guardNumber);
            }
            lastEvent = eventTime;
        }
    }

    private IEnumerator ConstructGameObjectEntries()
    {
        // take each 1440 minute section, instantiate the entry, create the texture, attach it to the image component of the entry, and expand the container height 
        int days = minuteSchedule.Length / 1440;
        for(int i = 1; i < days-1; i++)
        {
            GameObject entry = Instantiate(dayEntry, contentParent.transform);
            int[] minutesDay = minuteSchedule.Skip(1440 * i).Take(60).ToArray();
            Texture2D texture = new Texture2D(1440, 1);
            entry.GetComponentsInChildren<Image>()[2].material.mainTexture = texture;
            entry.GetComponentInChildren<TextMeshProUGUI>().text = startOfArrayTime.AddHours(i * 24).ToShortDateString();

            for (int j = 0; j < minutesDay.Length; j++)
            {
                if(minutesDay[j] > 0)
                {
                    for(int w = 0; w < 24; w++)
                    {
                        texture.SetPixel(j*24 + w, 0, Color.black);
                    }
                }
            }
            texture.Apply();
            dayEntries.Add(entry);

            Vector2 parentSize = contentParent.GetComponent<RectTransform>().sizeDelta;
            contentParent.GetComponent<RectTransform>().sizeDelta = new Vector2(parentSize.x, parentSize.y + entry.GetComponent<RectTransform>().rect.height);
            yield return null;
        }
    }

    // returns (int minute, int day) tuple
    private int GetTimeIndex(DateTime time)
    {
        int minutes_difference = (int) Math.Round((time - startOfArrayTime).TotalMinutes);
        return minutes_difference;
    }

    private void SetInterval(int a, int b, int value)
    {
        for(int i = a; i < b; i++)
        {
            minuteSchedule[i] = value;
        }
    }

    private void Part1()
    {
        ParseInput();
        ConstructTimetable();
        StartCoroutine(ConstructGameObjectEntries());

        minutesByDay = new List<int[]>();
        int days = minuteSchedule.Length / 1440;

        for (int i = 1; i < days - 1; i++)
        {
            minutesByDay.Add(minuteSchedule.Skip(1440 * i).Take(60).ToArray());
        }

        // group by guard, select guard numbers & counts, and pick the guard with most sleeping time
        int guardNumber = minutesByDay.SelectMany(arr => arr).GroupBy(guard => guard).Select(group => (group.Key, group.Count())).OrderBy(tuple => -tuple.Item2).Skip(1).First().Key;

        int[] asleepMinutes = new int[60];
        foreach(int[] day in minutesByDay)
        {
            for (int i = 0; i < 60; i++)
            {
                if(day[i] == guardNumber)
                {
                    asleepMinutes[i]++;
                }
            }
        }

        int max_index = -1;
        int max = int.MinValue;
        for(int i = 0; i < 60; i++)
        {
            if(asleepMinutes[i] > max)
            {
                max = asleepMinutes[i];
                max_index = i;
            }
        }

        print($"Guard {guardNumber} sleeps the most at minute: {max_index}. Product: {guardNumber* max_index}");
    }

    private void Part2()
    {
        List<(int, int, int)> guardMostAsleepOnMinute = new List<(int, int, int)>();
        for(int i = 0; i < 60; i++)
        {
            guardMostAsleepOnMinute.Add(minutesByDay.Select(arr => arr[i])
                                                    .GroupBy(guard => guard)
                                                    .Select(group => (group.Key, group.Count(), i))
                                                    .OrderBy(tuple => -tuple.Item2)
                                                    .Where(tup => tup.Key != 0)
                                                    .FirstOrDefault());
        }

        (int guard, int amount, int minute) mostFreqAsleep = guardMostAsleepOnMinute.OrderBy(g => -g.Item2).First();
        print($"Guard most commonly asleep on minute {mostFreqAsleep.minute} is {mostFreqAsleep.guard} with {mostFreqAsleep.amount} sleeps. Product: {mostFreqAsleep.guard * mostFreqAsleep.minute}");
    }

    public void Start()
    {
        Part1();
        Part2();
    }
}
