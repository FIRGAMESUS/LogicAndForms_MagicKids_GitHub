﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    const int formsCount = 9;
    public Slider slider;
    public Transform FormHolder, EffectHolder, Bridge, VariantsZone, EffectsBridgeHolder;
    public List<GameObject> formsList = new List<GameObject>(formsCount);
    public List<GameObject> effectsFormsList = new List<GameObject>(6);
    public List<GameObject> effectsBridgeList = new List<GameObject>(6);
    public List<Sprite> variantsSpriteList = new List<Sprite>(formsCount);
    public List<GameObject> variantsButtons = new List<GameObject>(3);
    int indexTrueVariant;


    void Start()
    {
        slider.maxValue = formsCount;
        slider.value = 0;
        FormAppear((int)slider.value);
        Bridge.gameObject.SetActive(false);
        VariantsZone.gameObject.SetActive(false);
    }


    void Update()
    {

    }

    public void NextButtonPressed()
    {
        slider.value += slider.maxValue / formsCount;
        FormAppear((int)slider.value);
    }

    public void BackButtonPressed()
    {
        slider.value -= slider.maxValue / formsCount;
        FormAppear((int)slider.value);
    }

    void FormAppear(int index)
    {
        if (FormHolder.childCount > 0) foreach (Transform child in FormHolder) Destroy(child.gameObject);
        if (EffectHolder.childCount > 0) foreach (Transform child in EffectHolder) Destroy(child.gameObject);
        if (index < 0) return;
        if (index >= formsCount)
        {
            BridgeAppear(0);
            return;
        }
        GameObject Form = Instantiate(formsList[index], FormHolder);
        GameObject Effect = Instantiate(effectsFormsList[Random.Range(0, effectsFormsList.Count)], EffectHolder);
        Vector3 localScale = Form.transform.localScale;
        Form.transform.localScale = new Vector3(0, 0, 0);
        Form.transform.DOScale(localScale, 1f).SetEase(Ease.OutBack);
    }

    void BridgeAppear(int index)
    {
        slider.value = index;
        if (EffectsBridgeHolder.childCount > 0) foreach (Transform child in EffectsBridgeHolder) Destroy(child.gameObject);
        for (int i = 0; i < Bridge.Find("bridge2").childCount; i++)
            Bridge.Find("bridge2").GetChild(i).gameObject.SetActive(false);
        Bridge.gameObject.SetActive(true);
        VariantsZone.gameObject.SetActive(true);
        GameObject Effect = Instantiate(effectsBridgeList[Random.Range(0, effectsBridgeList.Count)], EffectsBridgeHolder);
        List<int> variantsList = new List<int>(3) { -1, -1, -1 }; // Индексы форм по кнопкам
        indexTrueVariant = Random.Range(0, variantsList.Count); // Выбор кнопки с верным ответом
        variantsList[indexTrueVariant] = index; // Назначение верной кнопки индекса верного спрайта
        Debug.Log("Верный номер спрайта = " + index + " по индексу = " + indexTrueVariant);
        for (int i = 0; i < variantsList.Count; i++)
        {
            int j;
            if (i != indexTrueVariant)
            {
                do
                {
                    j = Random.Range(0, variantsSpriteList.Count); // Выбор рандомного неверного ответа
                } while (variantsList.IndexOf(j) != -1);
                variantsList[i] = j;
            }
        }
        foreach (int i in variantsList) Debug.Log(i);
        for (int i = 0; i < variantsList.Count; i++)
        {
            //Sprite _sprite = variantsButtons[i].GetComponent<Image>().sprite;
            if (i == indexTrueVariant)
                variantsButtons[i].GetComponent<Image>().sprite = variantsSpriteList[index];
            else
            {
                variantsButtons[i].GetComponent<Image>().sprite = variantsSpriteList[variantsList[i]];
            }
        }
        Bridge.Find("bridge2").GetChild(index).gameObject.SetActive(true);
    }

    public void AnswerButtonPressed(int index)
    {
        if (index == indexTrueVariant)
        {
            Debug.Log("true");
            variantsButtons[index].GetComponent<Image>().color = Color.green;
            Bridge.Find("bridge2").GetChild((int)slider.value + formsCount).gameObject.SetActive(true);
            slider.value += slider.maxValue / formsCount;
            StartCoroutine(MoveNext());
        }
        else
        {
            Debug.Log("false");
            variantsButtons[index].GetComponent<Image>().color = Color.red;
        }
    }

    IEnumerator MoveNext()
    {
        foreach (var i in variantsButtons) i.GetComponent<Button>().interactable = false;
        GameObject Effect = Instantiate(effectsFormsList[Random.Range(0, effectsFormsList.Count)], Bridge.Find("EffectsFormHolder"));
        
        yield return new WaitForSeconds(2);
        if (slider.value >= slider.maxValue)
        {
            Debug.Log("FINISH");
        }
        else
        {
            BridgeAppear((int)slider.value);
            foreach (var i in variantsButtons)
            {
                i.GetComponent<Button>().interactable = true;
                i.GetComponent<Image>().color = Color.white;
            }
        }
    }
}
