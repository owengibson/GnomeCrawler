using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler.Systems
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _objectiveTextBox;
        [SerializeField] private GameObject[] _popUps;
        [SerializeField] private string[] _objectives;

        private int _currentPopupIndex = -1;

        private void Start()
        {
            ShowTutorialPopup(0);
        }

        private void ChangeObjectiveText(Objective objective, int enemiesRemaining)
        {
            switch (objective)
            {
                case Objective.Undefined:
                    break;
                case Objective.KillEnemies:

                    break;
                case Objective.FindExit:

                    break;
                case Objective.Heal:

                    break;
                case Objective.KillBoss:

                    break;
                default:
                    break;
            }
        }

        private void ShowTutorialPopup(int index)
        {
            if (index <= _currentPopupIndex || index >= _currentPopupIndex + 2) return;
            if (_currentPopupIndex >= 0)
            {
                _popUps[_currentPopupIndex].SetActive(false);
            }
            _popUps[index].SetActive(true);
            _currentPopupIndex = index;
        }

        private void OnEnable()
        {
            EventManager.OnTutoialPopupQuery += ShowTutorialPopup;
            EventManager.OnObjectiveChange += ChangeObjectiveText;
        }

        private void OnDisable()
        {
            EventManager.OnTutoialPopupQuery -= ShowTutorialPopup;
            EventManager.OnObjectiveChange -= ChangeObjectiveText;
        }
    }
}
