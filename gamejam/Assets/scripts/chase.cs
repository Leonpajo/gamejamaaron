using UnityEngine;

public class DragonEnemy : MonoBehaviour
{
    public Transform player;
    public float speed = 6f;
    public float chaseRange = 20f;
    public float floatHeight = 3f;
    private bool activated = false;

    private void Start()
    {
        Invoke(nameof(ActivateDragon), 4f);
    }

    public void ActivateDragon()
    {
        activated = true;
    }

    private void Update()
    {
        if (!activated) return;

        Vector3 targetPos = new Vector3(player.position.x, player.position.y + floatHeight, player.position.z);
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance <= chaseRange)
        {
            Vector3 direction = (targetPos - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
            transform.LookAt(targetPos);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            DeathScreen.Instance.ShowDeathScreen();
        }
    }
}