using Assets.Scripts.Helper;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

///  <summary>
///  EasyJoystick
///  Developed by Hamza Herbou
///  -------------------------------------------------------
///  Email    : hamza95herbou@gmail.com
///  Github   : https://github.com/herbou/
///  Youtube  : https://youtube.com/c/hamzaherbou
///
///  </summary>

namespace EasyJoystick
{
    public class MyJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public static MyJoystick instance;

        [SerializeField] private RectTransform parent;
        [SerializeField] private RectTransform boundRect;

        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform handle;

        /// <summary>
        /// Enable it if you want to use your keyboard's arrow keys as a joystick too.
        /// </summary>
        [Space(20f)]
        [Tooltip("enable it if you want to use your keyboard's arrow keys as a joystick too.")]
        public bool ArrowKeysSimulationEnabled = false;

        private Vector2 point;
        private Vector2 normalizedPoint;

        private float maxLength;

        private bool _isTouching = false;

        public bool IsTouching { get { return _isTouching; } }

        private PointerEventData pointerEventData;
        private Camera cam;

        private Vector3 firstPos;
        private float maxLength2;

        private void Awake()
        {
            instance = this;
            maxLength = (container.sizeDelta.x / 2f) - (handle.sizeDelta.x / 2f) - 5f;
            maxLength2 = (container.sizeDelta.x / 2f) - (handle.sizeDelta.x / 2f) + 30f;
            this.RegisterListener(EventID.UpdateController, UpdateController);
        }

        private void UpdateController(object obj)
        {
            var isJoystick = ES3.Load(StringDefine.IsJoystick, 0);
            if (isJoystick == 1)
                firstPos = RectTransformUtility.WorldToScreenPoint(Camera.main, container.transform.position);
        }

        public void AfterCameraGen()
        {
            StartCoroutine(IEGameOver());
            IEnumerator IEGameOver()
            {
                yield return new WaitForSeconds(0.5f);
                firstPos = RectTransformUtility.WorldToScreenPoint(Camera.main, container.transform.position);
            }
        }

        public void OnPointerDown(PointerEventData e)
        {
            _isTouching = true;
            cam = e.pressEventCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, e.position, cam, out point);
            boundRect.localPosition = point;

            pointerEventData = e;
        }

        public void OnDrag(PointerEventData e)
        {
            pointerEventData = e;

            var hasPoint = RectTransformUtility.RectangleContainsScreenPoint(boundRect, e.position, cam);
            if (!hasPoint)
            {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, e.position, cam, out point);
                boundRect.localPosition = point;
            }
        }

        void Update()
        {
            if (_isTouching && RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pointerEventData.position, cam, out point))
            {
                point = Vector2.ClampMagnitude(point, maxLength);
                handle.anchoredPosition = point;

                float length = Mathf.InverseLerp(0f, maxLength, point.magnitude);

                if (length < 0.1) return;

                //Debug.Log("length:" + length);

                normalizedPoint = Vector2.ClampMagnitude(point, length);
                var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);
                if (PlayerController.instance != null && PlayerController.instance.gameObject.activeInHierarchy)
                    PlayerController.instance.MoveByJoyStick(angle);
            }
        }

        public void OnPointerUp(PointerEventData e)
        {
            _isTouching = false;
            normalizedPoint = Vector3.zero;
            handle.anchoredPosition = Vector3.zero;

            PlayerController.instance.SetDirectionButton(Assets.Scripts.Define.DynamicEntityDirection.None);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, firstPos, cam, out point);
            boundRect.localPosition = point;
        }

        /// <summary>
        /// Returns horizontal movement. clamped between -1 and 1
        /// </summary>
        public float Horizontal()
        {
            if (ArrowKeysSimulationEnabled)
                return (normalizedPoint.x != 0) ? normalizedPoint.x : Input.GetAxis("Horizontal");

            return normalizedPoint.x;
        }

        /// <summary>
        /// Returns vertical movement. clamped between -1 and 1
        /// </summary>
        public float Vertical()
        {
            if (ArrowKeysSimulationEnabled)
                return (normalizedPoint.y != 0) ? normalizedPoint.y : Input.GetAxis("Vertical");

            return normalizedPoint.y;
        }

    }

    //public class Joystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    //{

    //    [SerializeField] private RectTransform container;
    //    [SerializeField] private RectTransform handle;

    //    /// <summary>
    //    /// Enable it if you want to use your keyboard's arrow keys as a joystick too.
    //    /// </summary>
    //    [Space(20f)]
    //    [Tooltip("enable it if you want to use your keyboard's arrow keys as a joystick too.")]
    //    public bool ArrowKeysSimulationEnabled = false;

    //    private Vector2 point;
    //    private Vector2 normalizedPoint;

    //    private float maxLength;

    //    private bool _isTouching = false;

    //    public bool IsTouching { get { return _isTouching; } }

    //    private PointerEventData pointerEventData;
    //    private Camera cam;


    //    private void Awake()
    //    {
    //        maxLength = (container.sizeDelta.x / 2f) - (handle.sizeDelta.x / 2f) - 5f;
    //    }

    //    public void OnPointerDown(PointerEventData e)
    //    {
    //        _isTouching = true;
    //        cam = e.pressEventCamera;
    //        OnDrag(e);
    //    }

    //    public void OnDrag(PointerEventData e)
    //    {
    //        pointerEventData = e;
    //    }

    //    void Update()
    //    {
    //        if (_isTouching && RectTransformUtility.ScreenPointToLocalPointInRectangle(container, pointerEventData.position, cam, out point))
    //        {
    //            point = Vector2.ClampMagnitude(point, maxLength);
    //            handle.anchoredPosition = point;

    //            float length = Mathf.InverseLerp(0f, maxLength, point.magnitude);
    //            if (length < 0.1f) return;

    //            normalizedPoint = Vector2.ClampMagnitude(point, length);

    //            var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);

    //            PlayerController.instance.MoveByJoyStick(angle);
    //        }
    //    }

    //    public void OnPointerUp(PointerEventData e)
    //    {
    //        _isTouching = false;
    //        normalizedPoint = Vector3.zero;
    //        handle.anchoredPosition = Vector3.zero;

    //        PlayerController.instance.SetDirectionButton(Assets.Scripts.Define.DynamicEntityDirection.None);
    //    }

    //    /// <summary>
    //    /// Returns horizontal movement. clamped between -1 and 1
    //    /// </summary>
    //    public float Horizontal()
    //    {
    //        if (ArrowKeysSimulationEnabled)
    //            return (normalizedPoint.x != 0) ? normalizedPoint.x : Input.GetAxis("Horizontal");

    //        return normalizedPoint.x;
    //    }

    //    /// <summary>
    //    /// Returns vertical movement. clamped between -1 and 1
    //    /// </summary>
    //    public float Vertical()
    //    {
    //        if (ArrowKeysSimulationEnabled)
    //            return (normalizedPoint.y != 0) ? normalizedPoint.y : Input.GetAxis("Vertical");

    //        return normalizedPoint.y;
    //    }

    //}

}