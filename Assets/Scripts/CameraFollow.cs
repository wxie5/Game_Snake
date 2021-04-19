using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private static CameraFollow _instance;
    public static CameraFollow Instance
    {
        get { return _instance; }
    }

    public delegate void OnBKIChange();
    public OnBKIChange onBKIChange;
    public BackGroundInfo defaultBKI;

    public GameObject POI;
    public float lerpU = 0.1f;

    private BackGroundInfo bki;
    private Camera cam;

    private bool isConstrain;

    public BackGroundInfo BKI
    {
        set 
        {
            bki = value;
            onBKIChange();
        }
        get { return bki; }
    }

    public bool IsConstrain
    {
        set { isConstrain = value; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("CameraFollow-Awake(): Try to create more than one instance of CameraFollow(Singleton Design)");
        }
    }

    private void Start()
    {
        bki = defaultBKI;
        cam = Camera.main;
        isConstrain = true;
    }

    private void FixedUpdate()
    {
        if(POI == null)
        {
            return;
        }
        Vector3 followPos = new Vector3(POI.transform.position.x, POI.transform.position.y, transform.position.z);
        if (isConstrain && bki != null)
        {
            float xMin = bki.Center.x - (bki.Length / 2f) + cam.orthographicSize * cam.aspect / 2f;
            float xMax = bki.Center.x + (bki.Length / 2f) - cam.orthographicSize * cam.aspect / 2f;
            float yMin = bki.Center.y - (bki.Width / 2f) + cam.orthographicSize / 2f;
            float yMax = bki.Center.y + (bki.Width / 2f) - cam.orthographicSize / 2f;
            followPos.x = Mathf.Clamp(followPos.x, xMin, xMax);
            followPos.y = Mathf.Clamp(followPos.y, yMin, yMax);
        }
        cam.transform.position = Vector3.Slerp(cam.transform.position, followPos, lerpU);
    }

}
