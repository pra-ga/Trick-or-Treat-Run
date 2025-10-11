using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//TODO: Candy model, collider, delete on collision
//TODO: Candy particle effects
//TODO: Points system

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float laneOffset = 2f;
    Rigidbody rb;
    int trackNumber = 3; // 1 = left, 2 = middle, 3 = right

    [Header("Jump")]
    [SerializeField] float jumpForce = 10f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckRadius = 0.2f;
    public Vector3 sphereOffset = new Vector3(0, -0.1f, 0);

    [Header("Flags")]
    bool isGrounded;
    bool isLaneChanging = false;

    [Header("Animation")]
    Animator anim;

    [Header("Candy Magnet")]
    [SerializeField] Transform candyDetectionRayOrigin;
    [SerializeField] float magnetRange = 5f; //Raycast distance
    [SerializeField] string candyTag = "Candy";
    GameObject currentCandy;
    public int intCandiesCollected = 0;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI candyCounterText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
    }

    void FixedUpdate()
    {
        Vector3 sphereCenter = transform.position + sphereOffset;
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, groundCheckRadius, groundLayer);
        isGrounded = hitColliders.Length > 0;

        // Move forward continuously
        Vector3 newPosition = rb.position + transform.forward * speed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        // Helpful debug
        // Debug.Log($"Track: {trackNumber}");
    }

    void Update()
    {
        CandyDetection();
        UpdateCandyCounter();
    }

    public void OnJump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("IsJumping");
        }
    }

    // --------- Input callbacks (preferred: receive context and check ctx.performed) ----------
    // Wire these in the PlayerInput inspector (they will be invoked with the CallbackContext)
    public void OnTrackHigher(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TryTrackHigher();
    }

    public void OnTrackLower(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        TryTrackLower();
    }

    // If you prefer hooking methods without parameters in the inspector, use these wrappers:
    public void TrackHigherButton() => TryTrackHigher();
    public void TrackLowerButton() => TryTrackLower();

    // ---------- Core lane logic ----------
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
        yield return new WaitForSeconds(0.0f); // keep your delay if needed
        MoveToLane();
        isLaneChanging = false;
    }

    void MoveToLane()
    {
        float newX = (trackNumber - 2) * laneOffset; // lane 1 => -laneOffset, 2 => 0, 3 => +laneOffset
        Vector3 newPos = new Vector3(newX, transform.position.y, transform.position.z);
        // Instant teleport to lane (if physics interactions are important you can use rb.MovePosition instead)
        transform.position = newPos;
        // If you prefer rigidbody move:
        // rb.position = newPos;
        // rb.MovePosition(newPos);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, groundCheckRadius);

        //Candy detection
        Gizmos.color = Color.red;
        Gizmos.DrawRay(candyDetectionRayOrigin.position, Vector3.forward * magnetRange);
    }

    void CandyDetection()
    {
        Vector3 rayOrigin = candyDetectionRayOrigin.position;
        Vector3 rayDirection = Vector3.forward;
        RaycastHit hit;
        // Perform the raycast
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, magnetRange))
        {
            // Check if the hit object has the target tag
            if (hit.collider.CompareTag(candyTag))
            {
                Debug.Log("Raycast hit object with tag '" + candyTag + "': " + hit.collider.gameObject.name);
                currentCandy = hit.collider.gameObject;
                currentCandy.GetComponent<CollectCandy>().isCollected = true;
            }
        }
    }


    void UpdateCandyCounter()
    {
        candyCounterText.text = intCandiesCollected.ToString();
    }  
}
