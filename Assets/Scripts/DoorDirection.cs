using UnityEngine;

public class DoorDirection : MonoBehaviour
{
    public DoorDir doorDirection;

    [Header("Set in Inspector")]
    public BackGroundInfo bki_1;
    public BackGroundInfo bki_2;
}

public enum DoorDir
{
    UpAndDown,
    LeftAndRight
}
