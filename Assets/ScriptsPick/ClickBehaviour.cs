using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class ClickBehavior : MonoBehaviour, IPointerClickHandler {
	

	public void  OnPointerClick(PointerEventData data)
	{
		Component[] renderes;
		GamePick gp = (GamePick)GameObject.FindWithTag("GameController").GetComponent<GamePick>();

		if(gameObject.tag == "Focus")
		{

			gameObject.tag = "Untagged";
			gameObject.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
			gameObject.transform.position = gp.originalPosition ;
			renderes = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderes) {
				renderer.sortingOrder = renderer.sortingOrder - 200;
			}

			if(!gp.chosen)  gp.pick.SetActive (false);

		}
		else
		{	
			GameObject focus  = GameObject.FindWithTag("Focus");
			if(focus!=null) 
			{   
				focus.tag = "Untagged";
				focus.transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
				focus.transform.position = gp.originalPosition ;
				renderes = focus.GetComponentsInChildren<Renderer>();
				foreach (Renderer renderer in renderes) {
					renderer.sortingOrder = renderer.sortingOrder - 200;
				}

			}
			gp.originalPosition  = gameObject.transform.position; 
			gameObject.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
			gameObject.transform.position = new Vector3(0,0,0);
			renderes = GetComponentsInChildren<Renderer>();
			foreach (Renderer renderer in renderes) {
				renderer.sortingOrder = renderer.sortingOrder + 200;
			}
			gameObject.tag = "Focus";


			if(!gp.chosen) gp.pick.SetActive (true);

			
		}
	}
	
	
}
