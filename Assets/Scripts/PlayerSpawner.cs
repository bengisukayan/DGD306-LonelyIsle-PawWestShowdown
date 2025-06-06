using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform playerParent; // Assign your empty GameObject here

    void Start()
    {
        if (CharacterSelection.selectedCharacterPrefab != null)
        {
            Instantiate(CharacterSelection.selectedCharacterPrefab, spawnPoint.position, Quaternion.identity, playerParent);
        }
    }
}
