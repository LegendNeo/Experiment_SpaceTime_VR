// controls the procedure of a time reproduction task
using System.Timers;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
public class TRT_Procedure
{
    private const float TIME_BETWEEN_STIMULUS_AND_SCENE_CHANGE = 1f;

    public const string SCENENAME_FOR_TRT = "Menu";

    public string originalSceneName;
    public string state = "STIMULUS_OFF";
    TRT_Stimulus_Behavior stimulus;
    TRT_Reproduction_Feedback_Behavior reprodFeedback;

    Stopwatch reproductionStopwatch;
    Stopwatch globalStopwatch;

    float timeInSeconds;
    /**
    * timeInSeconds: Which time should be reproduced in Time reproduction task
    * callbackObject: Some object that can be called back when the Time Reproduction Task is finished
    * sceneName: The name of the scene where the Time Reproduction Task starts.
    * globalRoomStopwatch: A Stopwatch that measures the total time spent in a room.
    */
    public TRT_Procedure(float timeInSeconds, object callbackObject, string sceneName, Stopwatch globalRoomStopwatch)
    {
        this.timeInSeconds = timeInSeconds;
        reproductionStopwatch = new Stopwatch();
        originalSceneName = sceneName;
        globalStopwatch = globalRoomStopwatch;
    }

    public void SetStimulus (TRT_Stimulus_Behavior newStimulus)
    {
        stimulus = newStimulus;
    }

    public void SetReproductionFeedback (TRT_Reproduction_Feedback_Behavior newReprodFeedback)
    {
        reprodFeedback = newReprodFeedback;
    }

    // starts the stimulation part of the time reproduction task.
    // switches on the lamp and sets a timer for a certain interval of time.
    // please assign scripts for the stimulus and the reproduction feedback before you start!
    public void stimulate_start()
    {
        stimulus.SwitchOn();
        state = "STIMULUS_ON";

        Timer stimulusTimer = new Timer(timeInSeconds * 1000);
        stimulusTimer.Elapsed += stimulate_end;
        stimulusTimer.AutoReset = false;
        stimulusTimer.Start();
    }

    //callback function that is called after the lamp has been on for a certain amount of time
    public void stimulate_end(object source, ElapsedEventArgs e)
    {
        stimulus.SwitchOff();
        state = "STIMULUS_OFF";
        Timer timeUntilSceneChange = new Timer(TIME_BETWEEN_STIMULUS_AND_SCENE_CHANGE * 1000);
        timeUntilSceneChange.Elapsed += reproduce_start;
        timeUntilSceneChange.AutoReset = false;
        timeUntilSceneChange.Start();
    }

    public void reproduce_start(object source, ElapsedEventArgs e)
    {
        globalStopwatch.StopTiming();
        SceneManager.LoadScene(SCENENAME_FOR_TRT, LoadSceneMode.Single);
        state = "REPRODUCTION_IDLE";

        TRT_Reproduction_Feedback_Behavior feedbackObject = GameObject.FindObjectOfType<TRT_Reproduction_Feedback_Behavior>();
        SetReproductionFeedback(feedbackObject);
        

        //implement here: Code that make button down trigger reproduce_on_button_down()

    }

    public void reproduce_on_button_down(InputAction.CallbackContext obj)
    {
        state = "REPRODUCTION_ON";
        reproductionStopwatch.StartTiming();
        reprodFeedback.switchOn();

        //implement code that make button up trigger reproduce_end()
    }

    public void reproduce_on_button_up(InputAction.CallbackContext obj)
    {
        state = "REPRODUCTION_FINISHED";
        reproductionStopwatch.StopTiming();
        reprodFeedback.switchOff();
        reproductionStopwatch.lockStopwatch();
        float reproducedTime = reproductionStopwatch.GetTime();

        // implement a callback that gives the measured time to some bigger instance or records it
        // idk how to exactly implement it atm

        reproduce_end();
        //if transition is too abrupt, maybe call reproduce_end after a timer
    }

    public void reproduce_end()
    {
        SceneManager.LoadScene(originalSceneName);
        globalStopwatch.StartTiming();
    }
}