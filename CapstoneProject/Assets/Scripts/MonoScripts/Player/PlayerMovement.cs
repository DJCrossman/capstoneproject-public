using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	// Attribute variables
	public static float MaxSpeed = 5f;
    public static float JumpSpeed = 10f;
    public Transform GroundCheck;
    public Transform UI;

    private UserController _userController;
    private ChatInputManager _chatInputManager;
    private bool _facingRight = true;

    // Player Controls
    private bool _jump = false;
    private float _move = 0f;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    private void Awake() 
    {
        enabled = networkView.isMine;
    }
	void Start () 
    {
	    _userController = global::UserController.Instance;
	    _chatInputManager = global::ChatInputManager.Instance;
	}

    void OnGUI()
    {
        if (IsControllable() == false) return;

        if ((Input.GetKeyDown(KeyCode.Space)) && (IsGrounded() == true)) {
            _jump = true;
        }

        if (_move < 0.0001f) {
            _move = Input.GetAxis("Horizontal");
        }
    }   

	// Updates every frame that the screen updates
	void FixedUpdate() {
		if (networkView.isMine == false) return;            //stop control if player is not yours

		// move character on x axis
		rigidbody2D.velocity = new Vector2 (_move * MaxSpeed, rigidbody2D.velocity.y);

        // flip character to face direction of movement
	    if (_move > 0 && !_facingRight) {
	        Flip();
        } else if (_move < 0 && _facingRight) {
            Flip();
        }

        // jump the player
	    if (_jump == true) {
	        rigidbody2D.velocity = Vector2.up * JumpSpeed;
	        _jump = false;
	    }

        // allow for new movement parameter
	    _move = 0f;
	}

    /************************************************************
	 * Helper Methods
	 ************************************************************/

	private void Flip() {
		_facingRight = !_facingRight;
		Vector3 playerScale = transform.localScale;
		playerScale.x *= -1;
		transform.localScale = playerScale;
	    Vector3 playerUiScale = UI.transform.localScale;
	    playerUiScale.x *= -1;
	    UI.transform.localScale = playerUiScale;
	}

    bool IsGrounded()
    {
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 groundPos = new Vector2(GroundCheck.position.x, GroundCheck.position.y);

        return Physics2D.Linecast(playerPos, groundPos, 1 << LayerMask.NameToLayer("Ground"));
    }

    private bool IsControllable()
    {
        return networkView.isMine == true &&
            _userController != null &&
            _userController.GetPlayer().IsActive == true &&
            _userController.GetPlayer().PlayerNetwork.GetUserModel().Dead == false &&
            _chatInputManager.GetActive() == false;
    }
}