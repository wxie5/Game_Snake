using UnityEngine;

public class BackGroundInfo : MonoBehaviour
{
    private Vector3 _center;
    private float _length;
    private float _width;

    public float Length
    {
        get { return _length; }
    }

    public float Width
    {
        get { return _width; }
    }

    public Vector3 Center
    {
        get { return _center; }
    }

    private void Start()
    {
        _center = transform.position;
        _width = gameObject.transform.localScale.y * 0.2f;
        _length = gameObject.transform.localScale.x * 0.2f;
    }
}
