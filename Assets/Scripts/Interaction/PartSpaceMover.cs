using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using CommonUsages = UnityEngine.XR.CommonUsages;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Interaction
{
    internal enum Contollers_t
    {
        Right,
        Left
    }

    public class PartSpaceMover : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] [Tooltip("The geometry gameobject that holds the part Mesh")]
        private GameObject m_geometry;
    
        [Space]
        [Header("Inputs")]
        [SerializeField] [Tooltip("The reference to the action of gripping the part space to move, rotate and scale it")]
        InputActionReference m_worldGripRight;

        [SerializeField] [Tooltip("The reference to the action of releasing the part space to move, rotate and scale it")]
        InputActionReference m_worldReleaseRight;

        [SerializeField] [Tooltip("The reference to the action of gripping the part space to move, rotate and scale it")]
        InputActionReference m_worldGripLeft;

        [SerializeField] [Tooltip("The reference to the action of gripping the part space to move, rotate and scale it")]
        InputActionReference m_worldReleaseLeft;

        private UnityEngine.XR.InputDevice[] _controllers = new UnityEngine.XR.InputDevice[2]; 
        private bool[] _grabbing = new bool[2];
        private Pose?[] _controllerPoses = new Pose?[2];

        private Transform _partTransform;

        private void Start()
        {
            InitializeDevices();
        }

        void OnEnable()
        {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;

            m_worldGripRight.action.performed += GrabbedRight;
            m_worldReleaseRight.action.performed += LetGoRight;
            m_worldGripLeft.action.performed += GrabbedLeft;
            m_worldReleaseLeft.action.performed += LetGoLeft;
        }

        private void OnDisable()
        {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
            
            m_worldGripRight.action.performed -= GrabbedRight;
            m_worldReleaseRight.action.performed -= LetGoRight;
            m_worldGripLeft.action.performed -= GrabbedLeft;
            m_worldReleaseLeft.action.performed -= LetGoLeft;
        }

        private void OnDeviceConnected(InputDevice device)
        {
            InitializeDevices();
        }
        private void OnDeviceDisconnected(InputDevice device)
        {
            InitializeDevices();
        }
        private void InitializeDevices()
        {
            InputDeviceCharacteristics rightCtrlChara =
                (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice |
                 InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Right);

            InputDeviceCharacteristics leftCtrlChara =
                (InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.TrackedDevice |
                 InputDeviceCharacteristics.HeldInHand | InputDeviceCharacteristics.Left);

            List<InputDevice> devices = new List<InputDevice>();

            InputDevices.GetDevicesWithCharacteristics(rightCtrlChara, devices);
            if (devices.Count > 0)
                _controllers[(int)Contollers_t.Right] = devices[0];

            devices.Clear();

            InputDevices.GetDevicesWithCharacteristics(leftCtrlChara, devices);
            if (devices.Count > 0)
                _controllers[(int)Contollers_t.Left] = devices[0];
        }

        private void GrabbedRight(InputAction.CallbackContext context)
        {
            OnGrabbed(Contollers_t.Right);
        }
        private void LetGoRight(InputAction.CallbackContext context)
        {
            OnLetGo(Contollers_t.Right);
        }
        private void GrabbedLeft(InputAction.CallbackContext context)
        {
            OnGrabbed(Contollers_t.Left);
        }
        private void LetGoLeft(InputAction.CallbackContext context)
        {
            OnLetGo(Contollers_t.Left);
        }

        private void OnGrabbed(Contollers_t controller)
        {
            _grabbing[(int)controller] = true;

            // save starting point to calculate movement vector later
            if (GetControllerPose(controller, out Pose? pose))
            {
                _controllerPoses[(int)controller] = pose;

                // also save transform of the part if not already moving
                if (controller == Contollers_t.Right)
                {
                    if (!_grabbing[(int)Contollers_t.Left])
                    {
                        _partTransform = m_geometry.transform;
                    }
                }
                else if (!_grabbing[(int)Contollers_t.Right])
                {
                    _partTransform = m_geometry.transform;
                }
            }
        }

        private void OnLetGo(Contollers_t controller)
        {
            _grabbing[(int)controller] = false;
        }

        private bool GetControllerPose(Contollers_t controller, out Pose? pose)
        {
            pose = null;
            bool success = false;

            success = _controllers[(int)controller].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
            if (!success) return false;
            _controllers[(int)controller].TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

            pose = new Pose(position, rotation);
            return true;
        }

        private void Update()
        {
            if (_grabbing[(int)Contollers_t.Right] && _grabbing[(int)Contollers_t.Left])
            {
            
            }
            else if (_grabbing[(int)Contollers_t.Right])
            {
                if (GetControllerPose(Contollers_t.Right, out Pose? currentPose))
                {
                    m_geometry.transform.position = _partTransform.position +
                                                    ((Pose)currentPose).position -
                                                    ((Pose)_controllerPoses[(int)Contollers_t.Right]).position;

                    _controllerPoses[(int)Contollers_t.Right] = currentPose;
                }
            }
            else if (_grabbing[(int)Contollers_t.Left])
            {
                if (GetControllerPose(Contollers_t.Left, out Pose? currentPose))
                {
                    m_geometry.transform.position = _partTransform.position +
                                                    ((Pose)currentPose).position -
                                                    ((Pose)_controllerPoses[(int)Contollers_t.Left]).position;

                    _controllerPoses[(int)Contollers_t.Left] = currentPose;
                }
            }
        }
    }
}