using UnityEngine;
using UnityEngine.UI;

public class PlayerNetwork : MonoBehaviour {

    public Text NameText;
    public GameObject[] Colliders;
    public AudioClip[] DeathSounds;
    public int UserIndex;
 
    private Color _color;

    private UserController _userController;
    private TeamsController _teamsController;
    private Animator _animator;
    private SpriteRenderer[] _spriteRenderers;
    private float _xPosition;
    private bool _isFirstRun = true;
    private float _elapsedTime = 0;
    private float _speed = 0;

    /************************************************************
	 * Event Handlers
	 ************************************************************/

    // Use this for initialization
    private void Awake()
    {
        _teamsController = global::TeamsController.Instance;
        _animator = GetComponent<Animator>();
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (networkView.isMine) {
            _userController = global::UserController.Instance;
            _userController.SetPlayer(GetComponent<Player>());
        }
    }

    private void FixedUpdate()
    {
        if (networkView.isMine == true) {
            if (_isFirstRun == true) {
                _isFirstRun = false;
            } else {
                _speed = Mathf.Abs(transform.position.x - _xPosition)/Time.fixedDeltaTime;
            }

            _xPosition = transform.position.x;
        } else {
            _elapsedTime += Time.fixedDeltaTime;
            if (_elapsedTime >= 0.1) {
                _speed = Mathf.Abs(transform.position.x - _xPosition)/_elapsedTime;
                _xPosition = transform.position.x;
                _elapsedTime = 0f;
            }
        }

        _animator.SetFloat("Speed", _speed);
    }

    /************************************************************
	 * Getters and Setters
	 ************************************************************/

    // Name
    public UserModel GetUserModel()
    {
        return _teamsController.GetUserModels()[UserIndex];
    }

    public void SetName(string userName)
    {
        NameText.text = GetUserModel().UserName;
    }

    // Team
    public void SetTeam(Team team)
    {
        if(networkView.isMine == true) _userController.SetTeam(team);

        LayerMask layer = team == Team.Team1 ? LayerMask.NameToLayer("Team1Player") : LayerMask.NameToLayer("Team2Player");
        tag = team == Team.Team1 ? "Team1Player" : "Team2Player";
        foreach (var gObject in Colliders) {
            gObject.layer = layer;
        }
    }

    public void SetColorIndex(Team team, int colorIndex)
    {
        _color = team == Team.Team1
            ? ColorConfiguration.Team1.memberColors[colorIndex]
            : ColorConfiguration.Team2.memberColors[colorIndex];

        // Set text and player sprites to the colour
        NameText.color = new Color(_color.r - 0.25f, _color.g - 0.25f, _color.b - 0.25f);
        foreach (SpriteRenderer spriteRenderer in _spriteRenderers) {
            spriteRenderer.color = _color;
        }
    }

    // Colour
    public void SetUserIndex(int userIndex) 
    {
        networkView.RPC("SetUserIndexForAll", RPCMode.All, userIndex);
    }

    /************************************************************
	 * Remote Procedure Call Handlers
	 ************************************************************/

    [RPC]
    private void KillForAll()
    {
        _teamsController.UpdatePlayerMortality(GetUserModel().Uid, (Team) GetUserModel().CurrentTeam);
        Destroy(gameObject, 0.6f);
        _animator.SetBool("Dead", true);

        // Pick a random sound from the death clip array and play it
        int deathSoundsIndex = UnityEngine.Random.Range( /*inclusive */ 0, /*exclusive*/ DeathSounds.Length);
        AudioSource.PlayClipAtPoint(DeathSounds[deathSoundsIndex], transform.position);
    }

    [RPC]
    private void SetUserIndexForAll(int userIndex)
    {
        UserIndex = userIndex;
        var team = (Team) GetUserModel().CurrentTeam;

        SetName(GetUserModel().UserName);
        SetColorIndex(team, GetUserModel().ColorIndex);
        SetTeam(team);
    }

}

