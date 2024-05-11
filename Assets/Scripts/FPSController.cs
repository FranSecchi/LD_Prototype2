using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSController : MonoBehaviour
{

    public float m_speed;
    float m_yaw;
    float m_pitch;

    public Transform m_PitchController;

    public float m_mouseSensitivityX;
    public float m_mouseSensitivityY;
    public float m_rotationSpeed;

    public bool flag;

    public float m_angleVisionY;

    public KeyCode jump;
    public float m_jumpImpulse;


    public KeyCode run;
    private CharacterController m_cc;
    private Vector3 m_front;
    private Vector3 m_right;
    private Vector3 m_movement;

    private float m_verticalVelocity;
    private bool m_OnGrounded;
    private bool isDead = false;
    private const float m_surfaceGravity = -9.8f;

    // Start is called before the first frame update
    void Start()
    {
        m_yaw = transform.rotation.x;
        m_pitch = transform.rotation.y;
        m_cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        InputRotation();
        if (isDead) return;
        InputMovement();

        if (Input.GetKeyDown(jump))
        {
            m_verticalVelocity = m_jumpImpulse;
        }
    }

    private void InputMovement()
    {
        m_front = transform.forward * Input.GetAxis("Vertical");
        m_right = transform.right * Input.GetAxis("Horizontal");

        m_movement = m_front + m_right;
        m_verticalVelocity += m_OnGrounded ? 0 : m_surfaceGravity * Time.deltaTime;
        m_movement.y = m_verticalVelocity;
        CollisionFlags collision = m_cc.Move(m_movement * m_speed * Time.deltaTime);
        if (collision.Equals(CollisionFlags.Below))
        {
            m_OnGrounded = true;
            m_verticalVelocity = 0f;
        }
        else
        {
            m_OnGrounded = false;
        }
    }

    private void InputRotation()
    {
        float mousePositionX = Input.GetAxis("Mouse X") * m_mouseSensitivityX;
        float mousePositionY = Input.GetAxis("Mouse Y") * m_mouseSensitivityY;
        //float smoothFactor = 0.1f; // Adjust as needed
        //float smoothedMousePositionX = Mathf.Lerp(0, mousePositionX, smoothFactor);
        //float smoothedMousePositionY = Mathf.Lerp(0, mousePositionY, smoothFactor);

        m_pitch -= mousePositionY;
        m_pitch = Mathf.Clamp(m_pitch, -m_angleVisionY / 2, m_angleVisionY / 2); // Ensure pitch stays within desired range

        //m_yaw += mousePositionX;
        m_PitchController.localRotation = Quaternion.Euler(m_pitch, 0.0f, 0.0f);
        transform.rotation *= Quaternion.Euler(0.0f, mousePositionX, 0.0f);
    }
    internal void Die()
    {
        isDead = true;
        StartCoroutine(Respawn());
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForSeconds(0.5f);
        isDead = false;
    }
}
