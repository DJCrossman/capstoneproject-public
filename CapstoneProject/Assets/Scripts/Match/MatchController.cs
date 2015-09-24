using System.Linq;
using UnityEngine;

using TeamDictionary = System.Collections.Generic.Dictionary<UnityEngine.NetworkPlayer, UserModel>;

public enum GameState {None, MatchStart, RoundStart, RoundInProgress, RoundEnd, MatchEnd};

public class MatchController : MonoBehaviour
{
    public AudioClip[] WinLoseClip;

    private TeamsController _teamsController;
    private SpawnController _spawnController;
    private MatchRoundMenuManager _matchRoundMenuManager;
    private HUDController _hudController;
    private MatchModel _matchModel;
    private CaptureController _captureController;
    private UserController _userController;
    private GameState _currentState = GameState.None;
    private GameState _previousState = GameState.None;
    private double _countdownTime = 0;
    private int _winningRoundPoints = 450;
    private int _baseRoundPoints = 300;
    private Team _roundWinner;
    private Team _matchWinner;
    private bool _startMatchFlag = false;
    private bool _allPlayersDead = false;
    private bool _areaCaptured = false;

    /************************************************************
   * Event Handlers
   ************************************************************/

    private void Awake()
    {
        _matchRoundMenuManager = global::MatchRoundMenuManager.Instance;
        _teamsController = global::TeamsController.Instance;
        _spawnController = global::SpawnController.Instance;
        _hudController = global::HUDController.Instance;
        _captureController = global::CaptureController.Instance;
        _userController = global::UserController.Instance;
    }

    void FixedUpdate()
    {
        if (Network.isClient) return;

        bool isNewState = _previousState != _currentState;
        _previousState = _currentState;

        switch (_currentState) {
            case GameState.None:
                _currentState = NoneFrame(isNewState);
                break;
            case GameState.MatchStart:
                _currentState = MatchStartFrame(isNewState);
                break;
            case GameState.RoundStart:
                _currentState = RoundStartFrame(isNewState);
                break;
            case GameState.RoundInProgress:
                _currentState = RoundInProgressFrame(isNewState);
                break;
            case GameState.RoundEnd:
                _currentState = RoundEndFrame(isNewState);
                break;
            case GameState.MatchEnd:
                _currentState = MatchEndFrame(isNewState);
                break;
        }
    }

    /************************************************************
   * State Frames
   ************************************************************/

    private GameState NoneFrame(bool isFirstRun)
    {
        return GameState.None;
    }

    private GameState MatchStartFrame(bool isFirstRun) {
        if (isFirstRun == true) {
            var userModels = _teamsController.GetUserModels();
            userModels.ForEach(delegate(UserModel model) {
                _teamsController.UpdateUserModelForAll(model.Uid, UserModelField.Money, _baseRoundPoints);
                _teamsController.UpdateUserModelForAll(model.Uid, UserModelField.Points, 0);
                _teamsController.UpdateUserModelForAll(model.Uid, UserModelField.Kills, 0);
                _teamsController.UpdateUserModelForAll(model.Uid, UserModelField.Deaths, 0);
            });
            networkView.RPC("MatchBeginRpc", RPCMode.All);
            _countdownTime = (double)ServerData.serverInfo.settings.EndOfRoundTime;
            _matchRoundMenuManager.SetCountdownTime(ServerData.serverInfo.settings.EndOfRoundTime);
            _matchRoundMenuManager.SetPrompt("A new match is about to begin in...");
            _matchRoundMenuManager.StartCountdown();
        }

        _countdownTime -= Time.fixedDeltaTime;
        if (_countdownTime <= 0) {
            _spawnController.Respawn();
            return GameState.RoundStart;
        }

        return GameState.MatchStart;
    }

    private GameState RoundStartFrame(bool isFirstRun) {
        if (isFirstRun == true) {
            _allPlayersDead = false;
            _areaCaptured = false;
            networkView.RPC("RoundBeginRpc", RPCMode.All);
            _countdownTime = (double)ServerData.serverInfo.settings.WarmupTime;
            _matchRoundMenuManager.SetCountdownTime(ServerData.serverInfo.settings.WarmupTime);
            _matchRoundMenuManager.SetPrompt("This round will start in...");
            _matchRoundMenuManager.StartCountdown();
        }

        _countdownTime -= Time.fixedDeltaTime;
        if (_countdownTime <= 0) {
            networkView.RPC("StartTimerRpc", RPCMode.All);
            _captureController.SetActive(true);
            return GameState.RoundInProgress;
        }

        return GameState.RoundStart;
    }

