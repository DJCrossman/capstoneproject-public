using System;
using UnityEngine;
using System.Collections;

public class ElevatorContainer : MonoBehaviour
{
    public ElevatorController[] ElevatorControllers;
    public AudioClip[] ElevatorSound;

    /**************************************************
     * Control Methods
     **************************************************/

    public void Deactivate()
    {
        Array.ForEach(ElevatorControllers, elevatorController => elevatorController.Deactivate());
    }
    
    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static ElevatorContainer _instance;

    public static ElevatorContainer Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<ElevatorContainer>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
