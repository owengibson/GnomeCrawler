using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler
{
    public class LeaderboardManager : MonoBehaviour
    {
        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> scores;

        private const string TOTAL_DEATHS_KEY = "3f8b9e1888cb3609908bfca91179658ccd692be562bedcb04a21906301276b03";

        public void GetLeaderboard()
        {
            LeaderboardCreator.GetLeaderboard(TOTAL_DEATHS_KEY, (msg) =>
            {
                int loopLength = msg.Length < names.Count ? msg.Length : names.Count;
                for (int i = 0; i < loopLength; i++)
                {
                    names[i].text = msg[i].Username;
                    scores[i].text = msg[i].Score.ToString();
                }
            });
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            LeaderboardCreator.UploadNewEntry(TOTAL_DEATHS_KEY, username, score, msg =>
            {
                GetLeaderboard();
            });
        }

    }
}
