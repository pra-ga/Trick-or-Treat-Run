//Obsolete
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyMagnet : MonoBehaviour
{
    [Header("Magnet Settings")]
    [Tooltip("How close candies need to be to start attracting.")]
    public float magnetRange = 1.0f;

    [Tooltip("How fast candies move toward the player when attracted.")]
    public float attractionSpeed = 8f;

    [Tooltip("How far to check in front of the player (Z direction).")]
    public float forwardAngleThreshold = 45f;

    [Tooltip("Tag of the candy objects.")]
    public string candyTag = "Candy";

    [Tooltip("Delay before candy is destroyed once collected (for VFX).")]
    public float collectDelay = 0.1f;

    private List<Transform> attractedCandies = new List<Transform>();

    void Update()
    {
        // Find all candies in the scene
        GameObject[] candies = GameObject.FindGameObjectsWithTag(candyTag);
        

        foreach (GameObject candy in candies)
        {
            if (candy == null)
            {
                Debug.LogWarning("Candy is null");
                continue;
            }

            Vector3 directionToCandy = (candy.transform.position - transform.position);
            float distance = directionToCandy.magnitude;

            // Skip if too far
            if (distance > magnetRange)
            {
                Debug.LogWarning("Skip if too far");
                continue;
            }

            // Check if candy is roughly in front of player (based on facing direction)
            float angle = Vector3.Angle(transform.forward, directionToCandy);
            if (angle > forwardAngleThreshold) continue;

            // Start attracting this candy
            if (!attractedCandies.Contains(candy.transform))
                attractedCandies.Add(candy.transform);
        }

        // Move attracted candies
        for (int i = attractedCandies.Count - 1; i >= 0; i--)
        {
            Transform candy = attractedCandies[i];
            if (candy == null)
            {
                attractedCandies.RemoveAt(i);
                continue;
            }

            // Smooth "swoosh" motion towards the player
            candy.position = Vector3.Lerp(candy.position, transform.position, Time.deltaTime * attractionSpeed);

            // Optional: add a little rotation for visual flair
            candy.Rotate(Vector3.up, 360 * Time.deltaTime);

            Debug.Log("Vector3.Distance(candy.position, transform.position): " + Vector3.Distance(candy.position, transform.position));
            // Collect if very close
            if (Vector3.Distance(candy.position, transform.position) < 0.2f)
            {
                Debug.Log("Collecting candy");
                StartCoroutine(CollectCandy(candy.gameObject));
                attractedCandies.RemoveAt(i);
            }
        }
    }

    private IEnumerator CollectCandy(GameObject candy)
    {
        Debug.Log("Collecting candy delay");
        // Optional: trigger particle, sound, or animation here before destroy
        yield return new WaitForSeconds(collectDelay);
        Destroy(candy);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize magnet range in Scene view
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, magnetRange);

        // Show facing direction
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, transform.forward * magnetRange);
    }
}
