using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.Experimental.XR;

public class FlagBehaviour : MonoBehaviour
{
    public Transform ball;

    public float ballDistTreshold = 3;
    public float height = 3;

    Transform camera;

    Vector3 flagXZPos;

    float flagStartYPos;

    private void Awake()
    {
        camera = GameObject.Find("AR Camera").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        flagStartYPos = transform.position.y;
        flagXZPos = new Vector3(transform.position.x, 0, transform.position.z);        
    }

    // Update is called once per frame
    void Update()
    {
        RotateToCam();
        MoveWhenBallNear();
    }

    void RotateToCam()
    {
        Vector3 camXZPos = new Vector3(camera.position.x, 0, camera.position.z);
        Vector3 dir = flagXZPos - camXZPos;

        dir.Normalize();

        transform.rotation = Quaternion.LookRotation(dir);
    }

    void MoveWhenBallNear()
    {
        Vector3 ballXZPos = new Vector3(ball.position.x, 0, ball.position.z);

        float dist = Vector3.Distance(ballXZPos, flagXZPos);

        if (dist < ballDistTreshold)
        {
            float yValue = (ballDistTreshold - dist) * (height / ballDistTreshold);
            transform.position = new Vector3(transform.position.x, flagStartYPos + yValue, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, flagStartYPos, transform.position.z);
        }
    }
}
