using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipElement : MonoBehaviour
{
    //текущий выбранный кораблик
    public static ShipElement currentElement;

    //список всех типов корабликов
    public static List<ShipElement> allElements = new List<ShipElement>();

    //текст количества корабликов
    public Text countText;

    //количество корабликов по-умолчанию
    public int count;

    //текущее количество корабликов
    public int currentCount;

    //изображение кораблика
    public Image image;

    //размер кораблика
    public int size;

    //инициализация
    private void Awake()
    {
        allElements.Add(this);

        currentCount = count;
        countText.text = "x" + currentCount.ToString();
    }

    //событие: перенос объекта
    private void OnMouseDrag()
    {
        if (DraggableElement.currentObject != null)
            return;

        if (currentCount == 0)
            return;

        AttachToDraggableElement();
    }

    //прикрепить кораблик к курсору
    public void AttachToDraggableElement()
    {
        DraggableElement.instance.Set(image.sprite, size, false);

        currentElement = this;
    }

    //уменьшить количество корабликов
    public void DecrementCount()
    {
        currentCount--;
        countText.text = "x" + currentCount.ToString();
    }

    //увеличить количество корабликов
    public void IncrementCount()
    {
        currentCount++;
        countText.text = "x" + currentCount.ToString();
    }

    //сбросить количество корабликов
    public void ResetCount()
    {
        currentCount = count;
        countText.text = "x" + currentCount.ToString();
    }

    //все кораблики расставлены?
    public static bool IsAllShipsPlaced()
    {
        int sum = 0;

        foreach (var item in allElements)
            sum += item.currentCount;

        if (sum == 0)
            return true;
        else
            return false;
    }
}
