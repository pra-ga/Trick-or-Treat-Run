
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandyMagnet : MonoBehaviour
{
    [SerializeField] GameObject player;
    PlayerController playerController;
    SphereCollider sphereCollider;

    void Start()
    {
        playerController = player.GetComponent<PlayerController>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Candy")
        {
            other.gameObject.GetComponent<CollectCandy>().isCollected = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize magnet range in Scene view
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        //Gizmos.DrawWireSphere(transform.position, sphereCollider.radius);
    }
}
