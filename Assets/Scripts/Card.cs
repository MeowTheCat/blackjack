using UnityEngine;
using System.Collections.Generic;

public class Card {

	public string name;
	public int price;
	public int retail_price;
	public string url;
	public GameObject picture;

	public Card(GameObject x, int y, int yy, string z)
	{
		picture = x;
		price = y;
		retail_price = yy;
		url = z;
	}

}
