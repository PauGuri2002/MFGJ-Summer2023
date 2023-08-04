using UnityEngine;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        transform.position = player.position;
    }
}
