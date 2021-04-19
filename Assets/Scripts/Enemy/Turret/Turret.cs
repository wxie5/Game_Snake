using UnityEngine;

public class Turret : MonoBehaviour
{
    public float fireRate = 2f;
    public int scoreToEarn = 100;
    public int secureLevel = 3;
    public int reward = 2;
    public ElementaryPower ePower;
    public GameObject prefabBullet;

    private float fireCounter = 0f;

    [Header("Freeze Effect")]
    public float freezeTime = 5f;
    private bool isFreezed;
    private float freezeCounter = 0f;
    public bool IsFreezed
    {
        set { isFreezed = value; }
    }

    private void Update()
    {
        if(isFreezed)
        {
            freezeCounter += Time.deltaTime;
            if(freezeCounter > freezeTime)
            {
                isFreezed = false;
            }
            return;
        }
        freezeCounter = 0;

        BarrelMovement();

        if(fireCounter > fireRate)
        {
            Fire();
            fireCounter = 0f;
        }
        fireCounter += Time.deltaTime;
    }

    public virtual void BarrelMovement() { }
    public virtual void Fire() { }
}

public enum ElementaryPower
{
    None,
    Fire,
    Ice,
    Metal
}
