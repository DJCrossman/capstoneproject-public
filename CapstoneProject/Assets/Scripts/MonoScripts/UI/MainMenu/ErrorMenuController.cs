using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

class ErrorMenuController : MenuController
{
    struct Log
    {
        public Guid id;
        public string message;
        public string stackTrace;
        public LogType type;
    }

    private List<string> _previouslyEnabledUI = new List<string>();
    public Text ErrorText;
    List<Log> logs = new List<Log>();

    /**************************************************
     * Error Handling Methods
     **************************************************/

    void Update()
    {
        if(logs.Count > 0)
            Show();
    }

    void OnEnable()
    {
        Application.RegisterLogCallback(HandleLog);
    }

    void OnDisable()
    {
        Application.RegisterLogCallback(null);
    }

    void HandleLog(string message, string stackTrace, LogType type)
    {
        logs.Add(new Log()
        {
            id = Guid.NewGuid(),
            message = message,
            stackTrace = stackTrace,
            type = type,
        });
    }

    /**************************************************
     * GUI Methods
     **************************************************/

    public override void Show()
    {
        // Disables all menus behind it
        var ui = this.transform.root;
        var components = ui.GetComponentsInChildren<CanvasGroup>();
        foreach (var canvas in components)
        {
            if (canvas.blocksRaycasts) {
                _previouslyEnabledUI.Add(canvas.name);
            }
            canvas.blocksRaycasts = false;
        }
        var lastLog = logs.Last();
        ErrorText.text = lastLog.message;
        logs.Remove(logs.First(l => l.id == lastLog.id));
        base.Show();
        CanvasGroup.interactable = true;
        CanvasGroup.blocksRaycasts = true;
    }

    public override void Hide()
    {
        var ui = this.transform.root;
        // Enables all menus that were disabled
        if (_previouslyEnabledUI.Count > 0) {
            foreach (var canvas in ui.GetComponentsInChildren<CanvasGroup>())
            {
                if(_previouslyEnabledUI.Exists(c => c == canvas.name))
                    canvas.blocksRaycasts = true;
            }
        }
        base.Hide();
        CanvasGroup.interactable = true;
    }

    public override void OnGUI() { }
}
