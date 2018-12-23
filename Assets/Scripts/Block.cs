using UnityEngine;

class Block
{
    public int ColorIndex { get; set; }
    public bool IsMarkedForDestroy { get; set; }
    public GameObject block;
    public SpriteRenderer sprite;
}
