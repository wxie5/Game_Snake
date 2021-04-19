using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTurret : Turret
{
    public float moveSpeed = 5f;
    public float offset = 1f;
    public float freezeRange = 3f;

    private CameraFollow cam;
    private Transform playerTrans;
    private Vector3 targetPos;


    private void Start()
    {
        cam = CameraFollow.Instance;
        playerTrans = Head.Instance.gameObject.transform;
        GenerateTargetPos();
        InvokeRepeating(nameof(DisCheck), 0f, 0.02f);
        InvokeRepeating(nameof(Freeze), 0f, 0.04f);
    }

    public override void BarrelMovement()
    {
        Vector2 dir = targetPos - transform.position;
        transform.Translate(dir.normalized * moveSpeed * Time.deltaTime);       
    }

    private void GenerateTargetPos()
    {
        float xMin = cam.BKI.Center.x - (cam.BKI.Length / 2f) + offset;
        float xMax = cam.BKI.Center.x + (cam.BKI.Length / 2f) - offset;
        float yMin = cam.BKI.Center.y - (cam.BKI.Width / 2f) + offset;
        float yMax = cam.BKI.Center.y + (cam.BKI.Width / 2f) - offset;
        targetPos = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0f);
    }

    private void DisCheck()
    {
        if(Vector2.Distance(transform.position, targetPos) < 0.1f)
        {
            GenerateTargetPos();
        }
    }

    private void Freeze()
    {
        if(Vector2.Distance(transform.position, playerTrans.position) < freezeRange)
        {
            Head.Instance.FreezeUp();
        }
    }
}
