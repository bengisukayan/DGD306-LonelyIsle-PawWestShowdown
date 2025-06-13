using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.Video;
using System.Collections.Generic;
public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    public float wordSpeed = 0.02f;
    public bool playerIsClose;

    private int _index;
    private bool _canSkip = false;
    private bool _hasSceneChanged = false;

    private Coroutine typingCoroutine;

    [Header("Event After Dialogue")]
    public UnityEvent onDialogueEnd;
    public VideoPlayer cutsceneVideo;
    public List<GameObject> objectsToHideDuringCutscene;


    public void OnInteract(InputAction.CallbackContext context)
    {
        if (!context.started || !playerIsClose) return;

        if (dialoguePanel.activeInHierarchy && _canSkip)
        {
            NextLine();
        }
        else if (!dialoguePanel.activeInHierarchy)
        {
            dialoguePanel.SetActive(true);
            typingCoroutine = StartCoroutine(Typing());
        }
    }

    void Update()
    {
        if (dialoguePanel.activeInHierarchy && dialogueText.text == dialogue[_index])
        {
            _canSkip = true;
        }
    }

    void ResetDialogue()
    {
        dialogueText.text = "";
        dialoguePanel.SetActive(false);
        _index = 0;
    }

    IEnumerator Typing()
    {
        dialogueText.text = "";
        foreach (char letter in dialogue[_index])
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    void NextLine()
    {
        _canSkip = false;

        if (_index < dialogue.Length - 1)
        {
            _index++;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            typingCoroutine = StartCoroutine(Typing());
        }
        else
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            ResetDialogue();
            onDialogueEnd?.Invoke(); // Trigger post-dialogue event
        }
    }

    public void LoadNextScene()
    {
        if (_hasSceneChanged) return;
        _hasSceneChanged = true;

        if (cutsceneVideo != null)
        {
            StartCoroutine(PlayCutscene());
        }
        else
        {
            LoadSceneDirectly();
        }
    }

    private IEnumerator PlayCutscene()
    {
        HideCutsceneObjects(true);
        cutsceneVideo.Play();

        while (cutsceneVideo.frame < (long)cutsceneVideo.frameCount - 1)
            yield return null;

        LoadSceneDirectly();
    }

    private void HideCutsceneObjects(bool hide)
    {
        foreach (GameObject obj in objectsToHideDuringCutscene)
        {
            if (obj != null)
                obj.SetActive(!hide);
        }
    }
    
    private void LoadSceneDirectly()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (_hasSceneChanged) return;
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            ResetDialogue();
        }
    }
}
