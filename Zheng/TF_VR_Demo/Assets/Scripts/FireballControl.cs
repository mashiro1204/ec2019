using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballControl : MonoBehaviour
{
    private int playerDamage = 5;
    private int enemyDamage = 45;
    private int enemyDamageSmall = 5;
    private Vector3 velocity;
    private Transform target;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = transform.position + velocity * Time.deltaTime;
        Vector3 direction = velocity;
        direction.Normalize();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit,(newPosition - transform.position).magnitude*7.0f))
        {
            GameObject other = hit.collider.gameObject;

            if (other.CompareTag("Obstacle") || other.CompareTag("Tile"))
            {
                Destroy(gameObject);
            }
            else if(other.CompareTag("Enemy") && tag == "PlayerFireball")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage(enemyDamage);
                Destroy(gameObject);
            }
            else if(other.CompareTag("Enemy") && tag == "PlayerFireballSmall")
            {
                Enemy enemy = other.GetComponent<Enemy>();
                enemy.TakeDamage(enemyDamageSmall);
                Destroy(gameObject);
            }
            else if(other.CompareTag("Player") && tag == "EnemyFireball")
            {
                Player player = other.GetComponent<Player>();
                player.TakeDamage(playerDamage);
                Destroy(gameObject);
            }
        }
        transform.position = newPosition;
    }

    public void SetTarget(Transform mytarget)
    {
        target = mytarget;
    }

    public void SetVelocity(Vector3 myvec)
    {
        velocity = myvec;
    }
}
