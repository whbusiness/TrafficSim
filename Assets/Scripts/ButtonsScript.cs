using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonsScript : MonoBehaviour
{
    [Header("Crossing")]
    public Button keepGoing;
    public Button stopGoing;
    public TextMeshProUGUI correctOrIncorrect;
    public TextMeshProUGUI textAfterBtns;
    [Header("Roundabout")]
    public Button r_keepGoing;
    public Button r_stopGoing;
    public TextMeshProUGUI r_correctOrIncorrect;
    public TextMeshProUGUI r_textAfterBtns;
    [Header("TrafficLight")]
    public Button l_keepGoing;
    public Button l_stopGoing;
    public TextMeshProUGUI l_correctOrIncorrect;
    public TextMeshProUGUI l_textAfterBtns;
    public void OnKeepGoing()
    {
        print("KEEP GOING");
        keepGoing.gameObject.SetActive(false);
        stopGoing.gameObject.SetActive(false);
        correctOrIncorrect.SetText("You Were Incorrect");
        textAfterBtns.gameObject.SetActive(true);
        correctOrIncorrect.gameObject.SetActive(true);
    }
    public void OnStopGoing()
    {
        print("STOP GOING");
        keepGoing.gameObject.SetActive(false);
        stopGoing.gameObject.SetActive(false);
        correctOrIncorrect.SetText("You Were Correct");
        textAfterBtns.gameObject.SetActive(true);
        correctOrIncorrect.gameObject.SetActive(true);
    }

    public void BeforeContinuingAfterCrossing()
    {
        stopGoing.gameObject.SetActive(true);
        keepGoing.gameObject.SetActive(true);
        textAfterBtns.gameObject.SetActive(false);
        correctOrIncorrect.gameObject.SetActive(false);
    }
    public void rOnKeepGoing()
    {
        print("KEEP GOING");
        r_keepGoing.gameObject.SetActive(false);
        r_stopGoing.gameObject.SetActive(false);
        r_correctOrIncorrect.SetText("You Were Incorrect");
        r_textAfterBtns.gameObject.SetActive(true);
        r_correctOrIncorrect.gameObject.SetActive(true);
    }
    public void rOnStopGoing()
    {
        print("STOP GOING");
        r_keepGoing.gameObject.SetActive(false);
        r_stopGoing.gameObject.SetActive(false);
        r_correctOrIncorrect.SetText("You Were Correct");
        r_textAfterBtns.gameObject.SetActive(true);
        r_correctOrIncorrect.gameObject.SetActive(true);
    }

    public void BeforeContinuingAfterRoundabout()
    {
        r_stopGoing.gameObject.SetActive(true);
        r_keepGoing.gameObject.SetActive(true);
        r_textAfterBtns.gameObject.SetActive(false);
        r_correctOrIncorrect.gameObject.SetActive(false);
    }
    public void lOnKeepGoing()
    {
        print("KEEP GOING");
        l_keepGoing.gameObject.SetActive(false);
        l_stopGoing.gameObject.SetActive(false);
        l_correctOrIncorrect.SetText("You Were Incorrect");
        l_textAfterBtns.gameObject.SetActive(true);
        l_correctOrIncorrect.gameObject.SetActive(true);
    }
    public void lOnStopGoing()
    {
        print("STOP GOING");
        l_keepGoing.gameObject.SetActive(false);
        l_stopGoing.gameObject.SetActive(false);
        l_correctOrIncorrect.SetText("You Were Correct");
        l_textAfterBtns.gameObject.SetActive(true);
        l_correctOrIncorrect.gameObject.SetActive(true);
    }

    public void BeforeContinuingAfterTrafficLight()
    {
        l_stopGoing.gameObject.SetActive(true);
        l_keepGoing.gameObject.SetActive(true);
        l_textAfterBtns.gameObject.SetActive(false);
        l_correctOrIncorrect.gameObject.SetActive(false);
    }
}
