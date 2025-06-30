using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    public AudioClip seClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySE()
    {
        if (seClip != null)
            audioSource.PlayOneShot(seClip);
        else
            Debug.LogWarning("SEクリップが設定されていません！");
    }
}