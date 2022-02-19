// controls the procedure of a time reproduction task
using System.Timers;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine;
using System;
public class TRT_Procedure: MonoBehaviour
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
    * init(): Replaces constructor after I had to make this class a MonoBehaviour (I love scripting in Unity)
    * So please, please call it when you make a new TRT
    * Arguments
    * timeInSeconds: Which time should be reproduced in Time reproduction task
    * callbackObject: Some object that can be called back when the Time Reproduction Task is finished
    * sceneName: The name of the scene where the Time Reproduction Task starts.
    * globalRoomStopwatch: A Stopwatch that measures the total time spent in a room.
    */
    
    public void init(float timeInSeconds, object callbackObject, string sceneName, Stopwatch globalRoomStopwatch)
    {
        this.originalSceneName = sceneName;
        this.timeInSeconds = timeInSeconds;
        this.globalStopwatch = globalRoomStopwatch;
    }
    public void start()
    {
        
    }
    public void update()
    {

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
        reproductionStopwatch = gameObject.AddComponent<Stopwatch>();
        DontDestroyOnLoad(this.gameObject);

        stimulus.SwitchOn();
        state = "STIMULUS_ON";
        CustomTimer stimulusTimer = FindObjectOfType<CustomTimer>();
        stimulusTimer.AddTimer("STIMULUS", timeInSeconds, stimulate_end);
        stimulusTimer.StartTimer("STIMULUS");

        //I hate Unity Thread handling I hate Unity Thread handling I hate Unity Thread handling 
        //Timer stimulusTimer = new Timer(timeInSeconds * 1000);
        //stimulusTimer.Elapsed += stimulate_end;
        //stimulusTimer.AutoReset = false;
        //stimulusTimer.Start();
    }

    //callback function that is called after the lamp has been on for a certain amount of time
    public void stimulate_end()//object source, ElapsedEventArgs e)
    {
        stimulus.SwitchOff();
        state = "STIMULUS_OFF";
        CustomTimer sceneChangeTimer = FindObjectOfType<CustomTimer>();
        sceneChangeTimer.AddTimer("CHANGE_TO_MENU", TIME_BETWEEN_STIMULUS_AND_SCENE_CHANGE, reproduce_load_scene);
        sceneChangeTimer.StartTimer("CHANGE_TO_MENU");
        //see above
        //Timer timeUntilSceneChange = new Timer(TIME_BETWEEN_STIMULUS_AND_SCENE_CHANGE * 1000);
        //timeUntilSceneChange.SynchronizingObject = null;
        //timeUntilSceneChange.Elapsed += reproduce_start;
        //timeUntilSceneChange.AutoReset = false;
        //timeUntilSceneChange.Start();
    }

    public void reproduce_load_scene()
    {
        globalStopwatch.StopTiming();
        MonoBehaviour.print("Got to reproduce_start()");
        try
        {
            SceneManager.LoadScene(SCENENAME_FOR_TRT);
        }
        catch(Exception e)
        {
            print(e);
        }
        MonoBehaviour.print("Got further into reproduce_start()");
        state = "REPRODUCTION_IDLE";

        CustomTimer untilSceneLoaded = FindObjectOfType<CustomTimer>();
        untilSceneLoaded.AddTimer("UNTIL_SCENE_LOADS", 5*Time.deltaTime, reproduce_start);
        untilSceneLoaded.StartTimer("UNTIL_SCENE_LOADS");
        
    }

    public void reproduce_start ()
    {
        GameObject feedbackObject = GameObject.Find("TRT Reproduction Feedback");
        print("l118");
        print(feedbackObject);
        //GameObject.FindObjectOfType<TRT_Reproduction_Feedback_Behavior>();
        TRT_Reproduction_Feedback_Behavior feedbackObjectComponent = feedbackObject.GetComponent<TRT_Reproduction_Feedback_Behavior>();
        print(feedbackObjectComponent);
        SetReproductionFeedback(feedbackObjectComponent);
        

        //implement here: Code that make button down trigger reproduce_on_button_down()
        //this one below just simulates it
        CustomTimer userPressSimulator = FindObjectOfType<CustomTimer>();
        userPressSimulator.AddTimer("UNTIL_USER_PRESS", 3, reproduce_on_button_down);
        userPressSimulator.StartTimer("UNTIL_USER_PRESS");
    }

    public void reproduce_on_button_down()
    {
        state = "REPRODUCTION_ON";
        
        print(reproductionStopwatch);
        reproductionStopwatch.StartTiming();
        reprodFeedback.switchOn();

        //implement code that make button up trigger reproduce_end()
        //this one below just simulates it
        CustomTimer userReleaseSimulator = FindObjectOfType<CustomTimer>();
        userReleaseSimulator.AddTimer("UNTIL_USER_RELEASE", 3, reproduce_on_button_down);
        userReleaseSimulator.StartTimer("UNTIL_USER_RELEASE");
        
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