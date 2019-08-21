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

        private Quaternion lastTransformRotate;
        private Vector3 lastMousePosition;
        Quaternion lastGyro = Quaternion.identity;
        public Vector2 rotationSpeed = Vector2.one;

        GameObject gamesystem;
        InputManager inputManager;


        private void Awake()
        {
            player_rigidbody = GetComponent<Rigidbody>();
            gamesystem = GameObject.Find("GameSystem");
            inputManager = gamesystem.GetComponent<InputManager>();
        }
        void Start()
        {
            Input.gyro.enabled = true;
            lastTransformRotate = Quaternion.Euler(Vector3.zero);
            lastGyro = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
        }

        void Update()
        {
            RefleshRotate();
            Move();
            CheckPlayerFall();
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
            #if UNITY_EDITOR //unityEditorでのデバッグ時
            RefleshRotationByMouseDrag();
            #else
            RefleshRotationByTouch();
            //RefleshRotationByGyro();
            #endif
        }


    #if UNITY_EDITOR

        void RefleshRotationByMouseDrag()
        {

            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                var currentMousePos = Input.mousePosition;
                var deltaMousePos = currentMousePos - lastMousePosition;
                transform.Rotate(deltaMousePos.y * rotationSpeed.y, -deltaMousePos.x * rotationSpeed.x, 0);
                transform.rotation *= Quaternion.Euler(new Vector3(deltaMousePos.y * rotationSpeed.y, -deltaMousePos.x * rotationSpeed.x, 0));
                var transformVec3 = transform.rotation.eulerAngles;
                transformVec3.z = 0;
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0);
                print(transform.rotation.eulerAngles);
                lastMousePosition = currentMousePos;
            }

        }
    #else
    void RefleshRotationByGyro(){
        //var currentGyro = Input.gyro.attitude;
        var currentGyro = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
        //var deltaRotation = currentGyro * Quaternion.Inverse(lastGyro);
        //transform.rotation *= Quaternion.Inverse(deltaRotation);
        var gyroEuler = currentGyro.eulerAngles;
        //transform.rotation = currentGyro;
        transform.rotation = Quaternion.Euler(new Vector3(lastTransformRotate.eulerAngles.x + gyroEuler.x , lastTransformRotate.eulerAngles.y + gyroEuler.y, gyroEuler.z));
        //lastGyro = currentGyro;
    }

    void RefleshRotationByTouch(){
        if (Input.touchCount > 0)
        {
            if(!EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)){
                if(!inputManager.isSliderSelect){
                    var lastTransformEuler = transform.rotation.eulerAngles;
                    var deltaTouchPos = Input.GetTouch(0).deltaPosition;
                    lastTransformRotate *= Quaternion.Euler(new Vector3(deltaTouchPos.y * rotationSpeed.y, -deltaTouchPos.x*rotationSpeed.x, 0));
                }
            }
            //transform.rotation = Quaternion.Euler(new Vector3(lastTransform.rotation.eulerAngles.x , lastTransform.rotation.eulerAngles.y , lastTransformEuler.z));
            //lastTransform = Quaternion.Euler(deltaTouchPos.y * rotationSpeed.y, -deltaTouchPos.x*rotationSpeed.x, 0);
            //transform.rotation *= Quaternion.Euler(lastTransform.eulerAngles.x, lastTransform.eulerAngles.y, 0);
        }
    }
    #endif
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
    }
}
