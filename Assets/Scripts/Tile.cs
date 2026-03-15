using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private int x;
    private int y;
    private GameController controller;

    public void Init(int x, int y, GameController controller)
    {
        this.x = x;
        this.y = y;
        this.controller = controller;

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        controller.OnTileClicked(x, y, this);
    }

    [SerializeField] private Image symbolImage;
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;

    public void SetSymbol(int player)
    {
        if (player == 1)
            symbolImage.sprite = xSprite;
        else if (player == 2)
            symbolImage.sprite = oSprite;
    }
}