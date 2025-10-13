using UnityEngine;
using System.Collections;

public class PumpkinSkipping : MonoBehaviour
{
    public Rigidbody rb;
    public float laneOffset = 2f;       // distance between lanes
    public int trackNumber = 2;        // 1,2,3 ; set to 2 to start at original spot
    public bool isLaneChanging = false;

    public float hopHeight = 0.8f;
    public float hopDuration = 0.3f;
    public float hopInterval = 1f;

    private int direction = 1; // +1 = forward (to higher tracks), -1 = backward
    private float baseY;
    private float baseZ;       // <<< base Z captured at Start()
    private GameObject player;
    private PlayerController playerController;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player) playerController = player.GetComponent<PlayerController>();

        rb = GetComponent<Rigidbody>();
        baseY = transform.position.y;
        baseZ = transform.position.z;   

        StartCoroutine(AutoHopRoutine());
    }

    IEnumerator AutoHopRoutine()
    {
        // if you don't have a playerController, just use while(true) instead
        while (playerController == null || !playerController.isDead)
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

        // stop any movement when loop ends (player dead)
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
        float targetZ = baseZ + (trackNumber - 2) * laneOffset; // use baseZ as center
        Vector3 endPos = new Vector3(startPos.x, baseY, targetZ);

        float elapsed = 0f;

        while (elapsed < hopDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / hopDuration);

            // Smooth interpolation between lanes along Z
            float newZPos = Mathf.Lerp(startPos.z, endPos.z, t);

            // Parabolic hop in Y (0 -> peak -> 0)
            float heightOffset = Mathf.Sin(t * Mathf.PI) * hopHeight;

            Vector3 newPos = new Vector3(startPos.x, baseY + heightOffset, newZPos);
            rb.MovePosition(newPos);

            yield return null;
        }

        // Ensure exact end
        rb.MovePosition(endPos);
        isLaneChanging = false;
    }

    void MoveToLane()
    {
        float targetZ = baseZ + (trackNumber - 2) * laneOffset;
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, targetZ);
        rb.MovePosition(newPos);
    }
}
