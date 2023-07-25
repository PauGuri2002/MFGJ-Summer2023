using System.Collections;
using UnityEngine;

public class FreezeMechanic : MonoBehaviour
{
    [SerializeField] private PlayerController player;
    [SerializeField] private float frozenTime = 5f;
    [SerializeField] private float frozenAccel = 1.6f, frozenDecel = 5f;
    [SerializeField] private GameObject iceCubePrefab;
    private Coroutine freezeCoroutine;
    private GameObject iceCubeObject;

    private float originalAccel, originalDecel;

    private void Start()
    {
        originalAccel = player.horizontalAccelTime;
        originalDecel = player.horizontalDecelTime;
    }

    public void StartFreeze()
    {
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

        yield return new WaitForSeconds(frozenTime);

        StopFreeze();
        freezeCoroutine = null;
    }

    void StopFreeze()
    {
        player.horizontalAccelTime = originalAccel;
        player.horizontalDecelTime = originalDecel;

        if (iceCubeObject != null)
        {
            Destroy(iceCubeObject);
            iceCubeObject = null;
        }
    }
}
