using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class Debugger : MonoBehaviour
{
    void Start()
    {
        int activeObjectsCount = FindObjectsOfType<GameObject>().Where(obj => obj.activeInHierarchy).Count();
        Debug.Log("Active GameObjects: " + activeObjectsCount);

        int activeMonoBehavioursCount = FindObjectsOfType<MonoBehaviour>().Where(script => script.enabled).Count();
        Debug.Log("Active MonoBehaviours: " + activeMonoBehavioursCount);
    }
}
