using UnityEngine;

public class Moovment : MonoBehaviour
{
    // Основные настройки движения
    public float speed = 6f;
    public float speedFust = 10f;
    public float jumpForce = 8f; // Сила прыжка
    public float gravity = -20f; // Увеличим гравитацию для более реалистичного прыжка

    // Настройки приседания
    public float crouchSpeed = 3f; // Скорость в приседе
    public float crouchHeight = 0.5f; // Высота в приседе (относительно исходной)
    public float crouchTransitionSpeed = 10f; // Скорость анимации приседания

    // Приватные переменные
    private CharacterController characterController;
    private Vector3 velocity; // Для учета вертикальной скорости (гравитация + прыжок)
    private bool isGrounded; // Проверка на земле ли персонаж
    private bool isCrouching; // Проверка приседания
    private float originalHeight; // Исходная высота CharacterController
    private Vector3 originalCenter; // Исходный центр CharacterController

    // Настройки проверки земли
    public Transform groundCheck; // Опционально: пустой объект для проверки земли
    public float groundDistance = 0.4f; // Дистанция проверки земли
    public LayerMask groundMask; // Слой земли

    void Start()
    {
        characterController = GetComponent<CharacterController>();

        if (characterController == null)
        {
            Debug.LogError("CharacterController не найден!");
            return;
        }

        // Сохраняем исходные параметры CharacterController
        originalHeight = characterController.height;
        originalCenter = characterController.center;

        // Если не задан groundCheck, используем позицию персонажа
        if (groundCheck == null)
        {
            groundCheck = transform;
        }
    }

    void Update()
    {
        // Проверка на земле ли персонаж
        isGrounded = characterController.isGrounded;
        // Альтернативный способ с Raycast (более точный):
        // isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Если на земле и падает вниз - обнуляем вертикальную скорость
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Небольшая отрицательная скорость для "прижатия" к земле
        }

        // Обработка приседания
        HandleCrouch();

        // Получаем ввод с клавиатуры
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Вычисляем движение
        Vector3 move = transform.right * horizontal + transform.forward * vertical;

        // Определяем скорость в зависимости от состояния
        float currentSpeed = isCrouching ? crouchSpeed : (Input.GetKey(KeyCode.LeftShift) ? speedFust : speed);

        // Применяем скорость и время
        move *= currentSpeed * Time.deltaTime;

        // Прыжок (только если на земле и не приседает)
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }

        // Применяем гравитацию
        velocity.y += gravity * Time.deltaTime;

        // Добавляем вертикальную скорость к движению
        move.y = velocity.y * Time.deltaTime;

        // Двигаем персонажа
        characterController.Move(move);
    }

    void HandleCrouch()
    {
        // Начало приседания (если на земле)
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.C))
        {
            if (isGrounded && !isCrouching)
            {
                isCrouching = true;
            }
        }
        // Прекращение приседания (отпустили клавишу)
        else if ((Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.C)) && isCrouching)
        {
            // Проверяем, нет ли препятствия над головой
            if (!CheckCeiling())
            {
                isCrouching = false;
            }
        }

        // Плавное изменение высоты CharacterController
        if (isCrouching)
        {
            characterController.height = Mathf.Lerp(
                characterController.height,
                originalHeight * crouchHeight,
                Time.deltaTime * crouchTransitionSpeed
            );

            // Корректируем центр контроллера
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

            // Возвращаем центр в исходное положение
            characterController.center = Vector3.Lerp(
                characterController.center,
                originalCenter,
                Time.deltaTime * crouchTransitionSpeed
            );
        }
    }

    bool CheckCeiling()
    {
        // Проверяем Raycast'ом, есть ли что-то над головой
        float rayLength = (originalHeight - characterController.height) + 0.1f;
        Vector3 rayStart = transform.position + Vector3.up * characterController.height;

        if (Physics.Raycast(rayStart, Vector3.up, rayLength))
        {
            return true; // Есть препятствие, нельзя вставать
        }
        return false; // Можно вставать
    }

    // Опционально: для визуализации проверки земли и потолка
    void OnDrawGizmosSelected()
    {
        if (characterController != null)
        {
            // Визуализация проверки потолка
            Gizmos.color = Color.yellow;
            Vector3 rayStart = transform.position + Vector3.up * characterController.height;
            float rayLength = (originalHeight - characterController.height) + 0.1f;
            Gizmos.DrawLine(rayStart, rayStart + Vector3.up * rayLength);
        }
    }
}