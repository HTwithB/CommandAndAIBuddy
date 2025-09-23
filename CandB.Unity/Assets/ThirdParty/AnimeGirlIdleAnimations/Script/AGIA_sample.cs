using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


namespace AGIA
{ 
public class AGIA_sample : MonoBehaviour
    {
        public Animator animator;
        public int animBase;
        public int animLayer;

	    // Use this for initialization
	    void Start ()
        {

            animator = GetComponent<Animator>();
            animator.SetInteger("animBaseInt", 1);

        }
	
	    // Update is called once per frame
	    void Update ()
        {
		
	    }

        public void animBaseChange()
        {
            animator.SetInteger("animOtherInt", 0);

            var clickedButton = EventSystem.current.currentSelectedGameObject.name;

            if(clickedButton == "Generic_01")
            {
                animator.SetInteger("animBaseInt", 1);
            }
            else if (clickedButton == "Angry_01")
            {
                animator.SetInteger("animBaseInt", 2);
            }
            else if (clickedButton == "Brave_01")
            {
                animator.SetInteger("animBaseInt", 3);
            }
            else if (clickedButton == "Calm_01")
            {
                animator.SetInteger("animBaseInt", 4);
            }
            else if (clickedButton == "Calm_02")
            {
                animator.SetInteger("animBaseInt", 5);
            }
            else if (clickedButton == "Concern_01")
            {
                animator.SetInteger("animBaseInt", 6);
            }
            else if (clickedButton == "Classy_01")
            {
                animator.SetInteger("animBaseInt", 7);
            }
            else if (clickedButton == "Cute_01")
            {
                animator.SetInteger("animBaseInt", 8);
            }
            else if (clickedButton == "Deny_01")
            {
                animator.SetInteger("animBaseInt", 9);
            }
            else if (clickedButton == "Energetic_01")
            {
                animator.SetInteger("animBaseInt", 10);
            }
            else if (clickedButton == "Energetic_02")
            {
                animator.SetInteger("animBaseInt", 11);
            }
            else if (clickedButton == "Sexy_01")
            {
                animator.SetInteger("animBaseInt", 12);
            }
            else if (clickedButton == "Pitiable_01")
            {
                animator.SetInteger("animBaseInt", 13);
            }
            else if (clickedButton == "Stress_01")
            {
                animator.SetInteger("animBaseInt", 14);
            }
            else if (clickedButton == "Surprise_01")
            {
                animator.SetInteger("animBaseInt", 15);
            }
            else if (clickedButton == "Think_01")
            {
                animator.SetInteger("animBaseInt", 16);
            }
            else if (clickedButton == "What_01")
            {
                animator.SetInteger("animBaseInt", 17);
            }

            ///Ver1.1
            else if (clickedButton == "Boyish_01")
            {
                animator.SetInteger("animBaseInt", 18);
            }

            else if (clickedButton == "Cry_01")
            {
                animator.SetInteger("animBaseInt", 19);
            }

            else if (clickedButton == "Laugh_01")
            {
                animator.SetInteger("animBaseInt", 20);
            }

            else if (clickedButton == "Cute_02")
            {
                animator.SetInteger("animBaseInt", 21);
            }

            else if (clickedButton == "Angry_02")
            {
                animator.SetInteger("animBaseInt", 22);
            }

            else if (clickedButton == "Fedup_01")
            {
                animator.SetInteger("animBaseInt", 23);
            }

            else if (clickedButton == "Fedup_02")
            {
                animator.SetInteger("animBaseInt", 24);
            }

            else if (clickedButton == "Cute_03")
            {
                animator.SetInteger("animBaseInt", 25);
            }

            ///Ver1.2
            else if (clickedButton == "Cat_01")
            {
                animator.SetInteger("animBaseInt", 26);
            }

            else if (clickedButton == "PointFinger_01")
            {
                animator.SetInteger("animBaseInt", 27);
            }

            else if (clickedButton == "Energetic_03")
            {
                animator.SetInteger("animBaseInt", 28);
            }

            else if (clickedButton == "Sexy_02")
            {
                animator.SetInteger("animBaseInt", 29);
            }

            else if (clickedButton == "Sexy_03")
            {
                animator.SetInteger("animBaseInt", 30);
            }

        }

