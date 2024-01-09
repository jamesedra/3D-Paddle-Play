using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        maxXSpeed = 20f,
        maxStartXSpeed = 2f,
        constantYSpeed = 10f,
        extents = 0.5f;

    public float Extents => extents;
    public Vector2 Position => position;

    public Vector2 Velocity => velocity;

    // Ball positioning are in 2D (XZ coords), Y value  in vector2 is the 3D Z value
    Vector2 position, velocity;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void UpdateVisualization() =>
        transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += velocity * Time.deltaTime;


    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();

        // and some random speed in every game
        velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
        // moves downward to the bottom player as the start movement
        velocity.y = -constantYSpeed;
        gameObject.SetActive(true);
    }

    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
    }

    // once the ball bounces in the Y axis, the trajectory
    // of the ball reverse
    // ex: the ball bounces upward when it hits the bottom, or downward
    // when the ball hits the top
    public void BounceY(float boundary)
    {
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
    }

    // same as BounceY
    public void BounceX(float boundary)
    {
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
    }

    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }


}
