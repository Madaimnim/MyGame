using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    private AudioSource sfxSource;

    private void Awake() {
        //if (Instance == null)
        //{
        //    Instance = this;
        //    DontDestroyOnLoad(gameObject);
        //
        //    sfxSource = gameObject.AddComponent<AudioSource>();
        //    sfxSource.playOnAwake = false;
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
        }
        sfxSource.playOnAwake = false;
    }

    public void PlaySFX(AudioClip clip, float volume) {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
}
