using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier = 0.5f; // Adjust for different layers
    private Transform cameraTransform;
    private Vector3 lastCameraPosition;
    private float textureUnitSizeZ; // Depth size of one background unit

    void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Calculate the depth of a single texture or mesh in world units
            textureUnitSizeZ = renderer.bounds.size.z;
            Debug.Log("Texture unit size Z: " + textureUnitSizeZ);
        }
        else
        {
            Debug.LogError("No Renderer found on " + gameObject.name + " — please add one with a visible mesh or sprite.");
        }
    }

    void LateUpdate()
    {
        // Calculate camera movement since last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Apply parallax effect along Z axis (depth)
        transform.position += new Vector3(0, 0, deltaMovement.z * parallaxEffectMultiplier);

        lastCameraPosition = cameraTransform.position;

        // Infinite scrolling along Z axis
        if (Mathf.Abs(cameraTransform.position.z - transform.position.z) >= textureUnitSizeZ)
        {
            float offsetPositionZ = (cameraTransform.position.z - transform.position.z) % textureUnitSizeZ;
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y,
                cameraTransform.position.z + offsetPositionZ
            );
        }
    }
}