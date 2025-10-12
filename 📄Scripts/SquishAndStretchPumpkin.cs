using UnityEngine;

public class SquishAndStretchPumpkin : MonoBehaviour
{
    [SerializeField] float hoppingStretchScale = 1.5f;
    [SerializeField] float hoppingSquishScale = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GhostHoppingScaleAnimation(Time.time % 1f);
    }

    private void GhostHoppingScaleAnimation(float t)
    {
        // Scale interpolation
        if (t < 0.5f) // Rising phase
        {   //AudioSource.PlayClipAtPoint(moveSound, transform.position); 
            float riseT = t / 0.5f; // normalized 0→1 during rising
            float xScale = Mathf.Lerp(hoppingStretchScale, hoppingSquishScale, riseT);
            float yScale = Mathf.Lerp(hoppingSquishScale, hoppingStretchScale, riseT);
            float zScale = Mathf.Lerp(hoppingStretchScale, hoppingSquishScale, riseT);
            transform.localScale = new Vector3(xScale, yScale, zScale);
        }
        else // Falling phase
        {
            float fallT = (t - 0.5f) / 0.5f; // normalized 0→1 during falling
            float xScale = Mathf.Lerp(hoppingSquishScale, hoppingStretchScale, fallT);
            float yScale = Mathf.Lerp(hoppingStretchScale, hoppingSquishScale, fallT);
            float zScale = Mathf.Lerp(hoppingSquishScale, hoppingStretchScale, fallT);
            transform.localScale = new Vector3(xScale, yScale, zScale);
        }
    }
}
