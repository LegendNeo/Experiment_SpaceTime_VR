using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TRT_Reproduction_Feedback_Behavior : MonoBehaviour
{
    MeshRenderer rend;
    public Color COLOR_ON = new Color(255,0,0,255);
    public Color COLOR_OFF = new Color(255,255,255,255);
    bool IsOn = false;
    void Start()
    {
        rend = GetComponent<MeshRenderer> ();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchOn()
    {
        if(!IsOn)
        {
            rend.material.SetColor("_Color", COLOR_ON);
            IsOn = true;
        }
    }

    public void switchOff()
    {
        if(IsOn)
        {
            rend.material.SetColor("_Color", COLOR_OFF);
            IsOn = false;
        }
    }

    public bool getIsOn()
    {
        return IsOn;
    }


}
