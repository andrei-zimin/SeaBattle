using UnityEngine;

public class MainMenu : MonoBehaviour
{
    //ссылка на родительский объект экрана
    public GameObject screenRoot;

    //играть
    public void Play()
    {
        screenRoot.SetActive(false);
        ShipPlacementScreen.instance.Show();
        AudioSystem.instance.PlayClick();
    }

    //выйти
    public void Exit()
    {
        Application.Quit();
    }
}
