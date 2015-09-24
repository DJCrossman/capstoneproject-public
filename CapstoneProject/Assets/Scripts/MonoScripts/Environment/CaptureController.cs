using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaptureController : MonoBehaviour {

    private const float CaptureTime = 30f;

    public Animator TerminalAnimator;
    public RectTransform InvaderBar;
    public RectTransform DefenderBar;
	public HashSet<Guid> Team1Insiders, Team2Insiders;
	public float Progress = 0f;

    private MatchController _matchController;
    private float _previousProgress = 0;
    private bool _isActive = false;
    private bool _hacked = false;
    

    /************************************************************
	 * Event Handlers
	 ************************************************************/

	void Awake () 
    {
		Team1Insiders = new HashSet<Guid> ();
		Team2Insiders = new HashSet<Guid> ();
    }

    void Start()
    {
        if (Network.isServer) _matchController = MatchController.Instance;
    }

    private void FixedUpdate()
    {
        if (Network.isClient) return;   // Only run on server
        if (_isActive == false) return;

        if ((Team1Insiders.Count > 0) && (Team2Insiders.Count == 0)) {
            // Team 1 is capturing area
            Progress += Time.fixedDeltaTime;
        } else if ((Team1Insiders.Count == 0) && (Team2Insiders.Count > 0)) {
            // Team 2 is capturing area
            Progress -= Time.fixedDeltaTime;
        }

        if (Mathf.Abs(Progress - _previousProgress) > 0.0001) UpdateProgress();
        _previousProgress = Progress;
    }

	void OnTriggerEnter2D(Collider2D collision) {
        if (Network.isClient) return;   // Only run on server
	    var collisionRoot = collision.transform.root;

		if (collisionRoot.CompareTag ("Team1Player")) {
            AddInsider(Team.Team1, collisionRoot.GetComponent<PlayerNetwork>().GetUserModel().Uid);
		} else if (collision.transform.root.CompareTag ("Team2Player")) {
            AddInsider(Team.Team2, collisionRoot.GetComponent<PlayerNetwork>().GetUserModel().Uid);
		}
	}

	void OnTriggerExit2D(Collider2D collision) {
        if (Network.isClient) return;   // Only run on server
        var collisionRoot = collision.transform.root;

		if (collisionRoot.CompareTag ("Team1Player")) {
            RemoveInsider(Team.Team1, collisionRoot.GetComponent<PlayerNetwork>().GetUserModel().Uid);
		} else if (collisionRoot.CompareTag ("Team2Player")) {
            RemoveInsider(Team.Team2, collisionRoot.GetComponent<PlayerNetwork>().GetUserModel().Uid);
		}
	}

	/************************************************************
	 * Control Methods
	 ************************************************************/

    private void AddInsider(Team team, Guid uid)
    {
        if (team == Team.Team1) {
            Team1Insiders.Add(uid);
        }
        else if (team == Team.Team2) {
            Team2Insiders.Add(uid);
        }

        Debug.Log("Player with UID " + uid + " entered the capture area.");
    }

    private void RemoveInsider(Team team, Guid uid)
    {
        if (team == Team.Team1) {
            Team1Insiders.Remove(uid);
        } else if (team == Team.Team2) {
            Team2Insiders.Remove(uid);
        }

        Debug.Log("Player with UID " + uid + " left the capture area.");
    }

	public void RemovePlayer(Guid uid)
    {
        if (Network.isServer == false) return;   // Only run on server

		RemoveInsider(Team.Team1, uid);
        RemoveInsider(Team.Team2, uid);
	}

    private void UpdateProgress()
    {
        if (Network.isClient) return;   // Only run on server

        networkView.RPC("UpdateProgressForAll", RPCMode.All, Progress);
        if (Progress >= CaptureTime)
        {
            Debug.Log("The Invaders captured the area.");
            _matchController.CapturedArea(Team.Team1);
            SetActive(false);
        }
        else if (Progress <= (-1 * CaptureTime))
        {
            Debug.Log("The Defenders captured the area.");
            _matchController.CapturedArea(Team.Team2);
            SetActive(false);
        }
    }

    /************************************************************
	 * Getters and Setters
	 ************************************************************/

    public void SetActive(bool isActive)
    {
        if (isActive == true) {
            _isActive = true;
            Team1Insiders.Clear();
            Team2Insiders.Clear();
            networkView.RPC("ResetProgressRpc", RPCMode.All);
            Debug.Log("Capture area cleared and progress reset.");

        } else {
            _isActive = false;
            Debug.Log("Capture area deactivated.");
        }
    }


    /************************************************************
	 * RPC Handlers
	 ************************************************************/

    [RPC]
    private void UpdateProgressForAll(float progress)
    {
        Progress = progress;

        if (Progress > 0) {
            InvaderBar.localPosition = new Vector3(Mathf.Lerp(-1100f, 0f, Progress/CaptureTime),
                InvaderBar.localPosition.y, InvaderBar.localPosition.z);
        } else {
            DefenderBar.localPosition = new Vector3(Mathf.Lerp(-1100f, 0f, Progress/(-1*CaptureTime)),
                DefenderBar.localPosition.y, DefenderBar.localPosition.z);
        }

        if (_hacked == false) {
            if (Mathf.Abs(Progress) >= 30) {
                TerminalAnimator.SetBool("Hacked", true);
                _hacked = true;
            }
        }
    }

    [RPC]
    private void ResetProgressRpc() 
    {
        Progress = 0;
        TerminalAnimator.SetBool("Hacked", false);
        _hacked = false;
        InvaderBar.localPosition = new Vector3(-1100f, InvaderBar.localPosition.y, InvaderBar.localPosition.z);
        DefenderBar.localPosition = new Vector3(-1100f, DefenderBar.localPosition.y, DefenderBar.localPosition.z);
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static CaptureController _instance;

    public static CaptureController Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<CaptureController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}