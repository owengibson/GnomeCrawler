using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler
{
    public class LeaderboardManager : Singleton<LeaderboardManager>
    {
        [SerializeField] private List<TextMeshProUGUI> names;
        [SerializeField] private List<TextMeshProUGUI> scores;

        public void GetLeaderboard()
        {
            Leaderboards.TotalDeaths.GetEntries((msg) =>
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
            Leaderboards.TotalDeaths.UploadNewEntry(username, score, msg => GetLeaderboard());
        }

    }
}
