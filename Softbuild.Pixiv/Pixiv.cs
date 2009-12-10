//#define PixivAPI

using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Drawing;

namespace Softbuild.Pixiv
{
    public class Pixiv : PixivBase
    {
        #region "Pixivへログインを行う"
        /// <summary>
        /// Pixivへログインを行う
        /// </summary>
        public void Login()
        {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            string reffer = "";

#if !PixivAPI
            // 1回目：ログイン前にCookie情報を残すために捨てアクセス
            req = this.GetRequest(ConstData.DummyAccessUrl, "");
            // 1回目応答：Cookieを取得する
            using (res = (HttpWebResponse)req.GetResponse())
            {
                Cookie = GetSessionID(res.GetResponseHeader("Set-Cookie"));
            }
#endif

            // 2回目：ログイン情報を送出する(POST)
            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms["mode"] = "login";
            parms["pixiv_id"] = PixivID;
            parms["pass"] = Password;
            parms["skip"] = "1";

#if !PixivAPI
            req = this.PostRequest(res.Headers["Location"], reffer, parms);
#else
            req = this.PostRequest(ConstData.LoginUrl, reffer, parms);
#endif
            // 2回目応答：Cookieを取得する
            HttpStatusCode statusCode = HttpStatusCode.NotFound;
            using (res = (HttpWebResponse)req.GetResponse())
            {
                Cookie = GetSessionID(res.GetResponseHeader("Set-Cookie"));
                statusCode = res.StatusCode;
                reffer = res.ResponseUri.AbsoluteUri;
            }
            if (statusCode == HttpStatusCode.Found)
            {
                // ログイン情報をPOSTしたらログイン完了してほしいが
                // 302(Found)で返ってくる事がありログインしたことにならないのでリダイレクトする
                req = (HttpWebRequest)HttpWebRequest.Create(res.Headers["Location"]);
                req.Proxy = Proxy;
                req.AllowAutoRedirect = false;
                req.UserAgent = ConstData.UserAgent;
#if !PixivAPI
                req.Referer = reffer;
                req.Headers.Add("Cookie", Cookie);
                req.GetResponse().Close();
#else
                req.Referer = "http://iphone.pxv.jp/";
                req.Headers.Add("Cookie", "current-language=ja; locale-country-code=JP; locale-identifier=ja_JP; locale-language-code=ja; software-version=1.0");
                try
                {
                    req.GetResponse().Close();
                }
                catch (WebException ex)
                {
                    // throw ex;
                }
#endif
            }

#if !PixivAPI
            // ちゃんとログイン出来たかを確認する
            if ((statusCode == HttpStatusCode.OK) ||
                (req.RequestUri.AbsoluteUri != ConstData.MyPageUrl))
            {
                throw new Exception("pixivマイページへの遷移に失敗");
            }
#endif
        }
        #endregion

        #region "特定ページのイラスト情報を取得する"
        /// <summary>
        /// 特定ページのイラスト情報を取得する
        /// </summary>
        /// <param name="page">ページ種別</param>
        /// <param name="pageCount">ページ数</param>
        /// <returns></returns>
        public List<Illust> GetIllusts(PixivPages page, int pageCount)
        {
#if !PixivAPI
            string pageUrl = string.Format(page.ToUrlFormat(), pageCount);
#else
            string pageUrl = string.Format(page.ToUrlFormat(), pageCount);
            pageUrl = pageUrl + "&" + this.Cookie;
#endif

            // 指定したページからイラスト一覧のテキストを取得する
            string text = GetHtml(pageUrl);

            // 1ページあたりのイラスト数を取得する
            int rankBase = 0;
            switch (page)
            {
                case PixivPages.NewIllust:
                case PixivPages.MyPixiv:
                case PixivPages.R18NewIllust:
                    rankBase = 20;
                    break;
                default:
                    rankBase = 50;
                    break;
            }

            // htmlからイラストのリストを取得する
            List<Illust> list = GetIllustsFromHtml(page, text);
            for (int i=0; i<list.Count; i++)
            {
                list[i].Rank = (rankBase * (pageCount - 1)) + i + 1;
            }

            return list;
        }
        #endregion

