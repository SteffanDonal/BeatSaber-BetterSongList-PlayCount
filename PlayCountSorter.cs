using BetterSongList.Interfaces;
using BetterSongList.SortModels;
using BetterSongList.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlayCount
{
    public class PlayCountSorter : ISorterPrimitive, ISorterWithLegend, ITransformerPlugin
    {
        static PlayerDataModel playerDataModel;

        public bool isReady => true;

        public string name => "Play Count";

        public bool visible => true;

        public IEnumerable<KeyValuePair<string, int>> BuildLegend(BeatmapLevel[] levels)
            => SongListLegendBuilder.BuildFor(levels, level => GetGroupName(CalculateGroupNumberForPlays(GetPlayCountFor(level))));

        string GetGroupName(int group)
        {
            if (group == 0)
                return "Never";
            else if (group < 10)
                return $"{group}x";
            else
                return $"{group}+";
        }
        int CalculateGroupNumberForPlays(int numPlays)
        {
            if (numPlays < 10)
                return numPlays;

            var groupScale = (int)Math.Pow(10, numPlays.ToString().Length - 1);
            var scaleMultiplier = numPlays / groupScale;

            return groupScale * scaleMultiplier;
        }

        public float? GetValueFor(BeatmapLevel song) => GetPlayCountFor(song);

        public int GetPlayCountFor(BeatmapLevel song)
        {
            return song
                .GetBeatmapKeys()
                .Select(GetPlayCountForKey)
                .Aggregate((a, b) => a + b);
        }

        IDictionary<BeatmapKey, PlayerLevelStatsData> PlayerLevelStatsData
        {
            get
            {
                if (playerDataModel == null)
                    playerDataModel = UnityEngine.Object.FindObjectOfType<PlayerDataModel>();

                return playerDataModel?.playerData?.levelsStatsData;
            }
        }
        int GetPlayCountForKey(BeatmapKey key)
        {
            if (PlayerLevelStatsData == null)
                return 0;

            if (!PlayerLevelStatsData.TryGetValue(key, out var level))
                return 0;

            return level.playCount;
        }

        public Task Prepare(CancellationToken cancelToken) => Task.CompletedTask;
        public void ContextSwitch(SelectLevelCategoryViewController.LevelCategory levelCategory, BeatmapLevelPack playlist) { }
    }
}
