using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FearEffectController : MonoBehaviour
{
    public Volume volume;

    ChromaticAberration chroma;
    FilmGrain grain;
    LensDistortion lens;
    MotionBlur motion;
    Vignette vig;

    float panic;

    void Start()
    {
        volume.profile.TryGet(out chroma);
        volume.profile.TryGet(out grain);
        volume.profile.TryGet(out lens);
        volume.profile.TryGet(out motion);
        volume.profile.TryGet(out vig);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.F))  // กด F = ผีใกล้
            panic = Mathf.Clamp01(panic + Time.deltaTime);
        else
            panic = Mathf.Clamp01(panic - Time.deltaTime * 0.6f);

        chroma.intensity.value = Mathf.Lerp(0.1f, 0.6f, panic);
        grain.intensity.value  = Mathf.Lerp(0.2f, 0.7f, panic);
        lens.intensity.value   = Mathf.Lerp(0f, -0.35f, panic);
        motion.intensity.value = Mathf.Lerp(0.1f, 0.6f, panic);
        vig.intensity.value    = Mathf.Lerp(0.2f, 0.45f, panic);
    }
}
