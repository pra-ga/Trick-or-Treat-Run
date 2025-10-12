using UnityEngine;

public class GoldManager : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float goldImpulseForce = 5f;
    [SerializeField] Vector3 directionVector = new Vector3 (0, 1, -1);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            rb.AddForce(directionVector * goldImpulseForce, ForceMode.Impulse);
        }
    }
}