        public void animLayerChange()
        {
            var clickedButton = EventSystem.current.currentSelectedGameObject.name;

            if (clickedButton == "Reset")
            {
                animator.Play("Layer_start", 1, 0.0f);
            }

            else if (clickedButton == "LookAway_01")
            {
                animator.Play("Layer_look_away_01", 1, 0.0f);
            }

            else if (clickedButton == "LookAwayAngry_01")
            {
                animator.Play("Layer_look_away_angry_01", 1, 0.0f);
            }

            else if (clickedButton == "NodOnce_01")
            {
                animator.Play("Layer_nod_once_01", 1, 0.0f);
            }

            else if (clickedButton == "NodTwice_01")
            {
                animator.Play("Layer_nod_twice_01", 1, 0.0f);
            }

            else if (clickedButton == "ShakeHead_01")
            {
                animator.Play("Layer_shake_head_01", 1, 0.0f);
            }

            else if (clickedButton == "SwingBody_01")
            {
                animator.Play("Layer_swing_body_01", 1, 0.0f);
            }

            ///Ver1.1///
            else if (clickedButton == "LaughUp_01")
            {
                animator.Play("Layer_laugh_up_01", 1, 0.0f);
            }

            else if (clickedButton == "LaughDown_01")
            {
                animator.Play("Layer_laugh_down_01", 1, 0.0f);
            }

            else if (clickedButton == "ShakeBody_01")
            {
                animator.Play("Layer_shake_body_01", 1, 0.0f);
            }

            else if (clickedButton == "Surprise_01")
            {
                animator.Play("Layer_surprise_01", 1, 0.0f);
            }

            else if (clickedButton == "TiltNeck_01")
            {
                animator.Play("Layer_tilt_neck_01", 1, 0.0f);
            }

            ///Ver1.2
            else if (clickedButton == "TurnRight_01")
            {
                animator.Play("Layer_turn_right_01", 1, 0.0f);
            }

            else if (clickedButton == "TurnLeft_01")
            {
                animator.Play("Layer_turn_left_01", 1, 0.0f);
            }

        }

        public void animOtherChange()
        {
            animator.SetInteger("animBaseInt", 0);

            var clickedButton = EventSystem.current.currentSelectedGameObject.name;
            if (clickedButton == "Walk_01")
            {
                animator.SetInteger("animOtherInt", 1);
            }

            else if (clickedButton == "Run_01")
            {
                animator.SetInteger("animOtherInt", 2);
            }

            else if (clickedButton == "WaveHand_01")
            {
                animator.SetInteger("animOtherInt", 3);
            }

            else if (clickedButton == "WaveHands_01")
            {
                animator.SetInteger("animOtherInt", 4);
            }

            else if (clickedButton == "WaveArm_01")
            {
                animator.SetInteger("animOtherInt", 5);
            }

            ///Ver1.1
            else if (clickedButton == "What_01_Emote_01")
            {
                animator.Play("Other_what_01", 0, 0.0f);
                animator.SetInteger("animBaseInt", 17);
                //animator.SetInteger("animOtherInt", 6);
            }

            else if (clickedButton == "Enegetic_02_Emote_01")
            {
                animator.Play("Other_enegetic_02", 0, 0.0f);
                animator.SetInteger("animBaseInt", 10);
            }

            else if (clickedButton == "Cute_02_Emote_01")
            {
                animator.Play("Other_cute_02", 0, 0.0f);
                animator.SetInteger("animBaseInt", 21);
            }

            ///Ver1.2
            else if (clickedButton == "Cat_01_Emote_01")
            {
                animator.Play("Other_cat_01", 0, 0.0f);
                animator.SetInteger("animBaseInt", 26);
            }

            else if (clickedButton == "PointFinger_01_Emote_01")
            {
                animator.Play("Other_point_finger_01", 0, 0.0f);
                animator.SetInteger("animBaseInt", 27);
            }

        }
    }
}
