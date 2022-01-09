using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public static BallController instance;

    public Image powerBarFill;

    public GameObject areaAffector;
    public GameObject aimArrow;
    public float maxForce, forceModifier;
    public LayerMask rayLayer;

    public float lineLength;
    public float lineOffsetY;

    private float force;
    private Rigidbody rb;

    //make force stronger at low power, to make game more intuative to player
    private float powerBalance = 3f;

    private Vector3 startPos, endPos;
    private bool canShoot = false;
    private bool isMoving = false;
    private Vector3 direction;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.sleepThreshold = 0.3f;
    }

    // Update is called once per frame
    void Update()
    {
        if(LevelManager.Instance.Loading) { return; }

        if(rb.velocity == Vector3.zero && isMoving)
        {
            isMoving = false;
            rb.angularVelocity = Vector3.zero;
            areaAffector.SetActive(true);
        }

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (!isMoving && !canShoot)
                    {
                        startPos = ClickedPoint(touch.position);
                        aimArrow.SetActive(true);
                    }
                    break;
                case TouchPhase.Moved:
                    if (!isMoving)
                    {
                        endPos = ClickedPoint(touch.position);
                        CalculateForce();
                    }
                    break;
                case TouchPhase.Ended:
                    if (!isMoving)
                    {
                        canShoot = true;
                        aimArrow.SetActive(false);
                    }
                    break;
            }
        }
        
        areaAffector.transform.position = transform.position;
        aimArrow.transform.position = transform.position;
        

    }

    private void FixedUpdate()
    {
        if (canShoot)
        {
            HitBall();
        }
    }

    void HitBall()
    {
        canShoot = false;
        direction = startPos - endPos;
        rb.AddForce(direction * force, ForceMode.Impulse);
        areaAffector.SetActive(false);
        isMoving = true;
        force = 0;
        startPos = endPos = Vector3.zero;
        LevelManager.Instance.IncrementShot();
    }

    void CalculateForce()
    {
        float startForce = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, maxForce);

        //at 0 power startForce * powerBalance | at max power startForce * 1
        force = startForce * (powerBalance - ((powerBalance - 1) * (startForce / maxForce)));

        powerBarFill.fillAmount = startForce / maxForce;
        Vector3 dir = startPos - endPos;
        if (dir.magnitude > lineLength)
        {
            dir.Normalize();
            dir *= lineLength;
        }
        aimArrow.transform.rotation = Quaternion.LookRotation(dir);
    }

    Vector3 ClickedPoint(Vector3 point)
    {
        Vector3 position = Vector3.zero;
        var ray = Camera.main.ScreenPointToRay(point);

        RaycastHit hit = new RaycastHit();

        if(Physics.Raycast(ray, out hit, Mathf.Infinity, rayLayer))
        {
            position = hit.point;
        }

        return position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            LevelManager.Instance.WinLevel();
        }
    }
}
