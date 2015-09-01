using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;
//using System.Linq;

using MiniJSON;


public class GameControl : MonoBehaviour {

	public GameObject current;
	public SpriteRenderer renderer ;
	public Sprite sprite;
	public Texture2D texture;
	public float screenWidthUnit, screenHeightUnit;
	public Deck player, computer;
	public GameObject hit, stand, deal, next, playerSum,computerSum,playerBust,computerBust,brand,priceTag,link,email,emailInput,emailSubmit,emailCancel,
	downLeft,downRight,upLeft,upRight,winParticle,resultSummary,bet,coinCnt,betCnt,sizeMenu,intro,connection;
	public Vector3 destination;
	public float pixelsToUnits;
	public int downOffset,upOffset;
	public int goal,stop;
	public int betCoin,totalCoin;
	public Texture anime;
	public Texture[] winPictureArray,losePictureArray;
	public int delay;
	public GameObject tagModel,linkModel,emailModel;
	public int result ;
	public AudioSource dealSound,cardSound,winSound,loseSound,bonusSound,chipSound,meowSound;
	public WWW www1,www2;
	public bool fadeDone;
	public Dictionary<string, int> size;
	public string bonusDate;

	private float fingerStartTime ;
	private Vector2 fingerStartPos;
	
	private bool isSwipe = false;
	private float minSwipeDist  = 50.0f;
	private float maxSwipeTime = 0.5f;

	
	void Awake()
	{
		screenHeightUnit = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera>().orthographicSize * 2.0f; 
		screenWidthUnit = screenHeightUnit * Screen.width/Screen.height ;
		hit = GameObject.Find("Hit");
		stand = GameObject.Find("Stand");
		deal = GameObject.Find("Deal");
		next = GameObject.Find("Next");
		intro = GameObject.Find ("Intro");
		connection = GameObject.Find ("Connection");

		hit.SetActive(false);
		stand.SetActive(false);
		next.SetActive (false);
		connection.SetActive(false);
		
		AudioSource[] audios = gameObject.GetComponents<AudioSource>();
		dealSound = audios[0];
		cardSound = audios[1];
		winSound = audios[2];
		meowSound = audios[3];
		bonusSound = audios[4];
		chipSound = audios [5];
		loseSound = audios [6];

		playerSum = GameObject.Find("PlayerSum");
		computerSum = GameObject.Find("ComputerSum");
		playerBust =  GameObject.Find("PlayerBust");
		computerBust =  GameObject.Find("ComputerBust");
	

		playerSum.SetActive (false);
		computerSum.SetActive (false);
		playerBust.SetActive (false);
		computerBust.SetActive (false);

		emailInput = GameObject.Find("EmailInput");	
		emailSubmit = GameObject.Find("EmailSubmit");	
		emailCancel = GameObject.Find("EmailCancel");	

		emailInput.SetActive (false);
		emailSubmit.SetActive (false);
		emailCancel.SetActive (false);

		brand = GameObject.Find ("Brand");
		brand.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, 720f/1.5f*0.85f);
	    brand.SetActive (false);

