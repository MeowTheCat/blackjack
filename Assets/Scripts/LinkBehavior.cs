using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;


public class LinkBehavior : MonoBehaviour, IPointerDownHandler {
	
	
	public void OnPointerDown(PointerEventData data)
	{
		GameControl gc = (GameControl)GameObject.FindWithTag("GameController").GetComponent<GameControl>();
		GameObject parent = gameObject.transform.parent.gameObject;
		
		for(int i=0; i<gc.player.cards.Count; i++)
		{
			if(gc.player.cards[i].picture == parent) 
			{
				Application.OpenURL(gc.player.cards[i].url);
			}
		}
		for(int i=0; i<gc.computer.cards.Count; i++)
		{
			if(gc.computer.cards[i].picture == parent) 
			{
				Application.OpenURL(gc.computer.cards[i].url); 
			}
		}
		

		
	}	
}

