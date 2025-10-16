using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

//TODO: ✅Candy model, collider, delete on collision
//TODO: ❌Candy particle effects
//TODO: ✅Points system
//TODO: ✅Candy magnet
//TODO: ✅[Highest] Jump Physics: The jump feels a little "floaty" almost like the character’s bouncing around on the moon 
//TODO: ✅[Highest] [Level Design] Redesign levels with different themes
//TODO: ✅[Highest] [Level Design] Create a house scene with TV
//TODO: ✅[Highest] Ghost colour: Maybe introduce a progressive color change like a deepening red or a pulsing red glow to signal danger as it gets closer.
//TODO: ✅[High] Power up: One idea: players could collect a "magnet" to pull in multiple candies at once this could add a skill-based layer and fun gameplay variety. You could even tie candy collection to extra lives (e.g. collect 1000 candies = +1 life).
//TODO: ✅ [High] Invincibility
//TODO: [High] [Level Design] Lighting.You might want to add some subtle lighting to balance out the mood and guide the player’s eye.
//TODO: [Medium] Ghost Design: Is the ghost wearing shades?
//TODO: [Medium] [Collectible] Flying player
//TODO: [Low] [Collectible] Drone: Picks up the player 
//TODO: [Low] [Collectible] Candy box with lots of candies to collect
//TODO: [Low] [Collectible] Candy rain
//TODO: [Low] Candies falling from the bucket


public class PlayerController : MonoBehaviour
{
    #region Player Movement
    public bool isGameRunning = false;

    [Header("Movement")]
    [SerializeField] float speed = 5f;
    [SerializeField] float laneOffset = 2f;
    [SerializeField] GameObject candyBucketPrefab;
    [SerializeField] Transform candyBucketHand;
    GameObject candyBucket;

    float originalSpeed = 0f;
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
    public bool isDead = false;

    [Header("Animation")]
    public Animator anim;
    #endregion

    [Header("Candy Magnet")]
    [SerializeField] Transform candyDetectionRayOrigin;
    [SerializeField] float magnetRange = 5f; //Raycast distance
    [SerializeField] string candyTag = "Candy";
    //[SerializeField] GameObject candyCollectionSpehere;
    GameObject currentCandy;
    public int intCandiesCollected = 0;

    [Header("Power Ups")]
    [SerializeField] int intMagnetMilestone = 20;
    public bool isMagnetActive = false;
    [SerializeField] int intSugarRushMilestone = 50;
    [SerializeField] float sugarRushSpeed = 20f;
    bool isSugarRushActive = false;
    int intSugarRushCounter = 0;
    int intMagnetCounter = 0;
    [SerializeField] float magnetRadius = 5f;
    [SerializeField] float pullSpeed = 20f;
    bool isInvincible = false;

    [Header("Hearts")]
    [SerializeField] int heartsCollected = 0;
    [SerializeField] int intHeartsMilestone = 30;
    public int intCandiesForHeart = 0;
    

    [Header("Jet pack")]
    [SerializeField] GameObject jetPack;
    [SerializeField] float jetPackSpeed = 20f;
    public bool isJetPackActive = false;
    [SerializeField] ParticleSystem jetPackParticles;


    [Header("UI")]
    [SerializeField] TextMeshProUGUI candyCounterText;
    [SerializeField] TextMeshProUGUI powerUpText;
    [SerializeField] TextMeshProUGUI magnetCounterText;
    [SerializeField] TextMeshProUGUI sugarRushCounterText;
    [SerializeField] TextMeshProUGUI totalCandiesText;
    [SerializeField] TextMeshProUGUI heartsCollectedText;
    [SerializeField] GameObject UIGameOverPanel;
    [SerializeField] GameObject UIStartPanel;

    #region Post Processing
    [Header("Post Processing")]
    [SerializeField] VolumeProfile postProcessingProfile;
    MotionBlur motionBlur;

    #endregion

    void Start()
    {
        //Time.timeScale = 0.5f; //WARNING: This is a hack
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //candyCollectionSpehere.SetActive(false);
        powerUpText.text = "";
        originalSpeed = speed;
        candyBucket = Instantiate(candyBucketPrefab, candyBucketHand.position, Quaternion.identity);
        UIGameOverPanel.SetActive(false);
        UIStartPanel.SetActive(true);
        isGameRunning = false;
        isDead = false;
        jetPack.SetActive(false);
    }

