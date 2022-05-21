using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _tiles;
    //Dictionary<int, GameObject> _tilePool = new Dictionary<int, GameObject>();
    List<KeyValuePair<int, GameObject>> _tilePool;
    [SerializeField] Transform _tilesHolder;

    [SerializeField] UiManager _uiManager;
    [SerializeField] PlayerController _player;

    public void StartGame()
    {
        _uiManager.StartGame();
        //todo: spawnovani veci, reset vseho
        _player.ResetPlayer();
    }

    void SpawnTile()
    {
        int tileIndex = Random.Range(0, _tiles.Count);
        if (_tilePool.Any(kvp => kvp.Key == tileIndex))
        {
            // spawn from pool
            GameObject tile = _tilePool.First(kvp => kvp.Key == tileIndex).Value;
            tile.transform.position = _tilesHolder.GetChild(_tilesHolder.childCount - 1).transform.position + new Vector3(0, 0, 200);
            tile.SetActive(true);
            //resetovat coiny, asi tim ze je vsechny prjedu a zapnu
        }
        else
        {
            //instantiate
            GameObject tile = Instantiate(_tiles[tileIndex], _tilesHolder);
            tile.transform.position = _tilesHolder.GetChild(_tilesHolder.childCount - 1).transform.position + new Vector3(0, 0, 200);
            tile.GetComponent<Tile>().Id = tileIndex;
            tile.GetComponent<Tile>().MapManager = this;
        }
    }

    public void ReturnTile(GameObject tile, int id)
    {
        tile.SetActive(false);
        _tilePool.Add(new KeyValuePair<int, GameObject>(id, tile));
    }
}