    private GameState RoundInProgressFrame(bool isFirstRun)
    {
        if (isFirstRun == true) {
            Debug.Log("Round in progress");
            _countdownTime = (double)ServerData.serverInfo.settings.RoundTime;
        }

        _countdownTime -= Time.fixedDeltaTime;
        if (_countdownTime <= 0) {
            _roundWinner = Team.None;
            return GameState.RoundEnd;
        }

        if ((_allPlayersDead == true) || (_areaCaptured == true)) {
           return GameState.RoundEnd;
        }

        return GameState.RoundInProgress;
    }

    private GameState RoundEndFrame(bool isFirstRun) {
        if (isFirstRun == true) {
            networkView.RPC("RoundEndRpc", RPCMode.All, (int) _roundWinner);

            _captureController.SetActive(false);

            if (_matchModel.GetTeamWins(Team.Team1) > (ServerData.serverInfo.settings.RoundsInMatch - _matchModel.GetDraws()) / 2) {
                _matchWinner = Team.Team1;
                return GameState.MatchEnd;
            }
            if (_matchModel.GetTeamWins(Team.Team2) > (ServerData.serverInfo.settings.RoundsInMatch - _matchModel.GetDraws()) / 2) {
                _matchWinner = Team.Team2;
                return GameState.MatchEnd;
            }
            _countdownTime = (double)ServerData.serverInfo.settings.EndOfRoundTime;
            _matchRoundMenuManager.SetCountdownTime(ServerData.serverInfo.settings.EndOfRoundTime);
            _matchRoundMenuManager.SetPrompt("A new round is about to begin in...");
            _matchRoundMenuManager.StartCountdown();
        }

        _countdownTime -= Time.fixedDeltaTime;
        if (_countdownTime <= 0) {
            _spawnController.Respawn();
            return GameState.RoundStart;
        }

        return GameState.RoundEnd;
    }

    private GameState MatchEndFrame(bool isFirstRun)
    {
        if (isFirstRun)
        {
            if (_matchWinner == Team.Team1) {
                _matchRoundMenuManager.SetPrompt("The match is over! Invaders win!");
            } else if (_matchWinner == Team.Team2) {
                _matchRoundMenuManager.SetPrompt("The match is over! Defenders win!");
            }
            
            _matchRoundMenuManager.ShowForAll();
        }

        return GameState.MatchEnd;
    }

   /************************************************************
   * Control Methods
   ************************************************************/

    public void StartMatch()
    {
        _currentState = GameState.MatchStart;
    }

    public void AllPlayersDead(Team team)
    {
        _allPlayersDead = true;

        if (team != Team.Team1) {
            _roundWinner = Team.Team1;
        } else {
            _roundWinner = Team.Team2;
        }
    }

    public void CapturedArea(Team team)
    {
        _areaCaptured = true;
        _roundWinner = team;
    }

    /**************************************************
     * Getters and Setter
     **************************************************/

    public MatchModel GetMatchModel()
    {
        return _matchModel;
    }

    public GameState GetCurrentGameState()
    {
        return _currentState;
    }

    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    private void StartTimerRpc()
    {
        _hudController.StartTimer(ServerData.serverInfo.settings.RoundTime);
    }

    [RPC]
    private void MatchBeginRpc()
    {
        _matchModel = new MatchModel();
    }

    [RPC]
    private void MatchEndRpc()
    {

    }

    [RPC]
    private void RoundBeginRpc()
    {

    }

    [RPC]
    private void RoundEndRpc(int winningTeam)
    {
        var guid = ServerData.serverInfo.userData.UID;
        var team = _teamsController.GetUserModels().First(model => model.Uid == guid).CurrentTeam;
        _hudController.StopTimer();

        if(winningTeam == team) {
            audio.clip = WinLoseClip[0];
            _teamsController.UpdateUserModelForAll(guid, UserModelField.MoneyIncrement, _winningRoundPoints);
            _teamsController.UpdateUserModelForAll(guid, UserModelField.PointsIncrement, _winningRoundPoints);
        } else {
            _teamsController.UpdateUserModelForAll(guid, UserModelField.MoneyIncrement, _baseRoundPoints);
            _teamsController.UpdateUserModelForAll(guid, UserModelField.PointsIncrement, _baseRoundPoints);
            audio.clip = WinLoseClip[1];
        }
        
        audio.Play();
        _matchModel.WinningTeam((Team) winningTeam);
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static MatchController _instance;

    public static MatchController Instance
    {
        get { return _instance ?? (_instance = FindObjectOfType<MatchController>()); }
    }

    public static void Reset()
    {
        _instance = null;
    }
}
