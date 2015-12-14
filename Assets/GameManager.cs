﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    GameBoard _gameBoard;
    Tile_Harvest _harvestTile;

    // UI
    int _food;
    public int Food
    {
        get
        {
            return _food;
        }

        set
        {
            _food = value;
            FoodText.text = _food.ToString();

            if(_food == 0)
            {
                FoodText.color = Color.red;
            }
            else if(_food < 0)
            {
                State = GameState.GameOver;
                FoodText.text = string.Empty;
            }
            else
            {
                FoodText.color = Color.white;
            }
        }
    }
    Text FoodText;
    GameObject GameOverPanel;

    // Hand of tiles
    List<Tile> _hand;
    int _handLimit = 3;
    GameObject _handHost;
    List<Tile.TileType> _tileTypeDistribution;

    enum GameState { Starting, Waiting, Advancing, GameOver, }
    GameState _state = GameState.Starting;
    private GameState State
    {
        get
        {
            return _state;
        }

        set
        {
            if(_state != value)
            {
                switch(_state)
                {
                    case GameState.Starting:
                        break;
                    case GameState.Waiting:
                        for(int i = 0; i < _hand.Count; i++)
                        {
                            _hand[i].Locked = true;
                        }
                        break;
                    case GameState.Advancing:
                        break;
                    case GameState.GameOver:
                        GameOverPanel.SetActive(false);
                        break;
                    default:
                        break;
                }

                _state = value;

                switch(_state)
                {
                    case GameState.Starting:
                        break;
                    case GameState.Waiting:
                        for(int i = 0; i < _hand.Count; i++)
                        {
                            _hand[i].Locked = false;
                        }
                        break;
                    case GameState.Advancing:
                        break;
                    case GameState.GameOver:
                        GameOverPanel.SetActive(true);
                        ClearHand();
                        break;
                    default:
                        break;
                }
            }
        }
    }


    void Awake()
    {
        _gameBoard = Transform.FindObjectOfType<GameBoard>();
        _handHost = GameObject.Find("Hand");
        _harvestTile = _handHost.transform.GetComponentInChildren<Tile_Harvest>();
        _harvestTile.GameBoard = _gameBoard;
        _harvestTile.TileContentHarvested += _harvestTile_TileContentHarvested;

        _hand = new List<Tile>();

        FoodText = GameObject.Find("FoodLabel").GetComponent<Text>();
        GameOverPanel = GameObject.Find("GameOverPanel");
        GameOverPanel.SetActive(false);

        _tileTypeDistribution = new List<Tile.TileType>();
        for(int i = 0; i < 30; i++)
        {
            _tileTypeDistribution.Add(Tile.TileType.Ground);
        }

        for(int i = 0; i < 10; i++)
        {
            _tileTypeDistribution.Add(Tile.TileType.Water);
        }

        for(int i = 0; i < 10; i++)
        {
            _tileTypeDistribution.Add(Tile.TileType.Seed);
        }

        for(int i = 0; i < 10; i++)
        {
            _tileTypeDistribution.Add(Tile.TileType.Sun);
        }
    }


    private void _harvestTile_TileContentHarvested(TileContent tileContent)
    {
        Food += tileContent.Health;
    }


    void Start()
    {
        Food = 20;

        ResetHand();

        State = GameState.Waiting;
    }


    public void OnHandTileActivated(Tile tile)
    {
        tile.Activated -= OnHandTileActivated;
        _hand.Remove(tile);
        Advance();
    }


    void Advance()
    {
        State = GameState.Advancing;

        StartCoroutine(DoAdvance());
    }


    System.Collections.IEnumerator DoAdvance()
    {
        yield return new WaitForSeconds(0.4f);
        
        _gameBoard.Advance();

        yield return new WaitForSeconds(0.4f);

        Food--;

        if(State != GameState.GameOver)
        {
            ResetHand();

            State = GameState.Waiting;
        }
    }


    void ResetHand()
    {
        ClearHand();
        
        for(int i = 0; i < _handLimit; i++)
        {
            Tile tile = RandomTile();
            Vector3 targetPosition = _handHost.transform.GetChild(i).transform.position;
            tile.transform.position = targetPosition + Vector3.back * 10;
            tile.MoveTo(targetPosition, 0.2f * i);
            tile.transform.SetParent(_handHost.transform);
            tile.Activated += OnHandTileActivated;
            tile.Locked = false;

            _hand.Add(tile);
        }

    }


    void ClearHand()
    {
        for(int i = 0; i < _hand.Count; i++)
        {
            _hand[i].Remove();
        }

        _hand.Clear();
    }


    Tile RandomTile()
    {
        Tile tile = null;

        Tile.TileType tileType = _tileTypeDistribution[Random.Range(0, _tileTypeDistribution.Count)];

        tile = _gameBoard.MakeTile(tileType);

        TerrainTile terrainTile = tile as TerrainTile;
        if(terrainTile != null && terrainTile.Type == Tile.TileType.Ground)
        {
            terrainTile.Health = Random.Range(0, 7);
            terrainTile.CheckHealth();
        }

        return tile;
    }


    public void OnRestartClicked()
    {
        Start();
        _gameBoard.Restart();
    }
}
