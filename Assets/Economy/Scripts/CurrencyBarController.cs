using TMPro;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections;

namespace Core.Screen
{
    public class CurrencyBarController : MonoBehaviour
    {
        [SerializeField] DBInt CurrencyWallet;
        [SerializeField] SOIntegerEvents DepositEvent, TransactionEvent, SoundEffectEvent;
        [SerializeField] GameObject CurrencyIcon, CurrencyShine;
        [SerializeField] TextMeshProUGUI CurrencyTxt;
        [SerializeField] RectTransform InitLocation, SubmitLocation;

        int _minCurrenyCount = 8, _maxCurrencyCount = 13, _currentAmount;
        float _circleRadious = 5, _moveDuration = 0.5f;
        GameObject[] _currencyIconArray = new GameObject[0];
        Coroutine _depositRotine, _transactionRotine;
        bool _isDeposting = false, _doingTransaction;

        private void OnEnable()
        {
            CurrencyShine.SetActive(false);
            _currentAmount = CurrencyWallet.Value;
            CurrencyTxt.text = _currentAmount.ToString();
            DepositEvent.EventHandler += DepositCash;
            TransactionEvent.EventHandler += TransactCash;
        }

        private void OnDisable()
        {
            DepositEvent.EventHandler -= DepositCash;
            TransactionEvent.EventHandler -= TransactCash;
            DepositEnd();
            TransactionEnd();
        }

        void DepositCash(int cashAmount)
        {
            if (!_isDeposting)
            {
                _isDeposting = true;
                _depositRotine = StartCoroutine(DepositAnimation(cashAmount));
            }
        }

        IEnumerator DepositAnimation(int cashAmount)
        {
            int cashCount = Random.Range(_minCurrenyCount, _maxCurrencyCount);
            _currencyIconArray = new GameObject[cashCount];
            int amountSegment = cashAmount / cashCount;
            float angleStep = 360f / cashCount;
            int iconNum = 0;

            for (int c = 0; c < cashCount; c++)
            {
                // Calculate the angle for this icon
                float angle = c * angleStep;
                // Convert angle to radians for Mathf functions
                float radians = angle * Mathf.Deg2Rad;

                // Calculate the position in the circle
                float x = Mathf.Cos(radians) * _circleRadious;
                float y = Mathf.Sin(radians) * _circleRadious; // Assuming y is up and z is forward

                // Create a new position vector relative to InitLocation
                Vector3 position = new Vector3(x, y, 0) + InitLocation.position;

                // Instantiate the icon at the calculated position
                GameObject newAmount = Instantiate(CurrencyIcon, position, Quaternion.identity, transform);

                // Randomize size
                float randomSize = Random.Range(0.9f, 1.1f);
                newAmount.transform.localScale = new Vector3(randomSize, randomSize, randomSize);

                // Randomize rotation
                float randomZRotation = Random.Range(-45f, 45f);
                newAmount.transform.rotation = Quaternion.Euler(0, 0, randomZRotation);
                _currencyIconArray[c] = newAmount;
            }
            yield return new WaitForSeconds(0.1f);

            for (int m= 0; m< cashCount; m++)
            {
                RectTransform cashTransform = _currencyIconArray[m].GetComponent<RectTransform>();
                _currencyIconArray[m].gameObject.SetActive(true);
                cashTransform.DOAnchorPos(SubmitLocation.localPosition, _moveDuration).SetEase(Ease.InBack).OnComplete(()=> {
                    _currencyIconArray[iconNum].SetActive(false);
                    iconNum++;
                    CurrencyShine.SetActive(true);
                    _currentAmount += amountSegment;
                    CurrencyTxt.text = _currentAmount.ToString();
                    SoundEffectEvent.InvokeSOEvent(8);
                });
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            CurrencyTxt.text = CurrencyWallet.Value.ToString();
            CurrencyShine.SetActive(false);
            DepositEnd();
        }

        void DepositEnd()
        {
            for (int c = 0; c < _currencyIconArray.Length; c++)
            {
                Destroy(_currencyIconArray[c]);
            }
            if (_depositRotine != null)
            {
                StopCoroutine(_depositRotine);
            }
            _isDeposting = false;
        }
    
        void TransactCash(int cashAmount)
        {
            if (!_doingTransaction)
            {
                //Debug.Log("line 123");
                _doingTransaction = true;
                _transactionRotine = StartCoroutine(TransactionAnimation(cashAmount));
            }
        }

        IEnumerator TransactionAnimation(int cashAmount)
        {
            int cashCount = Random.Range(_minCurrenyCount, _maxCurrencyCount);
            int amountSegment = cashAmount / cashCount;
            for (int t = 0; t < cashCount; t++)
            {
                SoundEffectEvent.InvokeSOEvent(8);
                CurrencyShine.SetActive(true);
                _currentAmount -= amountSegment;
                CurrencyTxt.text = _currentAmount.ToString();
                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(0.1f);
            CurrencyTxt.text = CurrencyWallet.Value.ToString();
            CurrencyShine.SetActive(false);
            TransactionEnd();
        }

        void TransactionEnd()
        {
            if (_transactionRotine != null)
            {
                StopCoroutine(_transactionRotine);
            }
            _doingTransaction = false;
        }
    }
}
