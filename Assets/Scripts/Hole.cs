using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    Collider2D col;
    List<Ball> balls;
    void Start()
    {
        balls = new List<Ball>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball == null)
            return;
        Debug.Log(string.Format("Adding {0}", ball));
        if (!balls.Contains(other.GetComponent<Ball>()))
            balls.Add(ball);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Ball ball = other.GetComponent<Ball>();
        if (ball == null)
            return;
        Debug.Log(string.Format("Removing {0}", ball));
        if (!balls.Contains(other.GetComponent<Ball>()))
            balls.Remove(ball);
    }
    public void Update()
    {
        foreach (Ball ball in balls)
        {
            if (col.OverlapPoint(ball.transform.position))
            {
                ball.rb.linearDamping = 50f;
                ball.rb.AddForce((transform.position - ball.transform.position) * 250);
                ball.depth += 2;
                ball.trailRenderer.emitting = false;
            }
            else
            {
                ball.rb.linearDamping = 0.5f;
                ball.trailRenderer.emitting = true;
            }
        }
    }
}
