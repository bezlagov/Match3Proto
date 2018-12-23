using UnityEngine;

class GameField : IGameField
{
    private Block[,] _gamefield;
    private Color[] _colors;
    private IPool _pool;
    private Config _config;

    private const float BLOCK_SIZE = 1f;
    private const float GAMEFIELD_OFFSET = 3f;

    //Init
    public GameField(Config config, IPool pool)
    {
        _config = config;
        _pool = pool;

        //Generate random colors
        _colors = new Color[config.countOfColors];
        GenerateColors(config.countOfColors);

        //generate base field structure and checking colors
        _gamefield = new Block[config.fieldWidth, config.fieldHeight];
        for (int i = 0; i < config.fieldWidth; i++)
        {
            for (int j = 0; j < config.fieldHeight; j++)
            {
                _gamefield[i, j] = _pool.GetBlock();
                _gamefield[i, j].block.transform.position = new Vector3(i * BLOCK_SIZE, -j * BLOCK_SIZE);
                GetColor(_gamefield[i, j], i, j);
                _gamefield[i, j].block.SetActive(true);
            }
        }

        //Set camera to center based on blocks count
        Camera.main.transform.position = new Vector3(_gamefield[(int)config.fieldWidth / 2, (int)config.fieldHeight / 2].block.transform.position.x, _gamefield[(int)config.fieldWidth / 2, (int)config.fieldHeight / 2].block.transform.position.y, Camera.main.transform.position.z);
        Camera.main.fieldOfView = config.fieldWidth * 90 / 10;
    }

