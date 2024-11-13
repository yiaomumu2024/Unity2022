namespace Product.Script
{
    using UnityEngine;

    using UnityEngine.Events;

    using UnityEngine.EventSystems;

    public class ButtonExtension : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {

        public float pressDurationTime = 1;

        public bool responseOnceByPress = false;

        public float doubleClickIntervalTime = 0.5f;

        [Header("点击的时候需要缩放的物体")]
        public RectTransform btnRect;

        public UnityEvent onDoubleClick;

        public UnityEvent onPress;

        public UnityEvent onClick;

        public UnityEvent onClickUp;

        public bool isDown = false;

        public bool isUnscaleTime = false;

        private bool isPress = false;

        private float downTime = 0;

        private float clickIntervalTime = 0;

        private float clickTimes = 0;

        public float pressedScaleRate = 0.8f;

        private Vector3 pressedScale;

        private Vector3 originalScale;

        [Header("长按时多久触发一次事件")]
        [SerializeField] private float longPressDuation = 0.1f;
        [Header("长按多久开始加速（填-1为不加速），从开始长按算起（点击按钮1秒后开始计时）")]
        [SerializeField] private float pressSpeedUpTime = -1f;
        [Header("加速次数（-1为无限次加速）")]
        [SerializeField] private float speedUpTimes = -1f;
        [Header("加速速率（%）")]
        [SerializeField] private int pressAddRate = 20;
        private float checkSpeedUpTime;
        private float pressClickDuation;
        private float orilongPressDuation;
        private float orispeedUpTimes;

        protected void Awake()
        {
            btnRect.pivot = new Vector2(0.5f, 0.5f);
            originalScale = btnRect.localScale;
            pressedScale = originalScale * pressedScaleRate;

            //第一次点击或者双击应该立即响应
            clickIntervalTime = doubleClickIntervalTime;
            pressClickDuation = longPressDuation;
            orilongPressDuation = longPressDuation;
            orispeedUpTimes = speedUpTimes;
        }

        void Update()
        {
            float delta = isUnscaleTime ? Time.unscaledDeltaTime : Time.deltaTime;
            if (isDown)
            {
                if (responseOnceByPress && isPress)
                {
                    return;
                }

                downTime += delta;
                pressClickDuation += delta;

                if (downTime > pressDurationTime)
                {
                    isPress = true;
                    CheckSpeedUp(delta);
                    if (pressClickDuation >= longPressDuation)
                    {  
                        onPress.Invoke();
                        pressClickDuation = 0;
                    }

                  
                }
            }

            if (clickTimes >= 0.5)
            {
                clickIntervalTime += delta;
                if (clickIntervalTime >= doubleClickIntervalTime)
                {
                    if (clickTimes >= 2)
                    {
                        onDoubleClick.Invoke();
                    }
                    else
                    {
                        onClick.Invoke();
                    }
                    clickTimes = 0;
                    clickIntervalTime = 0;
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isDown = true;
            downTime = 0;
            checkSpeedUpTime = 0;
            btnRect.localScale = pressedScale;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isDown = false;
            isPress = false;
            btnRect.localScale = originalScale;
            clickIntervalTime = doubleClickIntervalTime;
            longPressDuation = orilongPressDuation;
            speedUpTimes = orispeedUpTimes;
            checkSpeedUpTime = 0;
            onClickUp?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // isDown = false;
            // isPress = false;
            // btnRect.localScale = originalScale;
            // clickIntervalTime = doubleClickIntervalTime;
            // longPressDuation = orilongPressDuation;
            // speedUpTimes = orispeedUpTimes;
            // checkSpeedUpTime = 0;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isPress)
                //onClick.Invoke();
                clickTimes += 0.5f;
            else
                isPress = false;
        }

        //检查长按加速
        private void CheckSpeedUp(float delta)
        {
            checkSpeedUpTime += delta;

            //达到加速条件
            if (pressSpeedUpTime > 0 && checkSpeedUpTime >= pressSpeedUpTime && speedUpTimes != 0)
            {
                longPressDuation *= (1 - pressAddRate/100f);
                speedUpTimes -= 1;
                checkSpeedUpTime = 0;
            }

        }
    }
}