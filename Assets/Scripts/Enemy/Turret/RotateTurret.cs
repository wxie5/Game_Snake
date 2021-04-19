using UnityEngine;

public class RotateTurret : Turret
{
    public float rotateSpeed = 20f;

    private Transform barrelCenter;
    private Transform[] barrels;

    private void Start()
    {
        barrelCenter = transform.GetChild(0);
        barrels = new Transform[3];
        for(int i = 0; i < barrels.Length; i++)
        {
            barrels[i] = barrelCenter.GetChild(i);
        }
    }

    public override void BarrelMovement()
    {
        barrelCenter.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }

    public override void Fire()
    {
        foreach(Transform barrelTrans in barrels)
        {
            Instantiate(prefabBullet, barrelTrans.position, barrelTrans.rotation);
        }
    }
}
