using System.Collections;
using UnityEngine;

public class PumpkinLSManager : MonoBehaviour
{
    [SerializeField] float hopInterval = 1.0f;
    [SerializeField] float hopHeight = 0.8f;     // how high the hop goes
    [SerializeField] float hopDuration = 0.3f;   // time for a full up-and-down hop

    Rigidbody rb;
    bool isLaneChanging = false;
    public int trackNumber = 3;
    public float laneOffset = 2.0f;
    int direction = 1; // +1 = going right (to higher lanes), -1 = going left
    float baseY;
    PlayerController playerController;
    GameObject player;

    void Start()
    {
        // Begin automatic hopping
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
        baseY = transform.position.y;
        StartCoroutine(AutoHopRoutine());
    }


    IEnumerator AutoHopRoutine()
    {
        while (!playerController.isDead)
        {
            yield return new WaitForSeconds(hopInterval);

            if (direction == 1)
            {
                TryTrackLower(); // move from 1→2→3
                if (trackNumber >= 3) direction = -1;
            }
            else
            {
                TryTrackHigher(); // move from 3→2→1
                if (trackNumber <= 1) direction = 1;
            }
        }
        rb.linearVelocity = Vector3.zero;
    }

     void TryTrackHigher()
    {
        if (isLaneChanging) { Debug.Log("Ignored: lane change already in progress"); return; }
        if (trackNumber <= 1) return;
        trackNumber = Mathf.Max(1, trackNumber - 1);
        StartCoroutine(ChangeLanesDelay());
    }

    void TryTrackLower()
    {
        if (isLaneChanging) { Debug.Log("Ignored: lane change already in progress"); return; }
        if (trackNumber >= 3) return;
        trackNumber = Mathf.Min(3, trackNumber + 1);
        StartCoroutine(ChangeLanesDelay());
    }

    IEnumerator ChangeLanesDelay()
    {
        isLaneChanging = true;

        Vector3 startPos = rb.position;
        float newX = (trackNumber - 2) * laneOffset;
        Vector3 endPos = new Vector3(newX, baseY, startPos.z);

        float elapsed = 0f;

        while (elapsed < hopDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / hopDuration;

            // Smooth interpolation between lanes
            float newXPos = Mathf.Lerp(startPos.x, endPos.x, t);

            // Apply hop (parabolic curve)
            float heightOffset = Mathf.Sin(t * Mathf.PI) * hopHeight;

            Vector3 newPos = new Vector3(newXPos, baseY + heightOffset, startPos.z);
            rb.MovePosition(newPos);

            yield return null;
        }

        rb.MovePosition(endPos); // snap exactly to lane at end
        isLaneChanging = false;
    }

    void MoveToLane()
    {
        float newX = (trackNumber - 2) * laneOffset; // lane 1 => -laneOffset, 2 => 0, 3 => +laneOffset
        Vector3 newPos = new Vector3(newX, transform.position.y, transform.position.z);
        rb.MovePosition(newPos);
    }
    
}
