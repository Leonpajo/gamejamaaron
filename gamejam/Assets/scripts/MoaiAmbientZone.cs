using UnityEngine;

public class AmbientZone : MonoBehaviour
{
    public AudioSource mainAmbient;
    public AudioSource zoneAmbient;

    private static int zoneCount = 0;
    private static AudioSource currentZoneAmbient;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        zoneCount++;
        if (currentZoneAmbient != null && currentZoneAmbient != zoneAmbient)
            currentZoneAmbient.Stop();

        currentZoneAmbient = zoneAmbient;

        if (!zoneAmbient.isPlaying)
        {
            mainAmbient.Stop();
            zoneAmbient.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        zoneCount = Mathf.Max(0, zoneCount - 1);

        if (zoneCount == 0)
        {
            zoneAmbient.Stop();
            currentZoneAmbient = null;
            if (!mainAmbient.isPlaying)
                mainAmbient.Play();
        }
    }

    private void OnDisable()
    {
        zoneCount = 0;
        currentZoneAmbient = null;
    }
}