using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Manage audio data and playback
public class SoundEffect : MonoBehaviour
{
    [Header("Sound clips")]
    public AudioClip itemSelectAc; // Sound effect played when a UI item is selected
    public AudioClip hitPaddleAc; // Sound effect played when ball hits paddle
    public AudioClip hitWallAc; // Sound effect played when ball hits walls
    public AudioClip hitBrickAc; //Sound effect played when ball hits bricks

    private AudioSource audioSrc = null; // used to play AudioClips
    private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        if (audioSrc == null)
            throw new ArgumentException("audioSrc");
    }

    void Start()
    {
        if (hitPaddleAc == null)
            throw new ArgumentException("hitPaddleAc");
        if (hitWallAc == null)
            throw new ArgumentException("hitWallAc");
        if (hitBrickAc == null)
            throw new ArgumentException("hitBrickAc");
        if (itemSelectAc == null)
            throw new ArgumentException("itemSelect");
        audioClips.Add("hitPaddle", hitPaddleAc);
        audioClips.Add("hitWall", hitWallAc);
        audioClips.Add("hitBrick", hitBrickAc);
        audioClips.Add("itemSelect", itemSelectAc);
    }

    // Gets audio clip
    private AudioClip GetAudioClip(String sound)
    {
        AudioClip ac;
        if (!audioClips.TryGetValue(sound, out ac))
        {
            throw new ArgumentException($"${sound} is not found");
        }
        return ac;
    }

    // Plays sound
    public void PlaySound(String sound)
    {
        audioSrc.PlayOneShot(GetAudioClip(sound));
    }

    // Plays sound, then loads a scene.
    // Sound playback may be interrupted by a scene change,
    // so scene transition occurs after playback completes.
    public IEnumerator PlaySoundAndLoadScene(String sound, String sceneName)
    {
        AudioClip ac = GetAudioClip(sound);

        audioSrc.PlayOneShot(ac);

        // Wait until sound finishes playing
        yield return new WaitForSeconds(ac.length + 0.1f); // 0.1f = buffer

        SceneManager.LoadScene(sceneName);
    }
}
