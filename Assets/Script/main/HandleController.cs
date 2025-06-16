using System.Collections;
using UnityEngine;

/// <summary>
/// ����萧��  
/// �E����v����͏]���ǂ���  
/// �E�t�@�X�i�[�̏�Ԃɍ��킹�Ď���� Sprite �������ؑ�
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(SpriteRenderer))]
public class HandleController : MonoBehaviour
{
    [Header("�X���C�h���x (unit/�b)")]
    public float moveSpeed = 6f;

    [Header("�K�v keyId (�󕶎��Ȃ献�s�v)")]
    public string requiredKeyId = "red";

    [Header("�����p Sprite")]
    public Sprite solidHandleSprite;          // ���̎�
    public Sprite ghostHandleSprite;          // ���̎�

    public bool isMoving { get; private set; } = false;

    System_Fastener rail;
    SpriteRenderer srHandle;

#if UNITY_EDITOR
    /* �ҏW���[�h�� Sprite �𑦎����f */
    void OnValidate()
    {
        if (!srHandle) srHandle = GetComponent<SpriteRenderer>();
        if (!rail) rail = GetComponentInParent<System_Fastener>();
        UpdateHandleSprite();
    }
#endif

    void Start()
    {
        rail = GetComponentInParent<System_Fastener>();
        srHandle = GetComponent<SpriteRenderer>();
        UpdateHandleSprite();                 // �������f
    }

    /*------------------ ���J API ------------------*/
    /* �Z�����F�����݈̂ړ��i���g�O���j*/
    public void StartMoveAlone()
    {
        if (isMoving) return;
        rail.Toggle();                        // �t�@�X�i�[�ؑ�
        UpdateHandleSprite();                 // �����摜���ؑ�
        StartCoroutine(MoveCoroutine(null));
    }

    /* �������F�v���C���[�t���i���B��g�O���j*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving) return;
        // �� �����ł͐ؑւ��Ȃ��i���B��j
        StartCoroutine(MoveCoroutine(player));
    }

    /*------------------ �R���[�`�� ------------------*/
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

        /* ���������͓��B��Ƀg�O�� */
        if (player != null)
        {
            rail.Toggle();
            UpdateHandleSprite();
        }

        isMoving = false;
    }

    /*------------------ �����w���p�[ ------------------*/
    void UpdateHandleSprite()
    {
        if (!srHandle || !rail) return;

        if (rail.CurrentState == System_Fastener.State.Solid && solidHandleSprite)
            srHandle.sprite = solidHandleSprite;
        else if (rail.CurrentState == System_Fastener.State.Ghost && ghostHandleSprite)
            srHandle.sprite = ghostHandleSprite;
    }
}
