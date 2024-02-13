using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSoundEffects : MonoBehaviour
{
    [SerializeField] private AudioClip[] sounds;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
       
    }

    public void Countdown()
    {
        audioSource.PlayOneShot(sounds[0]);
    }

    public void StartSound()
    {
        audioSource.PlayOneShot(sounds[1]);
    }

    public void PlayerDamageSound()
    {
        audioSource.PlayOneShot(sounds[2]);
    }

    public void LevelUpSound()
    {
        audioSource.PlayOneShot(sounds[3]);
    }

    public void BlockCollisionSound()  //çUåÇéûå¯â âπ
    {
        audioSource.PlayOneShot(sounds[4]);
    }

    public void FlameItemSound()
    {
        audioSource.PlayOneShot(sounds[5]);
    }

    public void BlizzardItemSound()
    {
        audioSource.PlayOneShot(sounds[6]);
    }

    public void PlasmaItemSound()
    {
        audioSource.PlayOneShot(sounds[7]);
    }

    public void FlameBlockCollisionSound() //çUåÇéûå¯â âπ âä
    {
        audioSource.PlayOneShot(sounds[8]);
    }

    public void BlizzardBlockCollisionSound()
    {
        audioSource.PlayOneShot(sounds[9]);
    }
}
