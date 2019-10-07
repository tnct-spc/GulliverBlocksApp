using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

namespace VrScene
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] int default_move_speed = 1;
        [SerializeField] int run_move_speed = 2;
        Vector3 Direction;
        public Rigidbody player_rigidbody;
        public GameObject PlayerCamera;
        public GameObject TwoEyesModeCamera;
        public GameObject corner;
        public bool isDefault_speed = true;
        const string Stop = "Stop";
        const string Forward = "Forward";
        const string Right = "Right";
        const string Left = "Left";
        const string Back = "Back";
        const string Up = "Up";
        const string Down = "Down";

        public bool MoveRight, MoveLeft, MoveForward, MoveBack, MoveUp, MoveDown;

        bool isMoving = false;
        bool isDashChecking = false;

        RotateManager RotateManagerI;

        private void Awake()
        {
            RotateManagerI = new RotateManager(PlayerCamera.transform, transform);
        }

        void Start()
        {
            Input.gyro.enabled = true;
        }

        void Update()
        {
            Move();
            CheckPlayerFall();
            PlayerCamera.SetActive(!XRSettings.enabled);
            TwoEyesModeCamera.SetActive(XRSettings.enabled);
            RotateManagerI.UpdateRotate(corner.activeInHierarchy,corner.transform.position);
            if (!XRSettings.enabled)
            {
                PlayerCamera.transform.position = this.transform.position;
            }
            else
            {
                RotatePlayerInTwoEyesMode();
            }
            if (Direction == Vector3.zero)
            {
                isMoving = false;
                isDefault_speed = true;
            }
            else
            {
                isMoving = true;
                DashCheck();
            }
        }
        void RotatePlayerInTwoEyesMode()
        {
            this.transform.eulerAngles = new Vector3(0f, TwoEyesModeCamera.transform.eulerAngles.y, 0f);
            TwoEyesModeCamera.transform.position = this.transform.position;
        }

        void Move()
        {
            Direction = Vector3.zero;
            if (MoveForward) Direction.z += 1;
            if (MoveBack) Direction.z += -1;
            if (MoveRight) Direction.x += 1;
            if (MoveLeft) Direction.x += -1;
            if (MoveUp) Direction.y += 1;
            if (MoveDown) Direction.y += -1;

            Add_Velocity(Direction);
        }
        public void Add_Velocity(Vector3 moveDirection)
        {
            moveDirection.x *= player_rigidbody.transform.right.x;
            moveDirection.y *= player_rigidbody.transform.up.y;
            moveDirection.z *= player_rigidbody.transform.forward.z;

            if (MoveRight) moveDirection.z += player_rigidbody.transform.right.z;
            if (MoveLeft) moveDirection.z += player_rigidbody.transform.right.z * -1;
            if (MoveForward) moveDirection.x += player_rigidbody.transform.forward.x;
            if (MoveBack) moveDirection.x += player_rigidbody.transform.forward.x * -1;

            moveDirection.Normalize();
            if (player_rigidbody.useGravity == true) moveDirection.y = player_rigidbody.velocity.y;

            if (isDefault_speed) player_rigidbody.velocity = default_move_speed * moveDirection;
            else player_rigidbody.velocity = run_move_speed * moveDirection;
        }
        public void Flying(bool value)
        {
            if (value == true)
            {
                player_rigidbody.useGravity = false;
            }
            else
            {
                player_rigidbody.useGravity = true;
            }
        }

        private void RespawnPlayer()
        {
            transform.position = Vector3.zero;
            Vector3 pos = transform.position;
            pos.y = 1.0f;
            transform.position = pos;
        }

        private void CheckPlayerFall()
        {
            if (transform.position.y < -1)
            {
                RespawnPlayer();
            }
        }

        public void SetGyroEnable(bool f)
        {
            RotateManagerI.UseGyro = f;
	}

        public async void DashCheck()
        {
            if (isDashChecking) return;
            isDashChecking = true;
            for(int i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                if (isMoving == false)
                {
                    isDashChecking = false;
                    return;
                }
            }
            isDefault_speed = false;
            isDashChecking = false;
        }

        class RotateManager
        {
            public bool UseGyro = false;

            private Transform CameraTransform;
            private Transform PlayerTransform;
            private Vector2 lastMousePosition;
            private bool TouchMoveEnable;
            private bool isTouchPanel = false;
            private bool isTouchSecondFinger = false;
            private float CurrentRightLeftRotate;
            private float CurrentZRotate;
            private Gyroscope gyro;
            private Vector3 LastGyroRotate;

            public RotateManager(Transform cameraTransform,Transform playerTransform)
            {
                this.CameraTransform = cameraTransform;
                this.PlayerTransform = playerTransform;
                this.lastMousePosition = Vector2.zero;
                this.CurrentRightLeftRotate = 0f;
                this.CurrentZRotate = 0f;
                this.gyro = Input.gyro;
            }

            public void UpdateRotate(bool isActiveColorPanel,Vector2 CornerPosition)
            {
                /*
                 * # 各回転
                 *   X: 上下
                 *   Y: 左右
                 *   Z:ハンドル方向, 端末の画面に垂直な線を軸に回転
                 *
                 *　左右(Y)方向の回転は移動の前の基準となるのでPlayer本体を回転させてる
                 *　X,Z方向の回転はPlayerを回転させたくないのでPlayerCameraを回転させてる
                 * 
                 */
                if (Application.platform == RuntimePlatform.Android)
                {
                    CameraTransform.rotation = Quaternion.AngleAxis(-this.CurrentZRotate, CameraTransform.forward) * CameraTransform.rotation;
                    if (this.UseGyro) this.RotateXY(GyroDiff());
                    if (Input.touchCount > 0)
                    {
                        Touch[] touch = Input.touches;
                        for (int i = 0; i < touch.Length; i++)
                        {
                            if (i == 0)
                            {
                                if (touch[0].phase == TouchPhase.Began)
                                {
                                    this.OnClick(touch[0].position, CornerPosition, isActiveColorPanel);
                                }
                                else if (touch[0].phase == TouchPhase.Moved && !isTouchPanel && this.TouchMoveEnable)
                                {
                                    this.OnMove(touch[0].position);
                                }
                            }
                            else
                            {
                                if (touch[i].phase == TouchPhase.Moved)
                                {
                                    this.OnMove(touch[i].position);
                                }
                            }
                        }
                    }
                    CameraTransform.rotation = Quaternion.AngleAxis(this.CurrentZRotate, CameraTransform.forward) * CameraTransform.rotation;
                } else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        this.OnClick(Input.mousePosition,CornerPosition,isActiveColorPanel);

                    } else if (Input.GetMouseButton(0))
                    {
                        this.OnMove(Input.mousePosition);
                    }
                }
                PlayerTransform.rotation = Quaternion.AngleAxis(this.CurrentRightLeftRotate, Vector3.up); // Player本体は常に回転を固定する
            }

            private void OnClick(Vector2 position,Vector2 cornerposition,bool isActive)
            {
                /*
                 *カーソル(or タッチ)が初めてされたときの処理
                 */
                if (!isActive)
                {
                    var tappedObject = EventSystem.current.currentSelectedGameObject;
                    this.TouchMoveEnable = tappedObject == null || tappedObject.tag == "block" || tappedObject.tag == "floor";

                    this.lastMousePosition = position;

                    isTouchPanel = false;
                }
                else if (position.x < cornerposition.x || position.y > cornerposition.y)
                {
                    var tappedObject = EventSystem.current.currentSelectedGameObject;
                    this.TouchMoveEnable = tappedObject == null || tappedObject.tag == "block" || tappedObject.tag == "floor";

                    this.lastMousePosition = position;

                    isTouchPanel = false;
                }
                else
                {
                    isTouchPanel = true;
                }
            }

            private void OnMove(Vector2 position)
            {
                /*
                 *カーソル(or タッチ位置)が動いたときの処理
                 */
                Vector2 vec = position - this.lastMousePosition;
                vec = Quaternion.Euler(0, 0, this.CurrentZRotate) * vec;
                RotateXY(new Vector3(vec.x, vec.y, 0) * 10 * Time.deltaTime);
                this.lastMousePosition = position;
            }

            private void RotateXY(Vector3 direction)
            {
                this.CurrentRightLeftRotate -= direction.x;
                this.CurrentZRotate += direction.z;
                CameraTransform.RotateAround(PlayerTransform.position, PlayerTransform.right, direction.y);
            }
            private Vector3 GyroDiff()
            {
                var currentGyro = (Quaternion.AngleAxis(90.0f, Vector3.right) * gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward)).eulerAngles;
                var x = currentGyro.x - LastGyroRotate.x;
                var y = currentGyro.y - LastGyroRotate.y;
                var z = currentGyro.z - LastGyroRotate.z;
                LastGyroRotate = currentGyro;
                return new Vector3(-y, x, z);
            }
        }
    }
}
