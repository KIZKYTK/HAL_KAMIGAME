using System.Collections;
using UnityEngine;

/// <summary>
/// �����iHandle�j����  
/// �E�y�����F�����݈̂ړ��i�J�n���Ƀg�O���j  
/// �E�������F�v���C���[�t���ړ��i�J�n���Ƀg�O���j  
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    [Header("�X���C�h���x (unit/�b)")]
    public float moveSpeed = 6f;

    public bool isMoving { get; private set; } = false;   // �ړ���
    public bool playerAttached { get; private set; } = false;   // �t����

    private FastenerController rail;   // �e�t�@�X�i�[
    private Coroutine moveCo;

    void Start()
    {
        rail = GetComponentInParent<FastenerController>();
        if (rail == null)
            Debug.LogError("[Handle] �e�� FastenerController ������܂���");
    }

    /*�\�\ �y�����F�����݈̂ړ� �\�\*/
    public void StartMoveAlone()
    {
        if (isMoving || rail == null) return;

        rail.Toggle();                                // ���J�n���Ƀg�O��
        moveCo = StartCoroutine(MoveCoroutine(null)); // �v���C���[����
    }

    /*�\�\ �������F�v���C���[�t���ړ� �\�\*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving || rail == null) return;

        rail.Toggle();                                // ���J�n���Ƀg�O��
        playerAttached = true;
        moveCo = StartCoroutine(MoveCoroutine(player));
    }

    /*�\�\ ���C������ �\�\*/
    private IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;

        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();
        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        Rigidbody2D rbPlayer = null;
        if (player != null)
        {
            rbPlayer = player.GetComponent<Rigidbody2D>();
            rbPlayer.velocity = Vector2.zero;
            rbPlayer.isKinematic = true;
        }

        float t = 0f;
        float dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;
            if (rbPlayer != null) player.transform.position = pos;
            yield return null;
        }

        if (rbPlayer != null) rbPlayer.isKinematic = false;

        isMoving = false;
        playerAttached = false;
    }

#if UNITY_EDITOR
    /* �V�[���r���[�Ń��[�������� */
    void OnDrawGizmos()
    {
        if (rail == null) rail = GetComponentInParent<FastenerController>();
        if (rail == null) return;
        var (p0, p1) = rail.GetEndPoints();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p0, p1);
    }
#endif
}
