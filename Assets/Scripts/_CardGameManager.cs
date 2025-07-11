using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class _CardGameManager : MonoBehaviour
{

    //score and combo 
private int score = 0;
private int combo = 0;

[SerializeField]
private Text scoreLabel; 

public GameObject texto ; 

//last score 
private int lastScore = 0;

[SerializeField]
private Text lastScoreLabel;  





public AudioClip[] audioClips;
private AudioSource audioSource;

    // make it for row and col numbb
      public int gamerow ; 
    public int gamecol ; 

    public static _CardGameManager Instance;
   public  int gameSize => Mathf.Max(gamerow, gamecol);
    // gameobject instance
    [SerializeField]
    private GameObject prefab;
    // parent object of cards
    [SerializeField]
    private GameObject cardList;
    // sprite for card back
    [SerializeField]
    private Sprite cardBack;
    // all possible sprite for card front
    [SerializeField]
    private Sprite[] sprites;
    // list of card
    private _Card[] cards;

    //we place card on this panel
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
    // for preloading
    [SerializeField]
    private _Card spritePreload;
    // other UI
    [SerializeField]
    private Text sizeLabel;
    [SerializeField]
    private Slider sizeSlider;
    [SerializeField]
    private Text timeLabel;
    private float time;

  

    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
        score = 0;
        combo = 0;
        UpdateScoreUI();

        //last score
        lastScore = PlayerPrefs.GetInt("LastScore", 0);
        lastScoreLabel.text = "Last Score: " + lastScore;

    }

// score 
    private void UpdateScoreUI()
{
    scoreLabel.text = "Score: " + score;
}





    // Purpose is to allow preloading of panel, so that it does not lag when it loads
    // Call this in the start method to preload all sprites at start of the script
    private void PreloadCardImage()
    {
        for (int i = 0; i < sprites.Length; i++)
            spritePreload.SpriteID = i;
        spritePreload.gameObject.SetActive(false);
    }
    // Start a game
    public void StartCardGame()
    {
        if (gameStart) return; // return if game already running
        gameStart = true;
        // toggle UI
        panel.SetActive(true);
        info.SetActive(false);
        // set cards, size, position
        SetGamePanel();
        // renew gameplay variables
        cardSelected = spriteSelected = -1;
        cardLeft = cards.Length;
        // allocate sprite to card
        SpriteCardAllocation();
        StartCoroutine(HideFace());
        time = 0;
    }

    // Initialize cards, size, and position based on size of game
    private void SetGamePanel(){
        // if game is odd, we should have 1 card less
        int totalOdds = gamerow * gamecol ; 
       int   isOdd = totalOdds % 2 ;

        cards = new _Card[totalOdds- isOdd];
        // remove all gameobject from parent
        foreach (Transform child in cardList.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        // calculate position between each card & start position of each card based on the Panel
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scaleX = 1.0f / gamecol;
    float scaleY = 1.0f / gamerow;
    float xInc = row_size / gamecol;
    float yInc = col_size / gamerow;
    float curX = -xInc * (float)(gamecol / 2);
    float curY = -yInc * (float)(gamerow / 2);

        if (isOdd == 0) {
    if (gamecol % 2 == 0) curX += xInc / 2;
    if (gamerow % 2 == 0) curY += yInc / 2;
}



        float initialX = curX;
        // for each in y-axis
        for (int i = 0; i < gamerow; i++)
        {
            curX = initialX;
            // for each in x-axis
            for (int j = 0; j < gamecol; j++)
            {
                GameObject c;
                // if is the last card and game is odd, we instead move the middle card on the panel to last spot
                if (isOdd == 1 && i == (gameSize - 1) && j == (gameSize - 1))
                {
                        int index = gamerow / 2 * gamecol + gamecol / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                    // create card prefab
                    c = Instantiate(prefab);
                    // assign parent
                    c.transform.parent = cardList.transform;

                   int index = i * gamecol + j;
                    cards[index] = c.GetComponent<_Card>();
                    cards[index].ID = index;
                    // modify its size
                    c.transform.localScale = new Vector3(scaleX, scaleY);
                }
                // assign location
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    // reset face-down rotation of all cards
    void ResetFace()
    {
        for (int i = 0; i < gameSize; i++)
            cards[i].ResetRotation();
    }
    // Flip all cards after a short period
    IEnumerator HideFace()
    {
        //display for a short moment before flipping
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
    // Allocate pairs of sprite to card instances
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
        // sprite selection
        for (i = 0; i < cards.Length/2; i++)
        {
            // get a random sprite
            int value = Random.Range(0, sprites.Length - 1);
            // check previous number has not been selection
            // if the number of cards is larger than number of sprites, it will reuse some sprites
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

        // card sprite deallocation
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
        // card sprite pairing allocation
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
    // Slider update gameSize
    public void SetGameSize() {
        
        sizeLabel.text = gamerow + " X " + gamecol;
    }
    // return Sprite based on its id
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    // return card back Sprite
    public Sprite CardBack()
    {
        return cardBack;
    }
    // check if clickable
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
    // card onclick event
    public void cardClicked(int spriteId, int cardId)
    {
        // first card selected
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        { // second card selected
            if (spriteSelected == spriteId)
            {
                //correctly matched
                cards[cardSelected].Inactive();
                cards[cardId].Inactive();
                cardLeft -= 2;
                //play 
               

                combo++;
                score += 1 * combo;  
                 AudioPlayer.Instance.PlayAudio(3); 
                UpdateScoreUI();

                    if(combo > 1) {
                        texto.SetActive (true);
                    }

                CheckGameWin();
            }
            else
            {
                // incorrectly matched
                cards[cardSelected].Flip();
                cards[cardId].Flip();
                //combo 0 
                combo = 0;

                //play 
                AudioPlayer.Instance.PlayAudio(2); 
            }
            cardSelected = spriteSelected = -1;
        }
    }
    // check if game is completed
    private void CheckGameWin()
    {
        // win game
        if (cardLeft == 0)
        {
            EndGame();
            //AudioPlayer.Instance.PlayAudio(3);
        }
    }
    // stop game
    private void EndGame()
    {
        gameStart = false;
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
        panel.SetActive(false);
    }
    public void GiveUp()
    {
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
    // track elasped time
    private void Update(){
        if (gameStart) {
            time += Time.deltaTime;
            timeLabel.text = "Time: " + time + "s";
        }
    }
}
