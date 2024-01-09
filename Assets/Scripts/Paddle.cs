using TMPro;
using UnityEngine;

public class Paddle : MonoBehaviour
{
    [SerializeField] bool isAI;

    [SerializeField] TextMeshPro scoreText;

    // extents - paddle width
    // speed - movement speed
    // targeting bias - for AI, targets
    // where the ball should land on their paddle
    [SerializeField, Min(0f)]
    float
        minExtents = 4f,
        maxExtents = 4f,
        speed = 10f,
        maxTargetingBias = 0.75f;

    int score;

    // makes the AI paddle not only target
    // the ball to hit center
    float targetingBias, extents;

    private void Awake()
    {
        SetScore(0);
    }

    public void StartNewGame()
    {
        SetScore(0);
        ChangeTargetingBias();
    }

    public void Move(float target, float arenaExtents)
    {
        Vector3 p = transform.localPosition;
        p.x = isAI ? AdjustByAI(p.x, target) : AdjustByPlayer(p.x);
        float limit = arenaExtents - extents;
        p.x = Mathf.Clamp(p.x, -limit, limit);
        transform.localPosition = p;
    }

    float AdjustByPlayer (float x)
    {
        bool goRight = Input.GetKey(KeyCode.RightArrow);
        bool goLeft = Input.GetKey(KeyCode.LeftArrow);
        if (goRight && !goLeft)
        {
            return x + speed * Time.deltaTime;
        }
        else if (goLeft && !goRight) 
        { 
            return x - speed * Time.deltaTime;
        }
        return x;
    }

    float AdjustByAI (float x, float target)
    {
        target += targetingBias * extents;
        if (x < target)
        {
            return Mathf.Min(x + speed * Time.deltaTime, target);
        }
        return Mathf.Max(x - speed * Time.deltaTime, target);
    }

    // returns true if the ball hits the paddle
    // outs a hit factor that can determine the angle and position
    // where the ball landed in the paddle
    public bool HitBall(float ballX, float ballExtents, out float hitFactor)
    {
        ChangeTargetingBias();
        hitFactor = 
            (ballX - transform.localPosition.x) /
            (extents + ballExtents);
        return -1f <= hitFactor && hitFactor <= 1f;
    }

    // adds a score
    public bool ScorePoint(int pointsToWin)
    {
        SetScore(score + 1, pointsToWin);
        return score >= pointsToWin;
    }
    
    // updates score
    private void SetScore(int newScore, float pointsToWin = 1000f)
    {
        score = newScore;
        scoreText.SetText("{0}", newScore);
        SetExtents(Mathf.Lerp(maxExtents, minExtents, newScore / (pointsToWin - 1f)));
    }

    // shrinks player paddle every point gotten
    void SetExtents(float newExtents)
    {
        extents = newExtents;
        Vector3 s = transform.localScale;
        s.x = 2f * newExtents;
        transform.localScale = s;
    }

    private void ChangeTargetingBias()
    {
        targetingBias = Random.Range(-maxTargetingBias, maxTargetingBias);
    }
}
