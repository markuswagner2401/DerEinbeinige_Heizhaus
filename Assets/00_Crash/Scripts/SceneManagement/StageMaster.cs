using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using RPG.Saving;
using UnityEngine.XR;
using System;
using ObliqueSenastions.VRRigSpace;

namespace ObliqueSenastions.StageMasterSpace
{

    public class StageMaster : MonoBehaviour, ISaveable
    {
        [SerializeField] GoEvent[] goEvents;
        [SerializeField] StaticEvent[] staticEvents;
        [SerializeField] XRNode node;

        [SerializeField] bool goByController = false;
        InputDevice device;
        bool alreadyPressed = false;

        [System.Serializable]
        struct GoEvent
        {
            public string note;
            public UnityEvent reactions;
            public bool automaticlyPlayNext;
            public float waitTimeForNext;
        }


        [System.Serializable]
        struct StaticEvent
        {
            public string note;
            public UnityEvent reactions;

        }

        [SerializeField] int startIndex = 0;
        int currentIndex = 0;


        public delegate void OnFadeIn(float duration);

        public OnFadeIn onFadeIn;

        public delegate void OnFadeOut(float duration);

        public OnFadeOut onFadeOut;

        public delegate void OnGoEvent(string name);

        public OnGoEvent onGoEvent;


        void Start()
        {
            currentIndex = startIndex;
            device = InputDevices.GetDeviceAtXRNode(node);

            onFadeIn += PlaceholderOnFadeIn;
            onFadeOut += PlaceholderOnFadeOut;
            onGoEvent += PlaceholderOnGoEvent;
        }



        void Update()
        {
            // if (goByController)
            // {
            //     if (device.TryGetFeatureValue(CommonUsages.primaryButton, out bool buttonPressed) && buttonPressed)
            //     {
            //         if (alreadyPressed) return;
            //         alreadyPressed = true;
            //         PlayNextGoEvent();
            //     }
            //     else
            //     {
            //         alreadyPressed = false;
            //     }

            // }

        }

        ///

        public void FadeIn(float duration)
        {
            onFadeIn.Invoke(duration);
        }

        private void PlaceholderOnFadeOut(float duration)
        {

        }



        public void FadeOut(float duration)
        {
            onFadeOut.Invoke(duration);
        }

        private void PlaceholderOnFadeIn(float duration)
        {
        }

        public void PlayGoEvent(int index)
        {

            if (index >= goEvents.Length || index < 0) return;



            StartCoroutine(PlayGoEventRoutine(index));
        }

        private void PlaceholderOnGoEvent(string name)
        {
        }

        public void SendDelegateVoidOnGoEvent(string name)
        {
            onGoEvent.Invoke(name);
        }
        public void PlayGoEvent(string name)
        {
            
            int index = GetGoEventIndexByName(name);
            PlayGoEvent(index);
            print("play go event: " + name + " index: " + index);
        }

        private int GetGoEventIndexByName(string name)
        {
            for (int i = 0; i < goEvents.Length; i++)
            {
                if (goEvents[i].note == name)
                {
                    return i;
                }
            }

            return -1;
        }


        ///

        public void SetNextRig()
        {
            GameObject.FindGameObjectWithTag("Traveller").GetComponent<cameraTraveller>().SetNextTransitionPoint();
        }

        public void SetVRRig(int index, bool changeDuration, float duration, bool changeCurve, AnimationCurve curve)
        {
            GameObject.FindGameObjectWithTag("Traveller").GetComponent<cameraTraveller>().SetTransitionPoint(index, changeDuration, duration, changeCurve, curve);

        }

        public void SetVRRig(string name, bool changeDuration, float duration, bool changeCurve, AnimationCurve curve)
        {
            GameObject.FindGameObjectWithTag("Traveller").GetComponent<cameraTraveller>().SetTransitionPoint(name, changeDuration, duration, changeCurve, curve);

        }

        ///

        public void PlayNextGoEvent()
        {

            StartCoroutine(PlayGoEventRoutine(currentIndex));

        }

        public void SetCurrentIndex(int index)
        {
            currentIndex = index;
        }

        IEnumerator PlayGoEventRoutine(int index)
        {
            currentIndex = index;

            if (index < 0)
            {
                print("no more events");
                yield break;
            }

            print("Go Event: " + goEvents[index].note);

            goEvents[index].reactions.Invoke();


            if (goEvents[index].automaticlyPlayNext)
            {
                yield return new WaitForSeconds(goEvents[index].waitTimeForNext);
                index = NextIndex(index);
                PlayGoEvent(index);
                yield break;
            }

            index = NextIndex(index);
            yield break;
        }

        private int NextIndex(int index)
        {
            if (index + 1 > goEvents.Length - 1)
            {

                return -1;
            }

            return index += 1;
        }

        public void PlayStaticEvent(int index)
        {
            staticEvents[index].reactions.Invoke();
        }

        ///



        ////

        public object CaptureState()
        {
            return currentIndex;
        }

        public void RestoreState(object state)
        {
            currentIndex = (int)state;
        }

        ///


    }

}