    // Random generation of colors for game
    private void GenerateColors(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Color newColor;
            do
            {
                newColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                _colors[i] = newColor;


            } while (!CheckColorIdentity(newColor, i));
        }
    }

    //Check colors for doubles
    private bool CheckColorIdentity(Color newColor, int index)
    {
        bool isUnique = true;
        for (int i = 0; i < index; i++)
        {
            if (_colors[i] == newColor)
            {
                isUnique = false;
            }
        }
        return isUnique;
    }

    //Get random color with checking near blocks
    private void GetColor(Block block, int width, int height)
    {
        bool colorSetted = false;
        int colorIndex = 0;
        do
        {
            colorIndex = Random.Range(0, _colors.Length);
            var matches = 0;

            if (height <= 1) // top border 2 blocks
            {
                if (width > 1) // right side of field
                {
                    //check color metches
                    for (int i = 1; i < 3; i++)
                    {
                        if (_gamefield[width - i, height] != null)
                        {
                            if (_gamefield[width - i, height].ColorIndex == colorIndex)
                            {
                                matches++;
                            }
                        }

                    }

                    if (matches < 2)
                    {
                        colorSetted = true;
                    }
                }
                else
                {
                    colorSetted = true;
                }
            }
            else // bottom part of the field
            {
                if (width <= 1)  // left part of the field
                {
                    //check color metches
                    for (int i = 1; i < 3; i++)
                    {
                        if (_gamefield[width, height - i].ColorIndex == colorIndex)
                        {
                            matches++;
                        }
                    }
                    if (matches < 2)
                    {
                        colorSetted = true;
                    }
                }
                else// right part of the field
                {
                    //check color metches
                    for (int i = 1; i < 3; i++)
                    {
                        if (_gamefield[width, height - i].ColorIndex == colorIndex)
                        {
                            matches++;
                        }
                    }
                    if (matches < 2)
                    {

                        matches = 0;
                        for (int i = 1; i < 3; i++)
                        {
                            if (_gamefield[width - i, height].ColorIndex == colorIndex)
                            {
                                matches++;
                            }
                        }
                        if (matches < 2)
                        {
                            colorSetted = true;
                        }
                    }
                }
            }
            if (colorSetted)
            {
                block.ColorIndex = colorIndex;
                block.sprite.color = _colors[colorIndex];
                break;
            }
        }
        while (!colorSetted);
    }

    //Check position of block that was swaped
    public void SwapBlocksAction(GameObject block, Direction direction)
    {
        int x = 0, y = 0;
        //if we do automatic swap
        if (block == null)
        {
            block = _gamefield[Random.Range(0, _config.fieldWidth), Random.Range(0, _config.fieldHeight)].block;
        }

        //Searching block in array
        for (int i = 0; i < _config.fieldWidth; i++)
        {
            for (int j = 0; j < _config.fieldHeight; j++)
            {
                if (_gamefield[i, j].block == block)
                {
                    x = i;
                    y = j;
                }
            }
        }

        //Cut off directions that can't be swaped
        if (x < 1 && direction == Direction.LEFT || x > _config.fieldWidth - 2 && direction == Direction.RIGHT || y < 1 && direction == Direction.UP || y > _config.fieldHeight - 2 && direction == Direction.DOWN)
        {
            return;
        }

        //Choose move direction for swapping of blocks
        switch (direction)
        {
            case Direction.UP:
                SwapBlocks(x, y, false, -1);
                break;
            case Direction.DOWN:
                SwapBlocks(x, y, false);
                break;
            case Direction.LEFT:
                SwapBlocks(x, y, true, -1);
                break;
            case Direction.RIGHT:
                SwapBlocks(x, y, true);
                break;
            default:
                break;
        }
    }

    //Change blocks position and indexes in array
    private void SwapBlocks(int x, int y, bool isHorizontal, int modificator = 1)
    {

        if (isHorizontal) // Left/right
        {
            // Modificator 1 - right / -1 - left
            Block cache = _gamefield[x + 1 * modificator, y];
            _gamefield[x + 1 * modificator, y] = _gamefield[x, y];
            if (_gamefield[x + 1 * modificator, y] != null)
            {
                _gamefield[x + 1 * modificator, y].block.transform.position = new Vector3((x + 1 * modificator) * BLOCK_SIZE, -y * BLOCK_SIZE);
            }
            _gamefield[x, y] = cache;
            if (_gamefield[x, y] != null)
            {
                _gamefield[x, y].block.transform.position = new Vector3(x * BLOCK_SIZE, -y * BLOCK_SIZE);
            }
        }
        else// Top/Bottom
        {
            // Modificator 1 - down / -1 - up
            Block cache = _gamefield[x, y + 1 * modificator];

            _gamefield[x, y + 1 * modificator] = _gamefield[x, y];
            if (_gamefield[x, y + 1 * modificator] != null)
            {
                _gamefield[x, y + 1 * modificator].block.transform.position = new Vector3(x * BLOCK_SIZE, -(y + 1 * modificator) * BLOCK_SIZE);
            }
            _gamefield[x, y] = cache;
            if (_gamefield[x, y] != null)
            {
                _gamefield[x, y].block.transform.position = new Vector3(x * BLOCK_SIZE, -y * BLOCK_SIZE);
            }
        }
    }

    // Check neighbors for matches and marking for destroy
    public void CheckMetches()
    {
        for (int i = 0; i < _config.fieldWidth; i++)
        {
            for (int j = 0; j < _config.fieldHeight; j++)
            {
                if (i >= _config.fieldWidth - 2 && j < _config.fieldHeight - 2) // Cheking all field except last 2 lines and bottom 2 lines
                {
                    if (_gamefield[i, j].ColorIndex == _gamefield[i, j + 1].ColorIndex && (_gamefield[i, j].ColorIndex == _gamefield[i, j + 2].ColorIndex))
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            _gamefield[i, j + k].IsMarkedForDestroy = true;
                        }
                    }
                }
                else if (j >= _config.fieldHeight - 2 && i < _config.fieldWidth - 2) // Check other lines
                {
                    if (_gamefield[i, j].ColorIndex == _gamefield[i + 1, j].ColorIndex && _gamefield[i, j].ColorIndex == _gamefield[i + 2, j].ColorIndex)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            _gamefield[i + k, j].IsMarkedForDestroy = true;
                        }
                    }
                }
                else// Check other lines
                {
                    if (i > _config.fieldWidth - 3 && j > _config.fieldHeight - 3)
                    {
                        continue;
                    }

                    if (_gamefield[i, j].ColorIndex == _gamefield[i + 1, j].ColorIndex && _gamefield[i, j].ColorIndex == _gamefield[i + 2, j].ColorIndex)
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            _gamefield[i + k, j].IsMarkedForDestroy = true;
                        }
                    }
                    if (_gamefield[i, j].ColorIndex == _gamefield[i, j + 1].ColorIndex && (_gamefield[i, j].ColorIndex == _gamefield[i, j + 2].ColorIndex))
                    {
                        for (int k = 0; k < 3; k++)
                        {
                            _gamefield[i, j + k].IsMarkedForDestroy = true;
                        }
                    }
                }
            }
        }
    }

    //Counting null blocks or setted as mark for destroy
    public int GetEmptyCount(bool isNull = false)
    {
        int count = 0;
        for (int i = 0; i < _config.fieldWidth; i++)
        {
            for (int j = 0; j < _config.fieldHeight; j++)
            {
                if (!isNull)
                {
                    if (_gamefield[i, j].IsMarkedForDestroy)
                    {
                        count++;
                    }
                }
                else
                {
                    if (_gamefield[i, j] == null)
                    {
                        count++;
                    }
                }
            }
        }
        return count;
    }

    //Returning blocks to pool and setting null on gamefield
    public void DestroyBlocks()
    {
        for (int i = 0; i < _config.fieldWidth; i++)
        {
            for (int j = 0; j < _config.fieldHeight; j++)
            {
                if (_gamefield[i, j].IsMarkedForDestroy)
                {
                    _pool.ReturnBlock(_gamefield[i, j]);
                    _gamefield[i, j] = null;
                }
            }
        }
    }
    
    //Swap blocks to empty cells
    public void MoveBlocksToEmptyCell()
    {
        int emptyCount = 0;
        do
        {
            for (int i = 0; i < _config.fieldWidth; i++)
            {
                for (int j = 0; j < _config.fieldHeight; j++)
                {
                    if (_gamefield[i, j] == null)
                    {
                        if (j == 0) //  If top block  - getting new block from pool
                        {
                            _gamefield[i, j] = _pool.GetBlock();
                            _gamefield[i, j].block.transform.position = new Vector3(i * BLOCK_SIZE, j * BLOCK_SIZE);
                            GetColor(_gamefield[i, j], i, j);
                            _gamefield[i, j].block.SetActive(true);
                        }
                        else // Swaping blocks
                        {
                            SwapBlocks(i, j, false,-1);
                        }
                    }
                }
            }

            emptyCount = GetEmptyCount(true);
        } while (emptyCount != 0);
    }
}

