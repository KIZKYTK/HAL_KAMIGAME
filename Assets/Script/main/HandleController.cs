using System.Collections;
using UnityEngine;

/// <summary>
/// ����萧��  
/// requiredKeyId �� �g�h (�󕶎�) �Ȃ献�s�v�^�C�v�A  
/// ����ȊO�͓��� keyId �����v���C���[����������\�B
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    [Header("�X���C�h���x (unit/�b)")]
    public float moveSpeed = 6f;

    [Header("�K�v keyId (�󕶎��Ȃ献�s�v)")]
    public string requiredKeyId = "red";

    public bool isMoving { get; private set; } = false;

    System_Fastener rail;

    void Start() => rail = GetComponentInParent<System_Fastener>();

    /*--- �Z�����F����肾���ړ� ---*/
    public void StartMoveAlone()
    {
        if (isMoving) return;
        rail.Toggle();
        StartCoroutine(MoveCoroutine(null));
    }

    /*--- �������F�v���C���[�t���ړ� ---*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        rail.Toggle();
        StartCoroutine(MoveCoroutine(player));
    }

    /*--- �R���[�`���{�� ---*/
    IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;
        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();

        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        Rigidbody2D rbP = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rbP) { rbP.velocity = Vector2.zero; rbP.isKinematic = true; }

        float t = 0f, dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;
            if (rbP) player.transform.position = pos;
            yield return null;
        }

        if (rbP) rbP.isKinematic = false;
        isMoving = false;
    }
}
