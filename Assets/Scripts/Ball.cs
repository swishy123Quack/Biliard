using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Rigidbody2D rb;
    public TrailRenderer trailRenderer;
    public SpriteRenderer sprite;
    public Color color;
    public float depth = 0;

    public void Start()
    {
        sprite.color = color;
        trailRenderer.startColor = new Color(color.r, color.g, color.b, 0.5f);
        trailRenderer.endColor = new Color(color.r, color.g, color.b, 0);
    }
    public void Update()
    {
        depth -= 1;
        depth = Math.Clamp(depth, 0, 40);

        float alpha = 1 - (depth / 40.0f);
        sprite.color = new Color(color.r, color.g, color.b, alpha);
        trailRenderer.startColor = new Color(color.r, color.g, color.b, alpha / 2);
    }
}
