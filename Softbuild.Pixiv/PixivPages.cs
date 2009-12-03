using System;
using System.Collections.Generic;
using System.Text;

namespace Softbuild.Pixiv
{
    /// <summary>
    /// 
    /// </summary>
    public enum PixivPages
    {
        DailyRanking,
        WeeklyRanking,
        MonthlyRanking,
        R18DailyRanking,
        R18WeeklyRanking,
        NewIllust,
        R18NewIllust,
        Favorite,
        MyPixiv
    }

    public static class PixivPagesExt
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static string ToUrlFormat(this PixivPages e)
        {
            switch (e)
            {
                case PixivPages.DailyRanking:
                    return ConstData.RankingUrl + "?mode=day&num={0}";
                case PixivPages.WeeklyRanking:
                    return ConstData.RankingUrl + "?mode=week&num={0}";
                case PixivPages.MonthlyRanking:
                    return ConstData.RankingUrl + "?mode=month&num={0}";
                case PixivPages.R18DailyRanking:
                    return ConstData.R18RankingUrl + "?mode=day&num={0}";
                case PixivPages.R18WeeklyRanking:
                    return ConstData.R18RankingUrl + "?mode=week&num={0}";
                case PixivPages.NewIllust:
                    return ConstData.NewIllustUrl;
                case PixivPages.R18NewIllust:
                    return ConstData.R18NewIllustUrl;
                case PixivPages.Favorite:
                    return ConstData.FavoriteUrl;
                case PixivPages.MyPixiv:
                    return ConstData.MyPixivUrl;
                default: 
                    throw new ArgumentOutOfRangeException("e");
            }
        }
    }
}
