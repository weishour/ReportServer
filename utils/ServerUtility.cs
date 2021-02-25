using grsvr6Lib;
using ReportServer.models;

namespace ReportServer
{
    public class ReportGenerateInfo
    {
        public string ContentType;          //HTTP响应ContentType
        public string ExtFileBame;          //默认扩展文件名
        public bool IsGRD;                  //是否生成为 Grid++Report 报表文档格式
        public GRExportType ExportType;     //导出的数据格式类型
        public GRExportImageType ImageType; //导出的图像格式类型

        ///根据报表导出格式类型，生成对应的响应信息，将结果信息保存本类的成员变量中
        ///参数 ExportTypeText: 指定报表导出的导出格式类型
        ///参数 ImageTypeText: 指定生成的图像格式，仅当为导出图像时有效
        public void Build(string ExportTypeText, string ImageTypeText)
        {
            ExtFileBame = ExportTypeText;
            ContentType = "application/";
            IsGRD = (ExportTypeText == "grd" || ExportTypeText == "grp");

            if (IsGRD)
            {
                //ContentType += ExportTypeText; //application/grd
                ContentType += "octet-stream"; //application/octet-stream
            }
            else
            {
                switch (ExportTypeText)
                {
                    case "xls":
                        ExportType = GRExportType.gretXLS;
                        ContentType += "x-xls"; //application/vnd.ms-excel application/x-xls
                        break;
                    case "csv":
                        ExportType = GRExportType.gretCSV;
                        ContentType += "vnd.ms-excel"; //application/vnd.ms-excel application/x-xls
                        break;
                    case "txt":
                        ExportType = GRExportType.gretTXT;
                        ContentType = "text/plain"; //text/plain
                        break;
                    case "rtf":
                        ExportType = GRExportType.gretRTF;
                        ContentType += "rtf"; //application/rtf
                        break;
                    case "img":
                        ExportType = GRExportType.gretIMG;
                        //ContentType 要在后面根据图像格式来确定
                        break;
                    default:
                        ExtFileBame = "pdf"; //"type"参数如没有设置，保证 ExtFileBame 被设置为"pdf"
                        ExportType = GRExportType.gretPDF;
                        ContentType += "pdf";
                        break;
                }

                //导出图像处理
                if (ExportType == GRExportType.gretIMG)
                {
                    ExtFileBame = ImageTypeText;
                    switch (ImageTypeText)
                    {
                        case "bmp":
                            ImageType = GRExportImageType.greitBMP;
                            ContentType += "x-bmp";
                            break;
                        case "jpg":
                            ImageType = GRExportImageType.greitJPEG;
                            ContentType += "x-jpg";
                            break;
                        case "tif":
                            ImageType = GRExportImageType.greitTIFF;
                            ContentType = "image/tiff";
                            break;
                        default:
                            ExtFileBame = "png";
                            ImageType = GRExportImageType.greitPNG;
                            ContentType += "x-png";
                            break;
                    }
                }
            }
        }
    }

    public class Request
    {
        /// <summary>响应结果</summary>
        public DataResult dataResult = new DataResult();

        /// <summary>正确处理</summary>
        /// <param name="data">数据</param>
        /// <param name="message">正确信息</param>
        public DataResult Succes(dynamic data, string message)
        {
            dataResult.status = true;
            dataResult.data = data;
            dataResult.message = message;

            return dataResult;
        }

        /// <summary>错误处理</summary>
        /// <param name="message">错误信息</param>
        public DataResult Error(string message)
        {
            dataResult.status = false;
            dataResult.message = message;

            return dataResult;
        }
    }
}
