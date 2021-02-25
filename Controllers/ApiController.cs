using System;
using Microsoft.AspNetCore.Mvc;
using ReportServer.models;
using grsvr6Lib;

namespace ReportServer.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api")]
    public class ReportServerController : ControllerBase
    {
        public GridppReportServer report = new GridppReportServer();
        public ReportGenerateInfo GenerateInfo = new ReportGenerateInfo();
        public Request request = new Request();

        /// <summary>打印参数</summary>
        private ReportParam paramModel = new ReportParam();

        /// <summary>报表载入是否成功</summary>
        private bool loadGrf = false;

        /// <summary>报表数据载入是否成功</summary>
        private bool loadData = false;

        [HttpPost("[action]")]
        public DataResult Document(ReportParam param)
        {
            paramModel = param;

            if (param.grfUrl != null)
            {
                // 从指定的 URL 地址载入报表模板数据
                loadGrf = report.LoadFromURL(param.grfUrl);
            } 
            else
            {
                // 从字符串中载入报表模板数据
                loadGrf = report.LoadFromStr(param.grfStr);
            }

            if (!loadGrf) return request.Error("载入报表文件出错，请检查报表模板数据是否正确！");

            if (param.grfDataUrl != null)
            {
                // 从指定的 URL 地址载入报表明细数据
                loadData = report.LoadDataFromURL(param.grfDataUrl);
            } 
            else
            {
                // 从 XML 或 JSON 文字串中载入报表明细记录集数据，数据应符合约定的形式。
                loadData = report.LoadDataFromXML(paramModel.grfData);
            }

            if (!loadData) return request.Error("载入报表数据出错，请检查报表数据地址是否正确！");


            //确定导出数据类型及数据的ContentType
            GenerateInfo.Build(param.type, "png");

            IGRBinaryObject ResultDataObject;
            if (GenerateInfo.IsGRD)
            {
                ResultDataObject = report.GenerateDocumentData();
            }
            else
            {
                IGRExportOption ExportOption = report.PrepareExport(GenerateInfo.ExportType);

                if (GenerateInfo.ExportType == GRExportType.gretIMG)
                {
                    IGRE2IMGOption E2IMGOption = ExportOption.AsE2IMGOption;
                    E2IMGOption.ImageType = GenerateInfo.ImageType;
                    E2IMGOption.AllInOne = true; //所有页产生在一个图像文件中
                    E2IMGOption.DPI = param.dpi; //指定导出图像的分辨率DPI
                    E2IMGOption.DrawPageBox = false; //指定是否在导出的图像上绘制一个页边框
                    //E2IMGOption.VertGap = 20;    //页之间设置20个像素的间距
                } else if (GenerateInfo.ExportType == GRExportType.gretPDF)
                {
                    IGRE2PDFOption E2PDFOption = ExportOption.AsE2PDFOption;
                    E2PDFOption.AnsiTextMode = false; //指定是否将文本数据编码为ANSI字符方式
                    E2PDFOption.Compressed = false; //指定是否对 PDF 页面数据进行压缩
                }

                ResultDataObject = report.ExportToBinaryObject();
                report.UnprepareExport();
            }

            object Data = ResultDataObject.SaveToVariant();
            string DocumentData = Convert.ToBase64String((byte[])Data);

            return request.Succes(DocumentData, "生成数据成功");
        }
    }
}
