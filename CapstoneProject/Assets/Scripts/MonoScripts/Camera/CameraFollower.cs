using UnityEngine;
using System.Collections;

public class CameraFollower : MonoBehaviour {

	public Transform Target;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Target != null) {
			this.transform.position = new Vector3 (Target.position.x, Target.position.y, this.transform.position.z);
		}
	}
}
