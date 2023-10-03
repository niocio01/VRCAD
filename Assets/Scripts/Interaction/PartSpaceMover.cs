using System;
using System.Collections.Generic;
using Rendering;
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
        Left,
        Center
    }

    public class PartSpaceMover : MonoBehaviour
    {
        [Header("Objects")]
        [SerializeField] [Tooltip("The geometry gameobject that holds the part Mesh")]
        private GameObject m_Part;

        [SerializeField] [Tooltip("Prefab that indicates the movement and rotation center.")]
        private GameObject m_CenterPointIndicator;

        [SerializeField] [Tooltip("Prefab that indicates the center line between the controllers.")]
        private GameObject m_CenterLineIndicator;

        [SerializeField] [Tooltip("Parent for Center line and Point indicators")]
        private GameObject m_IndicatorParent;
    
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

        private readonly InputDevice[] _controllers = new InputDevice[2]; 
        private readonly bool[] _grabbing = new bool[2];
        private readonly Pose[] _controllerPoses = new Pose[3];
        private float _startLength;
        private float _startScale;
        

        private Transform _partTransform;
        private GameObject _centerLine;
        private LineController _lineController;
        private GameObject _centerPoint;
        private string _guiText;

        private void Awake()
        {
            _centerLine = Instantiate(m_CenterLineIndicator, Vector3.zero, Quaternion.identity, m_IndicatorParent.transform);
            _lineController = _centerLine.GetComponent<LineController>();

            _centerPoint = Instantiate(m_CenterPointIndicator, m_IndicatorParent.transform);
        }

        private void Start()
        {
            InitializeDevices();
        }

        void OnEnable()
        {
            InputDevices.deviceConnected += OnDeviceConnected;
            InputDevices.deviceDisconnected += OnDeviceDisconnected;

            m_worldGripRight.action.performed += GrabbedRight;
            m_worldGripLeft.action.performed += GrabbedLeft;
            m_worldReleaseRight.action.performed += LetGoRight;
            m_worldReleaseLeft.action.performed += LetGoLeft;

            m_CenterPointIndicator.GetComponent<Renderer>().enabled = false;
        }

        private void OnDisable()
        {
            InputDevices.deviceConnected -= OnDeviceConnected;
            InputDevices.deviceDisconnected -= OnDeviceDisconnected;
            
            m_worldGripRight.action.performed -= GrabbedRight;
            m_worldGripLeft.action.performed -= GrabbedLeft;
            m_worldReleaseRight.action.performed -= LetGoRight;
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
        private void GrabbedLeft(InputAction.CallbackContext context)
        {
            OnGrabbed(Contollers_t.Left);
        }
        private void LetGoLeft(InputAction.CallbackContext context)
        {
            OnLetGo(Contollers_t.Left);
        }
        private void LetGoRight(InputAction.CallbackContext context)
        {
            OnLetGo(Contollers_t.Right);
        }

        private void OnGrabbed(Contollers_t controller)
        {
            // save starting point to calculate movement vector later
            if (GetControllerPose(controller, out Pose pose))
            {
                _grabbing[(int)controller] = true;
                _partTransform = m_Part.transform;
                _controllerPoses[(int)controller] = pose;

                // save initial position and rotation
                if (_grabbing[(int)Contollers_t.Right] && _grabbing[(int)Contollers_t.Left])
                {
                    GetCombinedPosRotDist(out Vector3 centerPosition, out Quaternion rotation, out _);
                    _controllerPoses[(int)Contollers_t.Center] = new Pose(centerPosition, rotation);

                    _startLength = (_controllerPoses[(int)Contollers_t.Left].position -
                                    _controllerPoses[(int)Contollers_t.Right].position).magnitude;

                    _startScale = m_Part.transform.localScale.x;

                    _lineController.SetPoints(
                        _controllerPoses[(int)Contollers_t.Left].position,
                        _controllerPoses[(int)Contollers_t.Left].position);
                    _centerLine.GetComponent<Renderer>().enabled = true;

                    _centerPoint.transform.SetPositionAndRotation(
                        centerPosition,
                        rotation);
                    _centerPoint.GetComponent<Renderer>().enabled = true;
                }
            }
        }

        private void GetCombinedPosRotDist(out Vector3 centerPosition,out Quaternion rotation,out float distance)
        {
            Vector3 rightPos = _controllerPoses[(int)Contollers_t.Right].position;
            Vector3 leftPos = _controllerPoses[(int)Contollers_t.Left].position;

            centerPosition = Vector3.Lerp(leftPos, rightPos, 0.5f);
            distance = (leftPos - rightPos).magnitude;


            // Get the average quaternion of the controllers
            Quaternion controllerRotation = Quaternion.Lerp(
                _controllerPoses[(int)Contollers_t.Right].rotation,
                _controllerPoses[(int)Contollers_t.Left].rotation, 0.5f);

            Vector3 forward = Vector3.Cross(rightPos - leftPos, Vector3.up);
            Vector3 up = Vector3.Cross(forward,rightPos - leftPos);

            Quaternion forwardRotation = Quaternion.LookRotation(forward, up);

            Vector3 rightForward = controllerRotation * Vector3.forward;

            float controllerAngle = SignedAngleFromDirection(rightForward, forward, rightPos - leftPos);

            Quaternion axisRotation = Quaternion.AngleAxis(- controllerAngle, Vector3.right);

            rotation = forwardRotation * axisRotation * Quaternion.FromToRotation(new Vector3(0,0 ,1), new Vector3(0, -1, 0) );
        }

        private void OnLetGo(Contollers_t controller)
        {
            _grabbing[(int)controller] = false;

            _centerLine.GetComponent<Renderer>().enabled = false;
            _centerPoint.GetComponent<Renderer>().enabled = false;
        }

        private float SignedAngleFromDirection(Vector3 fromdir, Vector3 todir, Vector3 referenceup)
        {
            // calculates the the angle between two direction vectors, with a referenceup a sign in which direction it points can be calculated (clockwise is positive and counter clockwise is negative)
            Vector3 planenormal = Vector3.Cross(fromdir, todir); // calculate the planenormal (perpendicular vector)
            float
                angle = Vector3.Angle(fromdir,
                    todir); // calculate the angle between the 2 direction vectors (note: its always the smaller one smaller than 180°)
            float
                orientationdot =
                    Vector3.Dot(planenormal,
                        referenceup); // calculate wether the normal and the referenceup point in the same direction (>0) or not (<0), http://docs.unity3d.com/Documentation/Manual/ComputingNormalPerpendicularVector.html
            if (orientationdot > 0.0f) // the angle is positive (clockwise orientation seen from referenceup)
                return angle;
            return -angle; // the angle is negative (counter-clockwise orientation seen from referenceup)
        }

        private bool GetControllerPose(Contollers_t controller, out Pose pose)
        {
            pose = new Pose();

            bool success = _controllers[(int)controller].TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
            if (!success) return false;
            _controllers[(int)controller].TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

            pose = new Pose(position, rotation);
            return true;
        }

        private void Update()
        {
            _partTransform = m_Part.transform;
            if (_grabbing[(int)Contollers_t.Right] && _grabbing[(int)Contollers_t.Left])
            {
                if (GetControllerPose(Contollers_t.Right, out Pose currentRightPose) &&
                    GetControllerPose(Contollers_t.Left, out Pose currentLeftPose))
                {   
                    GetCombinedPosRotDist(
                        out Vector3 currentCenterPosition,
                        out Quaternion currentRotation,
                        out float currentLength);

                    _centerPoint.transform.SetPositionAndRotation(currentCenterPosition, currentRotation);
                    _lineController.SetPoints(currentLeftPose.position, currentRightPose.position);
                    
                    Vector3 newPos = 
                        _partTransform.position +
                        currentCenterPosition - 
                        _controllerPoses[(int)Contollers_t.Center].position;
                    m_Part.transform.position = newPos;

                    // quaternion multiplication is the equivalent of vector addition
                    // Quaternion subtraction is done by multiplying with the inverse of the second Quaternion
                    Quaternion rotation = currentRotation * Quaternion.Inverse(_controllerPoses[(int)Contollers_t.Center].rotation);
                    m_Part.transform.Rotate(rotation.eulerAngles, Space.World);

                    // Compute and set scale
                    float newScale = _startScale * (currentLength / _startLength);
                    m_Part.transform.localScale = new Vector3(newScale, newScale, newScale);

                    // save current values for next iteration
                    _controllerPoses[(int)Contollers_t.Center] = new Pose(currentCenterPosition, currentRotation);
                    _controllerPoses[(int)Contollers_t.Right] = currentRightPose;
                    _controllerPoses[(int)Contollers_t.Left] = currentLeftPose;
                }
            }
            else if (_grabbing[(int)Contollers_t.Right])
            {
                if (GetControllerPose(Contollers_t.Right, out Pose currentPose))
                {
                    m_Part.transform.position = _partTransform.position + currentPose.position - _controllerPoses[(int)Contollers_t.Right].position;
                    _controllerPoses[(int)Contollers_t.Right] = currentPose;
                }
            }
            else if (_grabbing[(int)Contollers_t.Left])
            {
                if (GetControllerPose(Contollers_t.Left, out Pose currentPose))
                {
                    m_Part.transform.position = _partTransform.position + currentPose.position - _controllerPoses[(int)Contollers_t.Left].position;
                    _controllerPoses[(int)Contollers_t.Left] = currentPose;
                }
            }
        }
    }
}