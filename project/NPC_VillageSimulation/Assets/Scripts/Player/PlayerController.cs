using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public CharacterController characterController;

    private float _lastYPos;

    private void Start()
    {
        characterController.detectCollisions = true;
        _lastYPos = transform.position.y;
    }

    private void Update()
    {
        float moveInput = Input.GetAxis("Vertical");
        float turnInput = Input.GetAxis("Horizontal");
        float currentYPos = transform.position.y;
        bool alreadyJumping = !Mathf.Approximately(_lastYPos, currentYPos);

        float moveSpeed = 0.25f;
        if (Input.GetKey(KeyCode.LeftShift)) moveSpeed = 0.45f;
        else if (Input.GetKey(KeyCode.LeftAlt)) moveSpeed = 0.15f;

        Vector3 move = transform.TransformDirection(Vector3.forward * moveInput * moveSpeed * Time.unscaledDeltaTime * 40);
        if (Input.GetKey(KeyCode.Space) && !alreadyJumping) move.y = 4f;
        else move.y -= 9.81f * Time.unscaledDeltaTime;

        characterController.Move(move);
        transform.Rotate(0, turnInput * Time.unscaledDeltaTime * 200f, 0);

        _lastYPos = currentYPos;
    }
}