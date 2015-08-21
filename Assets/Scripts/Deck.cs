using System.Collections.Generic;
using UnityEngine;


public class Deck{

	public List<Card> cards;

	public int sum = 0;
	
	public Deck()
	{
		cards = new List<Card>();
	}

	public void AddCard(Card c)
	{
		cards.Add (c);
		sum = sum + c.price;

	}
	public void CleanDeck()
	{ 
		foreach (Card card in cards) {
			Object.Destroy(card.picture);
		}
	}
}