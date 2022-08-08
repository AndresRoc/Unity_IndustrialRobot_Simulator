using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class myButton : Button, IPointerDownHandler, IPointerUpHandler {

	public bool isPressed;

	public override void OnPointerDown(PointerEventData eventData){
		isPressed = true;
		base.OnPointerDown(eventData); //base is the Button class from which myButton inherits!
	}

	public override void OnPointerUp(PointerEventData eventData){
		isPressed = false;
		base.OnPointerUp(eventData);
	}

}
