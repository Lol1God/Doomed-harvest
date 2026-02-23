using UnityEngine;

public class SasiniMouve : MonoBehaviour
{
    public float speed = 6f;
    private Transform player;
    public bool is_trigger = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if(playerObject != null )
        {
            player = playerObject.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Если игрок найден и началась погоня
        if (is_trigger && player != null)
        {
            // Двигаемся к игроку
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                speed * Time.deltaTime
            );

            // Поворачиваемся к игроку (опционально)
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
        }
    }
}