		downLeft = GameObject.Find ("DownLeft");
		downLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, -256f );
		downLeft.SetActive (false);

		downRight = GameObject.Find ("DownRight");
		downRight.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, -256f );
		downRight.SetActive (false);

		upLeft = GameObject.Find ("UpLeft");
		upLeft.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, 256f );
		upLeft.SetActive (false);

		upRight = GameObject.Find ("UpRight");
		upRight.GetComponent<RectTransform>().anchoredPosition = new Vector2 (0f, 256f );
		upRight.SetActive (false);

		winParticle = GameObject.Find ("WinParticle");
		resultSummary = GameObject.Find ("ResultSummary");
		resultSummary.SetActive (false);

		bet = GameObject.Find ("Bet");
			
		totalCoin = PlayerPrefs.GetInt("coin", 100000);
		coinCnt = GameObject.Find ("CoinCnt");
		coinCnt.GetComponent<Text>().text = totalCoin.ToString();

		betCnt = GameObject.Find ("BetCnt");
		if (totalCoin > 100)
			betCoin = 100;
		else
			betCoin = totalCoin;
		betCnt.GetComponent<Text>().text = betCoin.ToString();

		sizeMenu = GameObject.Find ("SizeMenu");

		size= new Dictionary<string, int>();
		size.Add( "XS", PlayerPrefs.GetInt("size_XS", 0)); 
		size.Add( "S", PlayerPrefs.GetInt("size_S", 0)); 
		size.Add( "M", PlayerPrefs.GetInt("size_M", 0)); 
		size.Add( "L", PlayerPrefs.GetInt("size_L", 0)); 
		size.Add( "XL", PlayerPrefs.GetInt("size_XL", 0)); 
		foreach (KeyValuePair<string, int> pair in size)
		{
			//Debug.Log (pair.Key);
			if (pair.Value == 0) GameObject.Find(pair.Key).GetComponent<Image>().sprite =  Resources.Load<Sprite>("button_gray");
			//Debug.Log (pair.Key);
		}
		if(size["XS"] * size["S"] * size["M"]* size["L"] * size["XL"]== 0 && (size["XS"] + size["S"] + size["M"] + size["L"] + size["XL"]) > 0  ) sizeMenu.SetActive (false);
	}


	void Start()
	{
		bonusDate = PlayerPrefs.GetString("bonusdate", "");
		if (bonusDate != System.DateTime.Now.ToString("MM/dd/yyyy"))  StartCoroutine(DailyBonus());
	}

	void Update () {
		
		if (result != 0 && Input.touchCount > 0){
			
			foreach (Touch touch in Input.touches)
			{
				switch (touch.phase)
				{
				case TouchPhase.Began :
					/* this is a new touch */
					isSwipe = true;
					fingerStartTime = Time.time;
					fingerStartPos = touch.position;
					break;
					
				case TouchPhase.Canceled :
					/* The touch is being canceled */
					isSwipe = false;
					break;
					
				case TouchPhase.Ended :
					
					float gestureTime = Time.time - fingerStartTime;
					float gestureDist = (touch.position - fingerStartPos).magnitude;
					
					if (isSwipe && gestureTime < maxSwipeTime && gestureDist > minSwipeDist){
						Vector2 direction = touch.position - fingerStartPos;
						if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
						{
							if(fingerStartPos.y <= 0.45f * Screen.height && fingerStartPos.y >= 0.15f *  Screen.height) 
							{
								DownOffset(Mathf.Sign(direction.x)>0);
								if (gestureDist > 2*minSwipeDist) DownOffset(Mathf.Sign(direction.x)>0);
							}
							if(fingerStartPos.y >= 0.55f * Screen.height && fingerStartPos.y <= 0.85f *  Screen.height) 
							{
								UpOffset(Mathf.Sign(direction.x)>0);
								if (gestureDist > 2*minSwipeDist) UpOffset(Mathf.Sign(direction.x)>0);
							}
						}					
					}
					isSwipe = false;
					break;
				}
			}
		}
	}
	
	public void ToggleMenu()
	{
		if (sizeMenu.activeSelf)
			sizeMenu.SetActive (false);
		else sizeMenu.SetActive (true);
	}
	
	public void ChangeSize(string sz)
	{
		if (size [sz] == 1) {
			GameObject.Find (sz).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("button_gray");
		}
		if (size [sz] == 0) {
			GameObject.Find (sz).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("button_yellow");
		}
		size [sz] = Mathf.Abs (size [sz] - 1);
		PlayerPrefs.SetInt("size_"+sz, size [sz] );
	}

	public void HalfBet()
	{
		chipSound.Play ();
		if ((betCoin / 2f) >= 1000)
			betCoin = Mathf.FloorToInt (betCoin / 2000f) * 1000;
		else if ((betCoin / 2f) >= 100)
			betCoin = Mathf.FloorToInt (betCoin / 200f) * 100;
		else if ((betCoin / 2f) >= 10)
			betCoin = Mathf.FloorToInt (betCoin / 20f) * 10;
		else if ((betCoin / 2f) >= 1)
			betCoin = Mathf.FloorToInt (betCoin / 2f);
		betCnt.GetComponent<Text>().text = betCoin.ToString();

	}

	public void DoubleBet()
	{
		chipSound.Play ();
		if (betCoin * 2 >= totalCoin || betCoin * 2 >= 99999 )
			betCoin = Mathf.Min(totalCoin, 99999);
		else if ((betCoin * 2) >= 1000)
			betCoin = Mathf.CeilToInt (betCoin / 500f) * 1000;
		else if ((betCoin * 2) >= 100)
			betCoin = Mathf.CeilToInt (betCoin / 50f) * 100;
		else if ((betCoin * 2) >= 10)
			betCoin = Mathf.CeilToInt (betCoin / 5f) * 10;
		else
			betCoin = betCoin * 2;
		betCnt.GetComponent<Text>().text = betCoin.ToString();
		
	}

	public void OnDeal()
	{

		StartCoroutine(LoadItem(-2));


		sizeMenu.SetActive (false);
		deal.SetActive(false);
		dealSound.Play ();
		bet.SetActive (false);
		downLeft.SetActive (false);
		downRight.SetActive (false);
		upLeft.SetActive (false);
		upRight.SetActive (false);

		intro.SetActive(false);
		connection.SetActive(false);

		result = 0;
		delay = 0;
		downOffset = 0; 
		upOffset = 0;

		playerSum.SetActive(true);
		computerSum.SetActive (true);
		
		playerSum.GetComponentInChildren<Text>().text = "$0";
		computerSum.GetComponentInChildren<Text>().text = "$0";
		
		playerSum.SetActive(false);
		computerSum.SetActive (false);
		
		playerBust.SetActive (false);
		computerBust.SetActive (false);
		
		if (player != null) {
			player.CleanDeck ();
		}
		if (computer != null) {
			computer.CleanDeck ();
		}
		
		player = new Deck();
		computer = new Deck();	

		Resources.UnloadUnusedAssets();
		//GC.Collect();

	}
	

	IEnumerator  LoadItem(int n) 
	{
		float time1 = Time.time;
		string path;

		string s = "";
		foreach (KeyValuePair<string, int> pair in size)
		{
			if (pair.Value == 1) s = s+ "*"+pair.Key;
		}

		path = "http://i1hm3pvto9.execute-api.us-east-1.amazonaws.com/prod/GetAItem?size="+s+"&nocache=" + Random.value.ToString();
		//Debug.Log (path);

		do {
			www1 = new WWW (path);
			if((Time.time - time1) > 3) connection.SetActive(true);
			yield return www1; 

		} while(!www1.text.Contains("http"));
		connection.SetActive(false);

		yield return www1; 
		var dict = Json.Deserialize(www1.text) as Dictionary<string,object>;
		float time3 = Time.time;
		//Debug.Log("stage 1 :" + (time3 - time1).ToString());

		www2 = new WWW(dict["image"].ToString());
		yield return www2;
		Resources.UnloadUnusedAssets();

		float time2 = Time.time;
		float timeDiff = time2 - time1;
		//Debug.Log("stage 2 :" + (time2 - time3) .ToString());
		//Debug.Log (dict ["image"].ToString ());
		if(timeDiff < n) {yield return new WaitForSeconds(n-timeDiff);}

		switch (n)
		{
		    case -3:  
		    {
				while(!fadeDone) {yield return null;}
				AfterHit();
				return true;
			}
		    case -2:  { StartCoroutine(PlayerHand());return true;}
			case 2: 
			default: {return true;}
		}
	}

	IEnumerator Fade(Vector3 destination)
	{
		while (current.transform.position != destination ) {
			current.transform.localScale = Vector3.Lerp (current.transform.localScale, new Vector3(0.375f,0.375f,0.375f), 0.4f);
			current.transform.position = Vector3.Lerp (current.transform.position, destination, 0.4f);
			yield return null;
		}
		current.GetComponent<SpriteRenderer> ().sortingOrder = 0;
		priceTag.GetComponent<Renderer> ().sortingOrder = 0;
		link.GetComponent<Renderer> ().sortingOrder = 1;
		email.GetComponent<Renderer> ().sortingOrder = 1;
		current.AddComponent<BoxCollider2D>() ;
		current.AddComponent<CardBehavior>() ;
		fadeDone = true;
	}

	public IEnumerator Transition(GameObject obj, string str1, string str2) 
	{
		obj.GetComponent<TextMesh> ().text = "";
		obj.GetComponent<TextMesh> ().text = "$" + str1;
		yield return new WaitForSeconds (0.6f);
		while (obj.transform.localScale.x > 0.001) 
		{
			obj.transform.localScale = Vector3.Lerp (obj.transform.localScale, new Vector3(0.0001f, obj.transform.localScale.y,obj.transform.localScale.z), 0.3f);
			yield return null ;
		}
		obj.GetComponent<TextMesh> ().text = "<color=#ff0000ff>$" + str2  +"</color>"; 
		while (obj.transform.localScale.x < 0.0399 ) 
		{
			obj.transform.localScale = Vector3.Lerp (obj.transform.localScale, new Vector3(0.04f, obj.transform.localScale.y,obj.transform.localScale.z), 0.3f);
			yield return null;
		}
	}

   public void ShowCard()
	{   
		var dict = Json.Deserialize(www1.text) as Dictionary<string,object>;	
		cardSound.Play ();
		
		current = new GameObject();
		renderer =  current.AddComponent<SpriteRenderer>();
		renderer.sortingOrder = 100;	
		
		sprite = new Sprite();
		float pixelsToUnits = www2.texture.width / (screenWidthUnit/1.5f);
		sprite = Sprite.Create(www2.texture, new Rect(0, 0, www2.texture.width, www2.texture.height),new Vector2(0.5f, 0.5f),pixelsToUnits);
		renderer.sprite = sprite;

		brand.SetActive (true);
		brand.GetComponentInChildren<Text>().text = dict["brand"].ToString();

	}

	public void AddCard(bool isPlayer)
	{
		var dict = Json.Deserialize(www1.text) as Dictionary<string,object>;	
		
		priceTag = Instantiate(tagModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		priceTag.name = "PriceTag";
		priceTag.transform.parent = current.transform;
		priceTag.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f,  screenWidthUnit/8.0f*1.4f/0.385f , 0);
		priceTag.transform.localScale = new Vector3 (0.04f,0.04f,0.04f);
		priceTag.GetComponent<Renderer> ().sortingOrder = 100;
		StartCoroutine(Transition(priceTag,dict["retail_price"].ToString(), dict["sale_price"].ToString() )); 

		link = Instantiate(linkModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		link.transform.parent = current.transform;
		link.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f - 0.32f,  screenWidthUnit/8.0f*1.4f/0.385f -0.4f - 0.35f , 0);
		link.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		link.GetComponent<Renderer> ().sortingOrder = 101;

		email = Instantiate(emailModel, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
		email.transform.parent = current.transform;
		email.transform.localPosition = new Vector3 (screenWidthUnit/8.0f/0.4f - 0.32f,  screenWidthUnit/8.0f*1.4f/0.385f -0.4f - 0.35f -0.6f , 0);
		email.transform.localScale = new Vector3 (0.5f,0.5f,0.5f);
		email.GetComponent<Renderer> ().sortingOrder = 101;


		if(isPlayer) player.AddCard(new Card(current, int.Parse(dict ["sale_price"].ToString()),int.Parse(dict ["retail_price"].ToString()),dict ["url"].ToString(),dict ["category"].ToString(),dict ["brand"].ToString(),dict ["image"].ToString()));
		else computer.AddCard(new Card(current, int.Parse(dict ["sale_price"].ToString()),int.Parse(dict ["retail_price"].ToString()),dict ["url"].ToString(),dict ["category"].ToString(),dict ["brand"].ToString(),dict ["image"].ToString()));

		link.AddComponent<BoxCollider2D>() ;
		link.AddComponent<LinkBehavior>() ;
		
		email.AddComponent<BoxCollider2D>() ;
		email.AddComponent<EmailBehavior>() ;

	}
	
	public bool UpdateResult()
	{
		if (player.sum > goal) 
		{ 
			playerBust.SetActive(true);
			StartCoroutine(ShakeObject(playerBust));
			StartCoroutine(OnResult (-1));	
			return true;
		} 
		
		else if(player.sum == goal)
		{ 
			StartCoroutine(OnResult (1));	
			return true;
		} 

		if (computer.sum > goal) 
		{ 
			computerBust.SetActive(true);
			StartCoroutine(ShakeObject(computerBust));
			StartCoroutine(OnResult (1));	
			return true;	
		}
		else return false;
	}
	

	IEnumerator DealerHand()
	{
		ShowCard ();
		AddCard (false);
		computerSum.SetActive(true);
		computerSum.GetComponentInChildren<Text>().text = "$"+ Mathf.CeilToInt(computer.sum).ToString();

		yield return StartCoroutine(LoadItem(2));
		brand.SetActive (false);
					
		destination = new Vector3 (screenWidthUnit / 4.0f * (computer.cards.Count - 0.5f) - screenWidthUnit / 2.0f, 0.2f * screenHeightUnit);
		yield return StartCoroutine(Fade(destination));

		if (!UpdateResult ())
		{
			ShowCard ();
			hit.SetActive (true); 
			stand.SetActive (true);
		}
	}


	IEnumerator PlayerHand()
	{
		ShowCard ();
		AddCard (true);
		playerSum.SetActive(true);
		playerSum.GetComponentInChildren<Text>().text = "$"+ Mathf.CeilToInt(player.sum).ToString();

		yield return StartCoroutine(LoadItem(2));
		brand.SetActive (false);
	
		destination = new Vector3 (screenWidthUnit / 4.0f * (player.cards.Count - 0.5f) - screenWidthUnit / 2.0f, - 0.2f * screenHeightUnit);
		yield return StartCoroutine(Fade(destination));

		if (!UpdateResult ()) {
			StartCoroutine (DealerHand ());
		}
	}


	public void OnHit()
	{
		hit.SetActive(false);
		stand.SetActive(false);	

		AddCard (true);
		playerSum.GetComponentInChildren<Text>().text = "$"+ Mathf.CeilToInt(player.sum).ToString();
		if (player.sum < goal) {
			next.SetActive (true);
			fadeDone = false;
			StartCoroutine (LoadItem (-3));
		} 
		else
		{   brand.SetActive (false);
			StartCoroutine(LastCard());
		}	
	}

	IEnumerator LastCard()
	{ 
		yield return new WaitForSeconds (3);
		brand.SetActive (false);
		
		destination = new Vector3 (screenWidthUnit / 4.0f * (player.cards.Count +  downOffset - 0.5f) - screenWidthUnit / 2.0f, - 0.2f * screenHeightUnit);
		yield return StartCoroutine(Fade(destination));
		if((player.cards.Count + downOffset) >= 4) DownOffset(false);
		UpdateResult ();
	}


	public void OnNext()
	{
		next.SetActive (false);
		brand.SetActive (false);
		
		destination = new Vector3 (screenWidthUnit / 4.0f * (player.cards.Count +  downOffset - 0.5f) - screenWidthUnit / 2.0f, - 0.2f * screenHeightUnit);
		StartCoroutine(Fade(destination));
	}
	
	public void AfterHit()
	{ 	
		if((player.cards.Count + downOffset) >= 4) DownOffset(false);
		ShowCard ();
		hit.SetActive (true); 
		stand.SetActive (true);
	}
	

	public void OnStand()
	{
		hit.SetActive(false);
		stand.SetActive(false);	
		Destroy (current);
		StartCoroutine (ComputerRound());
	}


	IEnumerator ComputerRound(){
		while (computer.sum < stop) {
			ShowCard();
			AddCard (false);
			computerSum.GetComponentInChildren<Text>().text = "$"+ Mathf.CeilToInt(computer.sum).ToString();
			yield return StartCoroutine(LoadItem(2));
			brand.SetActive(false);
			destination = new Vector3 (screenWidthUnit / 4.0f * (computer.cards.Count  +  upOffset - 0.5f) - screenWidthUnit / 2.0f, 0.2f * screenHeightUnit);
			yield return StartCoroutine(Fade(destination));
			if((computer.cards.Count + upOffset) >= 4) UpOffset(false);

		}

		if (computer.sum > goal) 
		{ 
			computerBust.SetActive (true);
			StartCoroutine(ShakeObject(computerBust));
			StartCoroutine(OnResult (1));	  	
		}
		else if (computer.sum <= player.sum) { 
			StartCoroutine(ShakeObject(playerSum));
			StartCoroutine(OnResult (1));	
		} 
		else if (computer.sum > player.sum) { 
			StartCoroutine(ShakeObject(computerSum));
			StartCoroutine(OnResult (-1));	
		}
	}
	

	void OnGUI() {
		if (result == 1) {
			if (delay % 10 == 0) {
				if (delay >= 40) {
					delay = delay - 40;
				}    
				int i = delay / 10;
				anime = winPictureArray [i];
			} 
			delay++;
			GUI.DrawTexture (new Rect (Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.width / 3), anime, ScaleMode.ScaleToFit, true, 0);
		}

		if (result == -1) {
			if (delay % 10 == 0) {
				if (delay >= 90) {
					delay = delay - 90;
				} 
				int i = delay / 10 ;
				anime = losePictureArray[i];
			}      
			delay++;
			GUI.DrawTexture (new Rect (Screen.width / 3, Screen.height / 3, Screen.width / 3, Screen.width / 3), anime, ScaleMode.ScaleToFit, true, 0);
		}
	}

	public void DownOffset(bool direction)
	{
		if (!direction && ((player.cards.Count + downOffset) >= 4 || downOffset >= 1)) 
		{
			downOffset--;
			for(int i=0; i<player.cards.Count; i++)
			{
				player.cards[i].picture.transform.position = player.cards[i].picture.transform.position + new Vector3(-screenWidthUnit / 4.0f,0,0);
			}

		}
		if (direction && downOffset <= 0) 
		{
			downOffset++;
			for(int i=0; i<player.cards.Count; i++)
			{
				player.cards[i].picture.transform.position = player.cards[i].picture.transform.position + new Vector3(screenWidthUnit / 4.0f,0,0);
			}	
		}
	}

	
	public void UpOffset(bool direction)
	{
		if (!direction && ((computer.cards.Count + upOffset) >= 4 || upOffset >= 1)) 
		{
			upOffset--;
			for(int i=0; i<computer.cards.Count; i++)
			{
				computer.cards[i].picture.transform.position = computer.cards[i].picture.transform.position + new Vector3(-screenWidthUnit / 4.0f,0,0);
			}
			
		}
		if (direction && upOffset <= 0) 
		{
			upOffset++;
			for(int i=0; i<computer.cards.Count; i++)
			{
				computer.cards[i].picture.transform.position = computer.cards[i].picture.transform.position + new Vector3(screenWidthUnit / 4.0f,0,0);
			}	
		}	
	}


	IEnumerator ShakeObject(GameObject obj ) 
	{ 
		float time = 0, last_time = 0;
		int i = 1;

		while (time < 3)
		{
			time = time + Time.deltaTime;			
			if (time - last_time > 0.08) {
				obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (obj.GetComponent<RectTransform> ().anchoredPosition.x, obj.GetComponent<RectTransform> ().anchoredPosition.y + i * 10);
				i = -i;
				last_time = time;
			}
			yield return null;
		}
		if(i == -1) obj.GetComponent<RectTransform> ().anchoredPosition = new Vector2 (obj.GetComponent<RectTransform> ().anchoredPosition.x, obj.GetComponent<RectTransform> ().anchoredPosition.y + i * 10);
	}

	
	IEnumerator OnResult(int i ) 
	{ 
		if (i == 1) {
			winSound.Play ();
			winParticle.GetComponent<ParticleSystem> ().Play ();
			resultSummary.SetActive (true);
			resultSummary.GetComponentInChildren<Text>().text = "+"+ betCoin.ToString();
			totalCoin = totalCoin + betCoin;
			coinCnt.GetComponent<Text>().text = totalCoin.ToString();
			yield return new WaitForSeconds(2);
			resultSummary.SetActive (false);
			//yield return new WaitForSeconds(2);
			result = 1;
		}

		if (i == -1)
		{
			loseSound.Play ();
			//loseParticle.GetComponent<ParticleSystem> ().Play ();
			resultSummary.SetActive (true);
			resultSummary.GetComponentInChildren<Text>().text = "-" + betCoin.ToString();
			totalCoin = totalCoin - betCoin;
			if(totalCoin < 0) totalCoin = 0;
			coinCnt.GetComponent<Text>().text = totalCoin.ToString();
			yield return new WaitForSeconds(2);
			if(totalCoin<=0) 
			{ 
				totalCoin = 1000;
				resultSummary.SetActive (true);
				resultSummary.GetComponentInChildren<Text>().text = "+1000 life saver!" ;
				coinCnt.GetComponent<Text>().text = totalCoin.ToString();
				yield return new WaitForSeconds(2);
			}
			resultSummary.SetActive (false);
			//yield return new WaitForSeconds(2);
			meowSound.Play ();
			result = -1;
		}

		downLeft.SetActive (true);
		downRight.SetActive (true);
		upLeft.SetActive (true);
		upRight.SetActive (true);		

		PlayerPrefs.SetInt("coin", totalCoin);
		if (betCoin > totalCoin)		betCoin = totalCoin;
		bet.SetActive (true);
		betCnt.GetComponent<Text>().text = betCoin.ToString();
			
		deal.SetActive(true);

	}

	IEnumerator DailyBonus()
	{
		yield return new WaitForSeconds(1);
		resultSummary.SetActive (true);

		if ((int)System.DateTime.Now.DayOfWeek == 0 || (int)System.DateTime.Now.DayOfWeek == 6)
		{
			resultSummary.GetComponentInChildren<Text>().text = "+10000 weekend bonus!" ;
			totalCoin = totalCoin + 10000;
		}
        else
		{
			resultSummary.GetComponentInChildren<Text>().text = "+3000 daily bonus!" ;
			totalCoin = totalCoin + 3000;
		}
		coinCnt.GetComponent<Text>().text = totalCoin.ToString();

		PlayerPrefs.SetString("bonusdate",System.DateTime.Now.ToString("MM/dd/yyyy"));
		PlayerPrefs.SetInt("coin", totalCoin);
		yield return new WaitForSeconds(5);
		resultSummary.SetActive (false);
		
	}

	public void SubmitEmail()
	{
		PlayerPrefs.SetString("email", emailInput.GetComponent<InputField>().text);
		CancelEmail ();
	}

	public void CancelEmail()
	{
		emailInput.SetActive (false);
		emailSubmit.SetActive (false);
		emailCancel.SetActive (false);
	}

	public void ShowEmail()
	{
		emailInput.SetActive (true);
		emailSubmit.SetActive (true);
		emailCancel.SetActive (true);
		emailInput.GetComponent<InputField> ().text = PlayerPrefs.GetString ("email", "");
	}


}