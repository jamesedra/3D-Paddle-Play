using UnityEngine;

public class JostlingEffect : MonoBehaviour
{
    [SerializeField, Min(0f)] 
    float 
        springStrength = 100f,
        dampingStrength = 10f,
        jostleStrength = 40f,
        pushStrength = 1f;

    Vector3 anchorPosition, velocity;

    private void Awake()
    {
        anchorPosition = transform.localPosition;
    }

    public void JostleY() => velocity.y += jostleStrength;

    public void PushXZ (Vector2 impulse)
    {
        velocity.x += pushStrength * impulse.x;
        velocity.z += pushStrength * impulse.y;
    }

    private void LateUpdate()
    {
        Vector3 displacement = anchorPosition - transform.localPosition;

        // calculate acceleration based on spring force and damping force
        // spring force helps camera get back to the anchor (original) position
        // acceleration becomes negative to slow down the velocity
        Vector3 acceleration = springStrength * displacement - dampingStrength * velocity;
        velocity += acceleration * Time.deltaTime;
        transform.localPosition += velocity * Time.deltaTime;
    }

}
