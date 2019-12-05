using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZEQP.Print.Framework
{
    public class PrintModel
    {
        /// <summary>
        /// 打印机名称
        /// </summary>
        public string PrintName { get; set; }

        //打印份数
        public int Copies { get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 内容数据源
        /// </summary>
        public Dictionary<string, string> DicCotent { get; set; }
        public PrintModel()
        {
            this.Copies = 1;
            this.DicCotent = new Dictionary<string, string>();
        }
    }
}
