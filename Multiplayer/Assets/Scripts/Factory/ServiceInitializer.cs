using System.Collections.Generic;
using UnityEngine;

public class ServiceInitializer : MonoBehaviour
{
    [SerializeField] private List<GameObject> prefabs;

    private void Awake()
    {
        IPrefabService prefabService = new PrefabService(prefabs);
        ServiceProvider.RegisterService(prefabService);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Instantiate Player");
          //  NetObjectFactory.NetInstance(prefabs[0], Vector3.zero, Quaternion.identity, Vector3.one, null);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("Instantiate Bullet");
            //NetObjectFactory.NetInstance(prefabs[1], Vector3.zero, Quaternion.identity, Vector3.one, prefabs[0]);
        }

    }
}
