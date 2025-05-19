using System.Collections;
using UnityEngine;

/// <summary>
/// �n���h���i�����j����  
/// �E�Z�����^�������̂ǂ���ł��A�J�n���Ƀt�@�X�i�[�i����?���́j���g�O��  
/// �ErequiredKeyId �̓v���C���[���ێ����錮 ID �ƈ�v�������̂ݑ���\����ɗp����
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    /* ===== �C���X�y�N�^�ݒ� ===== */
    [Header("�X���C�h���x (unit/�b)")]
    public float moveSpeed = 6f;

    [Header("�K�v�� keyId (���̐F�Ȃ�)")]
    public string requiredKeyId = "red";         // ��: "red" / "blue" ...

    /* ===== ���s���X�e�[�^�X ===== */
    public bool isMoving { get; private set; } = false;  // �ړ����t���O

    /* ===== �Q�ƃL���b�V�� ===== */
    private FastenerController rail;                      // �e�t�@�X�i�[

    /*--------------------------------------------------*/
    void Start()
    {
        // �e�K�w���� FastenerController ���擾
        rail = GetComponentInParent<FastenerController>();
    }

    /* ---------------- �Z�����F�����݈̂ړ� ---------------- */
    public void StartMoveAlone()
    {
        if (isMoving) return;     // ���Ɉړ����Ȃ疳��
        rail.Toggle();            // �J�n���Ƀt�@�X�i�[���g�O��
        StartCoroutine(MoveCoroutine(null));   // �v���C���[����
    }

    /* ---------------- �������F�v���C���[�t���ړ� ---------------- */
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        rail.Toggle();            // �������J�n���Ƀg�O��
        StartCoroutine(MoveCoroutine(player)); // �v���C���[�t��
    }

    /* ---------------- �R�A�ړ����� ---------------- */
    private IEnumerator MoveCoroutine(GameObject player)
    {
        isMoving = true;

        // ���[�����[���[���h���W
        (Vector2 p0, Vector2 p1) = rail.GetEndPoints();

        // �����̌��݈ʒu����߂������X�^�[�g�A���������S�[���ɐݒ�
        Vector2 start = Vector2.Distance(transform.position, p0) <
                        Vector2.Distance(transform.position, p1) ? p0 : p1;
        Vector2 end = (start == p0) ? p1 : p0;

        // �v���C���[���̂��ꎞ�Œ�
        Rigidbody2D rbP = player ? player.GetComponent<Rigidbody2D>() : null;
        if (rbP)
        {
            rbP.velocity = Vector2.zero;
            rbP.isKinematic = true;
        }

        /* ���`��Ԃňړ� */
        float t = 0f;
        float dist = Vector2.Distance(start, end);
        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed / dist;  // 0��1 �̐i�s��
            Vector2 pos = Vector2.Lerp(start, end, t);
            transform.position = pos;                // �����ړ�
            if (rbP) player.transform.position = pos; // �t�����̃v���C���[��Ǐ]
            yield return null;
        }

        // �v���C���[�𕨗����A
        if (rbP) rbP.isKinematic = false;

        isMoving = false;   // �I���t���O
    }
}

