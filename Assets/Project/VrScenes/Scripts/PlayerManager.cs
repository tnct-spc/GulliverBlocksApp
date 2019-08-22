using UnityEngine;
using UnityEngine.XR;
using UnityEngine.EventSystems;

namespace VrScene
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] int default_move_speed = 1;
        [SerializeField] int run_move_speed = 2;
        Rigidbody player_rigidbody;
        private bool isDefault_speed = true;
        const string Stop = "Stop";
        const string Forward = "Forward";
        const string Right = "Right";
        const string Left = "Left";
        const string Back = "Back";
        const string Up = "Up";
        const string Down = "Down";

        public bool MoveRight, MoveLeft, MoveForward, MoveBack, MoveUp, MoveDown;


        RotateManager RotateManagerI;

        GameObject gamesystem;


        private void Awake()
        {
            player_rigidbody = GetComponent<Rigidbody>();
            gamesystem = GameObject.Find("GameSystem");
            var playerCamera = GameObject.Find("PlayerCamera");
            RotateManagerI = new RotateManager(playerCamera.transform, transform);
        }
        void Start()
        {
            Input.gyro.enabled = true;
        }

        void Update()
        {
            Move();
            CheckPlayerFall();
            RotateManagerI.UpdateRotate();
        }

        void Move()
        {
            Vector3 Direction = Vector3.zero;
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

            if (isDefault_speed) player_rigidbody.velocity = default_move_speed * moveDirection;
            else player_rigidbody.velocity = run_move_speed * moveDirection;
        }

        public void RefleshRotate()
        {
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
        }

        private void CheckPlayerFall()
        {
            if (transform.position.y < -1)
            {
                RespawnPlayer();
            }
        }

        class RotateManager
        {
            public Vector2 rotationSpeed = Vector2.one;

            private Transform CameraTransform;
            private Transform PlayerTransform;
            private Vector2 lastMousePosition;
            private bool TouchMoveEnable;
            private float CurrentRightLeftRotate;
            private float CurrentZRotate; //車のハンドル方向の回転
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

            public void UpdateRotate()
            {
                /* Zはハンドル方向, 端末の画面に垂直な線を軸に回転
                 * 
                 */
                PlayerTransform.rotation = Quaternion.AngleAxis(this.CurrentRightLeftRotate, Vector3.up); // Player本体は常に回転を固定する
                if (Application.platform == RuntimePlatform.Android)
                {
                    CameraTransform.rotation = Quaternion.AngleAxis(-this.CurrentZRotate, CameraTransform.forward) * CameraTransform.rotation; 
                    this.RotateXY(GyroDiff());
                    if (Input.touchCount > 0)
                    {
                        var touch = Input.GetTouch(0);
                        if (touch.phase == TouchPhase.Began)
                        {
                            this.OnClick(touch.position);
                        } else if (touch.phase == TouchPhase.Moved)
                        {
                            this.OnMove(touch.position);
                        }
                    }
                    CameraTransform.rotation = Quaternion.AngleAxis(this.CurrentZRotate, CameraTransform.forward) * CameraTransform.rotation;
                } else
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        this.OnClick(Input.mousePosition);
                    } else if (Input.GetMouseButton(0))
                    {
                        this.OnMove(Input.mousePosition);
                    }
                }
            }

            private void OnClick(Vector2 position)
            {
                var tappedObject = EventSystem.current.currentSelectedGameObject;
                this.TouchMoveEnable = tappedObject == null || tappedObject.tag == "block" ||  tappedObject.tag == "floor";

                this.lastMousePosition = position;
            }

            private void OnMove(Vector2 position)
            {
                if (!this.TouchMoveEnable) return;
                Vector2 vec = position - this.lastMousePosition;
                vec = Quaternion.Euler(0, 0, this.CurrentZRotate) * vec;
                RotateXY(new Vector3(vec.x, vec.y, 0)*10*Time.deltaTime);
                this.lastMousePosition = position;
            }

            private void RotateXY(Vector3 direction)
            {
                 // 上下方向はカメラを, 左右方向は本体ごと回す

                this.CurrentRightLeftRotate -= direction.x;
                CameraTransform.RotateAround(PlayerTransform.position, PlayerTransform.right, direction.y);
                this.CurrentZRotate += direction.z;
            }
            private Vector3 GyroDiff()
            {
                var cGyro = (Quaternion.AngleAxis(90.0f, Vector3.right) * gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward)).eulerAngles;
                //var cGyro = gyro.attitude.eulerAngles;
                var x = cGyro.x - LastGyroRotate.x;
                var y = cGyro.y - LastGyroRotate.y;
                var z = cGyro.z - LastGyroRotate.z;
                LastGyroRotate = cGyro;
                return new Vector3(-y, x, z);
            }
        }
    }
}
