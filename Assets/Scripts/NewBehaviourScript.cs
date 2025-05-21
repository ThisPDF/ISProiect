using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 20;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            Debug.Log("Projectile hit " + enemy.name + " for " + damage + " damage.");
        }

        Destroy(gameObject);
    }
}
