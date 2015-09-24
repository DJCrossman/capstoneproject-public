using System;
using UnityEngine;
using System.Collections;

public class ElevatorController : MonoBehaviour
{
    // Public settings
    public float Speed = 2.0f;
    public float ActivationRadius = 0.2f;
    public float PauseTime = 1.5f;

    // Public requirements
    public Transform T; // transform for the moving platform object
    public Transform[] Nodes; // nodes along the path the platform will follow
    public bool[] Ease;
    public GameObject Button; // button to activate elevator

    // State Variables
    private bool _inActivationArea = false;
    private bool _isActivated = false; // is elevator activated
    private float _waitTime = 0; // current waiting time since last stage completed
    private int _currentNode = 0; // current position on node path

    private ElevatorContainer _elevatorContainer;
    private AudioSource _audioSource;
    private bool _startIsPlaying = false;
    private bool _stopIsPlaying = false;

    /**************************************************
     * Event Handlers
     **************************************************/

    private void Awake()
    {
        _elevatorContainer = global::ElevatorContainer.Instance;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_isActivated == false) return;

        if (_startIsPlaying == false) {
            _startIsPlaying = true;
            PlayClipAt(0, T.position);
        }

        if (Ease[_currentNode] == true) {
            Vector2 velocity = (Nodes[_currentNode].position - T.position).normalized*Speed*Time.fixedDeltaTime;

            // Check if node has been reached
            float yDif = Nodes[_currentNode].position.y - T.position.y;
            float xDif = Nodes[_currentNode].position.x - T.position.x;

            if (Math.Abs(xDif) <= 0.05 && Math.Abs(yDif) <= 0.03) {
                T.Translate(xDif, yDif, 0f);

                if ((_stopIsPlaying == false) && (_currentNode == 0)) {
                    _stopIsPlaying = true;
                    PlayClipAt(1, T.position);
                }

                // Update waiting time
                _waitTime += Time.deltaTime;

                // Move on to next node on path once @pauseTime seconds has passed
                if (_waitTime >= PauseTime) {
                    _waitTime = 0;
                    velocity = Vector2.ClampMagnitude(velocity, (Nodes[_currentNode].position - T.position).magnitude);
                    _currentNode += 1;
                }
            } else {
                T.Translate(velocity);
            }
        } else {
            T.position = Nodes[_currentNode].position;
            _currentNode += 1;
        }

        if (_currentNode == Nodes.Length) {
            _isActivated = false;
            _startIsPlaying = false;
            _stopIsPlaying = false;
            _currentNode = 0;
        }
    }

    private void OnGUI()
    {
        if ((Input.GetKeyDown("e") == true) && (_inActivationArea == true)) {
            networkView.RPC("RequestElevator", RPCMode.All);
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        PlayerNetwork playerNetwork = collider.transform.root.GetComponent<PlayerNetwork>();

        if(playerNetwork != null) {
            if ((playerNetwork.networkView.isMine == true) && (playerNetwork.GetUserModel().Dead == false)) {
                _inActivationArea = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        PlayerNetwork playerNetwork = collider.transform.root.GetComponent<PlayerNetwork>();

        if (playerNetwork != null) {
            if ((playerNetwork.networkView.isMine == true) && (playerNetwork.GetUserModel().Dead == false)) {
                _inActivationArea = false;
            }
        }
    }

    /**************************************************
     * Control Methods
     **************************************************/

    public void Deactivate()
    {
        _inActivationArea = false;
    }

    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    private void PlayClipAt(int index, Vector3 pos)
    {
        if (_audioSource != null) Destroy(_audioSource); // remove any existing clip
        GameObject tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        _audioSource = tempGO.AddComponent<AudioSource>(); // add an audio source
        _audioSource.clip = _elevatorContainer.ElevatorSound[index]; // define the clip
        _audioSource.Play(); // start the sound
        Destroy(tempGO, _elevatorContainer.ElevatorSound[index].length); // destroy object after clip duration
    }

    [RPC]
    private void RequestElevator()
    {
        networkView.RPC("RespondElevator", RPCMode.Others);
        _isActivated = true;
    }

    [RPC]
    private void RespondElevator()
    {
        _isActivated = true;
    }
}