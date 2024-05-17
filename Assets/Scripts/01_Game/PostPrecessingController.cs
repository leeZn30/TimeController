using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using VolFx;


public class PostPrecessingController : Singleton<PostPrecessingController>
{
    Volume globalVolume;

    Vignette vignette;
    FilmGrain filmGrain;
    ChromaticAberration chromatic;
    VhsVol vhs;

    Coroutine teleportEffect;
    Coroutine parryEffect;

    private void Awake()
    {
        globalVolume = GetComponent<Volume>();

        globalVolume.profile.TryGet(out vignette);
        globalVolume.profile.TryGet(out filmGrain);
        globalVolume.profile.TryGet(out chromatic);
        globalVolume.profile.TryGet(out vhs);
    }

    public void CallTeleportStartEffect()
    {
        if (teleportEffect != null)
        {
            StopCoroutine(teleportEffect);
        }

        teleportEffect = StartCoroutine(teleport(0.4f, 1f));
    }
    public void CallTeleportFinishEffect()
    {
        if (teleportEffect != null)
        {
            StopCoroutine(teleportEffect);
        }

        teleportEffect = StartCoroutine(teleport(0f, 0f));
    }
    IEnumerator teleport(float targetVignette, float targetFilmGrain)
    {
        float duration = 1f;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            float newVignette = Mathf.Lerp(vignette.intensity.value, targetVignette, currentTime / duration);
            vignette.intensity.value = newVignette;
            float newGrain = Mathf.Lerp(filmGrain.intensity.value, targetFilmGrain, currentTime / duration);
            filmGrain.intensity.value = newGrain;

            currentTime += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    public void CallRewindEffect(float value)
    {
        vhs._weight.value = value;
    }

    public void CallParryEffectStart() { }

}
