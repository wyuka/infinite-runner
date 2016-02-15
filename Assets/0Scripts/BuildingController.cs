using UnityEngine;
using System.Collections;

public class BuildingController : MonoBehaviour {
	public static float speed = 15f;
	public float thresholdZ = -20;
	public float respawnZ = 400;
	public float currentSpeed;
	private Vector3 startPosition;
	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		currentSpeed = speed;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = transform.position;
		pos.z -= speed * Time.deltaTime;
		if (pos.z < thresholdZ) {
			pos.z = respawnZ + pos.z - thresholdZ;
		}
		transform.position = pos;
		speed += 0.00005f * Time.deltaTime;
		currentSpeed = speed;
	}
}
