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

    public bool MoveX, MoveY, MoveZ;

    private void Awake()
    {
        player_rigidbody = GetComponent<Rigidbody>();
    }
    void Start()
    {
        Input.gyro.enabled = true;
        MoveX = MoveY = MoveZ = false;
    }

    void Update ()
    {
        Rotate();
    }
    public void Add_Velocity(Vector3 move_direction)
    {
        if (MoveX == true)
        {
            //if (MoveY == false) move_direction.y = 0;
            //if (MoveZ == false) move_direction.z = 0;
            move_direction.x *= player_rigidbody.transform.right.x;
        }

        if (MoveY == true)
        {
            //if (MoveX == false) move_direction.x = 0;
            //if (MoveZ == false) move_direction.z = 0;
            move_direction.y *= player_rigidbody.transform.up.y;
        }

        if (MoveZ == true)
        {
            //if (MoveX == false) move_direction.x = 0;
            //if (MoveY == false) move_direction.y = 0;
            move_direction.z *= player_rigidbody.transform.forward.z;
        }

        move_direction.Normalize();

        Debug.Log(move_direction);

        if(isDefault_speed) player_rigidbody.velocity = default_move_speed * move_direction;
        else player_rigidbody.velocity = run_move_speed * move_direction;
    }

    public void Rotate()
    {
        transform.rotation = Quaternion.AngleAxis(90.0f, Vector3.right) * Input.gyro.attitude * Quaternion.AngleAxis(180.0f, Vector3.forward);
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
}
