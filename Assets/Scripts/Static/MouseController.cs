using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MouseController {

	static bool inMenu = true;
	public static bool InMenu { 
		get { return inMenu; }
		set 
		{ 
			inMenu = value;
			if (value) {
				CurrentCursorLockMode = CursorLockMode.None;
			}
			else if (!overriden)
			{
				CurrentCursorLockMode = gameCursorLockMode;
			}
		}
	}
	static bool overriden = false;
	public static bool Overriden { 
		get { return overriden; }
		set
		{
			overriden = value;
			if (value)
			{
				CurrentCursorLockMode = CursorLockMode.None;
			}
			else if (!inMenu)
			{
				CurrentCursorLockMode = gameCursorLockMode;
			}
		}
	}
	static CursorLockMode CurrentCursorLockMode
	{
		get 
		{ 
			return CurrentCursorLockMode; 
		}
		set
		{
			Cursor.lockState = value;
			switch (value)
			{
				case CursorLockMode.Locked:
					Cursor.visible = false;
				break;
				default:
					Cursor.visible = true;
				break;
			}
		}
	}
	static CursorLockMode gameCursorLockMode = CursorLockMode.None;

	public static void ChangeMouseLockMode (CursorLockMode changeTo)
	{
		gameCursorLockMode = changeTo;
		if (!overriden || !inMenu) CurrentCursorLockMode = gameCursorLockMode;
	}

}
