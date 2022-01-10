using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public static BallController instance;

    [Header("UI")]
    public Image powerBarFill;

    public GameObject areaAffector;
    public GameObject aimArrow;

    [Header("Particles")]
    public GameObject hitParticle;

    [Header("Gameplay")]
    public LayerMask rayLayer;
    public float maxForce, forceModifier;

    public float stopVelocity;

    private float force;
    private Rigidbody rb;

    //make force stronger at low power, to make game more intuative to player
    private float powerBalance = 3f;

    private Vector3 startPos, endPos;
    private bool canShoot = false;
    private bool isMoving = false;
    private Vector3 direction;

    private bool velocityCheck = false;

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
                        Instantiate(hitParticle, transform.position, Quaternion.identity);
                    }
                    break;
            }
        }
        
        areaAffector.transform.position = transform.position;
        aimArrow.transform.position = transform.position;
        

    }

    private void FixedUpdate()
    {
        if (velocityCheck) { return; }

        if (rb.velocity.magnitude <= stopVelocity && isMoving)
        {
            StartCoroutine(CheckVelocity());
        }

        if (canShoot)
        {
            HitBall();
        }
    }

    void StopBall()
    {
        isMoving = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        areaAffector.SetActive(true);
        LevelManager.Instance.IncrementShot();
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
    }

    void CalculateForce()
    {
        float startForce = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, maxForce);

        //at 0 power startForce * powerBalance | at max power startForce * 1
        force = startForce * (powerBalance - ((powerBalance - 1) * (startForce / maxForce)));

        powerBarFill.fillAmount = startForce / maxForce;
        Vector3 dir = startPos - endPos;
        dir.Normalize();
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

    IEnumerator CheckVelocity()
    {
        velocityCheck = true;

        float startVelocity = rb.velocity.magnitude;

        yield return new WaitForSeconds(0.3f);

        if(rb.velocity.magnitude <= startVelocity)
        {
            StopBall();
        }

        velocityCheck = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Hole"))
        {
            LevelManager.Instance.WinLevel();
        }
    }
}
