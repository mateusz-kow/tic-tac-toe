using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text currentResultText;
    [SerializeField] private TMP_Text inputFieldErrorMessageText;

    private int n = 10;
    private int[,] grid;
    private bool playerTurn = true;
    private bool gameEnded = false;
    private int playerWins = 0;
    private int computerWins = 0;
    private int winner = 0;
    private Computer computer;

    private InputSystem_Actions controls;

    void Awake()
    {
        controls = new InputSystem_Actions();
    }

    void Start ()
    {
        inputFieldErrorMessageText.gameObject.SetActive(false);
        restartButton.SetActive(false);
        currentResultText.gameObject.SetActive(false);
        computer = new SmartComputer();
    }

    void OnEnable()
    {
        controls.Enable();
        controls.UI.Submit.performed += OnSubmitInput;
        controls.UI.Restart.performed += OnRestartInput;
    }

    void OnDisable()
    {
        controls.UI.Submit.performed -= OnSubmitInput;
        controls.UI.Restart.performed -= OnRestartInput;
        controls.Disable();
    }

        private void OnSubmitInput(InputAction.CallbackContext context)
    {
        if (startButton.activeSelf)
        {
            StartGame();
        }
    }

    private void OnRestartInput(InputAction.CallbackContext context)
    {
        if (restartButton.activeSelf)
        {
            RestartGame();
        }
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
        startButton.SetActive(false);
        currentResultText.gameObject.SetActive(false);
        board.SetActive(true);
        restartButton.SetActive(true);

        this.winner = 0;
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

        float cellSize = 18 * 20 / n;

        layout.cellSize = new Vector2(cellSize, cellSize);

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

    public void RestartGame() {
        inputField.text = "";
        inputFieldErrorMessageText.gameObject.SetActive(false);

        titleText.gameObject.SetActive(true);
        inputField.gameObject.SetActive(true);
        startButton.SetActive(true);

        if (this.winner == 1) {
            currentResultText.text = $"Player wins!\nPlayer {this.playerWins} : {this.computerWins} Computer";
        } else if (this.winner == 2) {
            currentResultText.text = $"Computer wins!\nPlayer {this.playerWins} : {this.computerWins} Computer";
        } else {
            currentResultText.text = $"Player {this.playerWins} : {this.computerWins} Computer";
        }
        

        currentResultText.gameObject.SetActive(true);
        board.SetActive(false);
        restartButton.SetActive(false);
    }

    public void CheckWin() {
        this.winner = DetermineWinner();

        if (this.winner != 0) {
            gameEnded = true;
            if (this.winner == 1) {
                this.playerWins++;
            } else {
                this.computerWins++;
            }
            RestartGame();
        }
    }

    public int DetermineWinner()
{
    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            int current = grid[i, j];
            if (current == 0) continue;

            if (CheckLine(i, j, 1, 0, current)) return current;
            if (CheckLine(i, j, 0, 1, current)) return current;
            if (CheckLine(i, j, 1, 1, current)) return current;
            if (CheckLine(i, j, 1, -1, current)) return current;
        }
    }
    return 0;
}

private bool CheckLine(int x, int y, int dx, int dy, int player)
{
    for (int k = 1; k < 5; k++)
    {
        int nx = x + dx * k;
        int ny = y + dy * k;

        if (nx < 0 || nx >= n || ny < 0 || ny >= n) 
            return false;

        if (grid[nx, ny] != player) 
            return false;
    }
    return true;
}

    public void OnTileClicked(int x, int y, Tile tile)
    {
        if (gameEnded || !playerTurn)
            return;

        if (grid[x, y] != 0)
            return;

        grid[x, y] = 1;
        tile.SetSymbol(1);
        playerTurn = false;

        CheckWin();

        if (gameEnded)
            return;

        var (cx, cy) = computer.Move(grid);
        int index = cx * n + cy;
        Tile computerTile = board.transform.GetChild(index).GetComponent<Tile>();

        grid[cx, cy] = 2;
        computerTile.SetSymbol(2);

        CheckWin();

        playerTurn = true;
    }
}