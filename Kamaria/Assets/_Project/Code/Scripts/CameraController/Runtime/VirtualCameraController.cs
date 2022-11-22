using Cinemachine;
using UnityEngine;

namespace Kamaria.CameraController
{
    public sealed class VirtualCameraController : MonoBehaviour
    {
        [SerializeField] private CinemachineFollowZoom cinemachineFollowZoom;
        [SerializeField] private float minZoom;
        [SerializeField] private float maxZoom;
        
        public void ZoomCameraOut()
        {
            cinemachineFollowZoom.m_Width = maxZoom;
        }

        public void ZoomCameraDefault()
        {
            cinemachineFollowZoom.m_Width = minZoom;
        }
    }
}