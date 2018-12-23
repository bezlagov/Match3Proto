using UnityEngine;

public class Config : MonoBehaviour
{
    [Range(3,10)]
    public int fieldWidth,
               fieldHeight,
               countOfColors;
}
