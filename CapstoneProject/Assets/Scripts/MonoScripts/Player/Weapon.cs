using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class Weapon : MonoBehaviour
{
    public GameObject BulletPrefab;

    private const float BulletStartOffset = 0.3f;
    private const float BulletForce = 2000f;

    private HUDController _hudController;
    private UserController _userController;
    private int _currentAmmo;
    private int _damage;
    private float _fireRate;
    private bool _shoot = false;
    private float _timeToFire;

    /**************************************************
     * Event Handlers
     **************************************************/

    private void Awake()
    {
        if (networkView.isMine == true) {
            _userController = global::UserController.Instance;
            _hudController = global::HUDController.Instance;
            _damage = 10;
            _currentAmmo = 40;
            _fireRate = 0;
            _hudController.AmmoText.text = _currentAmmo.ToString();
        }
    }

    private void OnGUI()
    {
        if (networkView.isMine == false) return;

        if (Input.GetMouseButtonDown(0) && IsControllable()) {
            if (_fireRate <= 0.001f) {
                _shoot = true;
            } else if (Time.time > _timeToFire) {
                _timeToFire = Time.time + 1/_fireRate;
                _shoot = true;
            }
        }
    }

    private void Update()
    {
        if (_shoot == false || Input.GetMouseButtonUp(0) == false) return;

        _shoot = false;
        if (_currentAmmo > 0) {
            float angle = transform.parent.eulerAngles.z * Mathf.Deg2Rad;
            int multiplier = transform.parent.parent.localScale.x < 0 ? -1 : 1;

            networkView.RPC("ShootRpc", RPCMode.All, new object[] {
                transform.position,
                angle,
                _damage,
                multiplier,
                Quaternion.identity
            });

            _hudController.AmmoText.text = (--_currentAmmo).ToString();
        }
    }

    /**************************************************
     * Helper Methods
     **************************************************/

    public int GetAmmo()
    {
        return _currentAmmo;
    }

    /**************************************************
     * Helper Methods
     **************************************************/

    private bool IsControllable()
    {
        return 
            EventSystem.current.IsPointerOverGameObject() == false &&
            _userController.GetPlayer().IsActive == true &&
            _userController.GetPlayer().PlayerNetwork.GetUserModel().Dead == false;
    }

    public void UpdateWeapon(int ammo, int damage, float fireRate)
    {
        _currentAmmo = ammo;
        _damage = damage;
        _fireRate = fireRate;
    }

    public void UpdateAmmo(int ammo)
    {
        _currentAmmo += ammo;
    }

    /**************************************************
     * RPC Handlers
     **************************************************/

    [RPC]
    public void ShootRpc(Vector3 position, float angle, int damage, int multiplier, Quaternion rotation)
    {
        var userModel = transform.root.GetComponent<PlayerNetwork>().GetUserModel();
        var bulletDirection = new Vector2(multiplier * Mathf.Sin(angle), -1 * Mathf.Cos(angle));
        var team = (Team) userModel.CurrentTeam;
        var bullet = (GameObject) Instantiate(BulletPrefab);
        var bulletScript = bullet.GetComponent<Bullet>();

        bullet.transform.position = new Vector2(position.x + BulletStartOffset * bulletDirection.x, position.y + BulletStartOffset * bulletDirection.y);
        bulletScript.BulletBelongsTo = team;
        bulletScript.Damage = damage;
        bulletScript.OwnerGuid = userModel.Uid;

        if (team == Team.Team1) {
            bullet.tag = "Team1Projectile";
        } else {
            bullet.tag = "Team2Projectile";
        }

        bullet.rigidbody2D.AddRelativeForce(new Vector2(bulletDirection.x*BulletForce, bulletDirection.y*BulletForce));
        bullet.transform.rotation = bulletDirection.x <= 0
            ? Quaternion.Euler(0, 0, -(angle*Mathf.Rad2Deg) + 270)
            : Quaternion.Euler(0, 0, (angle*Mathf.Rad2Deg) - 90);
    }
}
