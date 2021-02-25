using System;

namespace ReportServer.models
{
    public class ReportParam
    {
        /// <summary>
        /// 指定生成的数据类型
        /// 可选[pdf|xls|csv|txt|rtf|img|grd|grp]。如果不指定，默认为pdf
        /// </summary>
        public string type = "img";

        /// <summary>图片分辨率</summary>
        public int dpi = 300;

        /// <summary>载入报表的模板数据</summary>
        public string grfStr;

        /// <summary>载入报表的URL地址</summary>
        public string grfUrl;

        /// <summary>载入报表的数据</summary>
        public string grfData;

        /// <summary>载入报表数据的URL地址</summary>
        public string grfDataUrl;
    }

    public class DataResult
    {
        /// <summary>状态</summary>
        public bool status;

        /// <summary>数据</summary>
        public dynamic data;

        /// <summary>结果</summary>
        public string message;
    }
}
