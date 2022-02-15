using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class RoomSwitcher : MonoBehaviour
{
    public float timeToSwitch = 30f;
    private float resetTime;
    public string sceneName;
    public bool counting = true;

    //public bool emitLight = true;

    public InputActionReference backButton;

    //public GameObject xyz;
    //public Light lightComp;

    void Start() {
        resetTime = timeToSwitch;
        backButton.action.performed += StopCounting;
    }


    // Update is called once per frame
    void Update()
    {
        if (counting)
        {
            timeToSwitch -= Time.deltaTime;

            if (timeToSwitch <= 0f)
            {
                timeToSwitch = resetTime;
                SceneManager.LoadScene(sceneName);
            }
        }

        //if (!emitLight)
        //{
            //xyz.SetActive(false);
            //lightObject.GetComponent<Light>().enabled = false;
        //}

    }

    void StopCounting(InputAction.CallbackContext obj) {
        counting = false;
    }

}
