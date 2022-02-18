using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StopWatchTesting : MonoBehaviour
{
    Stopwatch myStopWatch;
    
    public InputActionReference startButton;
    public InputActionReference stopButton;

    public TRT_Stimulus_Behavior trtStim;
    public TRT_Reproduction_Feedback_Behavior trtFeedback;
    TRT_Procedure trt;
    // Start is called before the first frame update
    void Start()
    {
        GameObject globalStopwatch = new GameObject("globalStopwatch");
        myStopWatch = globalStopwatch.AddComponent<Stopwatch>();
        DontDestroyOnLoad(globalStopwatch);
        myStopWatch.StartTiming();

        DontDestroyOnLoad(this.gameObject);

        trt = gameObject.AddComponent<TRT_Procedure>();
        trt.init(6, null, "Room Small", myStopWatch);
        trt.SetStimulus(trtStim);
        trt.stimulate_start();
    }

    // Update is called once per frame
    void Update()
    {
        print(trt.state);
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
