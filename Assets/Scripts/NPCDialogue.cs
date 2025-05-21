using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class NPCDialogue : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public string[] dialogue;
    public float wordSpeed = 0.02f;
    public bool playerIsClose;

    private int _index;
    private bool _canSkip = false;
    private Coroutine typingCoroutine;

    [Header("Event After Dialogue")]
    public UnityEvent onDialogueEnd;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && playerIsClose) //change keycode
        {
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

        if (dialogueText.text == dialogue[_index])
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
            onDialogueEnd?.Invoke(); // post-dialogue event
        }
    }


    public void LoadNextScene()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene set in Build Settings!");
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
        if (collision.CompareTag("Player"))
        {
            playerIsClose = false;
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            ResetDialogue();
        }
    }
}
