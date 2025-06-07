using UnityEngine;
using Cinemachine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        if (CharacterSelection.selectedCharacterPrefab != null)
        {
            GameObject player = Instantiate(CharacterSelection.selectedCharacterPrefab, spawnPoint.position, Quaternion.identity);
            virtualCamera.Follow = player.transform;
            player.transform.localScale = Vector3.one * 5;
        }
    }
}
