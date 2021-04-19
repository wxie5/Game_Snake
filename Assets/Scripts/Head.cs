using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Head : MonoBehaviour
{
    private static Head _instance;
    public static Head Instance
    {
        get { return _instance; }
    }


    [Header("Body Setting")]
    public float distanceBetNode = 0.2f;
    public GameObject prefabNode;
    public int initBodyNum = 5;

    [Header("Movement Setting")]
    public float moveSpeed = 2f;
    public float rotateSpeed = 180f;

    [Header("Bounds Check")]
    public float boundOffset = 0.05f;

    [Header("Hack Setting")]
    public float hackRange = 5f;
    public int upgradeReq = 15;
    public int hackingLevel = 1;

    //this block will be discard when I have actual UI
    [Header("Set for UI testing")]
    public Text fireT;
    public Text iceT;
    public Text metalT;
    public Text levelT;
    public RectTransform expTrans;

    private Queue<Vector2> _pastPos;
    private Node headNode;
    private Node tailNode;
    private int maxLength;
    private int currentLength = 0;
    private SpriteRenderer spRenderer;
    private bool isMoving;
    private bool isRotating;
    private CameraFollow cam;
    private int sortOrder = 2;

    //smooth Add Note Vars
    private int smoothAddCounter = 0;
    private float smoothAddTimer = 0f;
    private float smoothAddCD = 0.5f;
    private int smoothAddBuffer = 0;

    private GameObject currentHackingObj = null;
    private bool isHacking = false;

    [Header("Max Skill Level")]
    //elementary power
    public int maxFireLevel = 5;
    public int maxIceLevel = 5;
    public int maxMetalLevel = 5;
    private int fireLevel = 0;
    private int iceLevel = 0;
    private int metalLevel = 0;
    public int FireLevel
    {
        get { return fireLevel; }
        set
        {
            if (value < 0)
            {
                fireLevel = 0;
            }
            else if (value > maxFireLevel)
            {
                fireLevel = maxFireLevel;
            }
            else
            {
                fireLevel = value;
            }
            fireT.text = "Fire: " + fireLevel;
        }
    }
    public int IceLevel
    {
        get { return iceLevel; }
        set
        {
            if (value < 0)
            {
                iceLevel = 0;
            }
            else if (value > maxIceLevel)
            {
                iceLevel = maxIceLevel;
            }
            else
            {
                iceLevel = value;
            }
            iceT.text = "Ice: " + iceLevel;
        }
    }
    public int MetalLevel
    {
        get { return metalLevel; }
        set
        {
            if (value < 0)
            {
                metalLevel = 0;
            }
            else if (value > maxMetalLevel)
            {
                metalLevel = maxMetalLevel;
            }
            else
            {
                metalLevel = value;
            }
            metalT.text = "Metal: " + metalLevel;
        }
    }

    [Header("Skill CountDown")]
    //fireSkills
    public int fireSkillCD;
    //IceSkills
    public int iceSkillCD;
    //MetalSkills
    public int metalSkillCD;

    [Header("Freeze Effect")]
    public float freezeTime = 2f;
    private bool isFreezed;
    private float freezeCounter = 0f;

    public Node TailNode
    {
        set { tailNode = value; }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Debug.LogError("Head-Awake(): Try to create more than one instance of Head(Singleton Design)");
        }
    }

    private void Start()
    {
        maxLength = (int)(distanceBetNode / (moveSpeed / 50f));
        headNode = null;
        tailNode = null;
        _pastPos = new Queue<Vector2>();
        spRenderer = GetComponent<SpriteRenderer>();
        isMoving = true;
        isRotating = true;
        cam = CameraFollow.Instance;

        smoothAddCounter = initBodyNum;
    }

    private void FixedUpdate()
    {
        Move();
        _pastPos.Enqueue(transform.position);
        if (_pastPos.Count < maxLength)
        {
            return;
        }
        else if (headNode == null)
        {
            _pastPos.Dequeue();
        }
        else
        {
            headNode.UpdatePastPos(_pastPos.Dequeue());
        }
    }

    private void Update()
    {
        if (isHacking)
        {
            return;
        }

        if (isFreezed)
        {
            freezeCounter += Time.deltaTime;
            if (freezeCounter > freezeTime)
            {
                isFreezed = false;
                moveSpeed = 2f;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                moveSpeed = 4f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                moveSpeed = 2f;
            }
            if (Input.GetMouseButtonDown(1))
            {
                moveSpeed = 1f;
            }
            if (Input.GetMouseButtonUp(1))
            {
                moveSpeed = 2f;
            }
        }

        SmoothAddNode();
        BoundCheck();

        if(Input.GetKeyDown(KeyCode.H))
        {
            Hack();
        }
        if(Input.GetKeyDown(KeyCode.J))
        {
            IceSkill();
        }
    }

    private void AddNode()
    {
        Vector3 instantPos = _pastPos.Peek();
        if(tailNode != null)
        {
            instantPos = tailNode.GetLastPos();
        }
        GameObject node = Instantiate(prefabNode, instantPos, Quaternion.identity) as GameObject;
        Node temp = node.GetComponent<Node>();
        temp.MaxLength = maxLength;
        sortOrder += 1;
        currentLength += 1;
        UpdateHackLevel();
        if (headNode == null)
        {
            headNode = tailNode = temp;
            spRenderer.sortingOrder = sortOrder;
            headNode.SetSortOrder(sortOrder - 1);
            return;
        }
        tailNode.Next = temp;
        tailNode = temp;

        spRenderer.sortingOrder = sortOrder;
        headNode.SetSortOrder(sortOrder - 1);
    }

    private void SmoothAddNode()
    {
        if (smoothAddBuffer >= 0)
        {
            smoothAddCounter += smoothAddBuffer;
            smoothAddBuffer = 0;
        }
        if (smoothAddCounter <= 0)
        {
            return;
        }
        if(smoothAddTimer > smoothAddCD)
        {
            smoothAddCounter--;
            smoothAddTimer = 0f;
            AddNode();
        }
        smoothAddTimer += Time.deltaTime;
    }

    public void RemoveNode(int numToRemove)
    {
        if (headNode == null || numToRemove > Length())
        {
            Die();
            return;
        }
        int removeCounter = Length() - numToRemove;
        Node startRemoveNode = headNode;
        if (removeCounter == 0)
        {
            headNode.Die();
            currentLength = 0;
            UpdateHackLevel();
            return;
        }
        currentLength -= numToRemove;
        UpdateHackLevel();
        while(removeCounter > 1)
        {
            removeCounter--;
            startRemoveNode = startRemoveNode.Next;
        }
        tailNode = startRemoveNode;
        startRemoveNode.Next.Die();
    }
    
    private void Move()
    {
        if(!isMoving)
        {
            return;
        }
        if (isRotating)
        {
            float xAxis = -Input.GetAxis("Horizontal");
            transform.Rotate(0f, 0f, xAxis * rotateSpeed * Time.fixedDeltaTime);
        }
        transform.Translate(0f, moveSpeed * Time.fixedDeltaTime, 0f);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if(coll.CompareTag("Food"))
        {
            int bodiesToAdd = coll.GetComponent<Food>().foodLevel;
            if(bodiesToAdd > 0)
            {
                smoothAddBuffer += bodiesToAdd;
            }
            RandomGenerator itemGenerator = GameObject.Find("ItemGenerator").GetComponent<RandomGenerator>();
            if (itemGenerator != null)
            {
                itemGenerator.RemoveElement(coll.gameObject);
            }
        }

        if(coll.CompareTag("Door"))
        {
            spRenderer.sortingOrder = -1;
            isMoving = false;
            isRotating = false;
            transform.position = coll.transform.position;
            DoorDir doorDir = coll.GetComponent<DoorDirection>().doorDirection;
            if(doorDir == DoorDir.UpAndDown)
            {
                if(transform.rotation.eulerAngles.z <= 90f || transform.rotation.eulerAngles.z >= 270f)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, 180);
                }
            }
            else if(doorDir == DoorDir.LeftAndRight)
            {
                if(transform.rotation.eulerAngles.z <= 180f)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 90);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0, 0, -90);
                }
            }

            cam.IsConstrain = false;
        }

        if(coll.CompareTag("EnemyBullet"))
        {
            Bullet b = coll.GetComponent<Bullet>();
            if (b.IsCollided)
            {
                return;
            }
            else
            {
                b.IsCollided = true;
            }
            int numToRemove = b.bulletDamage;
            RemoveNode(numToRemove);
            Destroy(coll.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if(coll.CompareTag("Door"))
        {
            spRenderer.sortingOrder = sortOrder;
            headNode.SetSortOrder(sortOrder - 1);
            isRotating = true;
            DoorDirection doorInfor = coll.GetComponent<DoorDirection>();
            if(cam.BKI == doorInfor.bki_1)
            {
                cam.BKI = doorInfor.bki_2;
            }
            else
            {
                cam.BKI = doorInfor.bki_1;
            }
        }
    }

    public void AllStopped()
    {
        Invoke(nameof(AllowMove), 2f);
    }

    private void AllowMove()
    {
        isMoving = true;
    }

    private void Die()
    {
        if (headNode != null)
        {
            headNode.Die();
        }
        tailNode = null;
        SceneManager.LoadScene(0);
        //Destroy(gameObject);
    }

    private void BoundCheck()
    {
        if(!isRotating)
        {
            return;
        }

        float xMin = cam.BKI.Center.x - cam.BKI.Length / 2f - boundOffset;
        float xMax = cam.BKI.Center.x + cam.BKI.Length / 2f + boundOffset;
        float yMin = cam.BKI.Center.y - cam.BKI.Width / 2f - boundOffset;
        float yMax = cam.BKI.Center.y + cam.BKI.Width / 2f + boundOffset;

        if(transform.position.x < xMin || transform.position.x > xMax || transform.position.y < yMin || transform.position.y > yMax)
        {
            Die();
        }
    }

    private int Length()
    {
        int counter = 0;
        if (headNode != null)
        {
            Node current = headNode;
            counter += 1;
            while (current.Next != null)
            {
                counter += 1;
                current = current.Next;
            }
        }
        return counter;
    }

    public bool Hack(int powerUp = 0)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, hackRange, LayerMask.GetMask("Enemy"));
        if (colls.Length > 0)
        {
            Collider2D closeOne = colls[0];
            Debug.Log(colls[0].name);
            if(colls.Length > 1)
            {
                float currentDis = Vector2.Distance(closeOne.transform.position, transform.position);
                for(int i = 1; i < colls.Length; i++)
                {
                    Debug.Log(colls[i].name);
                    float dis = Vector2.Distance(colls[i].transform.position, transform.position);
                    if (dis < currentDis) closeOne = colls[i];
                }
            }

            currentHackingObj = closeOne.gameObject;

            if (hackingLevel + powerUp >= currentHackingObj.GetComponent<Turret>().secureLevel)
            {
                isHacking = true;
                Time.timeScale = 0;
                cam.gameObject.SetActive(false);
                SceneManager.LoadSceneAsync("SmartGame_1", LoadSceneMode.Additive);
                return true;
            }
            else
            {
                //level too low
                return false;
            }
        }
        else
        {
            //No target in range
            return false;
        }
    }

    public void HackOver(bool isWin)
    {
        if(isWin && currentHackingObj != null)
        {
            Turret curTurret = currentHackingObj.GetComponent<Turret>();
            ElementaryPower ep = curTurret.ePower;
            int reward = curTurret.reward;
            if(ep != ElementaryPower.None)
            {
                switch(ep)
                {
                    case ElementaryPower.Fire:
                        FireLevel += reward;
                        break;
                    case ElementaryPower.Ice:
                        IceLevel += reward;
                        break;
                    case ElementaryPower.Metal:
                        MetalLevel += reward;
                        break;
                    default:
                        break;
                }
            }
            Destroy(currentHackingObj);
        }
        cam.gameObject.SetActive(true);
        isHacking = false;
        Time.timeScale = 1;
    }


    public void UpdateHackLevel()
    {
        if(currentLength == 0)
        {
            hackingLevel = 1;
        }
        else
        {
            hackingLevel = (int)(currentLength / upgradeReq) + 1;
        }

        levelT.text = hackingLevel.ToString();
        expTrans.localScale = new Vector3((currentLength % upgradeReq) / (float)upgradeReq, 1);
    }

    public void MetalSkill()
    {
        if(MetalLevel < maxMetalLevel)
        {
            return;
        }

        bool temp = Hack(1);
        if(temp)
        {
            MetalLevel = 0;
        }
    }

    public void IceSkill()
    {
        if(IceLevel < maxIceLevel)
        {
            return;
        }

        GameObject[] turrets = GameObject.FindGameObjectsWithTag("Enemy");
        if(turrets != null)
        {
            foreach(GameObject go in turrets)
            {
                go.GetComponent<Turret>().IsFreezed = true;
            }
        }
        IceLevel = 0;
    }

    public void FreezeUp()
    {
        isFreezed = true;
        moveSpeed = 1f;
        freezeCounter = 0;
    }

}
