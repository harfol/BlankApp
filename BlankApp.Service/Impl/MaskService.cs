using BlankApp.Service.Model.Object;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlankApp.Service.Impl
{
    public class MaskService : IMaskService
    {

        IConfigurationService _configurationService;
        public MaskService(IConfigurationService configurationService)
        {
            this._configurationService = configurationService;
        }
        public string Complex(string str)
        {
            TitleObject[] titles = _configurationService["Titles"] as TitleObject[];
            TitleObject title = titles.Where(t => t.Mask.Contains(str)).FirstOrDefault();

            return title?.Name ?? null;
        }



        public string Simplify(string str)
        {
            TitleObject[] titles = _configurationService["Titles"] as TitleObject[];
            TitleObject title =  titles.Where(t => t.Name.Contains(str)).FirstOrDefault();

            return title?.Mask ?? null;
        }


        public int[] ComplexNumbers(string str)
        {
            string[] row = str.Split(',');
            int[][] nums = new int[row.Length][];
            for (int i = 0; i < row.Length; i++)
            {
                string[] split = row[i].Split('-');
                nums[i] = Array.ConvertAll<string, int>(split, int.Parse);
            }

            var ns = new List<int>();
            for (int i = 0; i < nums.Length; i++)
            {
                if( nums[i].Length > 1)
                {
                    for (int j = nums[i][0]; j <= nums[i][1]; j++)
                    {
                        ns.Add(j);
                    }
                }
                else if(nums[i].Length == 1)
                {
                    ns.Add(nums[i][0]);
                }
            }
            int[] s = ns.ToArray();
            Array.Sort(s);
            return s;
        }

        public string SimplifiedNumbers(int[] nums)
        {
            Array.Sort(nums);
            string ret = nums[0].ToString();
            for (int i = 1; i < nums.Length-1; i++)
            {
                if(nums[i]+1 != nums[i + 1])
                {
                    ret += $"-{nums[i]},{nums[i+1]}";
                    i++;
                }
            }
            if(nums[nums.Length-1]-1 != nums[nums.Length-2])
            {
                ret += $"-{nums.Last()}";
            }
            return ret;
        }

    }
}
