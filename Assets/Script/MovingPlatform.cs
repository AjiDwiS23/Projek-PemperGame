using UnityEngine;
using System;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Vector3 pointA;
    [SerializeField] private Vector3 pointB;
    [SerializeField] private float speed = 2f;

    private Vector3 target;
    private Vector3 lastPosition;
    private Vector3 velocity;

    [SerializeField] private bool isActive = false;

    public Action OnReachedPointB; // Event untuk notifikasi

    private void Start()
    {
        target = pointB;
        lastPosition = transform.position;
    }

    private void Update()
    {
        if (isActive)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target) < 0.01f)
            {
                if (target == pointB)
                {
                    isActive = false; // Berhenti di point B
                    OnReachedPointB?.Invoke(); // Panggil event
                }
                target = (target == pointA) ? pointB : pointA;
            }
        }
        velocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }

    public Vector3 GetPlatformVelocity()
    {
        return velocity;
    }

    public void ActivatePlatform()
    {
        isActive = true;
    }

    public void DeactivatePlatform()
    {
        isActive = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(pointA, 0.1f);
        Gizmos.DrawSphere(pointB, 0.1f);
        Gizmos.DrawLine(pointA, pointB);
    }
}
