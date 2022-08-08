using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Transformations;

public class Menu : MonoBehaviour {

	public Slider Override;
	public Text OverrideValue;
	public Text[] JogLabels = new Text[6];
	public Button JogJoint, JogBase;
	public myButton[] btnJog = new myButton[12];
	public Transform[] Robot = new Transform[6];
	public Text[] txtJoint = new Text[6];
	public Text[] txtTCP = new Text[6];
	public TrailRenderer Trace;
	public Toggle TracePath;

	private int JogPressed = 0;
	private int JogMode = 0;
	private int JogAxis = 0;
	private int JogDirection;
	private double[] Joint = new double[6];
	private double[] TCP = new double[6];
	private double[] Zero = new double[6];

	private ColorBlock colBlock;
	private Color colWhite = new Color (1.0F, 1.0F, 1.0F); //Color.white
	private Color colGray = new Color(0.6F,0.6F,0.6F);

	// Use this for initialization
	void Start () {

		//set click events
		Override.onValueChanged.AddListener(delegate {SetOverride();});
		JogJoint.onClick.AddListener (delegate {JogToJoint ();});
		JogBase.onClick.AddListener (delegate {JogToBase ();});

		//set button colors
		colBlock = JogJoint.colors;
		colBlock.normalColor = colWhite;
		colBlock.highlightedColor = colWhite;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		colBlock.normalColor = colWhite;
		colBlock.highlightedColor = colWhite;
		JogJoint.colors = colBlock;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		JogBase.colors = colBlock;

		//set joints to home position
		for (int i = 0; i < 6; i++) {
			Joint [i] = 0.0;
		}

		//update real joint axes positions
		Robot [0].localEulerAngles = new Vector3 (0,(float)-Joint[0],0);
		Robot [1].localEulerAngles = new Vector3 (0,0,(float)-Joint[1]);
		Robot [2].localEulerAngles = new Vector3 (0,0,(float)-Joint[2]);
		Robot [3].localEulerAngles = new Vector3 ((float)-Joint[3],0,0);
		Robot [4].localEulerAngles = new Vector3 (0,0,(float)-Joint[4]);
		Robot [5].localEulerAngles = new Vector3 ((float)-Joint[5],0,0);

		//call DK to update TCP
		Arm.Direct (Joint, Zero, ref TCP);

		//update text labels
		for (int i = 0; i < 6; i++) {
			txtJoint [i].text = Joint [i].ToString ("F1");
			txtTCP [i].text = TCP [i].ToString ("F1");
		}

		//set trace to on as default
		TracePath.isOn = true;

	}


	//Speed override value changed
	public void SetOverride() {
		OverrideValue.text = Override.value.ToString ();
	}
		

	//Jog Joint pressed
	public void JogToJoint() {
		JogMode = 0;
		colBlock.normalColor = colWhite;
		colBlock.highlightedColor = colWhite;
		JogJoint.colors = colBlock;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		JogBase.colors = colBlock;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		JogLabels [0].text = "J1";
		JogLabels [1].text = "J2";
		JogLabels [2].text = "J3";
		JogLabels [3].text = "J4";
		JogLabels [4].text = "J5";
		JogLabels [5].text = "J6";
	}


	//Jog Base pressed
	public void JogToBase() {
		JogMode = 1;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		JogJoint.colors = colBlock;
		colBlock.normalColor = colWhite;
		colBlock.highlightedColor = colWhite;
		JogBase.colors = colBlock;
		colBlock.normalColor = colGray;
		colBlock.highlightedColor = colGray;
		JogLabels [0].text = "X";
		JogLabels [1].text = "Y";
		JogLabels [2].text = "Z";
		JogLabels [3].text = "A";
		JogLabels [4].text = "B";
		JogLabels [5].text = "C";
	}
		

	// Update is called once per frame
	void Update () {

		//check if any jog button is pressed
		JogPressed = 0;
		for (int i = 0; i < 12; i++) {
			if (btnJog [i].isPressed) {
				JogPressed = 1;
				JogAxis = i / 2;
				JogDirection = i % 2;
			}
		}

		//move robot accordingly
		if (JogPressed == 1) {

			switch (JogMode) {

			case 0: //Jog Joint Axes
				{
					double Inc = 0.01;

					//move Joint axes
					if (JogDirection == 1) {
						Joint [JogAxis] += Override.value * Inc;
					} else {
						Joint [JogAxis] -= Override.value * Inc;
					}

					//call DK to update TCP
					Arm.Direct (Joint, Zero, ref TCP);

					break;
				}

			case 1: //Jog TCP Base Axes
				{

					double Inc = 0.01;

					if (JogAxis < 3) { Inc *= 10; }

					//move TCP axes
					if (JogDirection == 1) {
						TCP [JogAxis] += Override.value * Inc;
					} else {
						TCP [JogAxis] -= Override.value * Inc;
					}

					//call IK to update Joints
					Arm.Inverse (TCP, Joint, ref Joint);

					break;
				}

			default:
				break;
					
			}

			//update real joint axes positions
			Robot [0].localEulerAngles = new Vector3 (0,(float)-Joint[0],0);
			Robot [1].localEulerAngles = new Vector3 (0,0,(float)-Joint[1]);
			Robot [2].localEulerAngles = new Vector3 (0,0,(float)-Joint[2]);
			Robot [3].localEulerAngles = new Vector3 ((float)-Joint[3],0,0);
			Robot [4].localEulerAngles = new Vector3 (0,0,(float)-Joint[4]);
			Robot [5].localEulerAngles = new Vector3 ((float)-Joint[5],0,0);

			//update text labels
			for (int i = 0; i < 6; i++) {
				txtJoint [i].text = Joint [i].ToString ("F1");
				txtTCP [i].text = TCP [i].ToString ("F1");
			}

		}
			
		//update trace status
		if (TracePath.isOn) {
			Trace.enabled = true;
		} else {
			Trace.enabled = false;
		}

	}


}
