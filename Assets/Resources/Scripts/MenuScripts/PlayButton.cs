using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void OnPress() {
        if(VariableHolder.left_weapon != 0 || VariableHolder.right_weapon != 0) {
            SceneManager.LoadScene("GameScene");
        }
    }
}
