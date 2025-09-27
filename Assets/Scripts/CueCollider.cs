using UnityEngine;

public class CueCollider : MonoBehaviour
{
    public Game game;
    public void Start()
    {
        game = FindAnyObjectByType<Game>();
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        game.ShootBall();
    }
}
