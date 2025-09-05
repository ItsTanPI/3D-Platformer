using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class DamageVignette : MonoBehaviour
{
    [Header("Post Processing")]
    [SerializeField] private Volume postProcessVolume;

    [Header("Vignette Settings")]
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color damageColor = Color.red;
    [SerializeField] private float normalIntensity = 0.25f;
    [SerializeField] private float damageIntensity = 0.5f;
    [SerializeField] private float flashDuration = 0.5f;

    private Vignette vignette;

    private void Start()
    {
        if (postProcessVolume.profile.TryGet(out vignette))
        {
            vignette.color.value = normalColor;
            vignette.intensity.value = normalIntensity;

        }
    }

    public void FlashOnDamage()
    {
        if (vignette != null)

            StartCoroutine(FlashVignette());
    }

    private IEnumerator FlashVignette()
    {

        vignette.color.value = damageColor;
        vignette.intensity.value = damageIntensity;

        yield return new WaitForSeconds(flashDuration);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / flashDuration;
            vignette.color.value = Color.Lerp(damageColor, normalColor, t);
            vignette.intensity.value = Mathf.Lerp(damageIntensity, normalIntensity, t);
            yield return null;
        }
    }
}
