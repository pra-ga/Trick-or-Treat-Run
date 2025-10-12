using System.Collections;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float goldImpulseForce = 5f;
    [SerializeField] Vector3 directionVector = new Vector3(0, 1, -1);
    [SerializeField] GameObject ghost;
    [SerializeField] ParticleSystem goldParticle;
    GhostManager ghostManager;
    GameObject player;
    bool isPlayerHitGold = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ghost = GameObject.FindGameObjectWithTag("Ghost");
        ghostManager = ghost.GetComponent<GhostManager>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Vector3 forceDirection = ghost.transform.position - transform.position;
            rb.AddForce(forceDirection * goldImpulseForce, ForceMode.Impulse);
            isPlayerHitGold = true;
        }

        if (other.gameObject.tag == "Ghost" && isPlayerHitGold)
        {
            ghostManager.distanceGhostAndPlayer = ghostManager.originalDistanceGhostAndPlayer;
            player.GetComponent<PlayerController>().anim.SetTrigger("IsThrowing");
            goldParticle.Play();
            ghostManager.ghostAnimator.SetTrigger("isHit");
            StartCoroutine(DestroyAfterDelay(0.2f));
        }
    }
    
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
