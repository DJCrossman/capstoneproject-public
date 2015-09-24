public class UserController
{

    private Player _player;
    private Team _team;
    private bool _isDead;

    /**************************************************
     * Getters and Setters
     **************************************************/

    public void SetPlayer(Player player)
    {
        _player = player;
    }

    public Player GetPlayer()
    {
        return _player;
    }

    public void SetDead(bool isDead)
    {
        _isDead = isDead;
    }

    public bool GetDead()
    {
        return _isDead;
    }

    public void SetTeam(Team team)
    {
        _team = team;
    }

    public Team GetTeam()
    {
        return _team;
    }

    /**************************************************
     * Singleton Declaration
     **************************************************/

    private static UserController _instance;

    public static UserController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new UserController();
            }
            return _instance;
        }
    }
    public static void Reset()
    {
        _instance = null;
    }
}
