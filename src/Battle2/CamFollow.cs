using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform target1; // ù ��° ĳ����
    public Transform target2; // �� ��° ĳ����

    private void Start()
    {
        if (CharacterSelection.selectedCharacters.Length >= 2)
        {
            if (CharacterSelection.selectedCharacters[0] != null)
                target1 = CharacterSelection.selectedCharacters[0].transform;

            if (CharacterSelection.selectedCharacters[1] != null)
                target2 = CharacterSelection.selectedCharacters[1].transform;
        }
    }

    private void LateUpdate()
    {
        if (target1 == null || target2 == null) return;

        // �� Ÿ���� �߽��� ���
        Vector3 centerPosition = (target1.position + target2.position) / 2;

        Vector3 newPosition = transform.position;
        newPosition.x = centerPosition.x; // �߽��� X�� ����
        newPosition.y = centerPosition.y + 3f;
        newPosition.z = centerPosition.z - 6f; 

        transform.position = newPosition;
    }
}
