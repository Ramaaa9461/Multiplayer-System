using UnityEngine;
using Net;

namespace Game
{

    public class bulletBehaviour : MonoBehaviour, INetObj
    {
        [SerializeField] float velocity;
        [SerializeField] float gravity = 9.8f;  // Valor de la gravedad
        private Vector3 velocityVector;

        int originPlayerID = -1;

        NetObj netObj;

        private void Start()
        {
            Destroy(gameObject, 5.0f);

            velocityVector = transform.forward * velocity;
        }

        public void SetOwnerID(int clientIdOrigin)
        {
            originPlayerID = clientIdOrigin;
        }

        void Update()
        {
            velocityVector.y -= gravity * Time.deltaTime;
            transform.position += velocityVector * Time.deltaTime;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (NetworkManager.Instance.isServer)
            {
                if (collision.transform.TryGetComponent(out PlayerController pc))
                {
                    if (pc.clientID != originPlayerID)
                    {
                        GameManager.Instance.OnBulletHit?.Invoke(pc.clientID);
                    }
                }
            }

            Destroy(gameObject);
        }

        public int GetID()
        {
            return netObj.ID;
        }

        public int GetOwnerID()
        {
            return netObj.OwnerId;
        }

        public NetObj GetNetObj()
        {
            return netObj;
        }
    }
}