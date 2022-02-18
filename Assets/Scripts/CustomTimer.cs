using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTimer : MonoBehaviour
{
    public delegate void callback();
    List<float> timerDurations = new List<float>();
    List<string> timerIDs = new List<string>();
    List<callback> timerCallbacks = new List<callback>();
    List<bool> timerIsActive = new List<bool> ();
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);   
    }

    // Update is called once per frame
    void Update()
    {
        if(timerDurations.Count == timerIDs.Count && timerIDs.Count == timerCallbacks.Count)
        {
            for(int i=0; i < timerIDs.Count; i++)
            {
                if(timerIsActive[i])
                {
                    timerDurations[i] -= Time.deltaTime;
                    if(timerDurations[i] <= 0)
                    {
                        callback callbackFunction = timerCallbacks[i];
                        RemoveTimer(timerIDs[i]);
                        callbackFunction();
                    }
                }
            }
        }
        else
        {
            MonoBehaviour.print("Something went wrong and the timers are not synced");
        }
    }

    public void AddTimer(string id, float duration, callback callback)
    {
        if(timerDurations.Count == timerIDs.Count && timerIDs.Count == timerCallbacks.Count)
        {
            timerIDs.Add(id);
            timerDurations.Add(duration);
            timerCallbacks.Add(callback);
            timerIsActive.Add(false);
        }
        else
        {
            MonoBehaviour.print("Something went wrong and the timers are not synced");
        }
    }


    public void RemoveTimer(string id)
    {
        int idx = timerIDs.IndexOf(id);
        timerIDs.RemoveAt(idx);
        timerDurations.RemoveAt(idx);
        timerCallbacks.RemoveAt(idx);
        timerIsActive.RemoveAt(idx);
    }
    public void StartTimer(string id)
    {
        {
            int idx = timerIDs.IndexOf(id);
            timerIsActive[idx] = true;
        }
    }

    public void PauseTimer(string id)
    {
        int idx = timerIDs.IndexOf(id);
        timerIsActive[idx] = false;
    }
}
