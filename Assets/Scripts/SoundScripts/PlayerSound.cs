using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    public AudioClip[] playerSounds;

    private AudioSource source;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void PlayWalkingSound()
    {
        // Play the walking sound
        AudioClip clip = playerSounds[2];
        source.clip = clip;
        source.volume = Random.Range(0.02f, 0.05f);
        source.pitch = Random.Range(0.8f, 1.2f);
        source.Play();
        Debug.Log(clip.name);


        
    }
}
