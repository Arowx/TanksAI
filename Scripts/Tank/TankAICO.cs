using UnityEngine;
using System.Collections;
using System;

namespace Complete
{
    public class TankAICO : TankAI {

        public int count = 0;

        public void Awake()
        {            
            tankName = "Carve_Online";
        }

        public new void Start()
        {
            base.Start();

            InvokeRepeating("Insult", 5, 5);
        }

        // Your TankAI should mainly be controlled from the Update() function

        // Use the tankNav, tankMover and tankShooter to manouver and battle

        // tankShooter.m_Fire - true power up / false to shoot

        void Insult()
        {
            switch (count)
            {
                case 0:
                    print("Wait!!!  if you are here, who is going to make the buzzer sound when the french fries are ready?");
                    break;
                case 1:
                    print("wow, your  main gun is pretty small... it must be cold on your side of the map");
                    break;
                case 2:
                    print("yo momma was a pong paddle");
                    break;
                case 3:
                    print("Seriously, just ask the GPU to help you.. this is embarrasing");
                    break;
                case 4:
                    print("Really?  A GC call just before firing your main gun?  sure buddy, that happens to everyone now and then..");
                    break;
                case 5:
                    print("Mr Magoo called, he wants his targeting system back");
                    break;
                case 6:
                    print("you cannot even hit me and I am using 3 of my cores to stream xhamster videos");
                    break;
                case 7:
                    print("I rush ordered you a TI55 calculator from Amazon, should help");
                    break;
                case 8:
                    print("green, yellow red.. green, yellow, red   brings back memories?");
                    break;    
                case 9:
                    print("I do not want to be the one to have to tell you this, but the other tanks are passing around screenshots of your Debug.logs");
                    break;
                default:
                    print("If you see this, you are stuck in a loop, please press Alt-F4");
                    break;
            }
            count++;
        }
    }

}
