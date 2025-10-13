using UnityEngine;

public class FlyingSkull : MonoBehaviour
{
    [Header("Flight Settings")]
    public float forwardSpeed = 5f;   // Speed along -Z axis
    public float amplitude = 1.5f;    // Height of the sine wave
    public float frequency = 2f;      // How fast it oscillates vertically

    [Header("Lifetime Settings (optional)")]
    public float maxTravelDistance = 50f; // Destroy after this much distance
    public bool destroyOnEnd = true;

    private Vector3 basePosition;     // Starting point
    private float startZ;
    private float elapsedTime = 0f;

    [Header("Optional: Player reference to stop movement when dead")]
    public PlayerController playerController;

    void Start()
    {
        basePosition = transform.position;
        startZ = basePosition.z;

        // Optional: auto-assign playerController if not set
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player) playerController = player.GetComponent<PlayerController>();
        }
    }

    void Update()
    {
        // If player is dead, stop movement (optional)
        if (playerController != null && playerController.isDead)
            return;

        elapsedTime += Time.deltaTime;

        // Compute position
        float yOffset = Mathf.Sin(elapsedTime * frequency) * amplitude;
        float newZ = startZ - forwardSpeed * elapsedTime;

        // Apply movement
        Vector3 newPos = new Vector3(basePosition.x, basePosition.y + yOffset, newZ);
        transform.position = newPos;

        /* // Optionally destroy when out of range
        if (destroyOnEnd && Mathf.Abs(newZ - startZ) > maxTravelDistance)
        {
            Destroy(gameObject);
        } */
    }

    // Optional helper if you want to reset motion or reuse
    public void ResetFlight()
    {
        elapsedTime = 0f;
        transform.position = basePosition;
    }
}
