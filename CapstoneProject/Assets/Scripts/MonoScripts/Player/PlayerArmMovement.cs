using UnityEngine;
using System.Collections;

public class PlayerArmMovement : MonoBehaviour {

	private GameObject Parent;

	// Use this for initialization
	void Start () {
		// This component should only exist for the local player
		if (networkView.isMine == false) {
			Destroy (this);
		}

		Parent = this.transform.parent.gameObject;
	}

	// Update is called once per frame
	void Update () {
		if (networkView.isMine) {
			RotatePart (this.transform); // Right Arm
		}
	}
	
	private void RotatePart(Transform trans) {
		int multiplier;
		Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - trans.position;
			difference.Normalize ();
		float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

		if (Parent.transform.localScale.x < 0) {
			multiplier = -1;
		} else {
			multiplier = 1;
		}

		if (multiplier == 1) {
			if (rotZ > 90) {
					rotZ = 90;
			} else if (rotZ < -90) {
					rotZ = -90;
			}
		} else {
			if (rotZ > 0 && rotZ < 90) {
				rotZ = 90;
			} else if (rotZ < 0 && rotZ > -90) {
				rotZ = -90;
			}

		}
		
		trans.rotation = Quaternion.Euler (0f, 0f, (rotZ + 90) * multiplier);	
	}
}
