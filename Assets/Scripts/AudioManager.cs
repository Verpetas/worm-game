using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicAudio;
    [SerializeField] AudioSource pickupAudio;
    [SerializeField] AudioSource explosionAudio;
    [SerializeField] AudioSource[] granAudio;

    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(gameObject);
        musicAudio.Play();
    }

    public void PlaySound(string sound)
    {
        switch (sound)
        {
            case "Pickup":
                pickupAudio.Play();
                break;
            case "Explosion":
                explosionAudio.Play();
                break;
            case "AngryGran":
                granAudio[Random.Range(0, granAudio.Length)].Play();
                break;
        }
    }
}
