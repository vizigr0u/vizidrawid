using UnityEngine;
using UnityEngine.UI;

public class LoadScreenOnClick : MonoBehaviour {

    public GameScreenManager.ScreenType ScreenToLoad;

    Button m_Button;
    Text m_Text;

    public void Start()
    {
        m_Button = GetComponent<Button>();
        m_Text = GetComponentInChildren<Text>();
    }

    public void LoadSelectedScreen()
    {
        GameScreenManager.Instance.LoadNextScreen(ScreenToLoad);
        DisableButton();
    }

    private void DisableButton()
    {
        m_Button.interactable = false;
        m_Text.color = Color.grey;
    }
}
