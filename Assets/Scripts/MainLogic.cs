using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainLogic : MonoBehaviour {

    //buttons
    public GameObject addButton;
    public GameObject subtractButton;
    public GameObject multiplyButton;
    public GameObject squareButton;
    public GameObject NumberButton1;
    public GameObject NumberButton2;
    public GameObject NumberButton3;
    public GameObject NumberButton4;
    public GameObject NumberButton5;
    public GameObject NumberButton6;
    public GameObject NumberButton7;
    public GameObject NumberButton8;
    public GameObject NumberButton9;
    public GameObject NumberButton10;
    public GameObject NumberButton11;
    public GameObject NumberButton12;
    public GameObject NumberButton13;
    public GameObject NumberButton14;

    private ArrayList numberButtonList = new ArrayList();

    //canvas(s)
    public GameObject MenuBoardUI;
    public GameObject PuzzleBoardUI;
    public GameObject TutorialBoardUI;
    public GameObject WinBoardUI;
    public GameObject finalWinBoardUI;
    public GameObject winBoardScore;
    public GameObject scoreUI;

    //Texts
    public Text attemptText;
    public Text HighScoreText;
    public Text answerText;
    public Text timerText;
    public Text finalWinText;

    private ArrayList options = new ArrayList();

    //Button values
    private int numberButton1Value;
    private int numberButton2Value;
    private int numberButton3Value;
    private int numberButton4Value;
    private int numberButton5Value;
    private int numberButton6Value;
    private int numberButton7Value;
    private int numberButton8Value;
    private int numberButton9Value;
    private int numberButton10Value;
    private int numberButton11Value;
    private int numberButton12Value;
    private int numberButton13Value;
    private int numberButton14Value;

    //other primitives for gameplay control
    private bool lvl1 = true;
    private bool lvl2 = false;
    private bool lvl3 = false;
    private bool lvl4 = false;
    private bool lvl5 = false;
    private bool lvl6 = false;
    private bool lvl7 = false;
    private int countDown = 0;
    private int score = 0;

    //primitive values use for basic calculation
    private int numToOperate;
    private bool hasOperator;
    private int operatorToUse;
    private int[] operatorList;

    //numbers used in algorithim 
    private int currValue;
    private int initValue;
    private int answer;
    private int holder;

    private int pseudoLvl = 1;
    private int lvl = 1; //max 7

    // Use this for initialization
    void Start()
    {
        attemptText.text = "";

        numberButtonList.Add(NumberButton1);
        numberButtonList.Add(NumberButton2);
        numberButtonList.Add(NumberButton3);
        numberButtonList.Add(NumberButton4);
        numberButtonList.Add(NumberButton5);
        numberButtonList.Add(NumberButton6);
        numberButtonList.Add(NumberButton7);
        numberButtonList.Add(NumberButton8);
        numberButtonList.Add(NumberButton9);
        numberButtonList.Add(NumberButton10);
        numberButtonList.Add(NumberButton12);
        numberButtonList.Add(NumberButton13);
        numberButtonList.Add(NumberButton14);

        StartCoroutine(timer());
        //reset memory of high score
        //PlayerPrefs.SetInt("HighScore", 0);
    }
	// Update is called once per frame
	void Update () {
        if (MenuBoardUI.activeInHierarchy)
        {
            if (PlayerPrefs.HasKey("HighScore"))
            {
                HighScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
            }
        }
    }
    public IEnumerator timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            countDown++;
            timerText.text = ":" + countDown;
        }
    }
    IEnumerator winState()
    {
        yield return new WaitForSeconds(.5f);

        PuzzleBoardUI.SetActive(false);
        WinBoardUI.SetActive(true);

        yield return new WaitForSeconds(1);

        WinBoardUI.SetActive(false);
        PuzzleBoardUI.SetActive(true);

        score += lvl * 100;
        score -= countDown;
        scoreUI.GetComponentInChildren<Text>().text = "Score: " + score;
        Play();
        attemptText.text = "";
    }
    IEnumerator finalWinState()
    {
        yield return new WaitForSeconds(.5f);

        PuzzleBoardUI.SetActive(false);
        finalWinBoardUI.SetActive(true);
        winBoardScore.GetComponentInChildren<Text>().text = "Score: " + score;
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

    }
    public void Skip()
    {
        score -= 150;
        scoreUI.GetComponentInChildren<Text>().text = "Score: " + score;
        Play();
        attemptText.text = "";
    }
    public void Play()
    {
        countDown = 0;
        options.Clear(); //resets number list

        answer = 10001; //resets answer

       //initial value is random number between 0-10
        initValue = Random.Range(1, 11);
        currValue = initValue;
        options.Add(initValue);
        //list of n operators used on initial value to get answer
        operatorList = new int[lvl];
        while (answer > 10000 || answer<-10000 || answer == 0)
        {
            for (int i = 0; i < lvl; i++)
            {
                operatorList[i] = Random.Range(0, 4);
            }

            //for loop that does the random order of operations to initial value
            //& adds values to number list
            for (int k = 0; k < operatorList.Length; k++)
            {
                //random num for -, +, x operation
                holder = Random.Range(1, 11);

                //adds value to Number List
                if (!options.Contains(holder) && operatorList[k] != 3 && holder != initValue)
                {
                    options.Add(holder);
                }

                if (operatorList[k] == 0)
                {
                    currValue = Add(currValue, holder);
                }
                else if (operatorList[k] == 1)
                {
                    currValue = Subtract(currValue, holder);
                }
                else if (operatorList[k] == 2)
                {
                    currValue = Multiply(currValue, holder);
                }
                else if (operatorList[k] == 3)
                {
                    currValue = Square(currValue);
                }
                if (k == operatorList.Length - 1)
                {
                    answer = currValue;
                    answerText.text = "" + answer;
                }
            }
            if (answer < 10000 && answer > -10000 && answer != 0)
            {
                break;
            }
        }
        fixOperatorButtonDisplay(operatorList); //fixes display of operator buttons
        fixNumberListButtonDisplay(); //fixes display of number list buttons.

        //precise if/else for lvl control
        
        if (pseudoLvl > 1 && lvl1)
        {
            lvl++; //lvl2 is active
            lvl1 = false;
        }
        else if(pseudoLvl > 4 && !lvl2)
        {
            lvl++; //lvl 3 is active
            lvl2 = true;
        }
        else if(pseudoLvl > 7 && !lvl3)
        {
            lvl++; //lvl 4 is active
            lvl3 = true;
        }
        else if (pseudoLvl > 10 && !lvl4)
        {
            lvl++; //lvl 5 is active
            lvl4 = true;
        }
        else if (pseudoLvl > 13 && !lvl5)
        {
            lvl++; //lvl 6 is active
            lvl5 = true;
        }
        else if (pseudoLvl > 16 && !lvl6)
        {
            lvl++; //lvl 7 is active
            lvl6 = true;
        }
        else if (pseudoLvl > 19 && !lvl7)
        {
            lvl7 = true;
            StartCoroutine(finalWinState());
        }


        pseudoLvl++;

    }
    public void Back()
    {
        if (TutorialBoardUI.activeInHierarchy)
        {
            TutorialBoardUI.SetActive(false);
        }
        else if (PuzzleBoardUI.activeInHierarchy)
        {
            options.Clear();
            fixNumberListButtonDisplay();
            PuzzleBoardUI.SetActive(false);
        }
        if (PlayerPrefs.HasKey("HighScore"))
        {
            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score);
            }
        }
        else
        {
            PlayerPrefs.SetInt("HighScore", score);
        }

        MenuBoardUI.SetActive(true);
    }
    public void Tutorial()
    {
        MenuBoardUI.SetActive(false);
        TutorialBoardUI.SetActive(true);
    }
    public void PlayAgain()
    {
        if (MenuBoardUI.activeInHierarchy)
        {
            MenuBoardUI.SetActive(false);
        }
        else if (finalWinBoardUI.activeInHierarchy)
        {
            finalWinBoardUI.SetActive(false);
        }
        countDown = 0;
        timerText.text = ":" + countDown; //reset timer
        attemptText.text = "";

        score = 0;
        scoreUI.GetComponentInChildren<Text>().text = "Score: " + score; //reset score

        //resets to the default values for lvl = 1
        pseudoLvl = 1;
        lvl = 1;
        lvl1 = true;
        lvl2 = false;
        lvl3 = false;
        lvl4 = false;
        lvl5 = false;
        lvl6 = false;
        lvl7 = false;

        Play();
        PuzzleBoardUI.SetActive(true);
    }
    public void clear()
    {
        numToOperate = 0;
        attemptText.text = "" + numToOperate;
    }
    private void fixOperatorButtonDisplay(int[] operatorList)
    {
        for(int i=0; i<operatorList.Length; i++)
        {
            if (operatorList[i] == 0)
            {
                addButton.SetActive(true);
            }
            else if(operatorList[i] == 1)
            {
                subtractButton.SetActive(true);
            }
            else if (operatorList[i] == 2)
            {
                multiplyButton.SetActive(true);
            }
            else if (operatorList[i] == 3)
            {
                squareButton.SetActive(true);
            }
        }
    }
    public void numberPressed1() //to 14
    {
        attemptText.text = "" + numberButton1Value;
        if (!hasOperator)
        {
            numToOperate = numberButton1Value;
        }
        else
        {
            calculation(numberButton1Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed2()
    {
        attemptText.text = "" + numberButton2Value;
        if (!hasOperator)
        {
            numToOperate = numberButton2Value;
        }
        else
        {
            calculation(numberButton2Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed3()
    {
        attemptText.text = "" + numberButton3Value;
        if (!hasOperator)
        {
            numToOperate = numberButton3Value;
        }
        else
        {
            calculation(numberButton3Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed4()
    {
        attemptText.text = "" + numberButton4Value;
        if (!hasOperator)
        {
            numToOperate = numberButton4Value;
        }
        else
        {
            calculation(numberButton4Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed5()
    {
        attemptText.text = "" + numberButton5Value;
        if (!hasOperator)
        {
            numToOperate = numberButton5Value;
        }
        else
        {
            calculation(numberButton5Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed6()
    {
        attemptText.text = "" + numberButton6Value;
        if (!hasOperator)
        {
            numToOperate = numberButton6Value;
        }
        else
        {
            calculation(numberButton6Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed7()
    {
        attemptText.text = "" + numberButton7Value;
        if (!hasOperator)
        {
            numToOperate = numberButton7Value;
        }
        else
        {
            calculation(numberButton7Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed8()
    {
        attemptText.text = "" + numberButton8Value;
        if (!hasOperator)
        {
            numToOperate = numberButton8Value;
        }
        else
        {
            calculation(numberButton8Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed9()
    {
        attemptText.text = "" + numberButton9Value;
        if (!hasOperator)
        {
            numToOperate = numberButton9Value;
        }
        else
        {
            calculation(numberButton9Value);


            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed10()
    {
        attemptText.text = "" + numberButton10Value;
        if (!hasOperator)
        {
            numToOperate = numberButton10Value;
        }
        else
        {
            calculation(numberButton10Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed11()
    {
        attemptText.text = "" + numberButton11Value;
        if (!hasOperator)
        {
            numToOperate = numberButton11Value;
        }
        else
        {
            calculation(numberButton11Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed12()
    {
        attemptText.text = "" + numberButton12Value;
        if (!hasOperator)
        {
            numToOperate = numberButton12Value;
        }
        else
        {
            calculation(numberButton12Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed13()
    {
        attemptText.text = "" + numberButton13Value;
        if (!hasOperator)
        {
            numToOperate = numberButton13Value;
        }
        else
        {
            calculation(numberButton13Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void numberPressed14()
    {
        attemptText.text = "" + numberButton14Value;
        if (!hasOperator)
        {
            numToOperate = numberButton14Value;
        }
        else
        {
            calculation(numberButton14Value);

            if (numToOperate == answer)
            {
                StartCoroutine(winState());
            }
        }
    }
    public void addButtonLogic()
    {
        operatorToUse = 0;
        if (!hasOperator)
        {
            hasOperator = true;
        }
    }
    public void subtractButtonLogic()
    {
        operatorToUse = 1;
        if (!hasOperator)
        {
            hasOperator = true;
        }
    }
    public void multiplyButtonLogic()
    {
        operatorToUse = 2;
        if (!hasOperator)
        {
            hasOperator = true;
        }
    }
    public void squareButtonLogic()
    {
        operatorToUse = 3;
        calculation(0);

        if (numToOperate == answer)
        {
            StartCoroutine(winState());
        }
    }
    public void calculation(int holder)
    {
        hasOperator = false;
        if (operatorToUse == 0)
        {
            attemptText.text = "" + Add(numToOperate, holder);
            numToOperate = Add(numToOperate, holder);
        }
        else if (operatorToUse == 1)
        {
            attemptText.text = "" + Subtract(numToOperate, holder);
            numToOperate = Subtract(numToOperate, holder);
        }
        else if (operatorToUse == 2)
        {
            attemptText.text = "" + Multiply(numToOperate, holder);
            numToOperate = Multiply(numToOperate, holder);
        }
        else if (operatorToUse == 3) 
        {
            attemptText.text = "" + Square(numToOperate);
            numToOperate = Square(numToOperate);
        }
    }

    private void fixNumberListButtonDisplay()
    {
        foreach(GameObject obj in numberButtonList)
        {
            if (obj.activeInHierarchy)
            {
                obj.SetActive(false);
            }
        }
        if (!NumberButton1.activeInHierarchy)
        {
            NumberButton1.SetActive(true);
            numberButton1Value = initValue;
            NumberButton1.GetComponentInChildren<Text>().text = "" + initValue;
            options.Remove(initValue);
        }
        foreach (int num in options)
        {
            if (!NumberButton2.activeInHierarchy)
            {
                NumberButton2.SetActive(true);
                numberButton2Value = num;
                NumberButton2.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton3.activeInHierarchy)
            {
                NumberButton3.SetActive(true);
                numberButton3Value = num;
                NumberButton3.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton4.activeInHierarchy)
            {
                NumberButton4.SetActive(true);
                numberButton4Value = num;
                NumberButton4.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton5.activeInHierarchy)
            {
                NumberButton5.SetActive(true);
                numberButton5Value = num;
                NumberButton5.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton6.activeInHierarchy)
            {
                NumberButton6.SetActive(true);
                numberButton6Value = num;
                NumberButton6.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton7.activeInHierarchy)
            {
                NumberButton7.SetActive(true);
                numberButton7Value = num;
                NumberButton7.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton8.activeInHierarchy)
            {
                NumberButton8.SetActive(true);
                numberButton8Value = num;
                NumberButton8.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton9.activeInHierarchy)
            {
                NumberButton9.SetActive(true);
                numberButton9Value = num;
                NumberButton9.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton10.activeInHierarchy)
            {
                NumberButton10.SetActive(true);
                numberButton10Value = num;
                NumberButton10.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton11.activeInHierarchy)
            {
                NumberButton11.SetActive(true);
                numberButton11Value = num;
                NumberButton11.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton12.activeInHierarchy)
            {
                NumberButton12.SetActive(true);
                numberButton12Value = num;
                NumberButton12.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton13.activeInHierarchy)
            {
                NumberButton13.SetActive(true);
                numberButton13Value = num;
                NumberButton13.GetComponentInChildren<Text>().text = "" + num;
            }
            else if (!NumberButton14.activeInHierarchy)
            {
                NumberButton14.SetActive(true);
                numberButton14Value = num;
                NumberButton14.GetComponentInChildren<Text>().text = "" + num;
            }
        }

    }
    //0
    public int Add(int val1, int val2)
    {
        return val1 + val2;
    }
    //1
    public int Subtract(int val1, int val2)
    {
        return val1 - val2;
    }
    //2
    public int Multiply(int val1, int val2)
    {
        return val1 * val2;
    }
    //3
    public int Square(int val)
    {
        return val * val;
    }
}
