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

    Room_Trial callback;

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
    
    public void init(float timeInSeconds, Room_Trial callbackObject, string sceneName, Stopwatch globalRoomStopwatch)
    {
        this.originalSceneName = sceneName;
        this.timeInSeconds = timeInSeconds;
        this.globalStopwatch = globalRoomStopwatch;
        this.callback = callbackObject;
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
        print("TIME IN SECONDS"+timeInSeconds);
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
        try
        {
            SceneManager.LoadScene(SCENENAME_FOR_TRT);
        }
        catch(Exception e)
        {
            print(e);
        }
        state = "REPRODUCTION_IDLE";

        CustomTimer untilSceneLoaded = FindObjectOfType<CustomTimer>();
        untilSceneLoaded.AddTimer("UNTIL_SCENE_LOADS", 5*Time.deltaTime, reproduce_start);
        untilSceneLoaded.StartTimer("UNTIL_SCENE_LOADS");
        
    }

    public void reproduce_start ()
    {
        GameObject feedbackObject = GameObject.Find("TRT Reproduction Feedback");
        //GameObject.FindObjectOfType<TRT_Reproduction_Feedback_Behavior>();
        TRT_Reproduction_Feedback_Behavior feedbackObjectComponent = feedbackObject.GetComponent<TRT_Reproduction_Feedback_Behavior>();
        SetReproductionFeedback(feedbackObjectComponent);
        

        //implement here: Code that make button down trigger reproduce_on_button_down()
        //this one below just simulates it
        HandPresence handPresence = FindObjectOfType<HandPresence>().GetComponent<HandPresence>();
        handPresence.bindToPrimaryDown(reproduce_on_button_down);
        handPresence.bindToPrimaryUp(reproduce_on_button_up);
    }

    public void reproduce_on_button_down()
    {
        state = "REPRODUCTION_ON";
        
        reproductionStopwatch.StartTiming();
        reprodFeedback.switchOn();

        //implement code that make button up trigger reproduce_end()
        //this one below just simulates it
        CustomTimer userReleaseSimulator = FindObjectOfType<CustomTimer>();
        
    }

    public void reproduce_on_button_up()
    {
        state = "REPRODUCTION_FINISHED";
        reproductionStopwatch.StopTiming();
        reprodFeedback.switchOff();
        reproductionStopwatch.lockStopwatch();

        reproduce_end();
        //if transition is too abrupt, maybe call reproduce_end after a timer
    }

    
    public void reproduce_end()
    {
        
        float reproducedTime = reproductionStopwatch.GetTime();

        // implement a callback that gives the measured time to some bigger instance or records it
        // idk how to exactly implement it atm
        this.callback.endTRT(reproducedTime);
    }
}