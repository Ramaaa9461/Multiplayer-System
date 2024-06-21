using Net;
using System.Collections;
using UnityEngine;

public class TowerTurns : MonoBehaviour
{
    [SerializeField, NetVariable(0)] float duration;
    [SerializeField] Transform initialPositionShooting;
    [SerializeField] GameObject bulletPrefab;

    Coroutine turnTower;
    Camera cam;

    PlayerController playerController;

    private void Awake()
    {
        cam = Camera.main;
        playerController = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (playerController.currentPlayer)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (turnTower == null)
                {
                    turnTower = StartCoroutine(TurnTower());
                }
            }
        }
    }

    void Shoot()
    {
        IPrefabService prefabService = ServiceProvider.GetService<IPrefabService>();
        int prefabID = prefabService.GetIdByPrefab(bulletPrefab);

        //        if (parentGameObject != null && parentGameObject.TryGetComponent(out INetObj netObj)) //COMO OBTENER EL PARENT ID
        //        {
        //            parentId = netObj.GetID();
        //        }

        NetObjFactory.NetInstance(prefabID, initialPositionShooting.position.x , initialPositionShooting.position.y, initialPositionShooting.position.z,
                                     initialPositionShooting.rotation.x , initialPositionShooting.rotation.y, initialPositionShooting.rotation.z, initialPositionShooting.rotation.w,
                                     bulletPrefab.transform.localScale.x, bulletPrefab.transform.localScale.y, bulletPrefab.transform.localScale.z,
                                     -1);

   //     Instantiate(bulletPrefab, initialPositionShooting.position, initialPositionShooting.rotation);
    }

    IEnumerator TurnTower()
    {
        float timer = 0;

        Quaternion initialRotation = transform.rotation;
        Quaternion newRotation = cam.transform.rotation;
        newRotation.x = 0;
        newRotation.z = 0;

        while (timer <= duration)
        {
            float interpolationValue = timer / duration;

            transform.rotation = Quaternion.Lerp(initialRotation, newRotation, interpolationValue);


            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = newRotation;
        turnTower = null;
        Shoot();
    }
}
