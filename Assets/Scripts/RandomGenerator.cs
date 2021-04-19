using System.Collections.Generic;
using UnityEngine;

public class RandomGenerator : MonoBehaviour
{
    public List<GameObject> generateList;
    public List<float> frequencyList;
    public int maxItemNum = 50;
    public float generateCD = 2f;
    public float boundsOffset = 1f;
    [SerializeField]
    private BackGroundInfo bki;

    private CameraFollow cam;
    private List<GameObject> existingItems;
    private float timeCounter;

    private void Start()
    {
        existingItems = new List<GameObject>();
        timeCounter = 0;

        cam = CameraFollow.Instance;
        bki = cam.BKI;
        cam.onBKIChange += ChangeGeneratePosition;
    }

    private void Update()
    {
        if(generateList == null || existingItems.Count >= maxItemNum)
        {
            return;
        }

        if (timeCounter > generateCD)
        {
            for (int i = 0; i < generateList.Count; i++)
            {
                if (frequencyList[i] > Random.value)
                {
                    float randomX = Random.Range(-bki.Length / 2 + bki.Center.x + boundsOffset, bki.Length / 2 + bki.Center.x - boundsOffset);
                    float randomY = Random.Range(-bki.Width / 2 + bki.Center.y + boundsOffset, bki.Width / 2 + bki.Center.y - boundsOffset);
                    GameObject go = Instantiate(generateList[i], new Vector3(randomX, randomY, 0f), Quaternion.identity);
                    existingItems.Add(go);

                }
            }
            timeCounter = 0f;
        }
        timeCounter += Time.deltaTime;
    }

    public void ChangeGeneratePosition()
    {
        foreach(GameObject go in existingItems)
        {
            if(go != null)
            {
                Destroy(go);
            }
        }
        existingItems.Clear();
        bki = cam.BKI;
    }

    public void RemoveElement(GameObject element)
    {
        if(existingItems.Contains(element))
        {
            existingItems.Remove(element);
            Destroy(element);
        }
    }
}
