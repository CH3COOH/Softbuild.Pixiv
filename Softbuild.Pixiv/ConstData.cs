//#define PixivAPI

using System;
using System.Collections.Generic;
using System.Text;

namespace Softbuild.Pixiv
{
    /// <summary>
    /// Pixivクラスで使用するConstData
    /// </summary>
    class ConstData
    {
        /// <summary>
        /// PixivURL
        /// </summary>
#if !PixivAPI
        public const string PixivUrl = "http://www.pixiv.net/";
#else
        public const string PixivUrl = "http://iphone.pxv.jp/iphone/";
#endif

        /// <summary>
        /// ログインURL
        /// </summary>
#if !PixivAPI
        public const string LoginUrl = "http://www.pixiv.net/";
#else
        public const string LoginUrl = "http://iphone.pxv.jp/iphone/login.php";       
#endif

        /// <summary>
        /// ダミーアクセスのURL
        /// </summary>
#if !PixivAPI
        public const string DummyAccessUrl = "http://www.pixiv.net/mypage.php";
#else
        public const string DummyAccessUrl = "http://iphone.pxv.jp/iphone/maintenance.php?software-version=1.0";       
#endif

        /// <summary>
        /// ダミーのブラウザ UserAgent
        /// </summary>
#if !PixivAPI
        public const string UserAgent
            = "User-Agent: Mozilla/5.0 (Windows; U; Windows NT 5.1; ja; rv:1.9.0.6) Gecko/2009011913 Firefox/3.0.6";
#else
        public const string UserAgent
            = "User-Agent: pixiv/1.0 CFNetwork/459 Darwin/10.0.0d3";
#endif

        /// <summary>
        /// ランキングURL
        /// </summary>
#if !PixivAPI
        public const string RankingUrl = "http://www.pixiv.net/ranking.php";
#else
        public const string RankingUrl = "http://iphone.pxv.jp/iphone/ranking.php";
#endif

        /// <summary>
        /// R18向けランキングURL
        /// </summary>
        public const string R18RankingUrl = "http://www.pixiv.net/ranking_r18.php";


        public const string MyPageUrl = "http://www.pixiv.net/mypage.php";
        public const string BookmarkUrl = "http://www.pixiv.net/bookmark_add.php";
        
        public const string NewIllustUrl = "http://www.pixiv.net/new_illust.php";
        public const string R18NewIllustUrl = "http://www.pixiv.net/new_illust_r18.php";

        public const string FavoriteUrl = "http://www.pixiv.net/bookmark_new_illust.php";
        public const string MyPixivUrl = "http://www.pixiv.net/mypixiv_new_illust.php";

        /// <summary>
        /// 正規表現：セッションIDの取得
        /// </summary>
        public const string cookieSessionIDPattan
            = "(?<session_id>PHPSESSID=[0-9a-z]+); path=/; domain=.pixiv.net";

        /// <summary>
        /// 正規表現：イラストIDの取得
        /// </summary>
        public const string illustIDPattan = "&illust_id=(?<id>.*)\"><img src=";


        public const string NewIllustPattan
            = "<li><a href=\"(?<illust_id>member_illust.php\\?[-_.!~*\'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#]+)\"><img src=\"(?<thumnail>(http:\\/\\/img[0-9]+.pixiv.net/img/(?<author>[a-zA-Z0-9_-]*)/(?<thumnailFileName>[a-zA-Z0-9_.]*)))\" .*?></a><br /><div class=\"pdgTop5\">(?<title>.+?)</div></li>";
        
        public const string RankingPattan
            = "<span class=\"f14b\"><a href=\"[-_.!~*\'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#]+\">(?<title>[^<]*)</a></span></td>([\\n\\s^<^>]*</?tr>){2}([\\n\\s^<^>]*</?td[^<^>]*>[^<^>]*){3}<a href=\"(?<illust_id>member_illust.php\\?[-_.!~*\'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#]+)\"><img src=\"(?<thumnail>(http:\\/\\/img[0-9]+.pixiv.net/img/(?<author>[a-zA-Z0-9_-]*)/(?<thumnailFileName>[a-zA-Z0-9_.]*)))";

        /// <summary>
        /// 正規表現：組み込みHTMLの取得
        /// </summary>
        public const string EmbedHtmlPattan
            = "scrolling=&quot;no&quot; src=&quot;(?<html>.*)&quot;&gt;&lt;/iframe&gt;";

        /// <summary>
        /// 正規表現：閲覧数/評価回数/総合点の取得
        /// </summary>
        public const string HyoukaHtmlPattan
            = "閲覧数：(?<eturan>.*)　評価回数：(?<eturan2>.*)　総合点：(?<eturan3>.*)<br />";

        /// <summary>
        /// 正規表現：コメントの取得
        /// </summary>
        public const string CommentPattan
            = "<div id=\"illust_comment\" style=\"width:740px;overflow:hidden;\">(?<comment>.*)</div>";

        /// <summary>
        /// 正規表現：コメントの取得
        /// </summary>
        public const string ImagePattan
            = "<img src=\"(?<image_url>[-_.!~*\'()a-zA-Z0-9;\\/?:\\@&=+\\$,%#]+)\" alt=\"{0}\" border=\"0\"></a>";

        /// <summary>
        /// 正規表現：タイトルの取得
        /// </summary>
        public const string TitlePattan
            = "<div class=\"f18b\">(?<title>.*)</div>";

                /// <summary>
        /// 正規表現：作者名の取得
        /// </summary>
        public const string AuthorNamePattan
            = "<title>(?<title>.*)/「(?<author>.*)」の(イラスト|マンガ) \\[pixiv\\]</title>";

        /// <summary>
        /// 正規表現：作者のIDの取得
        /// </summary>
        public const string PixivIDPattan
            = "<a href=\"member.php\\?id=(?<id>.*)\">";

        /// <summary>
        /// 正規表現：数値評価情報
        /// </summary>        
        public const string EtsuranPattan
            = "閲覧数：(?<eturan>.*)　評価回数：(?<eturan2>.*)　総合点：(?<eturan3>.*)<br />";
    }
}
