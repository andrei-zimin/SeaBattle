using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    //время удержания для перемещения кораблика
    private const float CLICKHOLDTIME = 0.5f;

    //изображение клетки
    public Image image;

    //кадры обновления
    private int markedFrame;

    //координаты клетки
    public int X, Y;

    //клетка занята?
    public bool isOccupied;

    //клетки кораблика
    public List<Tile> shipTiles = new List<Tile>();

    //кнопка клетки
    public Button button;

    //крестик
    public GameObject cross;

    //клетка с корабликом подбита?
    private bool isHited;

    //тип клетки
    public TileType type;

    //кораблик повёрнут вертикально?
    public bool isRotationVertical;

    //размер кораблика
    public int shipSize;

    //таймер нажатия
    private float _clicktime = float.MaxValue;

    public bool isHead;

    //отметить клетку
    public void SetMarked()
    {
        image.color = Color.red;
        markedFrame = 2;
    }

    //занять клетку
    public void SetOccupied()
    {
        isOccupied = true;

        image.color = Color.blue;
    }

    //заблокировать клетку
    public void SetBlocked()
    {
        isOccupied = true;
        image.color = Color.white;
    }

    //очистить клетку
    public void Clear()
    {
        image.color = Color.white;
        markedFrame = -1;
        shipTiles.Clear();
        cross.SetActive(false);
        isHited = false;
        isOccupied = false;
        isRotationVertical = false;
        shipSize = 0;
        isHead = false;
    }

    //установить клетки кораблика
    public void SetShipTiles(Tile[] tiles)
    {
        shipTiles.AddRange(tiles);
    }

    //установить клетки кораблика
    public void SetShipTiles(List<Tile> tiles)
    {
        shipTiles.AddRange(tiles);
    }

    //повернуть кораблик
    public void Rotate()
    {
        if (shipTiles.Count == 0)
            return;

        int size = shipTiles.Count;

        Tile shipHeadTile = GetShipHeadTile();

        int headX = shipHeadTile.X;
        int headY = shipHeadTile.Y;

        List<Tile> newShipTiles = new List<Tile>();

        if (IsRotationAvailable())
        {
            UnoccupyShipZone();

            if (!isRotationVertical)
            {
                for (int i = 0; i < size; i++)
                {
                    Tile tile = ShipPlacementScreen.field[headX, headY + i];
                    newShipTiles.Add(tile);
                }

                UnoccupyShipZone();

                List<Tile> clearList = new List<Tile>();

                clearList.AddRange(shipTiles);

                foreach (var item in clearList)
                    item.Clear();

                foreach (var item in newShipTiles)
                    item.SetShipTiles(newShipTiles);

                foreach (var item in newShipTiles)
                {
                    item.SetOccupied();
                    item.isRotationVertical = true;
                    item.shipSize = size;
                }

                OccupyShipZone();
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    Tile tile = ShipPlacementScreen.field[headX + i, headY];
                    newShipTiles.Add(tile);
                }

                UnoccupyShipZone();

                List<Tile> clearList = new List<Tile>();

                clearList.AddRange(shipTiles);

                foreach (var item in clearList)
                    item.Clear();

                foreach (var item in newShipTiles)
                    item.SetShipTiles(newShipTiles);

                foreach (var item in newShipTiles)
                {
                    item.SetOccupied();
                    item.isRotationVertical = false;
                    item.shipSize = size;
                }

                OccupyShipZone();
            }
        }
    }

    //освободить зону кораблика
    private void UnoccupyShipZone()
    {
        foreach (var item in shipTiles)
            item.UnoccupyTileZone();
    }

    //занять зону кораблика
    private void OccupyShipZone()
    {
        foreach (var item in shipTiles)
            item.OccupyTileZone();
    }

    //освободить зону клетки
    public void UnoccupyTileZone()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                if(X + i > 0 && X + i < 10 && Y + ii > 0 && Y + ii < 10)
                    ShipPlacementScreen.field[X + i, Y + ii].isOccupied = false;
            }
        }
    }

    //занять зону клетки
    public void OccupyTileZone()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                if (X + i > 0 && X + i < 10 && Y + ii > 0 && Y + ii < 10)
                    ShipPlacementScreen.field[X + i, Y + ii].isOccupied = true;
            }
        }
    }

    //дезактивировать зону кораблика
    public void InactivateShipZone()
    {
        foreach (var item in shipTiles)
            item.InactivateTileZone();
    }

    //дезактивировать зону клетки
    private void InactivateTileZone()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                if (X + i > -1 && X + i < 10 && Y + ii > -1 && Y + ii < 10)
                {
                    if (type == TileType.ENEMY)
                        BattleScreen.enemyField[X + i, Y + ii].button.interactable = false;
                    else if (type == TileType.PLAYER)
                        BattleScreen.playerField[X + i, Y + ii].button.interactable = false;
                }
            }
        }
    }

    //очистить зону корабля
    private void ClearShipZone()
    {
        List<Tile> clearList = new List<Tile>();

        clearList.AddRange(shipTiles);

        foreach (var item in clearList)
            item.ClearTileZone();
    }

    //очистить зону клетки
    private void ClearTileZone()
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                if (X + i > -1 && X + i < 10 && Y + ii > -1 && Y + ii < 10)
                {
                    ShipPlacementScreen.field[X + i, Y + ii].Clear();
                }
            }
        }
    }

    //получить зону клетки
    public List<Tile> GetTileZone()
    {
        List<Tile> result = new List<Tile>();

        for (int i = -1; i <= 1; i++)
        {
            for (int ii = -1; ii <= 1; ii++)
            {
                if (X + i > -1 && X + i < 10 && Y + ii > -1 && Y + ii < 10)
                {
                    if (type == TileType.ENEMY)
                        result.Add(BattleScreen.enemyField[X + i, Y + ii]);
                    else if (type == TileType.PLAYER)
                        result.Add(BattleScreen.playerField[X + i, Y + ii]);
                }
            }
        }

        return result;
    }

    //поворот возможен?
    private bool IsRotationAvailable()
    {
        int size = shipTiles.Count;

        Tile shipHeadTile = GetShipHeadTile();

        if (!isRotationVertical)
        {
            if (Y + size > 10)
                return false;

            int headX = shipHeadTile.X;
            int headY = shipHeadTile.Y;

            for (int i = -1; i <= 1; i++)
            {
                for (int ii = -1; ii <= size; ii++)
                {
                    if (ii == 0)
                        continue;

                    //if (headX + i > -1 && headX + i < 10 && headY + ii > -1 && headY + ii < 10)
                    //    ShipPlacementScreen.field[headX + i, headY + ii].image.color = Color.yellow;

                    if (headX + i > -1 && headX + i < 10 && headY + ii > -1 && headY + ii < 10 && ShipPlacementScreen.field[headX + i, headY + ii].shipTiles.Count > 0)
                        return false;
                }
            }

            return true;
        }
        else
        {
            if (X + size > 10)
                return false;

            int headX = shipHeadTile.X;
            int headY = shipHeadTile.Y;

            for (int i = -1; i <= size; i++)
            {
                if (i == 0)
                    continue;

                for (int ii = -1; ii <= 1; ii++)
                {
                    //if (headX + i > -1 && headX + i < 10 && headY + ii > -1 && headY + ii < 10)
                    //    ShipPlacementScreen.field[headX + i, headY + ii].image.color = Color.cyan;

                    if (headX + i > -1 && headX + i < 10 && headY + ii > -1 && headY + ii < 10 && ShipPlacementScreen.field[headX + i, headY + ii].shipTiles.Count > 0)
                        return false;
                }
            }

            return true;
        }
    }

    //получить первую клеточку кораблика
    private Tile GetShipHeadTile()
    {
        Tile result = null;

        if (!isRotationVertical)
        {
            int minPosX = int.MaxValue;

            for (int i = 0; i < shipTiles.Count; i++)
            {
                Tile tile = shipTiles[i];

                if (tile.X < minPosX)
                {
                    minPosX = tile.X;
                    result = tile;
                }
            }
        }
        else
        {
            int minPosY = int.MaxValue;

            for (int i = 0; i < shipTiles.Count; i++)
            {
                Tile tile = shipTiles[i];

                if (tile.Y < minPosY)
                {
                    minPosY = tile.Y;
                    result = tile;
                }
            }
        }
        return result;
    }

    //кораблик (x_x)?
    public bool IsShipSinked()
    {
        foreach (var item in shipTiles)
        {
            if (!item.isHited)
                return false;
        }

        return true;
    }

    //выстрелить в клетку
    public void Shot()
    {
        if (shipTiles.Contains(this))
        {
            isHited = true;
            cross.SetActive(true);
            image.color = Color.red;

            if (type == TileType.ENEMY)
                BattleScreen.instance.PlayerShot(this);

            if (IsShipSinked())
            {
                foreach (var item in shipTiles)
                {
                    item.image.color = Color.gray;
                }

                AudioSystem.instance.PlayExplosion();
            }
            else
            {
                AudioSystem.instance.PlayShot();
            }
        }
        else
        {
            if (type == TileType.ENEMY)
                BattleScreen.instance.EnemyStep();

            AudioSystem.instance.PlayWaterSplash();
        }

        button.interactable = false;
    }

    //обновление
    private void Update()
    {
        if (markedFrame == -1)
            return;

        if (markedFrame == 1 && !isOccupied)
            Clear();

        markedFrame--;
    }

    //событие: случилось нажатие
    private void OnMouseDown()
    {
        _clicktime = CLICKHOLDTIME;
    }

    //событие: указатель над клеточкой
    private void OnMouseOver()
    {
        if (type != TileType.PLACEMENT)
            return;

        if (Input.GetMouseButton(0))
        {
            if (shipTiles.Count == 0)
                return;

            if (_clicktime < 0)
            {
                Tile shipHeadTile = GetShipHeadTile();

                int size = shipHeadTile.shipSize;

                ShipElement element = null;

                foreach (var item in ShipElement.allElements)
                {
                    if (item.size == size)
                    {
                        element = item;
                        break;
                    }
                }

                element.IncrementCount();

                ShipElement.currentElement = element;

                DraggableElement.instance.Set(element.image.sprite, element.size, isRotationVertical);

                ShipPlacementScreen.instance.nextScreenButton.interactable = false;

                ClearShipZone();

                _clicktime = float.MaxValue;
            }

            _clicktime -= Time.deltaTime;
        }
    }

    //типы клеточек
    public enum TileType { PLACEMENT, PLAYER, ENEMY }
}
