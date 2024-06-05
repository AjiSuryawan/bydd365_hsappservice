using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bydd365_hsappservice.models
{
    internal class EmailConfig
    {
        [JsonProperty("sessionid_")]
        public string Sessionid { get; set; }
        public string InterfaceName { get; set; }
        public string CompanyId { get; set; }
        public string SmtpType { get; set; }
        public HoyaSmtp HoyaSmtp { get; set; }
        public SmtpStorageAttachment SmtpStorageAttachment { get; set; }
    }

    public class HoyaSmtp
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPassword { get; set; }
        public string SmtpFrom { get; set; }
        public object SmtpFromAlias { get; set; }
        public string SmtpTo { get; set; }
        public string SmtpCc { get; set; }
        public string SmtpSubject { get; set; }
        public string SmtpBody { get; set; }
        public int IsHtml { get; set; }
    }

    public class SmtpStorageAttachment
    {
        public string Path { get; set; }
        public string Key { get; set; }
        public string Account { get; set; }
        public int HasAttach { get; set; }
        public string FolderPath { get; set; }
        public List<string> Files { get; set; }
    }
}