        #region "イラストのBitmapデータの取得"
        /// <summary>
        /// イラストのBitmapデータの取得
        /// </summary>
        /// <param name="illust">イラスト情報</param>
        /// <returns>Bitmapオブジェクト</returns>
        public Bitmap GetBitmap(Illust illust)
        {
            Bitmap bmp = null;

            HttpWebRequest req = GetRequest(illust.ImageUrl, illust.Url);
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                try
                {
                    bmp = new Bitmap(res.GetResponseStream());
                }
                catch
                {
                    bmp = null;
                }
            }

            return bmp;
        }
        #endregion

        #region "サムネイルイラストのBitmapデータの取得"
        /// <summary>
        /// サムネイルイラストのBitmapデータの取得
        /// </summary>
        /// <param name="illust">イラスト情報</param>
        /// <returns></returns>
        public Bitmap GetThumnailBitmap(Illust illust)
        {
            Bitmap bmp = null;

            HttpWebRequest req = GetRequest(illust.ThumnailImageUrl, illust.Url);
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                bmp = new Bitmap(res.GetResponseStream());
            }

            return bmp;
        }
        #endregion

        #region "イラストを指定したファイルパスにダウンロードする(同期版)"
        /// <summary>
        /// イラストを指定したファイルパスにダウンロードする(同期版)
        /// </summary>
        /// <param name="illust">イラスト情報</param>
        /// <param name="filePath">ファイルパス</param>
        /// <returns></returns>
        public void DownloadThumnailIllust(Illust illust, string filePath)
        {
            DownloadFileAsync(illust.ThumnailImageUrl, filePath);
        }
        #endregion 

        #region "セッションIDの取得"
        /// <summary>
        /// セッションIDの取得
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string GetSessionID(string value)
        {
            string cookie = "";

            // ログイン情報を取得する為の正規表現
            Regex ssReg = new Regex(ConstData.cookieSessionIDPattan);
            if (ssReg.IsMatch(value))
            {
                Match match = ssReg.Match(value);
#if !PixivAPI
                cookie = match.Groups["session_id"].Value;
#else
                while (match.Success)
                {
                    cookie = match.Groups["session_id"].Value;
                    match = match.NextMatch();
                }
#endif

            }

            return cookie;
        }
        #endregion
        
        #region "Htmlテキストからイラスト情報を取得する"
        /// <summary>
        /// Htmlテキストからイラスト情報を取得する
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private List<Illust> GetIllustsFromHtml(PixivPages page, string text)
        {
            List<Illust> list = new List<Illust>();
            Regex Reg = null;

            switch (page)
            {
                case PixivPages.DailyRanking:
                case PixivPages.WeeklyRanking:
                case PixivPages.MonthlyRanking:
                case PixivPages.R18DailyRanking:
                case PixivPages.R18WeeklyRanking:
                case PixivPages.Favorite:
                case PixivPages.MyPixiv:
                    Reg = new Regex(ConstData.RankingPattan);
                    break;
                case PixivPages.NewIllust:
                case PixivPages.R18NewIllust:
                default:
                    Reg = new Regex(ConstData.NewIllustPattan);
                    break;
            }

            // イラスト情報を取得する
            var matchs = Reg.Match(text);
            while (matchs.Success)
            {
                Illust illust = new Illust();
                illust.Url = ConstData.PixivUrl + matchs.Groups["illust_id"].Value;
                illust.ThumnailImageUrl = matchs.Groups["thumnail"].Value;
                illust.Title = matchs.Groups["title"].Value;
                illust.AuthorPixivID = matchs.Groups["author"].Value;
                illust.ThumnailName = matchs.Groups["thumnailFileName"].Value;

                // リストにイラスト情報を追加
                list.Add(illust);

                // マッチングした次のイラスト情報
                matchs = matchs.NextMatch();
            }

            return list;
        }
        #endregion

        #region "Htmlテキストからイラスト情報を取得する"
        public void GetIllustDetail(ref Illust illust)
        {
            Regex reg = null;
            Match match = null;

            // イラストページのHtmlを取得する
            string text = GetHtml(illust.Url);

            // コメントを取得する
            if (illust.Comment != string.Empty)
            {
                reg = new Regex(ConstData.CommentPattan);
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.Comment = match.Groups["comment"].Value;
                    match = match.NextMatch();
                }
            }

            // 埋め込みhtmlを取得する
            if (illust.EmbedHtml != string.Empty)
            {
                reg = new Regex(ConstData.EmbedHtmlPattan);
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.EmbedHtml = match.Groups["html"].Value;
                    match = match.NextMatch();
                }
            }

            // イラストページに記載されているタイトルを取得する
            string title = "";
            reg = new Regex(ConstData.TitlePattan);
            match = reg.Match(text);
            while (match.Success)
            {
                title = match.Groups["title"].Value;
                match = match.NextMatch();
            }

            // イラストへの直URLを取得する
            if ((title != string.Empty) && (illust.ImageUrl == string.Empty || illust.ImageUrl == null))
            {
                title = title.Replace("|", @"\|");
                title = title.Replace("+", @"\+");
                title = title.Replace("*", @"\*");
                title = title.Replace("[", @"\[");
                title = title.Replace("]", @"\]");
                title = title.Replace("(", @"\(");
                title = title.Replace(")", @"\)");
                title = title.Replace("{", @"\(");
                title = title.Replace("}", @"\)");
                title = title.Replace("$", @"\$");
                title = title.Replace("^", @"\^");
                title = title.Replace(".", @"\.");
                reg = new Regex(string.Format(ConstData.ImagePattan, title));
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.ImageUrl = match.Groups["image_url"].Value;
                    match = match.NextMatch();
                }
            }

            // 作者の名前を取得する
            if (illust.AuthorName != string.Empty)
            {
                reg = new Regex(ConstData.AuthorNamePattan);
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.AuthorName = match.Groups["author"].Value;
                    match = match.NextMatch();
                }
            }
            
            // 作者のIDを取得する
            if (illust.AuthorID != string.Empty)
            {
                reg = new Regex(ConstData.PixivIDPattan);
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.AuthorID = match.Groups["id"].Value;
                    illust.AuthorUrl = ConstData.PixivUrl + "/index.php?id=" + match.Groups["id"].Value;
                    match = match.NextMatch();
                }
            }

            // イラストの数値評価情報を取得する
            if ((illust.閲覧数 == 0) && (illust.評価数 == 0) && (illust.総合点数 == 0))
            {
                reg = new Regex(ConstData.EtsuranPattan);
                match = reg.Match(text);
                while (match.Success)
                {
                    illust.閲覧数 = long.Parse(match.Groups["eturan"].Value);
                    illust.評価数 = long.Parse(match.Groups["eturan2"].Value);
                    illust.総合点数 = long.Parse(match.Groups["eturan3"].Value);                    
                    match = match.NextMatch();
                }
            }
        }
        #endregion

        public void AddFavUser(Illust illust)
        {
            string url = "http://www.pixiv.net/bookmark_add.php";

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms["mode"] = "add";
            parms["type"] = "user";
            parms["id"] = illust.AuthorID;
            parms["restrict"] = "0";
            parms["tag"] = "";

            HttpWebRequest req = PostRequest(url, illust.Url, parms);
            req.GetResponse().Close();
            //using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            //{



            //}
        }

        public void AddBookmark(Illust illust)
        {
            string url = "http://www.pixiv.net/bookmark_add.php";

            Dictionary<string, string> parms = new Dictionary<string, string>();
            parms["mode"] = "add";
            parms["type"] = "illust";
            parms["restrict"] = "0";
            parms["tag"] = "";
            parms["comment"] = "";
            parms["id"] = illust.IllustID.ToString();

            HttpWebRequest req = PostRequest(url, illust.Url, parms);
            req.GetResponse().Close();
            //using(HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            //{
                


            //}
        }

    }
}
