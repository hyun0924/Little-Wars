using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    public float damage;

    private void Update()
    {
        float newX = transform.position.x + moveSpeed * Time.deltaTime;
        transform.position = new Vector3(newX, transform.position.y, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        enemyHit(other.gameObject);
        Destroy(gameObject);
    }
    
    private void enemyHit(GameObject enemy)
    {
        Unit unit = enemy.GetComponent<Unit>();
        BaseController b = enemy.GetComponent<BaseController>();
        if (unit != null) unit.OnHit(damage);
        else if (b != null) b.OnHit(damage);
    }
}
