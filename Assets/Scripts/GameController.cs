using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject board;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text inputFieldErrorMessageText;

    private int n = 10;
    private int[,] grid;
    private bool playerTurn = true;
    private bool gameEnded = false;

    void Start ()
    {
        inputFieldErrorMessageText.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        if (!int.TryParse(inputField.text, out n) || n < 10 || n > 20)
        {
            inputFieldErrorMessageText.gameObject.SetActive(true);
            return;
        }
        inputFieldErrorMessageText.gameObject.SetActive(false);

        grid = new int[n, n];
        playerTurn = true;
        gameEnded = false;

        titleText.gameObject.SetActive(false);
        inputField.gameObject.SetActive(false);
        board.SetActive(true);

        GenerateBoard();
    }

    private void GenerateBoard()
    {
        foreach (Transform child in board.transform)
        {
            Destroy(child.gameObject);
        }

        GridLayoutGroup layout = board.GetComponent<GridLayoutGroup>();
        layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        layout.constraintCount = n;

        for (int x = 0; x < n; x++)
        {
            for (int y = 0; y < n; y++)
            {
                GameObject tile = Instantiate(tilePrefab, board.transform);

                Tile tileScript = tile.GetComponent<Tile>();
                tileScript.Init(x, y, this);
            }
        }
    }
    public void OnTileClicked(int x, int y, Tile tile)
    {
        if (gameEnded || !playerTurn)
            return;

        if (grid[x, y] != 0)
            return;

        grid[x, y] = 1;

        tile.SetSymbol(1);

        //if (CheckWin(1))
        //{
        //    EndGame("Gracz wygrywa!");
        //    return;
        //}

        //playerTurn = false;
        //Invoke(nameof(ComputerMove), 0.5f);
    }
}