﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TextFade : MonoBehaviour, IPointerClickHandler
{
	// 구조체
	enum DONEDESTROY
	{
		None,
		Script,
		Object
	}

	// 인스펙터 노출 변수
	// 일반
	[SerializeField]
    private Text		  textBox = null;                   // 텍스트가 입력될 텍스트 박스

    [Space(20)]
 
    // 수치
    [SerializeField]
    private float	      speed = 1f;                       // 텍스트 출력 속도
    [SerializeField]
    private bool		  touchSkip = false;                // 터치시 스킵기능 사용할지
	[SerializeField]
	private DONEDESTROY   doneDestroy = DONEDESTROY.None;	// 텍스트 출력이 끝나면 스크립트 또는 오브젝트를 파괴할것인지


	// 인스펙터 비노출 변수
	// 일반
	private string	      targetText;						// 현재 타겟 텍스트
	private Queue<string> texts;						    // 출력될 텍스트의 모음
    private char[]		  textArray;                        // char배열로 변환된 텍스트
    private int			  textIndex = 0;                    // 배열 인덱스
    private bool	      isDone = true;                    // 끝났는지
    

    // 시작 초기화
    private void Awake()
    {
		// 변수 초기화
        if (textBox == null)
        {
            textBox = GetComponent<Text>();
        }
	}

	// 마우스 클릭시
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (touchSkip)
		{
			// 텍스트 진행중 -> 텍스트 스킵
			if (!isDone)
			{
				Debug.Log("Skip");
				SkipOutputText();
			}
			// 텍스트 진행중 아님 -> 다음 텍스트 출력
			else
			{
				Debug.Log("Next");
				StartOutputText();
			}
		}
	}

	// 초기화
	public void Initialize(Queue<string> newTexts)
    {
		// 텍스트 모음 큐 초기화
		texts = new Queue<string>(newTexts);

		// 텍스트 출력 시작
		StartOutputText();
	}

    // 텍스트 출력 시작
    private void StartOutputText()
	{
		// 다음 텍스트가 있는지 확인
		if (NextText())
		{
			isDone = false;

			// 코루틴 시작
			StartCoroutine(OutputText());
		}
		else
		{
			// 종료
			DoneDestroy();
		}
    }

	// 다음 텍스트 설정
	private bool NextText()
	{
		// 텍스트박스 초기화
		textBox.text = "";

		// 텍스트가 남아있는지 확인
		if (texts.Count <= 0)
		{
			return false;
		}

		targetText = texts.Dequeue();

		// 텍스트 배열 할당
		textArray = targetText.ToCharArray();
		textIndex = 0;
		
		return true;
	}

	// 텍스트 출력 중지
	public void StopOutputText()
    {
		// 코루틴 중지
		textIndex = textArray.Length;
    }

    // 텍스트 출력 스킵
    public void SkipOutputText()
    {
		// 출력 종료
        StopOutputText();

		// 텍스트 강제변환
		textBox.text = targetText;
    }

	// 끝났을 때 스크립트 또는 오브젝트를 파괴
	private void DoneDestroy()
	{
		// 오브젝트 파괴
		if (doneDestroy == DONEDESTROY.Object)
		{
			Destroy(gameObject);
		}
		// 스크립트 파괴
		else if (doneDestroy == DONEDESTROY.Script)
		{
			Destroy(this);
		}
	}

	// 텍스트 출력
	private IEnumerator OutputText()
    {
		// 텍스트가 다출력됬는지 확인
        while (textIndex < textArray.Length)
        {
			// 다음 글자 출력
            textBox.text += textArray[textIndex++];

			// 일정시간 멈춤
            yield return new WaitForSeconds(0.01f * speed);
        }

		isDone = true;

        yield return null;
    }
}
