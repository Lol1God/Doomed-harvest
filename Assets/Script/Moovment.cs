using UnityEngine;

public class Moovment : MonoBehaviour
{
    // �������� ��������� ��������
    public float speed = 6f;
    public float speedFust = 10f;
    public float jumpForce = 8f; // ���� ������
    public float gravity = -20f; // �������� ���������� ��� ����� ������������� ������

    // ��������� ����������
    public float crouchSpeed = 3f; // �������� � �������
    public float crouchHeight = 0.5f; // ������ � ������� (������������ ��������)
    public float crouchTransitionSpeed = 10f; // �������� �������� ����������

    // ��������� ����������
    private CharacterController characterController;
    private Vector3 velocity; // ��� ����� ������������ �������� (���������� + ������)
    private bool isGrounded; // �������� �� ����� �� ��������
    private bool isCrouching; // �������� ����������
    private float originalHeight; // �������� ������ CharacterController
    private Vector3 originalCenter; // �������� ����� CharacterController

    // ��������� �������� �����
    public Transform groundCheck; // �����������: ������ ������ ��� �������� �����
    public float groundDistance = 0.4f; // ��������� �������� �����
    public LayerMask groundMask; // ���� �����

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController �� ������!");
            return;
        }

        // ��������� �������� ��������� CharacterController
        originalHeight = characterController.height;
        originalCenter = characterController.center;

        // ���� �� ����� groundCheck, ���������� ������� ���������
        if (groundCheck == null)
        {
            groundCheck = transform;
        }
    }

    void Update()
    {
        //22789304005976
        // �������� �� ����� �� ��������
        isGrounded = characterController.isGrounded;
        // �������������� ������ � Raycast (����� ������):
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // ���� �� ����� � ������ ���� - �������� ������������ ��������
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ��������� ������������� �������� ��� "��������" � �����
        }

        // ��������� ����������
        HandleCrouch();

        // �������� ���� � ����������
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // ��������� ��������
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // ���������� �������� � ����������� �� ���������
        float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? speedFust : speed);

        // ��������� �������� � �����
        move *= currentSpeed * Time.deltaTime;

        // ������ (������ ���� �� ����� � �� ���������)
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // ��������� ����������
        velocity.y += gravity * Time.deltaTime;

        // ��������� ������������ �������� � ��������
        move.y = velocity.y * Time.deltaTime;

        // ������� ���������
        characterController.Move(move);
    }

    void HandleCrouch()
    {
        // ������ ���������� (���� �� �����)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            if (isGrounded && !isCrouching)
            {
                isCrouching = true;
            }
        }
        // ����������� ���������� (��������� �������)
        else if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C)) && isCrouching)
        {
            // ���������, ��� �� ����������� ��� �������
            if (!CheckCeiling())
            {
                isCrouching = false;
            }
        }

        // ������� ��������� ������ CharacterController
        if (isCrouching)
        {
            characterController.height = Mathf.Lerp(
                characterController.height,
                originalHeight * crouchHeight,
                Time.deltaTime * crouchTransitionSpeed
            );

            // ������������ ����� �����������
            Vector3 newCenter = originalCenter;
            newCenter.y = characterController.height / 2f;
            characterController.center = Vector3.Lerp(
                characterController.center,
                newCenter,
                Time.deltaTime * crouchTransitionSpeed
            );
        }
        else
        {
            characterController.height = Mathf.Lerp(
                characterController.height,
                originalHeight,
                Time.deltaTime * crouchTransitionSpeed
            );

            // ���������� ����� � �������� ���������
            characterController.center = Vector3.Lerp(
                characterController.center,
                originalCenter,
                Time.deltaTime * crouchTransitionSpeed
            );
        }
    }

    bool CheckCeiling()
    {
        // ��������� Raycast'��, ���� �� ���-�� ��� �������
        float rayLength = (originalHeight - characterController.height) + 0.1f;
        Vector3 rayStart = transform.position + Vector3.up * characterController.height;

        if (Physics.Raycast(rayStart, Vector3.up, rayLength))
        {
            return true; // ���� �����������, ������ ��������
        }
        return false; // ����� ��������
    }

    // �����������: ��� ������������ �������� ����� � �������
    void OnDrawGizmosSelected()
    {
        if (characterController != null)
        {
            // ������������ �������� �������
            Gizmos.color = Color.yellow;
            Vector3 rayStart = transform.position + Vector3.up * characterController.height;
            float rayLength = (originalHeight - characterController.height) + 0.1f;
            Gizmos.DrawLine(rayStart, rayStart + Vector3.up * rayLength);
        }
    }
}