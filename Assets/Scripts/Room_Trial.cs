using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
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
    string TRTResultFilePath;

    int numTRT = 0;

    int ID;

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

    public void init(int ID, float additionalTime, string sceneName, Procedure callbackObject, string TRTResultFilePath, bool keepResults)
    {
        this.additionalTime = additionalTime;
        this.sceneName = sceneName;
        this.callbackObject = callbackObject;
        this.ID = ID;
        this.TRTResultFilePath = TRTResultFilePath;

        this.totalRoomStopwatch = gameObject.AddComponent<Stopwatch>();

        trtLengths = determineTRTLengths();
    }

    public float[] determineTRTLengths()
    {
        //DONE: we need to come up with some way of randomizing this sequence
        float [] sequence = callbackObject.trtOrders[0];
        callbackObject.trtOrders.RemoveAt(0);
        print(string.Join(",", sequence));
        return sequence;
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

        trt_stimulus = FindObjectOfType<TRT_Stimulus_Behavior>();
        print("Scene has been loaded");
        if(!initialSceneLoad)
        {
            initialSceneLoad = true;
            totalRoomStopwatch.StartTiming();

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
        if(keepResults)
        {
            try
            {
                StreamWriter writer = new StreamWriter(TRTResultFilePath, append:true);
                writer.WriteLine(TRTtoCSV(actualTime,result,sceneName));
                writer.Flush();
                writer.Close();
            }
            catch(IOException e)
            {
                print(e.Message);
            }
        }

        numTRT++;
        print("ending TRT Nr " +numTRT);
        if(numTRT < 3)
        {
            SceneManager.sceneLoaded += onSceneLoaded;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            SceneManager.sceneLoaded += initiateAdditionalStayTime;
            SceneManager.LoadScene(sceneName);
        }
    }

    string TRTtoCSV(float actualTime, float perceivedTime, string room){
        char del = Procedure.CSV_DELIMITER;
        string csvLine = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}", del,DateTime.Now, ID, room, actualTime, perceivedTime);
        return csvLine;
    }

    void initiateAdditionalStayTime(Scene scene, LoadSceneMode loadSceneMode)
    {
        SceneManager.sceneLoaded -= initiateAdditionalStayTime;
        generalTimer.AddTimer("ADDITIONAL_STAY_TIME", additionalTime, onEnd);
        generalTimer.StartTimer("ADDITIONAL_STAY_TIME");

    }

    void onEnd(){
        print("Reached end of RoomTrial");
        totalRoomStopwatch.StopTiming();
        SceneManager.sceneLoaded += onRoomExited;
        SceneManager.LoadScene(callbackObject.SCENE_BETWEEN_ROOM_TRIALS);
    }
    
    void onRoomExited(Scene scene, LoadSceneMode loadSceneMode){
        print("Room has been exited");
        SceneManager.sceneLoaded -= onRoomExited;
        float totalTime = totalRoomStopwatch.GetTime();
        callbackObject.endRoomTrial(totalTime);
        
    }

}
