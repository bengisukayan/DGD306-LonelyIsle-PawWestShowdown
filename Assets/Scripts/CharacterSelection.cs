using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CharacterSelection : MonoBehaviour
{
    public static GameObject selectedCharacterPrefab;

    public GameObject ada;
    public GameObject esat;

    public Image adaImage;
    public Image esatImage;

    public Color highlightColor = Color.white;
    public Color normalColor = Color.gray;

    private int selectedIndex = 0; // 1 = Ada, 0 = Esat
    private bool canNavigate = true;

    void Start()
    {
        UpdateSelectionVisuals();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed || !canNavigate) return;

        Vector2 input = context.ReadValue<Vector2>();
        if (input.x > 0.5f)
        {
            selectedIndex = 1;
            StartCoroutine(NavigationCooldown());
        }
        else if (input.x < -0.5f)
        {
            selectedIndex = 0;
            StartCoroutine(NavigationCooldown());
        }

        UpdateSelectionVisuals();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (selectedIndex == 0)
            SelectCharacterA();
        else
            SelectCharacterB();
    }

    private void UpdateSelectionVisuals()
    {
        adaImage.color = selectedIndex == 1 ? highlightColor : normalColor;
        esatImage.color = selectedIndex == 0 ? highlightColor : normalColor;
    }

    private System.Collections.IEnumerator NavigationCooldown()
    {
        canNavigate = false;
        yield return new WaitForSeconds(0.2f);
        canNavigate = true;
    }

    private void SelectCharacterA()
    {
        selectedCharacterPrefab = esat;
        SceneManager.LoadScene("Tutorial");
    }

    private void SelectCharacterB()
    {
        selectedCharacterPrefab = ada;
        SceneManager.LoadScene("Tutorial");
    }
}
