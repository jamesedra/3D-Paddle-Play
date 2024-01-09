using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    [SerializeField] Ball ball;
    [SerializeField] Paddle topPaddle, bottomPaddle;
    [SerializeField, Min(0f)]
    Vector2 arenaExtents = new Vector2(10f, 10f);

    [SerializeField] JostlingEffect jostlingCamera;
    [SerializeField] TextMeshPro countdownText;
    [SerializeField, Min(1f)] float newGameDelay = 3f;

    float countdownUntilNewGame;

    [SerializeField, Min(2)] int pointsToWin = 3;

    void Awake() => countdownUntilNewGame = newGameDelay;

    void StartNewGame()
    {
        ball.StartNewGame();
        bottomPaddle.StartNewGame();
        topPaddle.StartNewGame();
    }

    void Update()
    {
        bottomPaddle.Move(ball.Position.x, arenaExtents.x);
        topPaddle.Move(ball.Position.x, arenaExtents.x);

        if (countdownUntilNewGame <= 0f)
        {
            // start game
            UpdateGame();
        }
        else
        {
            UpdateCountdown();
        }
        
    }

    void UpdateGame ()
    {
        ball.Move();
        BounceYIfNeeded();
        BounceXIfNeeded(ball.Position.x);
        ball.UpdateVisualization();
    }

    void UpdateCountdown()
    {
        countdownUntilNewGame -= Time.deltaTime;
        if (countdownUntilNewGame <= 0f)
        {
            // removes the countdown
            countdownText.gameObject.SetActive(false);
            StartNewGame();
        }
        else
        {
            float displayValue = Mathf.Ceil(countdownUntilNewGame);
            if (displayValue < newGameDelay) {
                countdownText.SetText("{0}", displayValue);
            }
        }
 
    }

    private void BounceYIfNeeded()
    {
        float yExtents = arenaExtents.y - ball.Extents;
        if (ball.Position.y < -yExtents)
        {
            BounceY(-yExtents, bottomPaddle, topPaddle);
        }
        else if (ball.Position.y > yExtents)
        {
            BounceY(yExtents, topPaddle, bottomPaddle);
        }
    }

    private void BounceXIfNeeded(float x)
    {
        float xExtents = arenaExtents.x - ball.Extents;
        if (x < -xExtents)
        {
            jostlingCamera.PushXZ(ball.Velocity);
            ball.BounceX(-xExtents);
        }
        else if (x > xExtents)
        {
            jostlingCamera.PushXZ(ball.Velocity);
            ball.BounceX(xExtents);
        }
    }

    void BounceY(float boundary, Paddle defender, Paddle attacker)
    {
        // determines how long ago the bounce happened
        float durationAfterBounce = (ball.Position.y - boundary) / ball.Velocity.y;
        // calculate the x position of the ball when bounce happened
        float bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;

        // add bounce x if needed to calculate the trajectory when the bounce
        // happened in both directions (corner bounce)
        BounceXIfNeeded(bounceX);
        bounceX = ball.Position.x - ball.Velocity.x * durationAfterBounce;
        jostlingCamera.PushXZ(ball.Velocity);
        ball.BounceY(boundary);

        // if the paddle hits the ball, set the position and speed based on
        // the bounce x position, hit factor, and how long ago it happened
        if (defender.HitBall(bounceX, ball.Extents, out float hitFactor))
        {
            ball.SetXPositionAndSpeed(bounceX, hitFactor, durationAfterBounce);
        }
        else
        {
            jostlingCamera.JostleY();
            if (attacker.ScorePoint(pointsToWin))
            {
                EndGame();
            }
        }
        
    }

    void EndGame()
    {
        countdownUntilNewGame = newGameDelay;
        countdownText.SetText("GAME OVER");
        countdownText.gameObject.SetActive(true);
        ball.EndGame();
    }

}
