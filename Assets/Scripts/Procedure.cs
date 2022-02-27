using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Procedure : MonoBehaviour

{
    private int numRoomTrial = 0;
    private GameObject currentRoomTrialObject;

    private string [] roomOrder;
    private float [] timeOrder;

    private string SCENE_BETWEEN_ROOM_TRIALS = "Room Menu";

    bool canProceedToNextRoomTrial = false;

    // Start is called before the first frame update
    void Start()
    {
        int numActiveProcedures = FindObjectsOfType<Procedure>().GetLength(0);
        if(numActiveProcedures <= 1)
        {    
            print("Looks like I am the first Procedure skript running");
            print("Initiating Experiment");
            DontDestroyOnLoad(gameObject);
            print("Application.persistentDataPath "+Application.persistentDataPath);
            determineOrderOfConditions();
            startDummyTrial();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void determineOrderOfConditions()
    {
        //TODO: Get order from etruscan rectangle
        roomOrder = new string[]{"Room Small","Room Small","Room Small","Room Medium","Room Medium","Room Medium","Room Big","Room Big","Room Big"};
        timeOrder = new float[]{10,20,30,10,20,30,10,20,30};
    }

    void startRoomTrial(string room, float additionalTime)
    {
        currentRoomTrialObject = new GameObject("Room Trial " + numRoomTrial);
        DontDestroyOnLoad(currentRoomTrialObject);
        Room_Trial roomTrial = currentRoomTrialObject.AddComponent<Room_Trial>();
        roomTrial.init(additionalTime, room, this);
        roomTrial.startRoomTrial();
    }

    void startDummyTrial()
    {
        startRoomTrial("Room Menu", 5);
    }

    public void endRoomTrial(float totalTime)
    {
        print("Total time"+totalTime);
        print("Room Trial Completed!");
        SceneManager.LoadScene(SCENE_BETWEEN_ROOM_TRIALS);
        if(numRoomTrial < 9)
        {
            if(numRoomTrial != 0)
            {
                //TODO: record results of Trial
            }
            numRoomTrial++;
            HandPresence handpresence = FindObjectOfType <HandPresence>();
            handpresence.bindToTriggerDown(proceedToNextRoomTrial); //we only let the participant proceed to the next room when they press the trigger
        }
        else
        {
            print("All Room Trials Completed");
        }

        void proceedToNextRoomTrial()
        {
            Destroy(currentRoomTrialObject);
            startRoomTrial(roomOrder[numRoomTrial-1], timeOrder[numRoomTrial-1]);
        }

    }


}
