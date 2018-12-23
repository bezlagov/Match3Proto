using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class GamePool: IPool
{
    private List<Block> _pool;

    //Init
    public GamePool(GameObject prefab, int count)
    {
        _pool = new List<Block>();
        for (int i = 0; i < count; i++)
        {
            var newBlock = new Block();
            newBlock.block = MonoBehaviour.Instantiate(prefab);
            newBlock.sprite = newBlock.block.GetComponent<SpriteRenderer>();
            _pool.Add(newBlock);
            newBlock.block.SetActive(false);
        }
    }

    //Get free block
    public Block GetBlock()
    {
        var block = _pool.LastOrDefault();
        _pool.Remove(block);
        block.IsMarkedForDestroy = false;
        return block;
    }

    //Return used block
    public void ReturnBlock(Block block)
    {
        block.block.SetActive(false);
        _pool.Add(block);
    }
}
