using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //菜单类
    public class Menu
    {
        public int Id { get; set; }//id
        public int Pid { get; set; }//父节点id
        public int Sort { get; set; }//排序
        public string Icon { get; set; }//美化图标
        public string Name { get; set; }//菜单名
        public string Path { get; set; }//跳转路径
        public string Class { get; set; }//菜单类型
        public List<Menu> Sun { get; set; }//子菜单
    }
}