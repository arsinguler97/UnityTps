using UnityEngine;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI arrowCountText;
    [SerializeField] private PlayerMovement player;

    private void Update()
    {
        if (player != null && arrowCountText != null)
        {
            arrowCountText.text = $"{player.currentArrowCount} / {player.maxArrowInventory}";
        }
    }
}