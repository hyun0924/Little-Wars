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
        other.gameObject.GetComponent<Unit>().OnHit(damage);
        Destroy(gameObject);
    }
}
