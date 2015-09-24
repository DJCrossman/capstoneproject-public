using UnityEngine;
using System.Collections;
using System.Threading;

public class Player : MonoBehaviour {

	/**************************************************
	 * Variables
	 **************************************************/

	public PlayerHealth PlayerHealth;
	public PlayerNetwork PlayerNetwork;
    public PlayerMovement PlayerMovement;
    public Weapon PlayerWeapon;
    public Animator HitAnimator;
    public bool IsActive = false;

    private CameraController _cameraController;
    private bool _startActivationTimer = false;
    private double _countdownTime = 0;

	/**************************************************
	 * Event Handlers
	 **************************************************/

	// Use this for initialization
	private void Awake() 
    {
		enabled = networkView.isMine;		//disable component unless it belongs to me
    }

    private void Start() 
    {
        rigidbody2D.isKinematic = false;	//apply graivty

        _cameraController = global::CameraController.Instance;
        _cameraController.ChangeCameraView(0, true);
        _cameraController.Cameras[0].GetComponent<CameraFollower>().Target = transform;
        global::ElevatorContainer.Instance.Deactivate();
    }

    private void FixedUpdate()
    {
        if (_startActivationTimer == true) {
            if (_countdownTime <= 0) {
                IsActive = true;
                _startActivationTimer = false;
            } else {
                _countdownTime -= Time.fixedDeltaTime;
            }
        }
    }

	/**************************************************
	 * Control Methods
	 **************************************************/

    public void StartActivationTimer(int waitTime)
    {
        _countdownTime = (double) waitTime;
        _startActivationTimer = true;
    }

}
