using UnityEngine;

[ExecuteAlways]
public class Move_Ground : MonoBehaviour
{
    [System.Flags]
    public enum DirectionFlag
    {
        Up = 1 << 0,
        Down = 1 << 1,
        Left = 1 << 2,
        Right = 1 << 3,
    }

    [Header("ˆÚ“®Ý’è")]
    [EnumFlags]
    public DirectionFlag moveFlags = DirectionFlag.Right;
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private bool movingForward = true;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!Application.isPlaying)
        {
            // ŽÀs’†‚Å‚È‚¢‚Æ‚«‚Í startPosition ‚ðí‚ÉŒ»ÝˆÊ’u‚É
            startPosition = transform.position;
        }

        DrawRayInScene();

        if (Application.isPlaying)
        {
            MovePlatform();
        }
    }

    private void MovePlatform()
    {
        Vector2 direction = GetDirectionFromFlags();
        if (direction == Vector2.zero) return;

        Vector3 offset = direction.normalized * moveDistance;
        Vector3 target = startPosition + (movingForward ? offset : Vector3.zero);

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            movingForward = !movingForward;
        }
    }

    private Vector2 GetDirectionFromFlags()
    {
        Vector2 dir = Vector2.zero;
        if (moveFlags.HasFlag(DirectionFlag.Up)) dir += Vector2.up;
        if (moveFlags.HasFlag(DirectionFlag.Down)) dir += Vector2.down;
        if (moveFlags.HasFlag(DirectionFlag.Left)) dir += Vector2.left;
        if (moveFlags.HasFlag(DirectionFlag.Right)) dir += Vector2.right;
        return dir;
    }

    private void DrawRayInScene()
    {
        Vector2 dir = GetDirectionFromFlags();
        if (dir == Vector2.zero) return;

        Vector3 origin = transform.position;
        Vector3 rayDir = (Vector3)(dir.normalized * moveDistance);
        Debug.DrawRay(origin, rayDir, Color.green);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector2 dir = GetDirectionFromFlags();
        if (dir == Vector2.zero) return;

        Vector3 origin = transform.position;
        Vector3 end = origin + (Vector3)(dir.normalized * moveDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origin, end);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(end, 0.1f);
    }
#endif
}