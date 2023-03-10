using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class CameraController : Singleton<CameraController>
    {
        #region Inspector
        [Header("Components")]
        [SerializeField]
        private new Camera camera;

        [Header("Start Game Settings")]
        [SerializeField]
        private Transform startGameTarget;
        [SerializeField]
        private float startGameSpeed = 10.0f;
        [SerializeField]
        private Vector3 startGameOffset = new Vector3(0.0f, 10.0f, -5.0f);

        [Header("Settings")]
        [SerializeField]
        private Transform offsetTarget;
        [SerializeField]
        private Transform lookAtTarget;
        [SerializeField]
        private Vector3 minOffset = new Vector3(3.0f, 3.0f, 3.0f);
        [SerializeField]
        private Vector3 maxOffset = new Vector3(6.0f, 6.0f, 6.0f);
        [SerializeField]
        private float scrollSpeed = 1.0f;
        [SerializeField]
        private float rotationSpeed = 20.0f;
        [SerializeField]
        private float maxDotLimit = 0.75f;
        [SerializeField]
        private float minDotLimit = -0.4f;
        #endregion

        public Camera Camera
        {
            get { return camera; }
        }
        public Transform OffsetTarget
        {
            get { return offsetTarget; }
            set { offsetTarget = value; }
        }
        public Transform LookAtTarget
        {
            get { return lookAtTarget; }
            set { lookAtTarget = value; }
        }

        public List<LookAtCamera> lookAtCamera = new List<LookAtCamera>();

        private Vector3 euler;
        private float prevEulerX;

        private float zoom = 0.0f;

        private bool followPlayer = false;

        public bool ignoreInput = false;
        protected override void SingletonAwake()
        {
            
        }
        protected override void SingletonDestroy()
        {
           
        }
        private void Start()
        {
            Vector3 position = startGameTarget.position + startGameOffset;
            Quaternion rotation = CalcStartGameRotation(position);
            transform.SetPositionAndRotation(position, rotation);
        }

        private void LateUpdate()
        {
            if(followPlayer)
            {
                if (ignoreInput)
                    return;

                euler.y += Input.GetAxis("Mouse X") * rotationSpeed;
                euler.x += Input.GetAxis("Mouse Y") * rotationSpeed;

                zoom -= Input.GetAxis("Mouse ScrollWheel");
                zoom = Mathf.Clamp(zoom, 0.0f, 1.0f);

                this.ManualUpdate(Time.deltaTime);
            }
            else
            {
                StartGameUpdate();
            }
        }

        public void SwitchToPlayerFollowing()
        {
            followPlayer = true;
        }

        private void StartGameUpdate()
        {
            Vector3 right = transform.right;
            right.y = 0.0f;
            right.Normalize();

            Vector3 currentPosition = transform.position;
            currentPosition += right * startGameSpeed * Time.deltaTime;

            Quaternion rotation = CalcStartGameRotation(currentPosition);
            transform.SetPositionAndRotation(currentPosition, rotation);
        }

        public void ManualUpdate(float deltaTime)
        {
            Vector3 offset = Vector3.Lerp(minOffset, maxOffset, zoom);

            Vector3 forward = Quaternion.Euler(euler) * new Vector3(0.0f, 0.0f, 1.0f);

            float dot = Vector3.Dot(forward, new Vector3(0.0f, 1.0f, 0.0f));
            if (dot > maxDotLimit || dot < minDotLimit)
            {
                euler.x = prevEulerX;
                forward = Quaternion.Euler(euler) * new Vector3(0.0f, 0.0f, 1.0f);
            }
            prevEulerX = euler.x;
            Vector3 finalOffset = new Vector3(-forward.x * offset.x, -forward.y * offset.y, -forward.z * offset.z);

            Vector3 targetPosition = offsetTarget.transform.position;
            Vector3 finalPosition = targetPosition + finalOffset;

            Vector3 rayDir = (finalPosition - targetPosition);
            RaycastHit hit;
            if (Physics.Raycast(targetPosition, rayDir, out hit,
                rayDir.magnitude, GameController.Instance.TerrainLayer))
            {
                finalPosition = hit.point + (-rayDir * 0.1f);
            }

            Quaternion finalRotation = CalcTargetRotation(finalPosition);
            transform.SetPositionAndRotation(finalPosition, finalRotation);


            foreach (LookAtCamera look in lookAtCamera)
            {
                if (!look.enabled || !look.gameObject.activeSelf)
                    continue;

                // very simplistic, in a real project should calculate
                // position on screen with bounds and draw it flat with no transformation
                // for this will do tho
                Vector3 lookPosition = look.transform.position;
                lookPosition.y = finalPosition.y;
                Vector3 dir = finalPosition - lookPosition;
                look.transform.rotation = Quaternion.LookRotation(dir);
            }
        }

        private Quaternion CalcStartGameRotation(Vector3 currentPos)
        {
            Vector3 lookat = startGameTarget.transform.position;
            return Quaternion.LookRotation(lookat - currentPos);
        }
        private Quaternion CalcTargetRotation(Vector3 currentPos)
        {
            Vector3 lookat = lookAtTarget.transform.position;
            return Quaternion.LookRotation(lookat - currentPos);
        }

        #region Editor
        private void Reset()
        {
            camera = GetComponent<Camera>();
        }
        #endregion
    }
}
