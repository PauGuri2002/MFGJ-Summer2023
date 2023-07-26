using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.zero;
    [SerializeField] private float minAngle = 0f;
    [SerializeField] private float maxAngle = 360f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(axis.x * Random.Range(minAngle, maxAngle), axis.y * Random.Range(minAngle, maxAngle), axis.z * Random.Range(minAngle, maxAngle));
    }
}
