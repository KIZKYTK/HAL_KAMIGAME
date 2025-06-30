using System.Collections;
using UnityEngine;

/// <summary>
/// �����iHandle�j����
/// ------------------------------------------------------------
/// 1. requiredKeyId ����Ȃ�ŏ�������^�����  
/// 2. ���C�x���g(PlayerController.OnKeyCollected) ����M����Ɖ�����Collider���  
/// 3. Solid / Ghost ��ԂŎ���� Sprite �������ؑ�  
/// 4. �������J�n���A�q�m�[�h SpriteRenderer ���v���C���[�����ֈ�x������]  
/// ------------------------------------------------------------
/// �� �\����:
///   Fastener
///     ���� HandleRoot   (BoxCollider2D + HandleController)   �� localScale = 1
///         ���� HandleVisual (SpriteRenderer)                 �� srHandle ���Q��
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public class HandleController : MonoBehaviour
{
    /*========== Inspector �ɕ\������ݒ�l ==========*/
    [Header("�X���C�h���x (unit/�b)")]
    public float moveSpeed = 6f;

    [Header("�K�v keyId (�󕶎��Ȃ献�s�v)")]
    public string requiredKeyId = "red";

    [Header("����� Sprite")]
    public Sprite solidHandleSprite;   // �t�@�X�i�[�� Solid �̎�
    public Sprite ghostHandleSprite;   // �t�@�X�i�[�� Ghost �̎�

    [Header("�������J�n���Ƀv���C���[�����։�]")]
    public bool rotateToPlayer = true;
    [Tooltip("�����摜�̊������+X�ȊO�Ȃ�p�x�␳ (��: �����=90)")]
    public float spriteForwardAngle = 0f;

    [Header("�������m�[�h (SpriteRenderer)")]
    [SerializeField] private SpriteRenderer srHandle;   // HandleVisual �� SpriteRenderer

    /*========== �����^�C�� ==========*/
    public bool isMoving { get; private set; }

    System_Fastener rail;        // �e�̃t�@�X�i�[
    Transform visualTF;          // srHandle �� Transform

    /*------------------------------------------------------------*/
    void Start()
    {
        rail = GetComponentInParent<System_Fastener>();

        // SpriteRenderer �����擾�iInspector ���ݒ莞�̕ی��j
        if (!srHandle) srHandle = GetComponentInChildren<SpriteRenderer>(true);
        if (srHandle) visualTF = srHandle.transform;

        // �����擾�Ȃ��\����Collider ����
        bool unlocked = requiredKeyId == "";
        if (srHandle) srHandle.enabled = unlocked;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = unlocked;

        UpdateHandleSprite();

        // ���C�x���g�w��
        PlayerController.OnKeyCollected += HandleKeyCollected;
    }

    void OnDestroy()
    {
        PlayerController.OnKeyCollected -= HandleKeyCollected;
    }

    /*========== ���擾�C�x���g����M ==========*/
    void HandleKeyCollected(string keyId)
    {
        if (keyId != requiredKeyId) return;

        // ���� + Collider �L����
        if (srHandle) srHandle.enabled = true;
        var col = GetComponent<Collider2D>();
        if (col) col.enabled = true;

        UpdateHandleSprite();
        Debug.Log($"[Handle {name}] ��� key={keyId}");
    }

    /*========== F�L�[���� ==========
     * PlayerController ����ȉ� API ���Ă΂��
     =================================*/

    /*--- �Z�����F�����݈̂ړ� ---*/
    public void StartMoveAlone()
    {
        if (isMoving || !srHandle.enabled) return;

        var se = rail.GetComponentInChildren<SE>();
        if (se != null) se.PlaySE();

        rail.Toggle();            // �t�@�X�i�[��Ԃ𑦐ؑ�
        UpdateHandleSprite();
        StartCoroutine(MoveCoroutine(null));
    }

    /*--- �������F�v���C���[�t���ړ� ---*/
    public void StartMoveWithPlayer(GameObject player)
    {
        if (isMoving || !srHandle.enabled) return;

        // �v���C���[�����ֈ�x������]
        if (rotateToPlayer && player && visualTF)
        {
            Vector2 dir = (player.transform.position - visualTF.position).normalized;
            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - spriteForwardAngle;
            visualTF.rotation = Quaternion.Euler(0, 0, ang);
        }

        StartCoroutine(MoveCoroutine(player)); // �g�O���͓��B��
    }

    /*========== �ړ��R���[�`�� ==========*/
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

        // ���������F���B��Ƀt�@�X�i�[��ԃg�O��
        if (player != null)
        {
            var se = rail.GetComponentInChildren<SE>(); 
            if (se != null) se.PlaySE();               

            rail.Toggle();
            UpdateHandleSprite();
        }
        isMoving = false;
    }

    /*========== Sprite �ؑ� ==========*/
    void UpdateHandleSprite()
    {
        if (!srHandle || !rail) return;

        srHandle.sprite = rail.CurrentState == System_Fastener.State.Solid
            ? solidHandleSprite
            : ghostHandleSprite;
    }
}
