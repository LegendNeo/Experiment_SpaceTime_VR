using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StopWatchTesting : MonoBehaviour
{
    Stopwatch myStopWatch;
    
    public InputActionReference startButton;
    public InputActionReference stopButton;
    // Start is called before the first frame update
    void Start()
    {
        print("STOPWATCH CREATED");
        myStopWatch.ResetTiming();

        startButton.action.performed += StartUp;
        stopButton.action.performed += StopDown;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartUp(InputAction.CallbackContext obj)
    {
        myStopWatch.StartTiming();

    }

    void StopDown(InputAction.CallbackContext obj) 
    {
        myStopWatch.StopTiming();
            print(myStopWatch.GetTime());
            List <float> segments = myStopWatch.GetTimeSegments();
            foreach(float segmentLength in segments)
                print(segmentLength);
    }
}
