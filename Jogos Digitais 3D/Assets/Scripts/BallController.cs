using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    [SerializeField]
    private GameObject SelectionCamera, GameCamera, FollowCamera;
    [SerializeField]
    private GameObject[] ships;
    [SerializeField]
    private TextMeshProUGUI nameText, powerText;
    [SerializeField]
    private GameObject selection;
    [SerializeField]
    private Slider powerSlider;
    [SerializeField]
    private GameObject restartButton;

    [SerializeField]
    private TextMeshProUGUI spareText, strikeText;
    
    private Rigidbody rb;

    private GameObject selectedShip;

    private Camera mainCam;

    private Vector3 mainCamOriginalPos, maxRotationLeft, maxRotationRight, launchPos, originalPosition, originalRot;

    private int currentShipIndex, numberOfPlays;

    private bool selecting, position, pointer, power, restart;

    public bool left, rotLeft;

    private float rotation, powerCount, multiplier;

    private ScoreManager scoreManager;

    private void Start()
    {
        mainCam = Camera.main;
        mainCamOriginalPos = GameCamera.transform.position;
        selecting = true;
        ChangeShipStatusText();
        maxRotationLeft = new Vector3(-90, -90, -30);
        maxRotationRight = new Vector3(-90, -90, 30);
        powerCount = 0;
        multiplier = 1;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        //mainCam.transform.position = new Vector3(mainCam.transform.position.x - 55, mainCam.transform.position.y - 18, mainCam.transform.position.z);
    }

    private void Update()
    {
        if (selecting)
            SelectingShip();
        else if (restart)
            VerifyInputRestart();
        else if (position)
            VerifyInputPosition();
        else if (pointer)
            VerifyInputPointer();
        else if (power)
            SelectingPower();
    }

    private void FixedUpdate()
    {
        if (position)
            SelectingPosition();
        else if (pointer)
            SelectingPointer();
    }

    public void SelectingShip()
    {

        if (Input.GetKeyDown(KeyCode.RightArrow) && currentShipIndex < 3)
        {
            currentShipIndex++;
            if (ships[currentShipIndex] != null)
            {
                ships[currentShipIndex].SetActive(true);
                ChangeShipStatusText();
            }
            if (ships[currentShipIndex - 1] != null)
            {
                ships[currentShipIndex - 1].SetActive(false);
                ChangeShipStatusText();
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentShipIndex > 0)
        {
            currentShipIndex--;
            if (ships[currentShipIndex] != null)
            {
                ships[currentShipIndex].SetActive(true);
            }
            if (ships[currentShipIndex + 1] != null)
            {
                ships[currentShipIndex + 1].SetActive(false);
            }
           ChangeShipStatusText();
        }

        if (Input.GetKeyUp(KeyCode.Space) && ships[currentShipIndex] != null)
        {
            selectedShip = ships[currentShipIndex];
            selecting = false;
            position = true;
            rb = selectedShip.GetComponent<Rigidbody>();
            SelectionCamera.SetActive(false);
            GameCamera.SetActive(true);
            selection.SetActive(false);
            originalPosition = selectedShip.transform.position;
            originalRot = selectedShip.transform.eulerAngles;
        }

    }

    public void ChangeShipStatusText()
    {
        if (ships[currentShipIndex] != null)
            nameText.text = "Name: " + ships[currentShipIndex].GetComponent<ShipStatus>().Name;
        if (ships[currentShipIndex] != null)
            powerText.text = "Power: " + (10 - ships[currentShipIndex].GetComponent<Rigidbody>().mass).ToString();
    }

    public void SelectingPosition()
    {
        if (!left)
        {
            rb.MovePosition(selectedShip.transform.position + Vector3.forward * Time.fixedDeltaTime * 5);   
        }
        else if (left)
        {
            rb.MovePosition(selectedShip.transform.position - Vector3.forward * Time.fixedDeltaTime * 5);
        }

    }

    public void VerifyInputPosition()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            position = false;
            pointer = true;
        }
    }

    public void SelectingPointer()
    {
        if(rb.rotation.y >= -30 && rotLeft)
        {
            rotation -= Time.deltaTime * 45;
            selectedShip.transform.parent.transform.eulerAngles = new Vector3(0, rotation, 0);
            if (rotation <= -29f)
            {
                rotLeft = false;
            }
        }
        else if(rb.rotation.y <= 30 && !rotLeft) 
        {
            rotation += Time.deltaTime * 45; 
            selectedShip.transform.parent.transform.eulerAngles = new Vector3(0, rotation, 0);
            if (rotation >= 29f)
            {
                rotLeft = true;
            }
        }
    }

    public void VerifyInputPointer()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if(currentShipIndex == 0 || currentShipIndex == 3)
                launchPos = -selectedShip.transform.up;
            else if (currentShipIndex == 1)
                launchPos = selectedShip.transform.forward;
            else
            {
                launchPos = -selectedShip.transform.forward;
            }
            pointer = false;
            power = true;
            powerSlider.gameObject.SetActive(true);
        }
    }

    public void SelectingPower()
    {
        
        powerCount += Time.deltaTime * multiplier * 500;
        powerCount = Math.Clamp(powerCount, 0, 500);
        if (powerCount >= 500 || powerCount <= 1)
        {
            multiplier *= -1;
        }
        powerSlider.value = powerCount; 

        if(Input.GetKeyUp(KeyCode.Space))
        {
            LaunchShip();
            power = false;
        }

    }

    public void LaunchShip()
    {
        numberOfPlays++;
        // rb.AddForce(launchPos * powerCount, ForceMode.Impulse);
        rb.AddForce(launchPos * powerCount, ForceMode.Impulse);
        powerSlider.gameObject.SetActive(false);
        FollowCamera.transform.parent = selectedShip.transform;
        FollowCamera.transform.LookAt(selectedShip.transform.position);
        if (numberOfPlays > 1)
        {
            Invoke(nameof(VerifyStrike), 7f);
        }
        else
        {
            if(scoreManager.scoreCount < 10)
            {
                Invoke(nameof(RestartPlay), 7f);
            }
        }
    }

    public void VerifyStrike()
    {
        if(scoreManager.scoreCount >= 10 && numberOfPlays < 2)
        {
            strikeText.gameObject.SetActive(true);
            Invoke(nameof(MakeRestartButtonAvailable), 2f);
        }
        else if(scoreManager.scoreCount >= 10)
        {
            spareText.gameObject.SetActive(true);
            Invoke(nameof(MakeRestartButtonAvailable), 2f);
        }
        else if(numberOfPlays >= 2) {
            MakeRestartButtonAvailable();
        }
    }

    public void RestartPlay()
    {
        VerifyStrike();
        if(scoreManager.scoreCount < 10){
            power = false;
            selecting = true;
            ships[currentShipIndex].transform.position = originalPosition;
            ships[currentShipIndex].GetComponent<Rigidbody>().velocity = Vector3.zero;
            ships[currentShipIndex].GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            ships[currentShipIndex].transform.eulerAngles = originalRot;
            ships[currentShipIndex].GetComponent<Rigidbody>().drag = 0;
        }
    }

    public void MakeRestartButtonAvailable()
    {
        restartButton.gameObject.SetActive(true);
        restart = true;
        power = false;
        GameCamera.SetActive(false);
        FollowCamera.SetActive(true);
    }

    public void VerifyInputRestart()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

}
