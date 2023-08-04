using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AcornMechanic : MonoBehaviour
{
    [SerializeField] private MulticamManager multicamManager;
    [SerializeField] private float waitTime = 1f;
    [SerializeField] private float switchTime = 0.8f;
    [SerializeField] private Image overlay;
    [SerializeField] private HeatMechanic heatMechanic;

    private void Start()
    {
        overlay.CrossFadeAlpha(0, 0, true);
    }

    public void RegisterPineconeHit()
    {
        if (heatMechanic.currentStatus == HeatMechanic.Status.Burned) { return; }

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
        Season[] currentSeasons = multicamManager.CameraOrder;
        Season[] newSeasons;
        do
        {
            newSeasons = currentSeasons.OrderBy(x => Random.value).ToArray();
        } while (newSeasons.SequenceEqual(currentSeasons));

        return newSeasons;
    }
}
