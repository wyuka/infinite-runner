using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public bool jumping = false;
	private Rigidbody rb;
	private Animator anim;
	private CharacterController cc;
	private Vector3 moveDirection;
	private float jumpStartTime;
	private float slidingStartTime;
	public float totalSlideTime = 1.3f;
	private Vector2 touchStartPosition;
	private bool currentSwipeEnded;
	private float originalHeight;
	private float originalCenterY;
	public int currentLane = 1;
	public GameObject followCamera;
	private Vector3 originalCameraOffset;
	private float lastCameraY;

	// Takes values of 0, -1, 1
	private int xSwipe;
	private int ySwipe;

	public bool upInput;
	public bool downInput;
	public bool leftInput;
	public bool rightInput;
	public float damping = 3f;
	public float gravity;

	public float jumpSpeed = 8;
	public float normalGravity = 9;
	public float downGravity = 20;
	public float minXDisp = 20;
	public float minYDisp = 20;
	public bool trial = false;
	public float[] laneX = {-3.3f, 0f, 3.3f};
	public float strafeSpeed = 6;

	public bool sliding = false;
	public bool leftStrafe = false;
	public bool rightStrafe = false;

	int jumpingHash = Animator.StringToHash("jumping");
	int slidingHash = Animator.StringToHash("sliding");
	int heightHash = Animator.StringToHash("Height");
	// Use this for initialization
	void Start () {
		//rb = GetComponent<Rigidbody> ();
		anim = GetComponent<Animator> ();
		cc = GetComponent<CharacterController> ();
		moveDirection = new Vector3 (0, 0, 0);
		currentSwipeEnded = false;
		originalHeight = cc.height;
		originalCenterY = cc.center.y;
		gravity = normalGravity;

		originalCameraOffset = followCamera.transform.position - transform.position;
		lastCameraY = originalCameraOffset.y;
	}

	void UpdateSwipeInfo() {
		xSwipe = 0;
		ySwipe = 0;
		if (Input.touchCount < 1)
			return;

		Touch touch = Input.GetTouch (0);
		if (touch.phase == TouchPhase.Began) {
			touchStartPosition = touch.position;
			currentSwipeEnded = false;
		} else {
			if (currentSwipeEnded) return;
			Vector2 disp = touch.position - touchStartPosition;
			if (Mathf.Abs (disp.x) > minXDisp && Mathf.Abs (disp.y) > minYDisp) {
				if (Mathf.Abs (disp.x) > Mathf.Abs (disp.y)) {
					xSwipe = disp.x > 0 ? 1 : -1;
				} else {
					ySwipe = disp.y > 0 ? 1 : -1;
				}
				currentSwipeEnded = true;
			} else if (Mathf.Abs (disp.x) > minXDisp) {
				xSwipe = disp.x > 0 ? 1 : -1;
				currentSwipeEnded = true;
			} else if (Mathf.Abs (disp.y) > minYDisp) {
				ySwipe = disp.y > 0 ? 1 : -1;
				currentSwipeEnded = true;
			}
		}
	}

	void UpdateInputs() {
		UpdateSwipeInfo ();
		bool swipeUp = ySwipe == 1;
		bool upPress = Input.GetKey("space");
		upInput = swipeUp || upPress;

		bool swipeDown = ySwipe == -1;
		bool downPress = Input.GetKey("down");
		downInput = swipeDown || downPress;

		bool swipeLeft = xSwipe == -1;
		bool leftPress = Input.GetKey ("left");
		leftInput = swipeLeft || leftPress;

		bool swipeRight = xSwipe == 1;
		bool rightPress = Input.GetKey ("right");
		rightInput = swipeRight || rightPress;
	}

	void UpdateWhileJumping ()
	{
		if (downInput) {
			gravity = downGravity;
		}
		float velocityY = moveDirection.y / jumpSpeed;
		anim.SetFloat (heightHash, velocityY);
		cc.height = originalHeight * (0.5f + Mathf.Abs (velocityY / 2));
		cc.center = new Vector3 (cc.center.x, originalHeight - cc.height / 2, cc.center.z);
		if (cc.isGrounded) {
			Debug.LogError ("Ended jump");
			jumping = false;
			cc.height = originalHeight;
			cc.center = new Vector3 (cc.center.x, originalCenterY, cc.center.z);
			anim.SetBool (jumpingHash, false);
		} else {
			Debug.LogError ("Still jumping");
		}
	}

	void UpdateWhileSliding() {
		if (Time.time - slidingStartTime >= totalSlideTime) {
			sliding = false;
			anim.SetBool (slidingHash, false);
		} else {
			float fraction = 1 - Mathf.Abs ((Time.time - slidingStartTime) * 2 / totalSlideTime - 1f);
			cc.height = originalHeight - originalHeight * fraction * fraction * 0.8f;
			if (cc.height < 2 * cc.radius)
				cc.height = 2 * cc.radius;
			cc.Move(new Vector3(0, -Time.deltaTime, 0));
			transform.rotation = Quaternion.Euler(new Vector3(0f, fraction * 75f, 0f));
		}
	}

	void UpdateWhileLeft ()
	{
		if (transform.position.x > laneX[currentLane]) {
			cc.Move (new Vector3 (-strafeSpeed * Time.deltaTime, 0, 0));
		} else {
			leftStrafe = false;
			transform.position = new Vector3 (laneX[currentLane], transform.position.y, transform.position.z);
		}
	}

	void UpdateWhileRight ()
	{
		if (transform.position.x < laneX[currentLane]) {
			cc.Move (new Vector3 (strafeSpeed * Time.deltaTime, 0, 0));
		} else {
			rightStrafe = false;
			transform.position = new Vector3 (laneX[currentLane], transform.position.y, transform.position.z);
		}
	}

	void ApplyGravity ()
	{
		moveDirection.y -= gravity * Time.deltaTime;
		if (moveDirection.y < -jumpSpeed)
			moveDirection.y = -jumpSpeed;
		Debug.Log (moveDirection.y);
		cc.Move (moveDirection * Time.deltaTime);
		Debug.Log (cc.isGrounded);
	}

	bool IsFalling ()
	{
		return !jumping && !cc.isGrounded;
	}

	bool CanJumpOrSlide() {
		return cc.isGrounded && !jumping && !sliding && !leftStrafe && !rightStrafe;
	}

	bool CanLeftStrafe() {
		return !sliding && !leftStrafe && currentLane > 0;
	}

	bool CanRightStrafe() {
		return !sliding && !rightStrafe && currentLane < 2;
	}

	void Update () {
		UpdateInputs ();

		if (trial = CanJumpOrSlide ()) {
			if (upInput) {
				jumping = true;
				gravity = normalGravity;
				anim.SetBool (jumpingHash, true);
				moveDirection.y = jumpSpeed;
				Debug.LogError ("start jump");
			} else if (downInput) {
				sliding = true;
				slidingStartTime = Time.time;
				anim.SetBool (slidingHash, true);
			}
		}
		ApplyGravity ();

		if (leftInput && CanLeftStrafe()) {
			currentLane--;
			rightStrafe = false;
			leftStrafe = true;
		} else if (rightInput && CanRightStrafe()) {
			currentLane++;
			leftStrafe = false;
			rightStrafe = true;
		}
		if (jumping) {
			UpdateWhileJumping ();
		} else if (sliding) {
			UpdateWhileSliding ();
		}

		if (leftStrafe) {
			UpdateWhileLeft ();
		} else if (rightStrafe) {
			UpdateWhileRight ();
		}
		UpdateCamera ();
	}

	void UpdateCamera ()
	{
		Vector3 desiredPosition = originalCameraOffset + transform.position;
		float realDamping = cc.isGrounded ? damping : damping / 4;
		Vector3 position = Vector3.Lerp (followCamera.transform.position, desiredPosition, realDamping);
		followCamera.transform.position = position;
	}

	void OnTriggerEnter(Collider collider) {
		if (collider.GetComponent<CollisionController> () != null) {
			cc.Move (new Vector3 (0, 10, 0));
		}
	}
}
