using UnityEngine;
using Cinemachine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public CinemachineVirtualCamera virtualCamera;
    public VideoClip esatCutscene;
    public VideoClip adaCutscene;
    public VideoPlayer cutsceneVideoPlayer;

    void Start()
    {
        if (CharacterSelection.selectedCharacterPrefab != null)
        {
            GameObject player = Instantiate(CharacterSelection.selectedCharacterPrefab, spawnPoint.position, Quaternion.identity);
            virtualCamera.Follow = player.transform;

            if (SceneManager.GetActiveScene().name == "Level 2")
                AssignCutsceneVideo(CharacterSelection.selectedCharacterPrefab.name);
        }
    }

    void AssignCutsceneVideo(string characterName)
    {
        if (characterName.Contains("Esat"))
        {
            cutsceneVideoPlayer.clip = esatCutscene;
        }
        else if (characterName.Contains("Ada"))
        {
            cutsceneVideoPlayer.clip = adaCutscene;
        }
    }
}
