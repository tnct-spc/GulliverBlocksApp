using UnityEngine;

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

    private void Awake()
    {
        player_rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Input.gyro.enabled = true;
    }

    void Update ()
    {
        Rotate();
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

        if(isDefault_speed) player_rigidbody.velocity = default_move_speed * moveDirection;
        else player_rigidbody.velocity = run_move_speed * moveDirection;
    }

    public void Rotate()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
        }
    }

    public void Flying(bool value)
    {
        if(value == true)
        {
            player_rigidbody.useGravity = false;
        }else{
            player_rigidbody.useGravity = true;
        }
    }

    private void RespawnPlayer()
    {
        transform.position = Vector3.zero;
    }

    private void CheckPlayerFall()
    {
        if(transform.position.y < -1){
            RespawnPlayer();
        }
    }
}
