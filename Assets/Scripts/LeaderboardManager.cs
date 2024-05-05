using Dan.Main;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace GnomeCrawler
{
    public class LeaderboardManager : Singleton<LeaderboardManager>
    {
        [SerializeField] TextMeshProUGUI _totalDeathsTMP;
        public void GetLeaderboard()
        {
            int totalDeaths = 0;
            Leaderboards.TotalDeaths.GetEntries((msg) =>
            {
                foreach (var entry in msg)
                {
                    totalDeaths += entry.Score;
                }
                _totalDeathsTMP.text = totalDeaths + " \n other gnomes have met the same fate";
            });
        }

        public void SetLeaderboardEntry(string username, int score)
        {
            Leaderboards.TotalDeaths.UploadNewEntry(username, score, msg => GetLeaderboard());
        }

    }
}
