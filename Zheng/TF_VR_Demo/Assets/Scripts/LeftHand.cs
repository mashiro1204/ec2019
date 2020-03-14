using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftHand : MonoBehaviour
{
    public RectTransform inkCircle;
    public Text leftEnemiesText;
    // Start is called before the first frame update
    void Start()
    {
        SetLeftEnemiesText(GameObject.Find("GameManager").GetComponent<GameManager>().GetLevel() + 3);
    }

    // Update is called once per frame
    void Update()
    {
        inkCircle.Rotate(0, 0, Time.deltaTime * -130f);
    }

    public void SetLeftEnemiesText(int leftEnemiesNum)
    {
        leftEnemiesText.text ="Left:" + leftEnemiesNum + "";
    }
}
