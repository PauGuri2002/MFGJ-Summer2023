using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private MusicInfo[] music;
    [SerializeField] private AudioSource[] audioSources;
    [SerializeField] private float fadeTime;
    private AudioSource currentAudioSource;
    Coroutine waitForIntroCoroutine;

    public static MusicPlayer Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);

        if (audioSources.Length < 2)
        {
            Debug.LogError("Music Player must have at least 2 Audio Sources");
            return;
        }
        currentAudioSource = audioSources[0];
    }

    public void Play(string musicId)
    {
        MusicInfo clip = Array.Find(music, (m) => m.id == musicId);

        if (clip == null || clip.loopClip == null) { return; }

        if (currentAudioSource.isPlaying)
        {
            if (currentAudioSource.clip == clip.loopClip || currentAudioSource.clip == clip.introClip) { return; }
            if (waitForIntroCoroutine != null) { StopCoroutine(waitForIntroCoroutine); }

            Stop(currentAudioSource);

            // get next source as current audio source
            int index = Array.IndexOf(audioSources, currentAudioSource);
            index++;
            if (index >= audioSources.Length)
            {
                index = 0;
            }
            currentAudioSource = audioSources[index];
        }

        if (clip.introClip != null)
        {
            currentAudioSource.loop = false;
            currentAudioSource.clip = clip.introClip;
            currentAudioSource.Play();
            waitForIntroCoroutine = StartCoroutine(WaitForIntroEnd(clip.loopClip));
        }
        else
        {
            PlayLoop(clip.loopClip);
        }
    }

    IEnumerator WaitForIntroEnd(AudioClip loopClip)
    {
        yield return new WaitWhile(() => currentAudioSource.isPlaying);
        //yield return new WaitUntil(() => currentAudioSource.time >= (currentAudioSource.clip.length - Time.deltaTime * 2));
        currentAudioSource.Stop();
        PlayLoop(loopClip);
        waitForIntroCoroutine = null;
    }

    private void PlayLoop(AudioClip loopClip)
    {
        currentAudioSource.loop = true;
        currentAudioSource.clip = loopClip;
        currentAudioSource.Play();
    }

    public void Stop(AudioSource audioSource = null)
    {
        if (audioSource == null) { audioSource = currentAudioSource; }

        if (waitForIntroCoroutine != null)
        {
            StopCoroutine(waitForIntroCoroutine);
        }

        if (fadeTime > 0)
        {
            StartCoroutine(FadeOut(audioSource));
        }
        else
        {
            audioSource.Stop();
            audioSource.time = 0;
        }

    }

    IEnumerator FadeOut(AudioSource audioSource)
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= (1 / fadeTime) * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        audioSource.Stop();
        audioSource.time = 0;
        audioSource.volume = 1;
    }

    [Serializable]
    class MusicInfo
    {
        public string id;
        public AudioClip introClip;
        public AudioClip loopClip;
    }
}