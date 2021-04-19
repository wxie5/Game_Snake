using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f;
    public int bulletDamage = 2;

    private CameraFollow cam;
    private bool isCollided;

    public bool IsCollided
    {
        get { return isCollided; }
        set { isCollided = value; }
    }

    private void Start()
    {
        cam = CameraFollow.Instance;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * Time.deltaTime * bulletSpeed);

        float xMin = cam.BKI.Center.x - cam.BKI.Length / 2f;
        float xMax = cam.BKI.Center.x + cam.BKI.Length / 2f;
        float yMin = cam.BKI.Center.y - cam.BKI.Width / 2f;
        float yMax = cam.BKI.Center.y + cam.BKI.Width / 2f;

        if (transform.position.x < xMin || transform.position.x > xMax || transform.position.y < yMin || transform.position.y > yMax)
        {
            Destroy(gameObject);
        }
    }
}
