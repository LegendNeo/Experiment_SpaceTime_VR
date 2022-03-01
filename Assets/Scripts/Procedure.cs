using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
public class Procedure : MonoBehaviour

{
    public static char CSV_DELIMITER = ';';
    private int numRoomTrial = 0;
    private GameObject currentRoomTrialObject;

    private int ID;

    private string [] roomOrder;
    private float [] timeOrder;

    public string SCENE_BETWEEN_ROOM_TRIALS = "Neutral";

    bool canProceedToNextRoomTrial = false;

    string pathToTRTResults;
    string pathToRoomResults;
    string directoryPath;

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
            createCSVs();
            this.ID = getRunID();
            determineOrderOfConditions();
            //HandPresence handpresence = FindObjectOfType <HandPresence>(); //commented for testing purposes
            //handpresence.bindToTriggerDown(startDummyTrial);
            startDummyTrial();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void createCSVs()
    {
        string persistPath = Application.persistentDataPath;
        directoryPath = Path.Combine(persistPath, "Experiment Time Perception");
        pathToTRTResults = Path.Combine(directoryPath, "TRT Results.csv");
        pathToRoomResults = Path.Combine(directoryPath, "Room Results.csv");
        if(!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);


        }
        if (!File.Exists(pathToTRTResults))
        {
            try{
                FileStream trtResultsFS = File.Create(pathToTRTResults);
                trtResultsFS.Close();
                StreamWriter writer = new StreamWriter(pathToTRTResults, append:true);
                writer.WriteLine("Timestamp;ID;room;stimulusTime;reproducedTime");
                writer.Flush();
                writer.Close();
            }
            catch(IOException e)
            {
                print(e.Message);
            }
        }
        if(!File.Exists(pathToRoomResults))
        {
            try
            {
                FileStream roomResultsFS = File.Create(pathToRoomResults);
                roomResultsFS.Close();
                StreamWriter writer = new StreamWriter(pathToRoomResults);
                writer.WriteLine("Timestamp;ID;room;timeInRoom");
                writer.Flush();
                writer.Close();
            }
            catch(IOException e)
            {
                print(e.Message);
            }
        }
    }

    int getRunID()
    {
        return (int)UnityEngine.Random.Range(0,9999);
    }

    void determineOrderOfConditions()
    {
        //TODO: Get order from etruscan rectangle
        string pathToRoomLatSquare = Path.Combine(directoryPath, "room_latin_square.csv");
        
        
        if(File.Exists(pathToRoomLatSquare))
        {
            try
            {
                string [] rawLines = File.ReadAllLines(pathToRoomLatSquare);
                List<string[]> entries = new List<string[]>();
                string [] sequenceUsed = null;
                string [] newLines = new string[rawLines.Length];
                for (int i=0; i<rawLines.Length; i++)
                {
                    entries.Add(rawLines[i].Split(";"));
                    if(entries[i][9] == "0" && sequenceUsed == null)
                    {
                        sequenceUsed = entries[i];
                        entries[i][9] = "1";
                        print("viable row found");
                    }
                    newLines[i] = string.Join(";",entries[i]);
                    
                }
                List<string> rooms = new List<string>();
                List<float> times = new List<float>();
                for(int i = 0; i < 9; i++){
                    string[] conditions = sequenceUsed[i].Split('/');
                    switch(conditions[0]){
                        case "R1":rooms.Add("Room Small");break;
                        case "R2":rooms.Add("Room Medium");break;
                        case "R3":rooms.Add("Room Big");break;
                        default:print("Unknown Room");break;
                    }
                    switch(conditions[1]){
                        case "T1":times.Add(10f);break;
                        case "T2":times.Add(20f);break;
                        case "T3":times.Add(30f);break;
                        default:print("Unknown Time");break;
                    }
                }
            
                roomOrder = rooms.ToArray();
                timeOrder = times.ToArray();

                File.WriteAllLines(pathToRoomLatSquare,newLines);
            }
            catch(IOException e)
            {
                print(e.Message+" has occured while loading Latin Square for Room order"); 
            }
        }
        else
        {
            print("No Latin square for room order found");
        }

        //roomOrder = new string[]{"Room Small","Room Small","Room Small","Room Medium","Room Medium","Room Medium","Room Big","Room Big","Room Big"};
        //timeOrder = new float[]{10,20,30,10,20,30,10,20,30};
    }

    void startRoomTrial(string room, float additionalTime, bool keepResult)
    {
        currentRoomTrialObject = new GameObject("Room Trial " + numRoomTrial);
        DontDestroyOnLoad(currentRoomTrialObject);
        Room_Trial roomTrial = currentRoomTrialObject.AddComponent<Room_Trial>();
        roomTrial.init(ID, additionalTime, room, this, pathToTRTResults, keepResult);
        
        roomTrial.startRoomTrial();
    }

    void startDummyTrial()
    {
        startRoomTrial("Room Menu", 5, false);
    }

    public void endRoomTrial(float totalTime)
    {
        print("Total time"+totalTime);
        print("Room Trial Completed!");
        if(numRoomTrial < 9)
        {
            if(numRoomTrial != 0)
            {
                //TODO: record results of Trial
                string csv_line = string.Format("{1}{0}{2}{0}{3}{0}{4}", CSV_DELIMITER, DateTime.Now, ID, roomOrder[numRoomTrial], totalTime);
                try
                {
                    StreamWriter writer = new StreamWriter(pathToRoomResults, append:true);
                    writer.WriteLine(csv_line);
                    writer.Flush();
                    writer.Close();
                }
                catch(IOException e)
                {
                    print(e.Message);
                }
            }
            numRoomTrial++;
            HandPresence handpresence = FindObjectOfType <HandPresence>();
            handpresence.bindToTriggerDown(proceedToNextRoomTrial); //we only let the participant proceed to the next room when they press the trigger
        }
        else
        {
            print("All Room Trials Completed");
        }

    }
    void proceedToNextRoomTrial()
    {
        print("Proceeding to next room trial");
        Destroy(currentRoomTrialObject);
        startRoomTrial(roomOrder[numRoomTrial-1], timeOrder[numRoomTrial-1], true);
    }

}
