using UnityEngine;
using UnityEngine.UI;

namespace LaundaryMan
{
    public class HrPanelEventListener : MonoBehaviour
    {
        public Text dirtyWorkerAIText;
        public int[] dirtyBoxAI;


        public Text cleanWorkerAIText;
        public int[] cleanBasketAI;

        public Text carryCapacityOfAIText;
        public int[] carryCapacityAI;
        public Text speedOfAIText;

        public int[] speedAI;

        public void DirtyBoxAILock()
        {
            int dirtyIndex = ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked - 1;
            if (dirtyIndex >= dirtyBoxAI.Length) return; // Prevent out-of-bounds access
            int cost = dirtyBoxAI[Mathf.Max(0, dirtyIndex)];
            if (ReferenceManager.Instance.GameData.playerCash >= cost)
            {
                ReferenceManager.Instance.GameData.playerCash -= cost;
                ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked++;

                if (GlobalConstant.isLogger)
                    print("AI Unlocked: " + ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked);
                ReferenceManager.Instance.dirtyBoxAiManager.UnlockAi(ReferenceManager.Instance.GameData
                    .dirtyBoxAiUnlocked);
                ReferenceManager.Instance.SaveGameDataObserver();
                if (SoundManager.instance)
                {
                    SoundManager.instance.Play(SoundName.Upgrade);
                }
            }

            UiUpdate();
        }

        public void DirtyBoxAILockRewarded()
        {
            if (TssAdsManager._Instance)
            {
                TssAdsManager._Instance.ShowRewardedAd(() =>
                {
                    int dirtyIndex = ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked - 1;
                    if (dirtyIndex >= dirtyBoxAI.Length) return; // Prevent out-of-bounds access
                    int cost = dirtyBoxAI[Mathf.Max(0, dirtyIndex)];

                    ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked++;
                    if (GlobalConstant.isLogger)
                        print("AI Unlocked: " + ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked);
                    ReferenceManager.Instance.dirtyBoxAiManager.UnlockAi(ReferenceManager.Instance.GameData
                        .dirtyBoxAiUnlocked);
                    ReferenceManager.Instance.SaveGameDataObserver();

                    if (SoundManager.instance)
                    {
                        SoundManager.instance.Play(SoundName.Upgrade);
                    }
                }, "DirtyBoxAIUnlockLock");
            }

            UiUpdate();
        }

        public void CarryCapacityBoxAILockRewarded()
        {
            if (TssAdsManager._Instance)
            {
                TssAdsManager._Instance.ShowRewardedAd(() =>
                {
                    int dirtyIndex = ReferenceManager.Instance.GameData.carryCapacityOfAI - 1;
                    if (dirtyIndex >= carryCapacityAI.Length) return; // Prevent out-of-bounds access


                    ReferenceManager.Instance.GameData.carryCapacityOfAI++;

                    if (GlobalConstant.isLogger)
                        print("AI carryCapacityAI: " + ReferenceManager.Instance.GameData.carryCapacityOfAI);
                    if (SoundManager.instance)
                    {
                        SoundManager.instance.Play(SoundName.Upgrade);
                    }

                    UiUpdate();

                    ReferenceManager.Instance.SaveGameDataObserver();
                }, "CarryCapacityBoxAI");
            }

            UiUpdate();
        }

        public void CarryCapacityBoxAILock()
        {
            int dirtyIndex = ReferenceManager.Instance.GameData.carryCapacityOfAI - 1;
            if (dirtyIndex >= carryCapacityAI.Length) return; // Prevent out-of-bounds access
            int cost = carryCapacityAI[Mathf.Max(0, dirtyIndex)];
            if (ReferenceManager.Instance.GameData.playerCash >= cost)
            {
                ReferenceManager.Instance.GameData.playerCash -= cost;
                ReferenceManager.Instance.GameData.carryCapacityOfAI++;

                if (GlobalConstant.isLogger)
                    print("AI carryCapacityAI: " + ReferenceManager.Instance.GameData.carryCapacityOfAI);
                if (SoundManager.instance)
                {
                    SoundManager.instance.Play(SoundName.Upgrade);
                }

                ReferenceManager.Instance.SaveGameDataObserver();
            }

            UiUpdate();
        }

        private void OnEnable()
        {
            UiUpdate();
        }

        public void SpeedOfAILockRewarded()
        {
            if (TssAdsManager._Instance)
            {
                TssAdsManager._Instance.ShowRewardedAd(() =>
                {
                    int dirtyIndex = ReferenceManager.Instance.GameData.speedOfAI - 1;
                    if (dirtyIndex >= carryCapacityAI.Length) return; // Prevent out-of-bounds access

                    ReferenceManager.Instance.GameData.speedOfAI++;

                    if (GlobalConstant.isLogger)
                        print("AI carryCapacityAI: " + ReferenceManager.Instance.GameData.speedOfAI);
                    //Observer
                    ReferenceManager.Instance.SaveGameDataObserver();
                    if (SoundManager.instance)
                    {
                        SoundManager.instance.Play(SoundName.Upgrade);
                    }
                }, "SpeedOfAI");
            }

            UiUpdate();
        }

