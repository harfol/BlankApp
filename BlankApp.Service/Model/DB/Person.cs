using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Model.DB
{
    public class Person
    {
        public string create_by { get; set; }
        public DateTime create_time { get; set; }
        public string id { get; set; }
        public string id_card { get; set; }
        public string measure_id { get; set; }
        public string person_address { get; set; }
        public string person_name { get; set; }
        public string person_phone { get; set; }
        public string person_remarks { get; set; }
        public int person_sign_yn { get; set; }
        public int person_status { get; set; }
        public int person_type_id { get; set; }
        public string project_id { get; set; }
        public string project_villagelist_id { get; set; }
        public string relation { get; set; }
        public string sys_org_code { get; set; }
        public string update_by { get; set; }
        public DateTime update_time { get; set; }
    }
}
