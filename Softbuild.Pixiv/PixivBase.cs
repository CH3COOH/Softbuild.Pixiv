using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.IO;

namespace Softbuild.Pixiv
{
    public class PixivBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PixivBase()
        {
            this.Proxy = new WebProxy();
        }
        
        #region "Pixiv接続情報"
        /// <summary>
        /// ログイン情報
        /// </summary>
        protected string Cookie { get; set; }

        /// <summary>
        /// デフォルトで使用するプロキシ
        /// </summary>
        protected WebProxy Proxy { get; set; }
        #endregion

        #region "アカウント情報"
        /// <summary>
        /// PixivID
        /// </summary>
        public string PixivID { get; set; }

        /// <summary>
        /// パスワード
        /// </summary>
        public string Password { get; set; }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="referer"></param>
        /// <returns></returns>
        protected HttpWebRequest GetRequest(string url, string referer)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Proxy = Proxy;
            req.AllowAutoRedirect = false;
            req.UserAgent = ConstData.UserAgent;
            req.Referer = referer;
            req.Headers.Add("Cookie", Cookie);
            req.Timeout = 30 * 1000;

            return req;
        }

        #region "GETメソッドにて取得"
        /// <summary>
        /// GETメソッドにて取得
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        protected string GetHtml(string url)
        {
            string text = "";

            HttpWebRequest req = GetRequest(url, ConstData.MyPageUrl);
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                using (StreamReader sr = new StreamReader(res.GetResponseStream()))
                {
                    text = sr.ReadToEnd();
                }
            }

            return text;
        }
        #endregion

        #region "指定したファイルをダウンロードする(同期版)"
        /// <summary>
        /// 指定したファイルをダウンロードする(同期版)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="filePath"></param>
        protected void DownloadFileAsync(string url, string filePath)
        {
            HttpWebRequest req = GetRequest(url, ConstData.MyPageUrl);
            using (HttpWebResponse res = (HttpWebResponse)req.GetResponse())
            {
                using (FileStream fileStrm = new FileStream(filePath, FileMode.Create))
                {
                    using (BinaryReader br = new BinaryReader(res.GetResponseStream()))
                    {
                        long remain = res.ContentLength;
                        while (remain > 0)
                        {
                            int readSize = 0;

                            // 読み取り用のバッファを用意する
                            byte[] buf = new byte[Math.Min(1024, remain)];
                            readSize = br.Read(buf, 0, buf.Length);
                            fileStrm.Write(buf, 0, readSize);
                            remain -= readSize;
                        }
                    }
                }
            }
        }
        #endregion
    }
}
