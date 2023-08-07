using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FreezeMechanic : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float frozenTime = 5f;
    [SerializeField] private float frozenAccel = 1.6f, frozenDecel = 5f;
    [SerializeField] private GameObject iceCubePrefab;
    [SerializeField] private Image overlay;
    [SerializeField] private MulticamManager multicamManager;
    [SerializeField] private AudioSource audioSource;
    private Coroutine freezeCoroutine;
    private GameObject iceCubeObject;

    private float originalAccel, originalDecel;

    private static bool tutorialShown = false;

    private void Start()
    {
        originalAccel = player.horizontalAccelTime;
        originalDecel = player.horizontalDecelTime;
        overlay.CrossFadeAlpha(0, 0, true);
    }

    public void StartFreeze()
    {
        // TUTORIAL PROVISIONAL
        if (!tutorialShown)
        {
            tutorialShown = true;
            NoticeDisplayer.Instance.ShowNotice("I'm frozen! Ice makes me real slippery.");
        }

        multicamManager.Shake(Season.Winter, 0.1f, 10f);

        if (!audioSource.isPlaying)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.Play();
        }

        if (freezeCoroutine != null)
        {
            StopFreeze();
            StopCoroutine(freezeCoroutine);
        }
        freezeCoroutine = StartCoroutine(FreezePlayer());
    }

    IEnumerator FreezePlayer()
    {
        if (iceCubeObject == null)
        {
            iceCubeObject = Instantiate(iceCubePrefab, player.transform);
        }

        player.horizontalAccelTime = frozenAccel;
        player.horizontalDecelTime = frozenDecel;
        overlay.CrossFadeAlpha(1, 0.1f, false);

        yield return new WaitForSeconds(frozenTime);

        StopFreeze();
        freezeCoroutine = null;
    }

    void StopFreeze()
    {
        player.horizontalAccelTime = originalAccel;
        player.horizontalDecelTime = originalDecel;
        overlay.CrossFadeAlpha(0, 1f, false);

        if (iceCubeObject != null)
        {
            Destroy(iceCubeObject);
            iceCubeObject = null;
        }
    }
}
