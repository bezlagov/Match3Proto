interface IPool
{

    Block GetBlock();
    void ReturnBlock(Block block);
}