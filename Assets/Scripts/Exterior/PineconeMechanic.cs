using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PineconeMechanic : MonoBehaviour
{
    [SerializeField] private MulticamManager multicamManager;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float switchTime = 0.8f;
    [SerializeField] private Image overlay;

    private void Start()
    {
        overlay.CrossFadeAlpha(0, 0, true);
    }

    public void RegisterPineconeHit()
    {
        multicamManager.ShakeAll(0.1f, 10f);
        overlay.CrossFadeAlpha(1, 0.1f, false);
        StartCoroutine(SwitchCameras());
    }

    IEnumerator SwitchCameras()
    {
        yield return new WaitForSeconds(waitTime);

        overlay.CrossFadeAlpha(0, switchTime, false);
        multicamManager.SetGrid(switchTime, 0, GetRandomSeasonOrder());
    }

    Season[] GetRandomSeasonOrder()
    {
        Season[] currentSeasons = multicamManager.cameraOrder;
        Season[] newSeasons;
        do
        {
            newSeasons = currentSeasons.OrderBy(x => Random.value).ToArray();
        } while (newSeasons.SequenceEqual(currentSeasons));

        return newSeasons;
    }
}
