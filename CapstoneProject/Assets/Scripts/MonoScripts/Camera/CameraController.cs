using System;
using System.Linq;
using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Camera[] Cameras;
    public Camera UiCamera;

    private double _delayTimer = 0;
    private bool _timerActive = false;
    private int _cameraIndex;

    /************************************************************
     * Event Handlers
     ************************************************************/

    private void FixedUpdate()
    {
        if (_timerActive == true) {
            if (_delayTimer <= 0) {
                _timerActive = false;
                ChangeCameraView(_cameraIndex);
            } else {
                _delayTimer -= Time.fixedDeltaTime;
            }
        }
    }

    /************************************************************
     * Control Methods
     ************************************************************/

    public void ChangeCameraView(int cameraIndex, bool unbuffer = false)
    {
        _timerActive = !unbuffer;

        foreach (var cam in Array.FindAll(Cameras, cam => cam.enabled == true)) {
            cam.enabled = false;
            cam.gameObject.GetComponent<AudioListener>().enabled = false;
            cam.tag = "";
        }

        Cameras[cameraIndex].enabled = true;
        Cameras[cameraIndex].gameObject.GetComponent<AudioListener>().enabled = true;
        Cameras[cameraIndex].tag = "MainCamera";
    }

    public void ChangeCameraView(int cameraIndex, double delay)
    {
        _cameraIndex = cameraIndex;
        _delayTimer = delay;
        _timerActive = true;
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static CameraController _instance;

    public static CameraController Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<CameraController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
