using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleScreen : MonoBehaviour
{
    //ссылка на объект
    public static BattleScreen instance;

    //поля игрока и противника
    public static Tile[,] playerField = new Tile[10, 10];
    public static Tile[,] enemyField = new Tile[10, 10];

    //ссылки на элементы экрана
    public GameObject screenRoot, resultsPanel;

    //ссылки на родительские объекты полей
    public Transform playerFieldRoot, enemyFieldRoot;

    //текст результата матча
    public Text resultsText;

    //ссылки на показатели количества оставшихся корабликов
    public Text player4xShipText, player3xShipText, player2xShipText, player1xShipText;
    public Text enemy4xShipText, enemy3xShipText, enemy2xShipText, enemy1xShipText;

    //доступные для обстрела клетки игрока
    private List<Tile> playerAvailableTiles = new List<Tile>();

    //список приоритетных для обстрела клеток
    private Stack<Tile> predictedPlayerShipTiles = new Stack<Tile>();

    //списки клеток с корабликами
    private List<Tile> playerShipsTiles = new List<Tile>();
    private List<Tile> enemyShipsTiles = new List<Tile>();

    //клетка полседнего удачного выстрела противника
    private Tile lastEnemyHitedShipTile;

    //инициализация
    private void Awake()
    {
        instance = this;

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                playerField[i, ii] = playerFieldRoot.GetChild(i + ii * 10).GetComponent<Tile>();
                playerField[i, ii].type = Tile.TileType.PLAYER;
                playerField[i, ii].X = i;
                playerField[i, ii].Y = ii;

                playerAvailableTiles.Add(playerField[i, ii]);

                enemyField[i, ii] = enemyFieldRoot.GetChild(i + ii * 10).GetComponent<Tile>();
                enemyField[i, ii].type = Tile.TileType.ENEMY;
                enemyField[i, ii].X = i;
                enemyField[i, ii].Y = ii;
            }
        }
    }

    //инициализировать поле игрока
    public void SetPlayerField(Tile[,] field)
    {
        playerAvailableTiles.Clear();

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                playerAvailableTiles.Add(playerField[i, ii]);

                if (field[i, ii].shipTiles.Count > 0)
                {
                    Tile[] shipTiles = new Tile[field[i, ii].shipTiles.Count];

                    for (int iii = 0; iii < shipTiles.Length; iii++)
                    {
                        int X = field[i, ii].shipTiles[iii].X;
                        int Y = field[i, ii].shipTiles[iii].Y;

                        Tile tile = playerField[X, Y];

                        shipTiles[iii] = tile;
                    }

                    foreach (var item in shipTiles)
                    {
                        item.SetShipTiles(shipTiles);
                        item.SetOccupied();
                    }

                    shipTiles[0].shipSize = shipTiles.Length;
                    shipTiles[0].isHead = true;
                }

                if (playerField[i, ii].shipTiles.Count > 0)
                    playerShipsTiles.Add(playerField[i, ii]);
            }
        }
    }

    //ход игрока
    public void PlayerShot(Tile tile)
    {
        if (tile.IsShipSinked())
            tile.InactivateShipZone();

        enemyShipsTiles.Remove(tile);

        UpdateShipsCount();

        if (enemyShipsTiles.Count == 0)
        {
            resultsText.text = "Победа!";
            resultsPanel.SetActive(true);
        }
    }

    //ход противника
    public void EnemyStep()
    {
        Tile tile;

        if (predictedPlayerShipTiles.Count > 0)
        {
            tile = predictedPlayerShipTiles.Pop();
        }
        else
        {
            int rand = Random.Range(0, playerAvailableTiles.Count);
            tile = playerAvailableTiles[rand];
        }

        tile.Shot();

        if (playerShipsTiles.Contains(tile))
        {
            playerShipsTiles.Remove(tile);
            playerAvailableTiles.Remove(tile);

            if (tile.IsShipSinked())
            {
                tile.InactivateShipZone();
                predictedPlayerShipTiles.Clear();

                foreach (var item in tile.GetTileZone())
                    if (playerAvailableTiles.Contains(item))
                        playerAvailableTiles.Remove(item);

                lastEnemyHitedShipTile = null;

                UpdateShipsCount();
            }
            else
            {
                int X = tile.X;
                int Y = tile.Y;

                if (lastEnemyHitedShipTile == null)
                {
                    Tile predictedTile;

                    if (X + 1 < 10)
                    {
                        predictedTile = playerField[X + 1, Y];
                        if (playerAvailableTiles.Contains(predictedTile))
                            predictedPlayerShipTiles.Push(predictedTile);
                    }

                    if (Y + 1 < 10)
                    {
                        predictedTile = playerField[X, Y + 1];
                        if (playerAvailableTiles.Contains(predictedTile))
                            predictedPlayerShipTiles.Push(predictedTile);
                    }

                    if (X - 1 > -1)
                    {
                        predictedTile = playerField[X - 1, Y];
                        if (playerAvailableTiles.Contains(predictedTile))
                            predictedPlayerShipTiles.Push(predictedTile);
                    }

                    if (Y - 1 > -1)
                    {
                        predictedTile = playerField[X, Y - 1];
                        if (playerAvailableTiles.Contains(predictedTile))
                            predictedPlayerShipTiles.Push(predictedTile);
                    }

                    lastEnemyHitedShipTile = tile;
                }
                else
                {
                    int lastX = lastEnemyHitedShipTile.X;
                    int lastY = lastEnemyHitedShipTile.Y;

                    Tile predictedTile;

                    if (lastX != X)
                    {
                        for (int i = 5; i >= 0; i--)
                        {
                            if (X + i < 10)
                            {
                                predictedTile = playerField[X + i, Y];
                                if (playerAvailableTiles.Contains(predictedTile))
                                    predictedPlayerShipTiles.Push(predictedTile);
                            }

                            if (X - i > -1)
                            {
                                predictedTile = playerField[X - i, Y];
                                if (playerAvailableTiles.Contains(predictedTile))
                                    predictedPlayerShipTiles.Push(predictedTile);
                            }
                        }
                    }
                    else if (lastY != Y)
                    {
                        for (int i = 5; i >= 0; i--)
                        {
                            if (Y + i < 10)
                            {
                                predictedTile = playerField[X, Y + i];
                                if (playerAvailableTiles.Contains(predictedTile))
                                    predictedPlayerShipTiles.Push(predictedTile);
                            }

                            if (Y - i > -1)
                            {
                                predictedTile = playerField[X, Y - i];
                                if (playerAvailableTiles.Contains(predictedTile))
                                    predictedPlayerShipTiles.Push(predictedTile);
                            }
                        }
                    }

                    lastEnemyHitedShipTile = tile;
                }
            }

            EnemyStep();
        }

        if(playerAvailableTiles.Contains(tile))
            playerAvailableTiles.Remove(tile);

        if (playerShipsTiles.Count == 0)
        {
            resultsText.text = "Поражение!";
            resultsPanel.SetActive(true);
        }
    }

    //перезапустить матч
    public void RestartLevel()
    {
        screenRoot.SetActive(false);
        resultsPanel.SetActive(false);

        foreach (var item in enemyField)
        {
            item.button.interactable = true;
            item.Clear();
        }

        foreach (var item in playerField)
        {
            item.button.interactable = true;
            item.Clear();
        }

        enemyShipsTiles.Clear();
        playerShipsTiles.Clear();

        playerAvailableTiles.Clear();

        ShipPlacementScreen.instance.Show();
        ShipPlacementScreen.instance.ClearField();

        AudioSystem.instance.PlayClick();
    }

    //открыть экран
    public void Show()
    {
        screenRoot.SetActive(true);

        ShipPlacementScreen.instance.ClearField();

        ShipPlacementScreen.instance.AutoPlacement();

        Tile[,] field = ShipPlacementScreen.field;

        for (int i = 0; i < 10; i++)
        {
            for (int ii = 0; ii < 10; ii++)
            {
                if (field[i, ii].shipTiles.Count > 0)
                {
                    Tile[] shipTiles = new Tile[field[i, ii].shipTiles.Count];

                    for (int iii = 0; iii < shipTiles.Length; iii++)
                    {
                        int X = field[i, ii].shipTiles[iii].X;
                        int Y = field[i, ii].shipTiles[iii].Y;

                        Tile tile = enemyField[X, Y];

                        shipTiles[iii] = tile;
                    }

                    foreach (var item in shipTiles)
                    {
                        item.SetShipTiles(shipTiles);
                    }

                    shipTiles[0].shipSize = shipTiles.Length;
                    shipTiles[0].isHead = true;
                }

                if (enemyField[i, ii].shipTiles.Count > 0)
                    enemyShipsTiles.Add(enemyField[i, ii]);
            }
        }

        UpdateShipsCount();
    }

    //сдаться
    public void GiveUp()
    {
        resultsText.text = "Поражение!";
        resultsPanel.SetActive(true);
        
        AudioSystem.instance.PlayClick();
    }

    //обновить значения оставшихся корабликов
    private void UpdateShipsCount()
    {
        int player4x = 0;
        int player3x = 0;
        int player2x = 0;
        int player1x = 0;

        foreach (var item in playerField)
        {
            if (!item.isHead)
                continue;

            if (item.IsShipSinked())
                continue;

            switch (item.shipSize)
            {
                case 1:
                    player1x++;
                    break;
                case 2:
                    player2x++;
                    break;
                case 3:
                    player3x++;
                    break;
                case 4:
                    player4x++;
                    break;
            }
        }

        int enemy4x = 0;
        int enemy3x = 0;
        int enemy2x = 0;
        int enemy1x = 0;

        foreach (var item in enemyField)
        {
            if (!item.isHead)
                continue;

            if (item.IsShipSinked())
                continue;

            switch (item.shipSize)
            {
                case 1:
                    enemy1x++;
                    break;
                case 2:
                    enemy2x++;
                    break;
                case 3:
                    enemy3x++;
                    break;
                case 4:
                    enemy4x++;
                    break;
            }
        }

        player4xShipText.text = "x" + player4x.ToString();
        player3xShipText.text = "x" + player3x.ToString();
        player2xShipText.text = "x" + player2x.ToString();
        player1xShipText.text = "x" + player1x.ToString();

        enemy4xShipText.text = enemy4x.ToString() + "x";
        enemy3xShipText.text = enemy3x.ToString() + "x";
        enemy2xShipText.text = enemy2x.ToString() + "x";
        enemy1xShipText.text = enemy1x.ToString() + "x";
    }
}
