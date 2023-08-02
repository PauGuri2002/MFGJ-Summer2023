using UnityEngine;

public class AcornTrigger : MonoBehaviour
{
    private AcornMechanic mechanic;
    [SerializeField] private float startVerticalSpeed = 10f;
    [SerializeField] private float gravity = 10f;
    [SerializeField] private Vector3 minRotationSpeed = Vector3.zero;
    [SerializeField] private Vector3 maxRotationSpeed = Vector3.zero;
    [SerializeField] private Transform rotationPivot;
    private Vector3 rotationSpeed;
    private Vector2 horizontalPosition = Vector2.zero;
    private float verticalSpeed = 0f;
    private Vector2 targetHorizontal;
    private Vector2 startHorizontal;
    private float progress = 0;
    private bool goingUp = true;

    private void Start()
    {
        mechanic = FindObjectOfType<AcornMechanic>();
    }

    public void Init(Vector3 target)
    {
        targetHorizontal = new Vector2(target.x, target.z);
        startHorizontal = new Vector2(transform.position.x, transform.position.z);
        progress = 0;
        verticalSpeed = startVerticalSpeed;
        rotationSpeed = new Vector3(Random.Range(minRotationSpeed.x, maxRotationSpeed.x), Random.Range(minRotationSpeed.y, maxRotationSpeed.y), Random.Range(minRotationSpeed.z, maxRotationSpeed.z));
    }

    private void Update()
    {
        verticalSpeed -= gravity * Time.deltaTime;

        if (goingUp)
        {
            horizontalPosition = Vector2.Lerp(startHorizontal, targetHorizontal, progress);
            progress += (gravity / startVerticalSpeed) * Time.deltaTime;

            if (progress >= 1)
            {
                progress = 0;
                horizontalPosition = targetHorizontal;
                goingUp = false;
            }
        }

        rotationPivot.Rotate(rotationSpeed * Time.deltaTime);
        transform.position = new Vector3(horizontalPosition.x, transform.position.y + verticalSpeed * Time.deltaTime, horizontalPosition.y);

        // safety destruction
        if (transform.position.y < 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("pinecone collided with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            mechanic.RegisterPineconeHit();
        }

        //ParticleSystem particles = GetComponentInChildren<ParticleSystem>();
        //if (particles != null)
        //{

        //}

        Destroy(gameObject);
    }
}
