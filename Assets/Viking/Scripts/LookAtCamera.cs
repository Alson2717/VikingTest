using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Viking
{
    public class LookAtCamera : MonoBehaviour
    {
        protected virtual void Start()
        {
            CameraController.Instance.lookAtCamera.Add(this);
        }
    }
}
