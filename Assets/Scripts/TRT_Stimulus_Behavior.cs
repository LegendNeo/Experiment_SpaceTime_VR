using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class controls the behavior of the lamp that serves as the stimulus for the time reproduction tasks.
// currently, nothing much happens here because the lamp isn't actually here yet.
public class TRT_Stimulus_Behavior : MonoBehaviour
{
    bool IsOn = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchOn ()
    {
        if (!IsOn)
        {
            //Implement SWITCHING ON the lamp here
            print("Switching lamp ON");
            IsOn = true;
        }
    }

    public void SwitchOff ()
    {
        if (IsOn)
        {
            //Implement SWITCHING OFF the lamp here
            print("Switching lamp OFF");
            IsOn = false;
        }
    }
}
