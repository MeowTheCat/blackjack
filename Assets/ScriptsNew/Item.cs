using UnityEngine;
using System.Collections.Generic;

public class Item {
	
	public string name;
	public int price;
	public int retail_price;
	public string category;
	public string url;
	public string brand;
	public string image_url;
	public GameObject picture;
	
	public Item(GameObject x, int y, int yy, string z, string c,string b, string i)
	{
		picture = x;
		price = y;
		retail_price = yy;
		url = z;
		category = c;
		brand = b;
		image_url = i;
	}
	
}

