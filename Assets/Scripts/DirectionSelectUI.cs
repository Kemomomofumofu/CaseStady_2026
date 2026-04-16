using TMPro;
using UnityEngine;

/// <summary>
/// •ûŒü‘I‘ðUI
/// </summary>
public class DirectionSelectUI : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [SerializeField] private TextMeshProUGUI upText;
    [SerializeField] private TextMeshProUGUI rightText;
    [SerializeField] private TextMeshProUGUI downText;
    [SerializeField] private TextMeshProUGUI leftText;

    [SerializeField] private Color selectedColor = Color.white;
    [SerializeField] private Color normalColor = Color.gray;

    [SerializeField] private TurnDirection currentDirection = TurnDirection.Straight;

    private void Start()
    {
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        UpdateView();
    }

    private void Update()
    {
        if (playerController == null)
        {
            return;
        }

        TurnDirection nextDirection = playerController.QueuedTurnDirection;
        if (nextDirection == currentDirection)
        {
            return;
        }

        currentDirection = nextDirection;
        UpdateView();
    }

    public void SetDirection(TurnDirection _direction)
    {
        currentDirection = _direction;
        UpdateView();
    }

    private void UpdateView()
    {
        // Straight: ã, Right: ‰E, Back: ‰º, Left: ¶
        upText.color = currentDirection == TurnDirection.Straight ? selectedColor : normalColor;
        rightText.color = currentDirection == TurnDirection.Right ? selectedColor : normalColor;
        downText.color = currentDirection == TurnDirection.Back ? selectedColor : normalColor;
        leftText.color = currentDirection == TurnDirection.Left ? selectedColor : normalColor;
    }

    public TurnDirection CurrentDirection => currentDirection;
}