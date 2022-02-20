using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Room_Trial : MonoBehaviour
{
    float TIME_TO_GET_COMFORTABLE = 20f;

    public float additionalTime;
    public string sceneName;

    public object callback;

    public Stopwatch totalRoomStopwatch;

    bool isInCorrectScene = false;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void init(float additionalTime, string sceneName)
    {
        this.additionalTime = additionalTime;
        this.sceneName = sceneName;
    }

    public void startRoomTrial ()
    {
        print("In startRoomTrial()");
        SceneManager.LoadScene(sceneName);
    }

    public void onSceneLoaded()
    {
        print("Scene has been loaded");
        try
        {
            SceneManager.LoadScene("Room Big");
        }
        catch(UnityException e) {
            {
                print(e);
            }
        }
    }

    public void startTRT()
    {

    }
}
