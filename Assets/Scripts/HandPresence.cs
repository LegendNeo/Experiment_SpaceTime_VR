using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
public class HandPresence : MonoBehaviour
{
    public delegate void buttonAction();
    buttonAction onPrimaryDown;
    buttonAction onPrimaryUp;
    buttonAction onPrimaryHeld;

    bool isPrimaryDown;

    buttonAction onTriggerDown;
    buttonAction onTriggerUp;
    buttonAction onTriggerHeld;

    bool isTriggerDown;

    private InputDevice targetDevice;
    // Start is called before the first frame update
    void Start()
    {
        List<InputDevice> devices = new List<InputDevice>();
        InputDeviceCharacteristics rightControllerCharacteristics = InputDeviceCharacteristics.Right | InputDeviceCharacteristics.Controller;

        InputDevices.GetDevicesWithCharacteristics(rightControllerCharacteristics, devices);

        foreach(var item in devices){
            Debug.Log(item.name + item.characteristics);
        }

        if(devices.Count > 0){
            targetDevice = devices[0];
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetDevice.TryGetFeatureValue(CommonUsages.secondaryButton, out bool primaryButtonValue);
        targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerButtonValue);
        if(primaryButtonValue && !isPrimaryDown)
        {
            if(onPrimaryDown != null)
            {
                onPrimaryDown();
            }
            isPrimaryDown = true;
        }

        if(!primaryButtonValue && isPrimaryDown)
        {
            if(onPrimaryUp != null)
            {
                onPrimaryUp();
            }
            isPrimaryDown = false;
        }

        if(primaryButtonValue){
            if(onPrimaryHeld != null)
            {
                onPrimaryHeld();
            }
            Debug.Log("Pressing Primary Button");
        }

        if(triggerButtonValue && !isTriggerDown)
        {
            if(onTriggerDown != null)
            {
                onTriggerDown();
            }
            isTriggerDown = true;
        }

        if(!triggerButtonValue && isTriggerDown)
        {
            if(onTriggerUp != null)
            {
                onTriggerUp();
            }
            isTriggerDown = false;
        }

        if(triggerButtonValue){
            if(onTriggerHeld != null)
            {
                onTriggerHeld();
            }
            Debug.Log("Pressing Trigger Button");
        }

    }

    public void bindToPrimaryDown(buttonAction onDown)
    {
        onPrimaryDown = onDown;
    }
    
    public void bindToPrimaryUp(buttonAction onUp)
    {
        onPrimaryUp = onUp;
    }

    public void bindToTriggerDown(buttonAction onDown)
    {
        onTriggerDown = onDown;
    }

    public void bindToTriggerUp(buttonAction onUp)
    {
        onTriggerUp = onUp;
    }

    public void bindToTriggerHeld(buttonAction onHeld)
    {
        onTriggerHeld = onHeld;
    }
}
