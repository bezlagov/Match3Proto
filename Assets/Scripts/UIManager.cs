using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : IUiManager
{
    private Button _moveButton;
    private InputField _countsOfMove;
    private Text _score;

    //Init
    public UIManager(Transform canvas)
    {
        _moveButton = canvas.Find("MoveButton").GetComponent<Button>();
        _moveButton.onClick.AddListener(MakeMoves);
        _countsOfMove = canvas.Find("InputField").GetComponent<InputField>();
        _score = canvas.Find("Score").GetComponent<Text>();
    }

    //Button event that starts moveing blocks
    private void MakeMoves()
    {
        _moveButton.interactable = false;
        int count = 0;
        if (!String.IsNullOrEmpty(_countsOfMove.text))
        {
            count = Convert.ToInt32(_countsOfMove.text);
        }
        GameManager.instance.StartRandomMoves(count);
    }

    // Set button for interactable - true
    public void ChangeInteractable()
    {
        _moveButton.interactable = true;
    }

    // Change score layout
    public void UpdateScore(int score)
    {
        _score.text = score.ToString();
    }
}
