using UnityEngine;

interface IGameField
{
    void SwapBlocksAction(GameObject block, Direction direction);
    void CheckMetches();
    int GetEmptyCount(bool isNull = false);
    void DestroyBlocks();
    void MoveBlocksToEmptyCell();
}