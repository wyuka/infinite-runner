using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public GameObject target;
	public float damping = 1;

	private Vector3 originalPosition;
	private Vector3 offset;
	// Use this for initialization
	void Start () {
		originalPosition = transform.position;
		offset = transform.position - target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 desiredPosition = target.transform.position + offset;
		desiredPosition.y = originalPosition.y;
		Vector3 position = Vector3.Lerp (transform.position, desiredPosition, damping);
		transform.position = position;
	}
}
