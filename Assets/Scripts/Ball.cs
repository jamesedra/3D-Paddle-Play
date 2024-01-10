using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField, Min(0f)]
    float
        maxXSpeed = 20f,
        maxStartXSpeed = 2f,
        constantYSpeed = 10f,
        extents = 0.5f;

    [SerializeField] 
    ParticleSystem 
        bounceParticleSystem, 
        startParticleSystem,
        trailParticleSystem;

    [SerializeField] 
    int 
        bounceParticleEmission = 20,
        startParticleEmission = 100;

    // Ball positioning are in 2D (XZ coords), Y value  in vector2 is the 3D Z value
    Vector2 position, velocity;

    public float Extents => extents;
    public Vector2 Position => position;

    public Vector2 Velocity => velocity;

    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void StartNewGame()
    {
        position = Vector2.zero;
        UpdateVisualization();

        // and some random speed in every game
        velocity.x = Random.Range(-maxStartXSpeed, maxStartXSpeed);
        // moves downward to the bottom player as the start movement
        velocity.y = -constantYSpeed;
        gameObject.SetActive(true);
        startParticleSystem.Emit(startParticleEmission);
        SetTrailEmission(true);
        trailParticleSystem.Play();
    }
    public void EndGame()
    {
        position.x = 0f;
        gameObject.SetActive(false);
        SetTrailEmission(false);
    }

    public void UpdateVisualization() => trailParticleSystem.transform.localPosition =
        transform.localPosition = new Vector3(position.x, 0f, position.y);

    public void Move() => position += velocity * Time.deltaTime;


    // once the ball bounces in the Y axis, the trajectory
    // of the ball reverse
    // ex: the ball bounces upward when it hits the bottom, or downward
    // when the ball hits the top
    public void BounceY(float boundary)
    {
        float durationAfterBounce = (position.y - boundary) / velocity.y;
        position.y = 2f * boundary - position.y;
        velocity.y = -velocity.y;
        EmitBounceParticles(
            position.x - velocity.x * durationAfterBounce,
            boundary,
            boundary < 0f ? 0f : 180f
        );
    }

    // same as BounceY
    public void BounceX(float boundary)
    {
        float durationAfterBounce = (position.x - boundary) / velocity.x;
        position.x = 2f * boundary - position.x;
        velocity.x = -velocity.x;
        EmitBounceParticles(
            boundary,
            position.y - velocity.y * durationAfterBounce,
            boundary < 0f ? 90f : 270f
        );
    }
    public void SetXPositionAndSpeed(float start, float speedFactor, float deltaTime)
    {
        velocity.x = maxXSpeed * speedFactor;
        position.x = start + velocity.x * deltaTime;
    }

    void EmitBounceParticles(float x, float z, float rotation)
    {
        ParticleSystem.ShapeModule shape = bounceParticleSystem.shape;
        shape.position = new Vector3(x, 0f, z);
        shape.rotation = new Vector3(0f, rotation, 0f);
        bounceParticleSystem.Emit(bounceParticleEmission);
    }

    void SetTrailEmission(bool enabled)
    {
        ParticleSystem.EmissionModule emission = trailParticleSystem.emission;
        emission.enabled = enabled;
    }
}
