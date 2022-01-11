using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Model.DB
{
    public class Project
    {
        public string assess_link { get; set; }
        public string assess_name { get; set; }
        public string assist_leader { get; set; }
        public string construction_link { get; set; }
        public string construction_name { get; set; }
        public string coordinate { get; set; }
        public string create_by { get; set; }
        public DateTime create_time { get; set; }
        public string group_leader { get; set; }
        public string id { get; set; }
        public string order_code { get; set; }
        public string owner_link { get; set; }
        public string owner_name { get; set; }
        public string plan_img { get; set; }
        public DateTime project_approvaltime { get; set; }
        public DateTime project_closingtime { get; set; }
        public string project_iskeyproject { get; set; }
        public string project_leader { get; set; }
        public string project_name { get; set; }
        public string project_personincharge { get; set; }
        public string project_projectdescription { get; set; }
        public string project_remarks { get; set; }
        public string project_teammembers { get; set; }
        public decimal project_totalamount { get; set; }
        public decimal project_totalarea { get; set; }
        public string proxy_company { get; set; }
        public string survey_link { get; set; }
        public string survey_name { get; set; }
        public string sys_org_code { get; set; }
        public string update_by { get; set; }
        public DateTime update_time { get; set; }
        public decimal work_amount { get; set; }
    }
}
