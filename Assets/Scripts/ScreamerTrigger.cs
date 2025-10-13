using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.Universal;

public class ScreamerTrigger : MonoBehaviour
{
    [Header("Camera")]
    public Camera playerCamera;
    public float screamFocus = 0f;        
    public float screamDuration = 2f;     
    private float defaultFocus = 1f;

    public float screamFocal = 2.95f;
    private float defaultFocal = 9.76f;

    [Header("Post Processing")]
    public Volume volume;
    private SplitToning splitToning;

    private Color defaultHighlights;
    private Color screamHighlights = new Color(1.0f, 0.61f, 0.0f);

    [Header("Audio")]
    public AudioSource screamerSound1;
    public AudioSource screamerSound2;

    private bool isTriggered = false;

    private void Start()
    {
        if (volume != null && volume.profile.TryGet(out splitToning))
            defaultHighlights = splitToning.highlights.value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggered && other.CompareTag("Player"))
        {
            isTriggered = true;
            if (!screamerSound1.isPlaying && !screamerSound2.isPlaying)
            {
                screamerSound1.Play();
                screamerSound2.Play();
            }
            else if (screamerSound1.isPlaying && screamerSound2.isPlaying)
            {
                screamerSound1.Stop();
                screamerSound2.Stop();
            }
            StartCoroutine(ScreamerEffect());
        }
    }

    private System.Collections.IEnumerator ScreamerEffect()
    {
        float t = 0f;

        while (t < screamDuration)
        {
            t += Time.deltaTime;
            float lerp = t;

            if (playerCamera != null)
            {
                playerCamera.focusDistance = Mathf.Lerp(defaultFocus, screamFocus, lerp);
                playerCamera.focalLength = Mathf.Lerp(defaultFocal, screamFocal, lerp);
            }

            if (splitToning != null)
                splitToning.highlights.value = Color.Lerp(defaultHighlights, screamHighlights, lerp);

            yield return null;
        }

        // Возвращаем обратно
        t = 0f;
        while (t < screamDuration)
        {
            t += Time.deltaTime;
            float lerp = t / screamDuration;

            if (playerCamera != null)
            {
                playerCamera.focusDistance = Mathf.Lerp(screamFocus, defaultFocus, lerp);
                playerCamera.focalLength = Mathf.Lerp(screamFocal, defaultFocal, lerp);
            }


            if (splitToning != null)
                splitToning.highlights.value = Color.Lerp(screamHighlights, defaultHighlights, lerp);

            yield return null;
        }
    }
}
