using UnityEngine;

public class TractorBeam : MonoBehaviour
{
    public float pullForce = 10f;
    private Rigidbody playerRb;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerRb = other.GetComponent<Rigidbody>();
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerRb = null;
    }

    void FixedUpdate()
    {
        if (playerRb != null)
            playerRb.AddForce(Vector3.up * pullForce, ForceMode.Acceleration);
    }
}