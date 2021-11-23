using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public static BallController instance;

    public GameObject winScreen;

    public LineRenderer lineRenderer;
    public GameObject areaAffector;
    public float maxForce, forceModifier;
    public LayerMask rayLayer;

    public float arrowLength;

    private float force;
    private Rigidbody rb;

    private Vector3 startPos, endPos;
    private bool canShoot = false;
    private bool isMoving = false;
    private Vector3 direction;

    private bool win;

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
        if (win)
        {
            winScreen.SetActive(true);
        }
        else
        {
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
                            lineRenderer.gameObject.SetActive(true);
                            //lineRenderer.SetPosition(0, lineRenderer.transform.localPosition);
                            lineRenderer.SetPosition(0, Vector3.zero);
                        }
                        break;
                    case TouchPhase.Moved:
                        if (!isMoving)
                        {
                            endPos = ClickedPoint(touch.position);
                            force = Mathf.Clamp(Vector3.Distance(endPos, startPos) * forceModifier, 0, maxForce);
                            //lineRenderer.SetPosition(1, transform.InverseTransformPoint(endPos));
                            Vector3 dir = startPos - endPos;
                            if(dir.magnitude > 10)
                            {
                                dir.Normalize();
                                dir *= arrowLength;
                            }
                            lineRenderer.SetPosition(1, new Vector3(dir.x, transform.position.y, dir.z));
                        }
                        break;
                    case TouchPhase.Ended:
                        if (!isMoving)
                        {
                            canShoot = true;
                            lineRenderer.gameObject.SetActive(false);
                        }
                        break;
                }
            }
        
            areaAffector.transform.position = transform.position;
            lineRenderer.transform.position = transform.position;
        }
        

    }

    private void FixedUpdate()
    {
        if (canShoot)
        {
            canShoot = false;
            direction = startPos - endPos;
            rb.AddForce(direction * force, ForceMode.Impulse);
            areaAffector.SetActive(false);
            isMoving = true;
            force = 0;
            startPos = endPos = Vector3.zero;
        }
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
            win = true;
        }
    }
}
