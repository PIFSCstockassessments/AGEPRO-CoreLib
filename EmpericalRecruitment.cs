using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO_struct
{
    public class EmpericalRecruitment : Recruitment
    {
        public EmpericalRecruitment(int typeNum)
        {
            this.recruitType = typeNum.ToString();
        }
    }
}
