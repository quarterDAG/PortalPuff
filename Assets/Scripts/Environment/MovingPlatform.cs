using TarodevController;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Vector3 pointA;
    public Vector3 pointB;
    public float speed = 0.25f;

    [SerializeField] private bool isActive;

    private Vector3 targetPoint;
    private Vector3 startPosition;
    private float startTime;

    private Vector3 previousPosition;
    private Vector2 velocity;

    private bool movingTowardsB;


    void Start ()
    {
        previousPosition = transform.position;
        startPosition = transform.position;
        ChooseInitialTarget(true);
    }

    void Update ()
    {
        if (!isActive) return;

        float timeSinceStarted = (Time.time - startTime) * speed;
        float journeyFraction = timeSinceStarted / Vector3.Distance(startPosition, targetPoint);

        transform.position = Vector3.Lerp(startPosition, targetPoint, journeyFraction);

        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        if (journeyFraction >= 1)
        {
            ChooseInitialTarget(false);
        }
    }

    private void ChooseInitialTarget ( bool initializing )
    {
        if (!initializing) // If not initializing, update start position to current position
        {
            startPosition = transform.position;
            startTime = Time.time;
        }

        // Determine the direction to move based on the current or initial position
        if (Vector3.Distance(transform.position, pointA) < Vector3.Distance(transform.position, pointB))
        {
            targetPoint = movingTowardsB ? pointB : pointA;
        }
        else
        {
            targetPoint = movingTowardsB ? pointA : pointB;
        }

        movingTowardsB = !movingTowardsB;
    }


    private void OnCollisionStay2D ( Collision2D other )
    {
        if (!isActive)
        {
            ResetPlayerVelocity(other.gameObject);
        }

        else if (other.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            playerController.ApplyEnvironemntVelocity(velocity);
        }
    }


    private void OnCollisionExit2D ( Collision2D other )
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ResetPlayerVelocity(other.gameObject);
        }
    }

    private void ResetPlayerVelocity ( GameObject _player )
    {
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.ApplyEnvironemntVelocity(Vector2.zero);
    }

    public void ActivatePlatform ( bool _isActive )
    {
        isActive = _isActive;
        if (isActive)
        {
            startTime = Time.time;
            startPosition = transform.position;
        }
    }

    public void TogglePlatform ()
    {
        ActivatePlatform(!isActive);
    }
}
