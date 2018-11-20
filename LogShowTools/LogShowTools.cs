using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogShowTools : MonoBehaviour {

    public Text text;
    private void Awake() {
        Application.logMessageReceived += delegate (string condition, string stackTrace, LogType type) {
            switch (type) {
                case LogType.Error:
                    text.text += string.Format("\n<color=#ff5577> {0}_____{1}_____{2}</color>", type, condition, stackTrace);
                    break;
                case LogType.Warning:
                    text.text += string.Format("\n<color=#ffff00> {0}_____{1}_____{2}</color>", type, condition, stackTrace);
                    break;
                case LogType.Log:
                    text.text += string.Format("\n<color=#ffffff> {0}_____{1}_____{2}</color>", type, condition, stackTrace);
                    break;
            }

        };
    }
}
