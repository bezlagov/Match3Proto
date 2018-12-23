using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance = null;

    [SerializeField]
    private Config _config;

    [SerializeField]
    private GameObject _blockPrefab;

    private IPool _pool;
    private IGameField _field;
    private IUiManager _uIManager;

    private float _waitTime = 0.2f;
    private int _score;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitGame();
    }

    //Init
    private void InitGame()
    {
        _pool = new GamePool(_blockPrefab, _config.fieldWidth * _config.fieldHeight);
        _field = new GameField(_config, _pool);
        _uIManager = new UIManager(GameObject.Find("Canvas").transform);
    }

    //Moveing and swaping blocks 
    public void MoveBlock(GameObject block, Direction direction)
    {
        int count = 0;
        _field.SwapBlocksAction(block, direction);
        do
        {
            _field.CheckMetches();
            count = _field.GetEmptyCount();
            _score += count;
            _field.DestroyBlocks();
            _field.MoveBlocksToEmptyCell();

        } while (count != 0);
        _uIManager.UpdateScore(_score);
    }

    //Start random moves based on count
    public void StartRandomMoves(int count)
    {
        StartCoroutine(MakeMoves(count));
    }

    //Move delay
    private IEnumerator MakeMoves(int count)
    {
        int current = 0;
        while (current < count)
        {
            MoveBlock(null, (Direction)UnityEngine.Random.Range(0, 4));
            current++;
            yield return new WaitForSeconds(_waitTime);
        }
        _uIManager.ChangeInteractable();
    }
}
