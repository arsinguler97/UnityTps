using UnityEngine;

[CreateAssetMenu(fileName = "NewArrowConfig", menuName = "Game Configs/Arrow Config")]
public class ArrowConfig : ScriptableObject
{
    public float speed = 25f;
    public float gravity = 9.81f;
    public string enemyTag = "Enemy";
    public float minSpeed = 10f;
    public float maxSpeed = 30f;
    public float maxChargeTime = 3f;
}