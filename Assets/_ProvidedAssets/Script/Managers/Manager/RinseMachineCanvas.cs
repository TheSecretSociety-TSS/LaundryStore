using System;
using UnityEngine;
using UnityEngine.UI;

namespace LaundaryMan
{
    public class RinseMachineCanvas : MonoBehaviour
    {
        public MachineCanvasStates prev;
        public MachineCanvasStates currentState;
        public Image rinseTrackingImage;
        public GameObject checkCube;

        private void OnEnable()
        {
            CanvasStateChanger(MachineCanvasStates.Full);
            DetergentImageChange(DetergentType.Green);
        }

        public PressBasketHandler myDropper;

        public Image detargentImage;
        public Sprite[] detargentSprites;

        public void DetergentImageChange(DetergentType detargentuse)
        {
            detargentImage.preserveAspect = true;
            detargentImage.sprite = detargentSprites[(int)detargentuse];
        }

        public void MachineRefillNeeded()
        {
            index = myDropper.myIndex;
            ReferenceManager.Instance.detergentItemUI.index = index;
            CanvasStateChanger(MachineCanvasStates.RefillNeeded);
        }

        public RinseMachineCanvas rinManager;

        public void OnClickRefillButton()
        {
            ReferenceManager.Instance.canvasManager.CanvasStateChanger(CanvasStates.RefillDetergent);
        }

        public void CanvasStateChanger(MachineCanvasStates newState)
        {
            prev = currentState;
            currentState = newState;
            canvasStates[(int)prev].SetActive(false);
            canvasStates[(int)currentState].SetActive(true);
            switch (newState)
            {
                case MachineCanvasStates.Full:
                    checkCube.gameObject.SetActive(false);
                    break;
                case MachineCanvasStates.RefillNeeded:
                    ReferenceManager.Instance.notificationHandler.ShowNotification(
                        $"Rinse {index} needs to be refilled");
                    checkCube.gameObject.SetActive(true);
                    break;
            }
        }

        private int index;
        public WaitForSeconds waitForTextUpdate = new WaitForSeconds(.01f);
        [SerializeField] private GameObject[] canvasStates;
    }
}