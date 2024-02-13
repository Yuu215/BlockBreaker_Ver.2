using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSound : MonoBehaviour
{
    //�X�^�[�g���j���[�̃R���g���[���[

    [SerializeField] AudioClip[] sounds;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void FirstTapSound()
    {
        audioSource.PlayOneShot(sounds[0]);
    }

    public void MenuSound()
    {
        audioSource.PlayOneShot(sounds[1]);
    }

    public void SceneMoveSound()
    {
        audioSource.PlayOneShot(sounds[2]);
    }
}