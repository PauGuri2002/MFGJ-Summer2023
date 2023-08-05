using System.Collections;
using UnityEngine;

public class AcornSpawner : MonoBehaviour
{
    [SerializeField] private GameObject pineconePrefab;
    [SerializeField] private float minSpawnTime = 1f;
    [SerializeField] private float maxSpawnTime = 5f;
    [SerializeField] private float triggerDistance = 5f;
    [SerializeField] private Vector2 targetErrorArea = Vector2.one;
    [SerializeField] private AudioSource audioSource;
    private Transform player;

    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;
    }

    private void OnEnable()
    {
        ExteriorManager.OnPhaseChange += TryEnable;
    }

    private void OnDisable()
    {
        ExteriorManager.OnPhaseChange -= TryEnable;
    }

    void TryEnable(ExteriorManager.GamePhase phase)
    {
        if (phase == ExteriorManager.GamePhase.Search)
        {
            StartCoroutine(SpawnObjects());
            ExteriorManager.OnPhaseChange -= TryEnable;
        }
    }

    IEnumerator SpawnObjects()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            if (Vector2.Distance(new Vector2(player.position.x, player.position.z), new Vector2(transform.position.x, transform.position.z)) < triggerDistance)
            {
                GameObject instance = Instantiate(pineconePrefab, transform.position, Quaternion.identity);
                if (instance.TryGetComponent<AcornTrigger>(out var pineconeScript))
                {
                    pineconeScript.Init(player.position + Vector3.right * Random.Range(-targetErrorArea.x, targetErrorArea.x) + Vector3.forward * Random.Range(-targetErrorArea.y, targetErrorArea.y));
                }
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.Play();
            }
        }
    }
}
