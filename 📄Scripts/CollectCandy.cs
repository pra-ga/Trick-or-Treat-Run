using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectCandy : MonoBehaviour
{
    public bool isCollected = false;

    public Transform target; // The object to move towards
    public float duration = 2.0f; // Duration of the movement in seconds
    [SerializeField] float stepSize = 0.1f;
    //[SerializeField] ParticleSystem collectCandyParticles;

    private Vector3 startPosition;
    private float startTime;

    GameObject playerObject;
    GameObject candyDetectionRayOrigin;
    bool isMoving = false;

    void Start()
    {
        playerObject = GameObject.FindWithTag("Player");
        Transform childTransform = playerObject.transform.Find("candyDetectionRayOrigin");
        candyDetectionRayOrigin = childTransform.gameObject;
        Debug.Log("candyDetectionRayOrigin: " + candyDetectionRayOrigin.name);
    }


    void Update()
    {
        if (playerObject.GetComponent<PlayerController>().isGameRunning == false || playerObject.GetComponent<PlayerController>().isDead) return;

        if (isCollected && !isMoving)
        {
            target = candyDetectionRayOrigin.transform;
            startPosition = transform.position;
            StartCoroutine(MoveToTargetSmooth());
            //collectCandyParticles.Play();
            //Debug.Log("Particle System is playing: " + collectCandyParticles.isPlaying);
        }
    }

    IEnumerator MoveToTargetSmooth()
    {
        isMoving = true;
        startTime = Time.time;
        Vector3 initialPosition = transform.position;

        while (Time.time < startTime + duration)
        {
            float t = (Time.time - startTime) / duration;
            // Apply an easing function to 't' for ease-in and ease-out
            // For example, a simple ease-in-out using a curve:
            t = Mathf.SmoothStep(0.0f, stepSize, t);

            //collectCandyParticles.Play();
            transform.position = Vector3.Lerp(initialPosition, target.position, t);
            yield return null; // Wait for the next frame
        }

        // Ensure the object reaches the exact target position at the end
        transform.position = target.position;
        isMoving = false;
        playerObject.GetComponent<PlayerController>().intCandiesCollected++;
        //collectCandyParticles.Stop();
        Destroy(gameObject);
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            isCollected = true;
        }
    }
}