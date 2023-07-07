using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSystem : MonoBehaviour
{
    //ссылка на объект
    public static AudioSystem instance;

    //аудиоисточники
    public AudioSource click, shot, explossion, waterSplash;

    //изображение кнопки переключения звука
    public Image buttonImage;

    //цвет состояний кнопки переключения звука
    public Color soundsActiveColor, soundsInactiveColor;

    //звук включен?
    private bool isEnabled = true;

    //список аудиоисточников
    private List<AudioSource> sources = new List<AudioSource>();

    //инициализация
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < transform.childCount; i++)
        {
            AudioSource source = transform.GetChild(i).GetComponent<AudioSource>();

            sources.Add(source);
        }
    }

    //проиграть звук клика
    public void PlayClick()
    {
        if (!isEnabled)
            return;

        click.Play();
    }

    //проиграть звук выстрела
    public void PlayShot()
    {
        if (!isEnabled)
            return;

        shot.Play();
    }

    //проиграть звук взрыва
    public void PlayExplosion()
    {
        if (!isEnabled)
            return;

        explossion.Play();
    }

    //проиграть звук промаха
    public void PlayWaterSplash()
    {
        if (!isEnabled)
            return;

        waterSplash.Play();
    }

    //переключить состояние звука
    public void SwitchSounds()
    {
        isEnabled = !isEnabled;

        if (isEnabled)
        {
            buttonImage.color = soundsActiveColor;
        }
        else
        {
            buttonImage.color = soundsInactiveColor;

            foreach (var item in sources)
                item.Stop();
        }
    }
}
