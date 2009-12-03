using System;
using System.Collections.Generic;
using System.Text;

namespace Softbuild.Pixiv
{
    public class Illust
    {
        /// <summary>
        /// 順位
        /// </summary>
        public int Rank { get; set; }

        /// <summary>
        /// 閲覧数
        /// </summary>
        public long 閲覧数 { get; set; }

        /// <summary>
        /// 評価数
        /// </summary>
        public long 評価数 { get; set; }

        /// <summary>
        /// 総合点数
        /// </summary>
        public long 総合点数 { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 埋め込みHTML文
        /// </summary>
        public string EmbedHtml { get; set; }

        /// <summary>
        /// イラストページへのURL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// イラストへのURL
        /// </summary>
        public decimal IllustID
        { 
            get{
                if (this.Url == null)
                {
                    return decimal.Zero;
                }

                // イラストURLからイラストIDだけを抽出する
                string tag = "illust_id=";
                int startIndex = this.Url.IndexOf(tag) + tag.Length;
                decimal illustID = decimal.Parse(this.Url.Substring(startIndex));

                return illustID;
            } 
        }

        /// <summary>
        /// イラストへのURL
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// サムネイルイラストへのURL
        /// </summary>
        public string ThumnailImageUrl { get; set; }

        /// <summary>
        /// サムネイルイラストのファイル名
        /// </summary>
        public string ThumnailName { get; set; }

        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 作者の名前
        /// </summary>
        public string AuthorName { get; set; }

        /// <summary>
        /// 作者のPixivID
        /// </summary>
        public string AuthorPixivID { get; set; }

        /// <summary>
        /// 作者のID
        /// </summary>
        public string AuthorID { get; set; }

        /// <summary>
        /// 作者へのURL
        /// </summary>
        public string AuthorUrl { get; set; }
    }
}
