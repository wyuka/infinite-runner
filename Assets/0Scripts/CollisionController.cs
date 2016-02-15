using UnityEngine;
using System.Collections;

public class CollisionController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.name != "Player")
			return;
		Vector3 tr = collider.gameObject.transform.position;
		collider.gameObject.transform.position = new Vector3(tr.x, 5f, tr.z);
	}
}
