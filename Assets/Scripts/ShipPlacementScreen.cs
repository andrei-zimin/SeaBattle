using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipPlacementScreen : MonoBehaviour
{
    //ссылка на объект
    public static ShipPlacementScreen instance;

    //поле для размещения
    public static Tile[,] field = new Tile[10, 10];

    //ссылка на родительский объект экрана
    public GameObject screenRoot;

    //ссылка на родительский объект поля
    public Transform fieldRoot;

    //кнопка перехода на следующий экран
    public Button nextScreenButton;

    //список доступных для размещения клеток
    private List<Tile> availableTiles = new List<Tile>();

    //инициализация
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                field[i, ii] = fieldRoot.GetChild(i + ii * 10).GetComponent<Tile>();
                field[i, ii].type = Tile.TileType.PLACEMENT;
                field[i, ii].X = i;
                field[i, ii].Y = ii;
            }
        }
    }

    //открыть экран
    public void Show()
    {
        screenRoot.SetActive(true);
    }

    //открыть экран боя
    public void OpenBattleScreen()
    {
        screenRoot.SetActive(false);
        BattleScreen.instance.SetPlayerField(field);
        BattleScreen.instance.Show();

        AudioSystem.instance.PlayClick();
    }

    //разместить кораблик
    public void PlaceShip(int X, int Y, int size, bool isVertical)
    {
        Tile[] tiles = new Tile[size];

        if (!isVertical)
        {
            for (int i = 0; i < size; i++)
                tiles[i] = field[X + i, Y];
        }
        else
        {
            for (int i = 0; i < size; i++)
                tiles[i] = field[X, Y + i];
        }

        for (int i = 0; i < size; i++)
        {
            tiles[i].SetShipTiles(tiles);
            tiles[i].OccupyTileZone();
            tiles[i].SetOccupied();
            tiles[i].isRotationVertical = isVertical;
        }

        tiles[0].shipSize = size;
        tiles[0].isHead = true;
    }

    //авторазмещение
    public void AutoPlacement()
    {
        for (int i = ShipElement.allElements.Count - 1; i >= 0; i--) {

            ShipElement element = ShipElement.allElements[i];

            availableTiles.Clear();

            foreach (var item in field)
                availableTiles.Add(item);

            while (element.currentCount > 0 && availableTiles.Count > 0)
            {
                int rand = Random.Range(0, availableTiles.Count);

                Tile tile = availableTiles[rand];

                if (IsPlacementAvailable(tile.X, tile.Y, element.size, false))
                {
                    PlaceShip(tile.X, tile.Y, element.size, false);
                    element.DecrementCount();
                }

                availableTiles.Remove(tile);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                field[i, ii].Rotate();
            }
        }

        if (!ShipElement.IsAllShipsPlaced())
        {
            ClearField();
            AutoPlacement();
        }
            
        nextScreenButton.interactable = true;
    }

    //очистить поле
    public void ClearField()
    {
        foreach (var item in field)
            item.Clear();

        foreach (var item in ShipElement.allElements)
            item.ResetCount();

        nextScreenButton.interactable = false;
    }

    //авторазмещение (кнопка)
    public void AutoPlacementClick()
    {
        if (ShipElement.IsAllShipsPlaced())
            ClearField();

        AutoPlacement();

        AudioSystem.instance.PlayClick();
    }

    //очистить поле (кнопка)
    public void ClearFieldClick()
    {
        ClearField();

        AudioSystem.instance.PlayClick();
    }

    //размещение возможно?
    public bool IsPlacementAvailable(int X, int Y, int size, bool isVertical)
    {
        if (!isVertical)
        {
            if (X + size > 10)
                return false;

            for (int i = -1; i <= size; i++)
            {
                for (int ii = -1; ii <= 1; ii++)
                {
                    if (X + i > -1 && X + i < 10&& Y + ii > -1 && Y + ii < 10 && field[X + i, Y + ii].shipTiles.Count > 0)
                        return false;
                }
            }

            return true;
        }
        else
        {
            if (Y + size > 10)
                return false;

            for (int i = -1; i <= 1; i++)
            {
                for (int ii = -1; ii <= size; ii++)
                {
                    if (X + i > -1 && Y + ii > -1 && Y + ii < 10 && X + i < 10 && field[X + i, Y + ii].shipTiles.Count > 0)
                        return false;
                }
            }

            return true;
        }
    }
}
