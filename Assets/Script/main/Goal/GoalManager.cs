using UnityEngine;
using UnityEngine.SceneManagement; // �V�[���̓ǂݍ��݂ɕK�v

public class GoalManager : MonoBehaviour
{
    // �V�[�����ړ����邽�߂̃^�[�Q�b�g�V�[����
    public string targetSceneName;

    // �v���C���[�̃��C���[�ƐڐG�����Ƃ�
    private void OnTriggerEnter2D(Collider2D other)
    {
        // "Player"���C���[�ɑ�����I�u�W�F�N�g���m�F
        if (other.CompareTag("Player"))
        {
            // �V�[���̃��[�h
            SceneManager.LoadScene(targetSceneName);
        }
    }
}
