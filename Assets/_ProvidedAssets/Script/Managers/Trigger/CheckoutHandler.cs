using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Invector.vCharacterController;
using UnityEngine;

namespace LaundaryMan
{
    public class CheckoutHandler : MonoBehaviour
    {
        public Stack<ClothFragment> ReadyToShipClothes = new Stack<ClothFragment>();
        public GameObject counterStackPosition;
        private Coroutine _playerCoroutine;
        [SerializeField] bool isPlayerInside = false;


        #region TriggerEvent

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = false;
                if (_playerCoroutine != null)
                {
                    StopCoroutine(_playerCoroutine);
                    _playerCoroutine = null;
                }
            }

            if (other.CompareTag("AI"))
            {
                selectedAiReached = false;
            }
        }

        public WaitForSeconds waitForSecondsToCashier = new WaitForSeconds(0.5f);
        public bool isAICashierUnlocked;

        public IEnumerator AICashierUnlocked()
        {
            while (isPlayerInside)
            {
                if (_playerCoroutine == null)
                {
                    _playerCoroutine = StartCoroutine(CheckOut());
                }

                yield return waitForSecondsToCashier;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (isAICashierUnlocked && (other.CompareTag("Player")))
            {
                StartCoroutine(AICashierUnlocked());
                isPlayerInside = true;
            }
        }

        public GameObject playerPoint;
        public GameObject lookatPoint;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInside = true;

                if (_playerCoroutine == null)
                {
                    _playerCoroutine = StartCoroutine(CheckOut());

                   
                }
            }

            if (other.CompareTag("AI"))
            {
                selectedAiReached = true;
            }
        }

        public bool selectedAiReached;
        bool isCheckOut = false;
        private CustomerAI ai;
        int _index = 0;
        [SerializeField] private Vector3 offset;

        public Stack<ClothFragment> IronClothes = new Stack<ClothFragment>();

        public IEnumerator CheckOut()
        {
            while (isPlayerInside && ReferenceManager.Instance.queueSystem._washedClothQueueAi.Count > 0)
            {
                // foreach (var in)
                // {
                //     ReadyToShipClothes
                // }

                yield return new WaitForSeconds(.15f);
                if (ReadyToShipClothes.Count > 0)
                {
                    ai = (CustomerAI)ReferenceManager.Instance.queueSystem._washedClothQueueAi.Peek();
                    if (_index < ai.clothStack && ReadyToShipClothes.Count > 0)
                    {
                        ClothFragment cloth = ReadyToShipClothes.Pop();
                        IronClothes.Push(cloth);
                        isCheckOut = false; // Ensure it's false before waiting
                        yield return new WaitForSeconds(.15f);

                        if (ai.customerObjectToStack.myClothFragment.Count <= 0)
                        {
                            cloth.transform.DOMove(ai.customerObjectToStack.transform.position + offset, .2f)
                                .OnComplete(() =>
                                    {
                                        isCheckOut = true;
                                        cloth.transform.SetParent(ai.customerObjectToStack.transform);
                                    }
                                );
                            ai.SetIk(1);
                        }
                        else
                        {
                            cloth.transform.DOMove(ai.customerObjectToStack.myClothFragment[^1]
                                    .nextPosition.transform.position + offset, .2f)
                                .OnComplete(() => isCheckOut = true);
                            cloth.transform.SetParent(ai.customerObjectToStack.transform);
                        }

                        ai.customerObjectToStack.myClothFragment.Add(cloth);
                        yield return new WaitUntil(() => isCheckOut);
                        isCheckOut = false;
                        _index++;

                        // Check here to process queue removal inside the loop
                        if (_index == ai.clothStack)
                        {
                            ReferenceManager.Instance.queueSystem.DequeueAndDestroyWashedQueue();
                            yield return new WaitUntil(() => selectedAiReached);
                            yield return new WaitForSeconds(1f);
                            _index = 0;

                            StartCoroutine(CashGenerate());
                            if (!ReferenceManager.Instance.GameData.isTutorialCompleted)
                            {
                                ReferenceManager.Instance.tutorialHandler.TaskCompleted();
                                ReferenceManager.Instance.tutorialHandler.UnSubscribe();
                                ReferenceManager.Instance.queueSystem.OnTutorialCompleted();
                            }
                        }

                        ai.SetIk(1);
                    }
                }

                _playerCoroutine = null;
                yield return null;
            }
        }

        #endregion

        #region CashCollectionRegion

        [SerializeField] private Transform[] cashStackPositions = new Transform[3]; // Assign in Unity Inspector
        public int cashStackCount = 0;
        public CashScript cashPrefab;
        public GameObject InstentiatePoint;
        public float changeInY;

        private IEnumerator CashGenerate()
        {
            if (cashPrefab && cashStackPositions.Length == 3)
            {
                // Determine column index (0 = Left, 1 = Center, 2 = Right)
                int columnIndex = cashStackCount % 3;

                // Increase Y position after completing one row (every 3 cash items)
                if (columnIndex == 0 && cashStackCount > 0)
                {
                    changeInY += 0.1f; // Adjust stacking height
                }

                if (cashStackCount == 0)
                {
                    changeInY = 0.1f;
                }

                // Get base position from predefined transforms
                Transform basePosition = cashStackPositions[columnIndex]; // Use correct position

                // Instantiate cash at base position
                CashScript cash = Instantiate(cashPrefab, basePosition.position, Quaternion.identity);

                // Jump animation to target stacking position
                cash.transform.DOJump(
                    new Vector3(basePosition.position.x, basePosition.position.y + changeInY, basePosition.position.z),
                    1, 1, 0.2f // Adjust jump duration
                );

                // Set cash parent
                cash.transform.SetParent(basePosition);

                // Adjust height if stacking point exists
                Transform stackingPoint = cash.cashStackPoint?.transform;
                if (stackingPoint)
                {
                    cash.transform.position = stackingPoint.position;
                }

                // Add cash to stack
                cashCollectionTrigger.cashStack.Add(cash);

                // Play sound if available
                if (SoundManager.instance)
                {
                    SoundManager.instance.Play(SoundName.CashOut);
                }

                cashStackCount++; // Increment stack count
                yield return null;
            }
        }

        public CashCollectionTrigger cashCollectionTrigger;

        #endregion
    }
}