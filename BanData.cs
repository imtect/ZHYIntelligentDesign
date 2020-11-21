using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadPlugins
{
    public class BanData
    {
        public int code; //幅板的ID
        public double shortEdge; //短边的长度
        public double longEdge; //长边的长度
        public double width; //宽度
        public BanType type;
        public int count;
    }
    public enum BanType
    {
        BanLining, //中幅板
        ThreeQuartersBan, //四分之三板
        HalfBan, //二分之一板
        AbnormalBan //异形板
    }
}
