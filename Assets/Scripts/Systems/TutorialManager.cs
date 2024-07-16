using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler.Systems
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager instance;
        public static int StaticPopupIndex = -1;

        //[SerializeField] private TextMeshProUGUI _objectiveTextBox;
        [SerializeField] private GameObject[] _popUps;
        //[SerializeField] private string[] _objectives;

        public int _currentPopupIndex = -1;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            StaticPopupIndex = _currentPopupIndex;
        }

        private void ShowTutorialPopup(int indexToShow)
        {
            if (indexToShow <= _currentPopupIndex || indexToShow > _currentPopupIndex + 1) return;
            _popUps[indexToShow].SetActive(true);
            _currentPopupIndex = indexToShow;
        }

        private void HideTutorialPopup(int indexToHide)
        {
            if (_currentPopupIndex != indexToHide) return;
            _popUps[_currentPopupIndex].SetActive(false);
        }

        private void OnEnable()
        {
            EventManager.OnTutoialPopupQuery += ShowTutorialPopup;
            EventManager.OnRemoveTutoialPopupQuery += HideTutorialPopup;
        }

        private void OnDisable()
        {
            EventManager.OnTutoialPopupQuery -= ShowTutorialPopup;
            EventManager.OnRemoveTutoialPopupQuery -= HideTutorialPopup;
        }

        /*private void ChangeObjectiveText(Objective objective, int enemiesRemaining)
        {
            switch (objective)
            {
                case Objective.Undefined:
                    break;
                case Objective.KillEnemies:
                    _objectiveTextBox.text = $"{objective} \n {enemiesRemaining}";
                    break;
                case Objective.FindExit:
                    _objectiveTextBox.text = $"{objective} \n {enemiesRemaining}";
                    break;
                case Objective.Heal:
                    _objectiveTextBox.text = $"{objective} \n {enemiesRemaining}";
                    break;
                case Objective.KillBoss:
                    _objectiveTextBox.text = $"{objective} \n {enemiesRemaining}";
                    break;
                default:
                    break;
            }
        }*/
    }
}
