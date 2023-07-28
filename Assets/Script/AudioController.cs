using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private AudioSource soundContainerPrefab;

    public void PlaySound(string audioName, int pitchMultiplier)
    {
        Sound sound = sounds.FirstOrDefault(sound => sound.name == audioName);

        if (sound == null)
        {
            Debug.LogWarning($"Could not found {audioName} audio clip");
            return;
        }

        CreateSoundContainer(sound, out AudioSource source);

        source.pitch += 0.1F * pitchMultiplier;
        source.Play();
        StartCoroutine(DeleteSoundContainer(source));
    }

    private void CreateSoundContainer(Sound sound, out AudioSource source)
    {
        AudioSource audioSource = Instantiate(soundContainerPrefab, transform);
        audioSource.clip = sound.clip;
        audioSource.volume = sound.volume;
        audioSource.pitch = sound.pitch;
        source = audioSource;
    }
    

    private IEnumerator DeleteSoundContainer(AudioSource audioSource)
    {
        yield return new WaitUntil(() => audioSource.isPlaying == false);
        Destroy(audioSource.gameObject);
    }
}
