using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemSingleton : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsOfType<EventSystem>().Length > 1)
            Destroy(gameObject);
    }
}
