using UnityEngine;
using UnityEngine.EventSystems;

public class MoveBlock : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    // Get start drag coordinate
    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.localScale = new Vector3(8f, 8f, 1);
        _startPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
    }

    // Calculating diraction on end drag
    public void OnEndDrag(PointerEventData eventData)
    {
        transform.localScale = new Vector3(7, 7, 1);
        _endPosition = eventData.position;
        var xDist = (_endPosition.x - _startPosition.x)<0? (_endPosition.x - _startPosition.x)*-1: (_endPosition.x - _startPosition.x);
        var yDist = (_endPosition.y - _startPosition.y)<0? (_endPosition.y - _startPosition.y) *-1: (_endPosition.y - _startPosition.y);
        Direction direction;
        if (xDist > yDist)
        {
            direction = _startPosition.x > _endPosition.x ? Direction.LEFT : Direction.RIGHT;
        }
        else
        {
            direction = _startPosition.y > _endPosition.y ? Direction.DOWN : Direction.UP;
        }
        GameManager.instance.MoveBlock(gameObject, direction);
    }
}
