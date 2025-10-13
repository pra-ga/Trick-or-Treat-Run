using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GhostManager : MonoBehaviour
{
    #region Declarations
    [Header("Ghost Animation Params")]
    //[SerializeField] float speed = 5.5f;
    public float distanceGhostAndPlayer = 5f;
    public float originalDistanceGhostAndPlayer;
    public bool isHit = false;
    public Animator ghostAnimator;
    bool isChangingLane = false;
    [SerializeField] float distanceReductionRate = 0.1f;
    [SerializeField] GameObject playerObject;
    [SerializeField] Transform followTransform;
    [SerializeField] float delayLaneChange = 0.1f;
    [SerializeField] float hopDistance = 1f;       // distance to move along z each hop
    [SerializeField] float hopDuration = 0.5f;     // time to complete the hop
    [SerializeField] float waitDuration = 0.5f;    // wait on tile after hop
    [SerializeField] float hopHeight = 0.5f;       // max jump height
    float xMinScale = 0.8f; float xMaxScale = 1.0f;
    float zMinScale = 0.8f; float zMaxScale = 1.0f;
    float yMinPos = 0.0f; float yMaxPos = 0.4f;
    [SerializeField] float hoppingSquishScale = 0.8f;
    [SerializeField] float hoppingStretchScale = 1.0f;
    [SerializeField] float shakeMagnitude = 0.1f;  // how much it shakes before exploding
    [SerializeField] float inflateScale = 3f;      // how big it gets before destroying
    Vector3 originalScale;
    [SerializeField] float ghostMoveSpeed = 1f;
    Material ghostMaterialOriginal;
    [SerializeField] Material ghostMaterialDisobey;
    Rigidbody rb;
    PlayerController playerController;

    #endregion

    void Start()
    {
        //GhostIdleAnimation();
        GhostHoppingScaleAnimation(0f);
        rb = GetComponent<Rigidbody>();
        originalDistanceGhostAndPlayer = distanceGhostAndPlayer;
        ghostAnimator = GetComponentInChildren<Animator>();
        playerController = playerObject.GetComponent<PlayerController>();
    }

    void Update()
    {
        GhostHoppingScaleAnimation(Time.time % 1f);
        if (playerController.isGameRunning && !playerController.isDead)
        {
            ReduceDistanceOverTime();
        }

    }

    void FixedUpdate()
    {
        if (playerController.isGameRunning && !playerController.isDead)
        {
            // Gradually reduce distance here instead of Update()
            distanceGhostAndPlayer -= distanceReductionRate * Time.fixedDeltaTime;

            // Smoothly follow Z position
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, followTransform.position.z - distanceGhostAndPlayer);
            rb.MovePosition(targetPos);

            // Detect and apply X change
            if (!isChangingLane && Mathf.Abs(playerObject.transform.position.x - transform.position.x) > 0.01f)
            {
                StartCoroutine(ChangeXPosAfterDelay(delayLaneChange));
            }
        }
    }

    /* void FixedUpdate()
    {
        DetectChangeInXPosOfPlayer();
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, followTransform.position.z - distanceGhostAndPlayer);
        rb.MovePosition(targetPos);
    } */

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

    /* void GhostIdleAnimation()
    {
        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
        float newX = Mathf.Lerp(xMaxScale, xMinScale, t);
        float newZ = Mathf.Lerp(zMaxScale, zMinScale, t);
        float newYPos = Mathf.Lerp(yMinPos, yMaxPos, t);

        transform.localScale = new Vector3(
            newX,
            originalScale.y,
            newZ
        );

        transform.localPosition = new Vector3(
            transform.localPosition.x,
            newYPos,
            transform.localPosition.z
        );
    } */


    void LateUpdate()
    {
        if (playerController.isGameRunning && !playerController.isDead)
        {
            Vector3 ghostPos = transform.position;
            ghostPos.z = followTransform.position.z - distanceGhostAndPlayer;
            //transform.position = ghostPos;
            //Debug.Log("Distance: " + Vector3.Distance(transform.position, followTransform.position));
        }
    }


    /* IEnumerator ChangeXPosAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.position = new Vector3(playerObject.transform.position.x, transform.position.y, transform.position.z);
    } */

    IEnumerator ChangeXPosAfterDelay(float delay)
    {
        isChangingLane = true;
        yield return new WaitForSeconds(delay);

        // Move via Rigidbody, not transform
        Vector3 newPos = new Vector3(playerObject.transform.position.x, transform.position.y, transform.position.z);
        rb.MovePosition(newPos);

        isChangingLane = false;
    }

    void ReduceDistanceOverTime()
    {
        if (!playerController.isDead)
        {
            distanceGhostAndPlayer -= distanceReductionRate * Time.deltaTime;
        }
    }
}
