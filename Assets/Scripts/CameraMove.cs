using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    private int direction;
    [SerializeField] private float speed;
    [SerializeField] private GameObject Backgrounds;

    private void Awake()
    {
        direction = 0;
        float initialX = -2.15f;
        transform.position = new Vector3(initialX, 0, -10);
        Backgrounds.transform.GetChild(1).position = new Vector3(initialX/3f, 0, 0);
        Backgrounds.transform.GetChild(2).position = new Vector3(initialX/1.2f, 0, 0);
        Backgrounds.transform.GetChild(3).position = new Vector3(initialX, 0, 0);
    }

    private void Update()
    {
        if (direction == 0) return;

        float newX = transform.position.x + direction * speed;

        if (Mathf.Abs(newX) > 2.15f) return;
        transform.position = new Vector3(newX, transform.position.y, -10);
        Backgrounds.transform.GetChild(1).position = new Vector3(newX/3f, transform.position.y, 0);
        Backgrounds.transform.GetChild(2).position = new Vector3(newX/1.2f, transform.position.y, 0);
        Backgrounds.transform.GetChild(3).position = new Vector3(newX, transform.position.y, 0);
    }

    public void PointerDown(int direction)
    {
        this.direction = direction;
    }

    public void PointerUp()
    {
        direction = 0;
    }
}
