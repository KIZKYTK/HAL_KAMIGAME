using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSE : MonoBehaviour
{
    public AudioClip moveSE;
    public AudioClip jumpSE;

    [Header("SE Volume (0 = mute, 10 = very loud)")]
    [Range(0f, 10f)] public float seVolume = 3.0f; // Å© ç≈ëÂ10î{Ç…ïœçXÅI

    private AudioSource audioSource;
    private bool isMoving = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1.0f; // Base volume remains default
    }

    public void PlayMoveSE()
    {
        if (!isMoving && moveSE != null)
        {
            audioSource.PlayOneShot(moveSE, seVolume);
            isMoving = true;
            Invoke(nameof(ResetMoveSE), 0.3f);
        }
    }

    void ResetMoveSE()
    {
        isMoving = false;
    }

    public void PlayJumpSE()
    {
        if (jumpSE != null)
        {
            audioSource.PlayOneShot(jumpSE, seVolume);
        }
    }
}