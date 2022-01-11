using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Model.DB
{
    public class Archive
    {
        public string archives_number { get; set; }
        public string archives_remark { get; set; }
        public string archives_type { get; set; }
        public string create_by { get; set; }
        public DateTime create_time { get; set; }
        public DateTime deadline { get; set; }
        public string directory_number { get; set; }
        public string enclosure { get; set; }
        public string explains { get; set; }
        public string file_number { get; set; }
        public string fond_number { get; set; }
        public string id { get; set; }
        public string merger_name { get; set; }
        public string person { get; set; }
        public string project_id { get; set; }
        public string sequence_number { get; set; }
        public string sort_number { get; set; }
        public int step { get; set; }
        public string storage_address { get; set; }
        public string sys_org_code { get; set; }
        public string update_by { get; set; }
        public DateTime update_time { get; set; }
    }
}
