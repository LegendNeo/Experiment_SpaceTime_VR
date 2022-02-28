using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stopwatch: MonoBehaviour
{
    DateTime startTime = DateTime.MinValue;
    float totalTimeElapsed = 0;
    List <float> timeSegments = new List<float>();
    bool running = false;

    bool locked = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Starts Timer.
    public void StartTiming()
    {
        if(!locked)
        {
            if(!running)
            {
                MonoBehaviour.print("Starting time");
                running = true;
                startTime = DateTime.Now;
            }
            else
            {
                MonoBehaviour.print("Timing can't be started if stopwatch is still running");
            }
        }
        else
        {
            MonoBehaviour.print("Stopwatch is locked and can't be restarted");
        }
    }

    //Stops Timer. Time elapsed since last Start of Timer is saved as a segment and added totalTimeElapsed.
    public void StopTiming()
    {
        if(running)
        {
            running = false;
            
            TimeSpan elapsedTimeInSegment =  DateTime.Now - startTime;
            float elapsedTimeInSeconds = ((float)elapsedTimeInSegment.Seconds * 1000 + (float)elapsedTimeInSegment.Milliseconds) / 1000;
            timeSegments.Add(elapsedTimeInSeconds);
            totalTimeElapsed += elapsedTimeInSeconds;
            print("stopped time at"+totalTimeElapsed);
        }
        else
        {
            MonoBehaviour.print("Can't stop stopwatch if it hasn't even started");
        }
    }
    
    // Resets the timer. 
    // Without this, all previous time segments and total times persist, so USE THIS FUNCTION if you measure the time of something new.
    // Or better yet, create a new instance of Stopwatch.
    public void ResetTiming()
    {
        timeSegments.Clear();
        totalTimeElapsed = 0;
        running = false;
        startTime = DateTime.MinValue;
    }

    // Returns the total time of all segments.
    public float GetTime()
    {
        return totalTimeElapsed;
    }

    // Returns an ordered list of all individual time segments measured by this stopwatch.
    public List <float> GetTimeSegments()
    {
        return timeSegments;
    }

    //locks the stopwatch and prevents timing from being restarted
    public void lockStopwatch()
    {
        locked = true;
    }

    // unlocks the stopwatch, re-enables timing start
    public void unlockStopwatch()
    {
        locked = false;
    }
}
