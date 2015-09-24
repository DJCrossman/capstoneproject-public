using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioClip FireSound;
    public AudioClip HitSound;

    public int Damage = 10;
    public Team BulletBelongsTo;
    public Guid OwnerGuid;

    private UserController _userController;
    private bool _friendlyFire;

    /************************************************************
     * Event Handlers
     ************************************************************/

    private void Awake()
    {
        _userController = global::UserController.Instance;
        _friendlyFire = ServerData.serverInfo.settings.FriendlyFire;

        // Object will destroy after 2 seconds
        Destroy(this.gameObject, 2);
    }

    private void Start()
    {
        AudioSource.PlayClipAtPoint(FireSound, transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsPlayerHit(collision.gameObject)) {
            return;
        }

        if (IsBorderOrGroundHit(collision.gameObject)) {
            return;
        }
    }

    /************************************************************
     * Helper Methods
     ************************************************************/

    private bool IsPlayerHit(GameObject collidedObject)
    {
        if (collidedObject.transform.root.gameObject.CompareTag("Team1Player")) {
            ContactWithPlayer(Team.Team1, collidedObject);
            return true;
        }

        if (collidedObject.transform.root.gameObject.CompareTag("Team2Player")) {
            ContactWithPlayer(Team.Team2, collidedObject);
            return true;
        }

        return false;
    }

    private void ContactWithPlayer(Team team, GameObject collidedObject)
    {
        var PlayerHit = collidedObject.transform.root.GetComponent<Player>();

        // Play animation and audio
        AudioSource.PlayClipAtPoint(HitSound, transform.position);
        if (collidedObject.CompareTag("PlayerLeft")) {
            PlayerHit.HitAnimator.SetTrigger("HitLeft");
        } else {
            PlayerHit.HitAnimator.SetTrigger("HitRight");
        }

        // Calculate damage
        if ((PlayerHit.networkView.isMine == true) &&
            ((BulletBelongsTo != team) || (BulletBelongsTo == team && _friendlyFire == true))) {
            _userController.GetPlayer().PlayerHealth.Hit(Damage, OwnerGuid);
        }

        DestroyObject(gameObject);
    }

    private bool IsBorderOrGroundHit(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Ground") || collidedObject.CompareTag("Border")) {
            DestroyObject(gameObject);
            return true;
        }

        return false;
    }
}
