using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Game : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject anchor;
    public List<Ball> balls;
    public Ball cueBall;
    public Cue cue;
    public bool debug = false;
    InputAction clickAction;
    InputAction shootAction;
    bool anchorSet = false;
    bool edging = false;
    bool shooting = false;
    bool waiting = false;
    float offset = 0;
    float shootForce = 0;
    float cueTempPos = 0;
    RaycastHit2D hitPoint;
    public void Start()
    {
        clickAction = InputSystem.actions.FindAction("CLick");
        shootAction = InputSystem.actions.FindAction("Shoot");
    }
    public void Update()
    {
        if (!waiting)
        {
            var mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (!anchorSet)
            {
                anchor.transform.position = new Vector3(mousePos.x, mousePos.y, 0);
            }

            var ballPos = cueBall.transform.position;
            var anchorPos = anchor.transform.position;
            var dir = ballPos - anchorPos;

            if (clickAction.IsPressed() && !anchorSet)
            {
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                cue.transform.SetPositionAndRotation(anchorPos, Quaternion.Euler(0f, 0f, angle));
                cue.sprite.transform.localPosition = new Vector3(Vector3.Distance(ballPos, anchorPos) - 74, 0);

                anchorSet = true;
                offset = 0;
            }
            if (anchorSet)
            {
                if (!shooting)
                {
                    if (clickAction.IsPressed())
                    {
                        edging = true;
                        offset += Time.deltaTime;
                    }
                    else if (!clickAction.IsPressed())
                    {
                        edging = false;
                        offset -= Time.deltaTime * 3;
                    }
                    offset = Math.Clamp(offset, 0, 1);
                    if (!edging && offset == 0.0f)
                    {
                        float accuAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                        dir = mousePos - anchorPos;
                        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                        angle = Math.Clamp(angle, accuAngle - 10, accuAngle + 10);
                        cue.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                    }
                    if (edging && shootAction.IsPressed())
                    {
                        shooting = true;
                        shootForce = Vector3.Distance(anchorPos, ballPos) * offset * 300;
                        hitPoint = Physics2D.BoxCast(cue.transform.position, new Vector2(1, 1), 0, cue.transform.right, Mathf.Infinity, LayerMask.GetMask("Cue Ball"));

                        cueTempPos = cue.sprite.transform.localPosition.x;
                        offset = 0;
                    }
                }
                else
                {
                    offset += Time.deltaTime * 5;
                    offset = Math.Clamp(offset, 0, 1);
                }
                if (!shooting)
                    cue.sprite.transform.localPosition = new Vector3(Vector3.Distance(ballPos, anchorPos) * (1.0f - (float)Math.Pow(offset, 3)) - 74, 0);
                else
                {
                    float cueTargetPos = Vector3.Distance(hitPoint.point, anchorPos) - 70;
                    cue.sprite.transform.localPosition = new Vector3(cueTempPos + (cueTargetPos - cueTempPos) * (float)Math.Pow(offset, 2), 0);
                }
            }
        }
        if (debug)
        {
            hitPoint = Physics2D.BoxCast(cue.transform.position, new Vector2(1, 1), 0, cue.transform.right, Mathf.Infinity, LayerMask.GetMask("Cue Ball"));
            if (hitPoint.collider != null)
            {
                Debug.DrawLine(cue.transform.position, hitPoint.point, Color.red);
            }
        }
        else
        {
            bool stopped = true;
            bool activated = false;

            foreach (var ball in balls)
            {
                if (ball.rb.linearVelocity.magnitude > 0.1f)
                    stopped = false;
                if (ball.rb.linearVelocity.magnitude > 0)
                    activated = true; 
            }
            if (stopped && activated)
            {
                foreach (var ball in balls)
                {
                    ball.rb.linearVelocity = Vector2.zero;
                }
                waiting = false;
            }
        }
    }
    public void ShootBall()
    {
        if (!shooting)
            return;

        var dir = (cueBall.transform.position - anchor.transform.position).normalized;
        cueBall.rb.AddForceAtPosition(dir * shootForce, hitPoint.point);

        waiting = true;
        shooting = false;
        edging = false;
        anchorSet = false;
    }
}
