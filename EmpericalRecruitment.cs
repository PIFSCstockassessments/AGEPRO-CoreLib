using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGEPRO_struct
{
    public class EmpericalRecruitment : AGEPRO_Recruitment
    {
        public EmpericalRecruitment(int typeNum)
        {
            this.recruitType = typeNum.ToString();
        }
    }
}
