using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.GamePlay;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.Screen
{
    public class BoxRewardScreen : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] Currency CashCurrency;
        [SerializeField] DBInt[] PowersData;
        [SerializeField] ItemData[] StorItemsData;
        [SerializeField] RectTransform Body;
        [SerializeField] RectTransform[] Cards, ItemHolders;
        [SerializeField] Sprite[] PowerSprites;
        [SerializeField] Image PowerImage;

        float _tweenTime = 0.5f, _unboxingTime = 1, _cardSize = 1.25f;
        int _cardIndex = 0, _totalStoreItems = 18, _flamesLength = 4, _capsLength = 6;
        Vector2 _cardPosition = new Vector2(0, -350);
        string _flamesPath = "FlameStorage/Flame ", _capsPath = "CapStorage/Cap ", _spraysPath = "SprayStorage/Spray ";

        private void Start()
        {
            Body.DOAnchorPosX(0, _tweenTime).SetEase(Ease.OutBack).OnComplete(()=> CardReward());
        }

        void CardReward()
        {
            switch (_cardIndex)
            {
                case 0:
                    int randomReward = Random.Range(0, _totalStoreItems);
                    Debug.Log("Random Reward: " + randomReward);
                    if (randomReward < _flamesLength)
                    {
                        Instantiate(Resources.Load<GameObject>(_flamesPath + randomReward), ItemHolders[0]);
                        PowerImage.sprite = PowerSprites[0];
                        PowersData[0].Value += 1;
                    }
                    else if(randomReward < _capsLength+_flamesLength)
                    {
                        int capIndex = randomReward - _flamesLength;
                        Instantiate(Resources.Load<GameObject>(_capsPath + capIndex), ItemHolders[0]);
                        PowerImage.sprite = PowerSprites[1];
                        PowersData[1].Value += 1;
                    }
                    else
                    {
                        int sprayIndex = randomReward - (_flamesLength +_capsLength);
                        Instantiate(Resources.Load<GameObject>(_spraysPath + sprayIndex), ItemHolders[0]);
                        PowerImage.sprite = PowerSprites[2];
                        PowersData[2].Value += 1;
                    }
                    StorItemsData[randomReward].IsPurchased = true;
                    break;
                case 2:
                    CashCurrency.Amount += 300;
                    break;
            }
            StartUnBoxsing();
        }

        void StartUnBoxsing()
        {
            Debug.Log("Start Unboxing");
            SoundEffectEvent.InvokeSOEvent(3);
            Cards[_cardIndex].DOAnchorPos(_cardPosition, _unboxingTime).SetEase(Ease.OutQuad);
            Cards[_cardIndex].DOScale(_cardSize, _unboxingTime).SetEase(Ease.OutBack);
            Cards[_cardIndex].DOLocalRotate(new Vector3(0, 360f, 0), _unboxingTime / 2, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(3, LoopType.Incremental).OnComplete(() => {
                    Vector3 euler = transform.localEulerAngles;
                    euler.z = 0f;
                    Cards[_cardIndex].localEulerAngles = euler;
                    ItemHolders[_cardIndex].gameObject.SetActive(true);
                    Invoke(nameof(DestroyCard), 1.5f);
                });
        }

        void DestroyCard()
        {
            Cards[_cardIndex].gameObject.SetActive(false);
            _cardIndex++;
            if (_cardIndex < Cards.Length)
            {
                CardReward();
            }
            else
            {
                Body.DOAnchorPosX(1500, _tweenTime).SetEase(Ease.OutBack).OnComplete(() => gameObject.SetActive(false));
            }
        }
    }
}
