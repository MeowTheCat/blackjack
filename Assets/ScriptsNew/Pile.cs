using System.Collections.Generic;
using UnityEngine;


public class Pile{
	
	public List<int> pile;
	
	public int sum = 0;
	public int offset=0;
	
	public Pile()
	{
		pile = new List<int>();
	}
	
	public void AddNode(int c)
	{
		pile.Add (c);
		
	}

	public void RemoveNode(int c)
	{
		pile.RemoveAt (c);
		
	}

	public int Count()
	{
		return pile.Count;
	}

	public int Get(int i)
	{
		return pile [i];
	}

}