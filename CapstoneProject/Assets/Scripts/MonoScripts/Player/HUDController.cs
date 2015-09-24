using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HUDController : MonoBehaviour {

    public Transform Health;
    public Text AmmoText;
    public Text TimeText;

    private bool _timerActivated = false;
    private double _timeRemaining;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    private void FixedUpdate() 
    {
        if (_timerActivated == true) {
            if (_timeRemaining > 0) {
                _timeRemaining -= Time.fixedDeltaTime;
                int minutesLeft = ((int)_timeRemaining/60);
                int secondsLeft = ((int) (_timeRemaining - (60*minutesLeft)));
                string filler = secondsLeft < 10 ? ":0" : ":";
                TimeText.text = minutesLeft + filler + secondsLeft;
            } else {
                _timerActivated = false;
            }
        }
    }

    /************************************************************
	 * Control Methods
	 ************************************************************/

	public void UpdateHealthHUD(int health) {
	    float x = Mathf.Lerp(-398f, 0f, ((float) health)/100f);
        Health.localPosition = new Vector3(x, Health.localPosition.y, Health.localPosition.z);
	}

    public void StartTimer(double timeRemaining)
    {
        _timeRemaining = timeRemaining;
        _timerActivated = true;
    }

    public void StopTimer() 
    {
        _timerActivated = false;
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static HUDController _instance;

    public static HUDController Instance {
        get { return _instance ?? (_instance = FindObjectOfType<HUDController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
