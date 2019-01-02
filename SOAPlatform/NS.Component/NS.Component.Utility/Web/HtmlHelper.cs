using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace NS.Component.Utility
{
    public class HtmlHelper
    {
        /// <summary>
        /// 返回是否显示列表
        /// </summary>
        /// <param name="isDefault">如果为true多一项(请选择)</param>
        /// <returns></returns>
        public static IList<SelectListItem> GetDisplayList(bool isDefault = false)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            if (isDefault)
            {
                list.Add(new SelectListItem { Text = "请选择", Value = "all" });
            }
            list.Add(new SelectListItem { Text = "显示", Value = "True" });
            list.Add(new SelectListItem { Text = "不显示", Value = "False" });
            return list;
        }

        /// <summary>
        /// 绑定数据字典分类列表
        /// </summary>
        /// <param name="isDefault"></param>
        /// <returns></returns>
        public static IList<SelectListItem> GetDictTypeList(bool isDefault = false)
        {
            IList<SelectListItem> list = new List<SelectListItem>();
            if (isDefault)
            {
                list.Add(new SelectListItem { Text = "请选择", Value = "all" });
            }
            list.Add(new SelectListItem { Text = "学历", Value = Consts.DICTYPE_EDUCATION_FLAG });
            return list;
        }
    }
}
