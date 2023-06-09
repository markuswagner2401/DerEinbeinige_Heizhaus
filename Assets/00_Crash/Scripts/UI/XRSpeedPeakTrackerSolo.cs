using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using ObliqueSenastions.VRRigSpace;
using ObliqueSenastions.PunNetworking;
using Photon.Pun;

namespace ObliqueSenastions.UISpace
{

    public class XRSpeedPeakTrackerSolo : MonoBehaviour
    {


        //[SerializeField] Role role = Role.Rennfahrer;

        [SerializeField] float veloTheshold = 0.1f;

        bool active;
        [SerializeField] float resetTimer = 0f;
        [SerializeField] float resetAfterStillSeconds = 3f;
        bool timerResetted = false;

        [SerializeField] PeakTracker peakTracker;

        [System.Serializable]
        struct PeakTracker
        {
            public XRVelocityTracker velocityTracker;

            [Tooltip("doesnt need XR Device, is based on simple Vector calculations of the transform")]
            public bool useSimpleVelocityTracker;
            public SimpleVelocityTracker simpleVelocityTracker;
            public Tachonadel tachonadel;
            public UnityEvent unityEvent;
            public float currentSpeed;
            public float speedThreshold;
            public float speadPeak;
            public float speadMin;
            public float speedMax;
            public bool thresholdBroken;
            public float time;
            public float stayTimeAtPeak;
            public float resetTime;
            public float resetAfterStillSeconds;


        }

        float outputValueNormed;

        void Start()
        {

        }


        void Update()
        {
            // if(PhotonNetwork.IsConnected && MultiplayerConnector.instance.GetRole() != role)
            // {
            //     return;
            // }

            if (peakTracker.useSimpleVelocityTracker)
            {
                if(peakTracker.simpleVelocityTracker == null) return;
                peakTracker.currentSpeed = peakTracker.simpleVelocityTracker.GetLocalSpeed();
            }
            else
            {
                if(peakTracker.velocityTracker == null) return;
                peakTracker.currentSpeed = peakTracker.velocityTracker.GetSpeed();
            }

            peakTracker.time += Time.deltaTime;

            if (peakTracker.currentSpeed > veloTheshold)
            {
                // print(">velo");
                if (!(peakTracker.time > peakTracker.stayTimeAtPeak)) return;

                // print("> stay");

                if (peakTracker.thresholdBroken) return;

                peakTracker.thresholdBroken = true;

                //   print("broken threshold");

                StartCoroutine(CalculateSpeedPeak(peakTracker));


            }

            else
            {
                //peakTracker.thresholdBroken = false;
                peakTracker.resetTime += Time.deltaTime;

            }

            ///

            if (peakTracker.resetTime > peakTracker.resetAfterStillSeconds)
                {
                    //   print("reset");
                    //peakTrackers[i].tachonadel.SetTargetPositionNorm(0f);
                    outputValueNormed = 0f;
                    peakTracker.resetTime = 0f;
                }


        }

        public float GetCurrentNormedValue()
        {
            return outputValueNormed;
        }

        IEnumerator CalculateSpeedPeak(PeakTracker peakTracker)
        {
            print("calculate speed peak");
            // peakTracker.thresholdBroken = true;
            // print("calculate");
            float previousSpeed = 0f;

            float currentSpeed;

            if (peakTracker.useSimpleVelocityTracker)
            {
                currentSpeed = peakTracker.simpleVelocityTracker.GetLocalSpeed();
            }

            else
            {
                currentSpeed = peakTracker.velocityTracker.GetSpeed();
            }


            while (previousSpeed < currentSpeed)
            {
                print("while");

                previousSpeed = currentSpeed;

                yield return new WaitForSeconds(0.1f);
                
                if (peakTracker.useSimpleVelocityTracker)
                {
                    currentSpeed = peakTracker.simpleVelocityTracker.GetLocalSpeed();
                }
                else
                {
                    currentSpeed = peakTracker.velocityTracker.GetSpeed();
                }

                yield return null;


            }


            peakTracker.time = 0f;
            print("set timer to " + peakTracker.time);
            peakTracker.speadPeak = peakTracker.currentSpeed;
            //print("set spead peak: " + peakTracker.speadPeak);




            //peakTracker.unityEvent.Invoke();

            outputValueNormed = Mathf.InverseLerp(peakTracker.speadMin, peakTracker.speedMax, peakTracker.speadPeak);
            //peakTracker.tachonadel.SetTargetPositionNorm(Mathf.InverseLerp(peakTracker.speadMin, peakTracker.speedMax, peakTracker.speadPeak));



            peakTracker.thresholdBroken = false;

            print("threshold broken: " + peakTracker.thresholdBroken);
            // print("broken threshold= "+ peakTracker.thresholdBroken);


            yield break;
        }




    }



}
