using UnityEngine;

[AddComponentMenu("Camera/CameraMove")]
public class CameraMove : MonoBehaviour
{
	[SerializeField] private float mouseSensitivity = 0.4f;
	[SerializeField] private float moveSpeed = 20f;
	[Range(1, 10), SerializeField] private float shiftMultiplier = 3.7f;

	private Vector3 mousePreviousPos;
	private float rotationX;
	private float rotationY;

	private void Update()
	{
		Move();
		Rotate();
	}

	private void Move()
	{

		float shiftMultiplier = 1;
		if (Input.GetKey(KeyCode.LeftShift))
		{
			shiftMultiplier = this.shiftMultiplier;
		}

		float right = Input.GetAxis("Horizontal");
		float forward = Input.GetAxis("Vertical");
		float up = 0;

		if (Input.GetKey(KeyCode.E))
		{
			up = 1f;
		}
		else if (Input.GetKey(KeyCode.Q))
		{
			up = -1f;
		}

		Vector3 offset = new Vector3(right, up, forward).normalized * (moveSpeed * shiftMultiplier * Time.unscaledDeltaTime);
		transform.Translate(offset);
	}

	private void Rotate()
	{
		if (Input.GetMouseButtonDown(1))
		{
			mousePreviousPos = Input.mousePosition;
		}

		if (Input.GetMouseButton(1))
		{
			Vector3 mouseDelta = Input.mousePosition - mousePreviousPos;
			mousePreviousPos = Input.mousePosition;

			rotationX -= mouseDelta.y * mouseSensitivity;
			rotationY += mouseDelta.x * mouseSensitivity;

			transform.localEulerAngles = new Vector3(Mathf.Clamp(rotationX, -90, 90), rotationY, 0f);
		}
	}
}