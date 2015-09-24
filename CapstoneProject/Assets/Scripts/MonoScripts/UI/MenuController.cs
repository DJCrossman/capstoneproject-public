using UnityEngine;

public abstract class MenuController : MonoBehaviour
{

	protected CanvasGroup CanvasGroup;
	protected KeyCode ToggleKey;
    protected bool Toggle;
    protected bool ToggleEnabled = false;

	private bool _enabled;

    /**************************************************
     * Event Handlers
     **************************************************/

	protected virtual void Awake ()
	{
		CanvasGroup = GetComponent<CanvasGroup> ();

		if ((CanvasGroup.interactable == true)) {
			_enabled = true;
		} else {
			_enabled = false;
		}
	}

    /**************************************************
     * Control Methods
     **************************************************/

	public virtual void Show ()
	{
		CanvasGroup.alpha = 1;
		CanvasGroup.interactable = true;
		CanvasGroup.blocksRaycasts = true;
	    _enabled = true;
	}
	
	public virtual void Hide () {
		CanvasGroup.alpha = 0;
		CanvasGroup.interactable = false;
		CanvasGroup.blocksRaycasts = false;
	    _enabled = false;
	}

    public virtual void OnGUI () {
	    if (ToggleEnabled == true) {
            if (Input.GetKeyDown(ToggleKey) == true)
            {
                Toggle = true;
            }
	    }
	}

    void FixedUpdate() {
        if (Toggle == true) {
            if (_enabled == true) {
                Hide();
            } else {
                Show();
            }

            Toggle = false;
        }
    }

    /**************************************************
     * Getters and Setters
     **************************************************/

    public bool GetActive()
    {
        return _enabled;
    }
}