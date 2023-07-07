using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DraggableElement : MonoBehaviour
{
    //ссылка на объект
    public static DraggableElement instance;

    //ссылка на камеру
    public Camera mainCamera;

    //текущий привязанный объект
    public static GameObject currentObject;

    //изображение
    public Image image;

    //ссылка на пространственный контроллер
    public RectTransform rectTransform;

    //размер кораблика
    public int currentSize;

    //повёрнут вертикально?
    public bool isRotationVertical;

    //инициализация
    private void Awake()
    {
        instance = this;
    }

    //обновление
    private void Update()
    {
        if (currentObject == null)
            return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        transform.position = new Vector3(mousePos.x, mousePos.y, 0);

        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position - new Vector3(0, -2, 0));

        List<Tile> markedTiles = null;

        int X = 0;
        int Y = 0;

        if (hit.collider != null)
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != null)
            {
                X = tile.X;
                Y = tile.Y;

                if (ShipPlacementScreen.instance.IsPlacementAvailable(X, Y, currentSize, isRotationVertical))
                {
                    markedTiles = new List<Tile>();

                    if (!isRotationVertical)
                    {
                        for (int i = 0; i < currentSize; i++)
                        {
                            ShipPlacementScreen.field[X + i, Y].SetMarked();
                            markedTiles.Add(ShipPlacementScreen.field[X + i, Y]);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < currentSize; i++)
                        {
                            ShipPlacementScreen.field[X, Y + i].SetMarked();
                            markedTiles.Add(ShipPlacementScreen.field[X, Y + i]);
                        }
                    }
                }
            }
        }

        if (!Input.GetMouseButton(0))
        {
            if (markedTiles != null)
            {
                ShipPlacementScreen.instance.PlaceShip(X, Y, currentSize, isRotationVertical);

                ShipElement.currentElement.DecrementCount();

                if (ShipElement.IsAllShipsPlaced())
                    ShipPlacementScreen.instance.nextScreenButton.interactable = true;
            }

            image.enabled = false;
            currentObject = null;
        }
    }

    //установить объект
    public void Set(Sprite sprite, int size, bool isVertical)
    {
        image.enabled = true;
        image.sprite = sprite;

        rectTransform.sizeDelta = new Vector2(32 * size, 32);

        if (!isVertical)
            transform.rotation = Quaternion.Euler(0, 0, 0);
        else
            transform.rotation = Quaternion.Euler(0, 0, -90);

        isRotationVertical = isVertical;

        currentObject = image.gameObject;

        currentSize = size;
    }
}
