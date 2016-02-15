using UnityEngine;
using System.Collections;

public class TrackController : MonoBehaviour {
	public static float scrollSpeed = 2f;
	public Renderer rend;
	void Start() {
		rend = GetComponent<Renderer>();
	}
	void Update() {
		float offset = -Time.time * scrollSpeed;
		rend.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
		scrollSpeed += 0.00010f * Time.deltaTime / 15;
	}
}