        public void SpeedOfAILock()
        {
            int dirtyIndex = ReferenceManager.Instance.GameData.speedOfAI - 1;
            if (dirtyIndex >= carryCapacityAI.Length) return; // Prevent out-of-bounds access
            int cost = carryCapacityAI[Mathf.Max(0, dirtyIndex)];
            if (ReferenceManager.Instance.GameData.playerCash >= cost)
            {
                ReferenceManager.Instance.GameData.playerCash -= cost;
                ReferenceManager.Instance.GameData.speedOfAI++;

                if (GlobalConstant.isLogger)
                    print("AI carryCapacityAI: " + ReferenceManager.Instance.GameData.speedOfAI);
                //Observer
                ReferenceManager.Instance.SaveGameDataObserver();

                if (SoundManager.instance)
                {
                    SoundManager.instance.Play(SoundName.Upgrade);
                }
            }

            UiUpdate();
        }

        public Button maxOutDirty;
        public Button maxOutClean;
        public Button maxOutStack;
        public Button maxOutSpeed;
        public GameObject[] dirtyAdsButtons;
        public GameObject[] cleanAdsButtons;
        public GameObject[] carryAdsButtons;
        public GameObject[] speedOfAIButtons;

        void UiUpdate()
        {
            foreach (var clean in dirtyAdsButtons)
            {
                clean.SetActive((ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked < dirtyBoxAI.Length));
            }

            foreach (var clean in cleanAdsButtons)
            {
                clean.SetActive((ReferenceManager.Instance.GameData.cleanAIUnlocked < cleanBasketAI.Length));
            }

            foreach (var clean in carryAdsButtons)
            {
                clean.SetActive((ReferenceManager.Instance.GameData.carryCapacityOfAI < carryCapacityAI.Length));
            }

            //
            foreach (var clean in speedOfAIButtons)
            {
                clean.SetActive((ReferenceManager.Instance.GameData.speedOfAI < speedAI.Length));
            }

            if (ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked < dirtyBoxAI.Length)
            {
                maxOutDirty.gameObject.SetActive(false);


                dirtyWorkerAIText.text =
                    dirtyBoxAI[
                        ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked <= 0
                            ? 0
                            : ReferenceManager.Instance.GameData.dirtyBoxAiUnlocked] + "";
            }
            else
            {
                maxOutDirty.gameObject.SetActive(true);
            }

            //
            if (ReferenceManager.Instance.GameData.cleanAIUnlocked < cleanBasketAI.Length)
            {
                maxOutClean.gameObject.SetActive(false);

                cleanWorkerAIText.text =
                    cleanBasketAI[ReferenceManager.Instance.GameData.cleanAIUnlocked] + "";
            }
            else
            {
                maxOutClean.gameObject.SetActive(true);
            }


            if (ReferenceManager.Instance.GameData.speedOfAI < speedAI.Length)
            {
                maxOutSpeed.gameObject.SetActive(false);
                speedOfAIText.text =
                    speedAI[ReferenceManager.Instance.GameData.speedOfAI] + "";
            }
            else
            {
                maxOutSpeed.gameObject.SetActive(true);
            }

            //
            if (ReferenceManager.Instance.GameData.carryCapacityOfAI < carryCapacityAI.Length)
            {
                maxOutStack.gameObject.SetActive(false);
                carryCapacityOfAIText.text =
                    carryCapacityAI[ReferenceManager.Instance.GameData.carryCapacityOfAI] + "";
            }
            else
            {
                maxOutStack.gameObject.SetActive(true);
            }
        }

        public void OnClickCross()
        {
            ReferenceManager.Instance.canvasManager.CanvasStateChanger(CanvasStates.MainControls);
            ReferenceManager.Instance.SaveGameDataObserver();
            if (SoundManager.instance)
            {
                SoundManager.instance.Play(SoundName.Click);
            }
        }

        public void CleanBoxAILockRewarded()
        {
            if (TssAdsManager._Instance)
            {
                TssAdsManager._Instance.ShowRewardedAd(() =>
                {
                    int cleanIndex = ReferenceManager.Instance.GameData.cleanAIUnlocked;

                    if (cleanIndex >= cleanBasketAI.Length) return;

                    ReferenceManager.Instance.GameData.cleanAIUnlocked++;

                    ReferenceManager.Instance.taskHandler.OnTaskCompleted();
                    ReferenceManager.Instance.SaveGameDataObserver();
                    if (SoundManager.instance)
                    {
                        SoundManager.instance.Play(SoundName.Upgrade);
                    }
                }, "CLeanBoxAIUnlock");
            }

            UiUpdate();
        }

        public void CleanBoxAILock()
        {
            int cleanIndex = ReferenceManager.Instance.GameData.cleanAIUnlocked;

            if (cleanIndex >= cleanBasketAI.Length) return; // Prevent out-of-bounds access

            int cost = cleanBasketAI[Mathf.Max(0, cleanIndex)];
            if (ReferenceManager.Instance.GameData.playerCash >= cost)
            {
                ReferenceManager.Instance.GameData.playerCash -= cost;
                ReferenceManager.Instance.GameData.cleanAIUnlocked++;

                ReferenceManager.Instance.taskHandler.OnTaskCompleted();
                ReferenceManager.Instance.SaveGameDataObserver();
                if (SoundManager.instance)
                {
                    SoundManager.instance.Play(SoundName.Upgrade);
                }
            }

            UiUpdate();
        }
    }
}