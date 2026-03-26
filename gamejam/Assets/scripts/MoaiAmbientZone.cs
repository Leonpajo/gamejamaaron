using UnityEngine;

public class MoaiAmbientZone : MonoBehaviour
{
    public AudioSource mainAmbient;
    public AudioSource moaiAmbient;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            mainAmbient.Stop();
            moaiAmbient.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            moaiAmbient.Stop();
            mainAmbient.Play();
        }
    }
}