    void FixedUpdate()
    {
        Vector3 sphereCenter = transform.position + sphereOffset;
        Collider[] hitColliders = Physics.OverlapSphere(sphereCenter, groundCheckRadius, groundLayer);
        isGrounded = hitColliders.Length > 0;

        // Move forward continuously
        if (isGameRunning && !isDead)
        {
            Vector3 newPosition = rb.position + transform.forward * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);
        }
        else rb.linearVelocity = Vector3.zero;

    }

    void Update()
    {
        //Place bucket in player's hand
        candyBucket.transform.position = candyBucketHand.position;

        //Start magnet powerup only after picking up magnet
        if (isMagnetActive) StartCoroutine(MagnetPowerUp());

        if (isJetPackActive) StartCoroutine(JetPackFlight());

        //Update text on the end screen
        if (isDead)
        {
            totalCandiesText.text = intCandiesCollected.ToString();
            magnetCounterText.text = intMagnetCounter.ToString();
            sugarRushCounterText.text = intSugarRushCounter.ToString();
            candyCounterText.enabled = false;
            UIGameOverPanel.SetActive(true);
        }

        if (!isGameRunning || isDead) return;

        CandyDetection();
        UpdateCandyCounter();

        if (intCandiesCollected >= intHeartsMilestone)
        {
            heartsCollected++;
            intCandiesForHeart = 0;
            heartsCollectedText.text = heartsCollected.ToString();

            switch (heartsCollected)
            {
                case 0: intHeartsMilestone = 30;    break;
                case 1: intHeartsMilestone = 60;    break;
                case 2: intHeartsMilestone = 90;    break;
            }
        }

        if (intCandiesCollected >= intMagnetMilestone && !isMagnetActive)
        {
            //This is implemented in the magnet powerup Trigger collision event
            /* intMagnetCounter++;
            StartCoroutine(MagnetPowerUp()); */
        }

        if (intCandiesCollected >= intSugarRushMilestone && !isSugarRushActive)
        {
            intSugarRushCounter++;
            StartCoroutine(SugarRushPowerUp());
        }

        if(isInvincible) StartCoroutine(InvincibilityRoutine());

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
        // Method 1: Instant teleport to lane (if physics interactions are important you can use rb.MovePosition instead)
        transform.position = newPos;

        // Method 2: Use rigidbody move:
        //rb.position = newPos;
        //rb.MovePosition(newPos);

        //Method 3: Move toward new position

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, groundCheckRadius);

        //Candy detection
        Gizmos.color = Color.red;
        Gizmos.DrawRay(candyDetectionRayOrigin.position, Vector3.forward * magnetRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, magnetRadius);
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
                //Debug.Log("Raycast hit object with tag '" + candyTag + "': " + hit.collider.gameObject.name);
                currentCandy = hit.collider.gameObject;
                currentCandy.GetComponent<CollectCandy>().isCollected = true;
            }
        }
    }

    void UpdateCandyCounter()
    {
        candyCounterText.text = intCandiesCollected.ToString();
    }

    IEnumerator MagnetPowerUp()
    {
        isMagnetActive = true;
        powerUpText.text = "MAGNET";
        //candyCollectionSpehere.SetActive(true);

        float duration = 3f;
        float startTime = Time.time;

        // Cache the candies at activation time (optional, or re-scan each frame)
        Collider[] initialCandies = Physics.OverlapSphere(transform.position, magnetRadius);

        while (Time.time < startTime + duration)
        {
            foreach (Collider col in initialCandies)
            {
                if (col != null && col.CompareTag(candyTag))
                {
                    GameObject candy = col.gameObject;

                    /* // Compute progress (0 → 1)
                    float t = (Time.time - startTime) / duration;
                    // Apply easing for smooth start & end
                    float easedT = Mathf.SmoothStep(0f, 1f, t);

                    // Lerp the candy toward the player
                    Vector3 targetPos = transform.position;
                    candy.position = Vector3.Lerp(candy.position, targetPos, easedT * Time.deltaTime * pullSpeed); */

                    candy.GetComponent<CollectCandy>().isCollected = true;
                }
            }

            yield return null; // Wait for the next frame
        }

        isMagnetActive = false;
        powerUpText.text = "";
        //candyCollectionSpehere.SetActive(false);
    }

    IEnumerator SugarRushPowerUp()
    {
        isSugarRushActive = true;
        powerUpText.text = "SUGAR RUSH";
        originalSpeed = speed;
        speed = sugarRushSpeed;
        yield return new WaitForSeconds(5.0f);
        //isSugarRushActive = false;
        powerUpText.text = "";
        speed = originalSpeed;
    }

    void EnableMotionBlur(bool enable)
    {
        if (motionBlur != null)
        {

        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacle" && !isInvincible)
        {
            isDead = true;
            anim.SetTrigger("IsDead");
        }

        if (other.gameObject.tag == "Slider")
        {
            anim.SetTrigger("IsSliding");
        }

        if (other.gameObject.tag == "Magnet")
        {
            if (isMagnetActive) return;
            isMagnetActive = true;
            intMagnetCounter++;
        }

        if (other.gameObject.tag == "Jetpack")
        {
            isJetPackActive = true;
            jetPack.SetActive(true);
            Destroy(other.gameObject);
        }

        if (other.gameObject.tag == "Invincible")
        {
            isInvincible = true;
            StartCoroutine(InvincibilityRoutine());
            Destroy(other.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Ghost") && !isInvincible)
        {
            isDead = true;
            anim.SetTrigger("IsDead");
        }
    }

    /* void ReloadSceneCurrentSceneAfterDelay(float delaySeconds)
    {
        Invoke("ReloadSceneCurrentScene", delaySeconds);
    } */

    public void ReloadSceneCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        UIGameOverPanel.SetActive(false);
        UIStartPanel.SetActive(false);
        isGameRunning = true;
    }

    IEnumerator JetPackFlight()
    {
        if (isGrounded)
        {
            rb.AddForce(new Vector3(0, 1, 0) * jetPackSpeed, ForceMode.Impulse);
            anim.SetTrigger("IsJumping");
            jetPackParticles.Play();
            yield return new WaitForSeconds(1f);
            isJetPackActive = false;
            jetPack.SetActive(false);
            jetPackParticles.Stop();
        }
    }

    IEnumerator InvincibilityRoutine()
    {
        powerUpText.text = "INVINCIBILITY";
        originalSpeed = speed;
        speed = sugarRushSpeed;
        yield return new WaitForSeconds(3f);
        isInvincible = false;
        powerUpText.text = "";
        speed = originalSpeed;
    }
}
