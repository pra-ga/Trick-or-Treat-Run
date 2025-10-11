using UnityEngine;

public class SmoothUpDownMotion : MonoBehaviour
{
    [SerializeField] float speed = 1f; // Controls how fast the object moves up and down
    [SerializeField] float height = 0.1f; // Controls how high the object moves up and down

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position; // Store the initial position
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;

        // Update the object's position with the new Y value, keeping X and Z constant
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}