using System.Collections;
using UnityEngine;

public class TowerTurns : MonoBehaviour
{
    [NetVariable(0, Net.MessagePriority.Sorteable)] Vector3 towerTurnsPosition;
    
    [SerializeField] float duration;
    [SerializeField] Transform initialPositionShooting;
    [SerializeField] GameObject bulletPrefab;

    Coroutine turnTower;
    Camera cam;

    PlayerController playerController;

    private void Awake()
    {
        cam = Camera.main;
        playerController = GetComponentInParent<PlayerController>();

        towerTurnsPosition = transform.position;
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
        Instantiate(bulletPrefab, initialPositionShooting.position, initialPositionShooting.rotation);
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
