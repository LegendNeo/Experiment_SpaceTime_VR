using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Room_Trial : MonoBehaviour
{
    float TIME_TO_GET_COMFORTABLE = 20f;
    float TIME_BETWEEN_TRTS = 2f;

    public float additionalTime;
    public string sceneName = "";

    public Stopwatch totalRoomStopwatch;

    bool initialSceneLoad = false;
    bool keepResults = true;

    float [] trtLengths = new float [3]; 

    int numTRT = 0;

    CustomTimer generalTimer;

    TRT_Stimulus_Behavior trt_stimulus;

    Procedure callbackObject;


    // Start is called before the first frame update
    void Start()
    {
        this.generalTimer = gameObject.AddComponent<CustomTimer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init(float additionalTime, string sceneName, Procedure callbackObject)
    {
        this.additionalTime = additionalTime;
        this.sceneName = sceneName;
        this.callbackObject = callbackObject;

        this.totalRoomStopwatch = gameObject.AddComponent<Stopwatch>();

        trtLengths = determineTRTLengths();
    }

    public float[] determineTRTLengths()
    {
        //TODO: we need to come up with some way of randomizing this sequence
        return new float [3]{2,4,6};
    }

    public void startRoomTrial ()
    {
        print("In startRoomTrial()");
        SceneManager.LoadScene(sceneName);

        SceneManager.sceneLoaded += onSceneLoaded;
    }

    public void onSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= onSceneLoaded; //ensures that this method is only called when this particular scene is loaded
        totalRoomStopwatch.StartTiming();
        trt_stimulus = FindObjectOfType<TRT_Stimulus_Behavior>();
        print("Scene has been loaded");
        if(!initialSceneLoad)
        {
            initialSceneLoad = true;

            generalTimer.AddTimer("GET_COMFORTABLE", TIME_TO_GET_COMFORTABLE, startTRT);
            generalTimer.StartTimer("GET_COMFORTABLE");
        }
        else
        {
            generalTimer.AddTimer("BETWEEN_TRT", TIME_BETWEEN_TRTS, startTRT);
            generalTimer.StartTimer("BETWEEN_TRT");
        }
    }

    public void startTRT()
    {

        print("starting TRT");
        TRT_Procedure trt = gameObject.AddComponent<TRT_Procedure> ();
        trt.init(trtLengths[numTRT], this, sceneName, totalRoomStopwatch);
        trt.SetStimulus(trt_stimulus);
        trt.stimulate_start();
    }
    
    public void endTRT(float result)
    {
        float actualTime = trtLengths[numTRT];
        //TODO: record actualTime and result

        numTRT++;
        print("ending TRT Nr " +numTRT);
        if(numTRT < 3)
        {
            SceneManager.sceneLoaded += onSceneLoaded;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            generalTimer.AddTimer("ADDITIONAL_STAY_TIME", additionalTime, onEnd);
            generalTimer.StartTimer("ADDITIONAL_STAY_TIME");
        }
    }

    void onEnd(){
        print("Reached end of RoomTrial");
        totalRoomStopwatch.StopTiming();
        float totalTime = totalRoomStopwatch.GetTime();
        callbackObject.endRoomTrial(totalTime);
    }
}
