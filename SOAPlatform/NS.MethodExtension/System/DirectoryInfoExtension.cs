#region << 版 本 注 释 >>
/*
*======================================================================
* Copyright(c) 北京东方正通科技有限公司, All Rights Reserved.
*======================================================================
*
* 【当前类文件的功能】
*
*
* 作者： 何戈洲 时间：2012/7/26 17:41:30
* 文件名：DirectoryInfoExtension
* 版本：V1.0.0
*
*修改者：           时间：
*
* 修改说明：
* ========================================================================
*/
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class DirectoryInfoExtension
    {
        public static string[] GetFiles(this System.IO.DirectoryInfo directoryInfo,string sourceFolder,string filter,System.IO.SearchOption searchOption)
        {
            List<string> fileInfos = new List<string>();

            // Create an array of filter string
            string[] multipleFilters = filter.Split('|');

            // for each filter find mathing file names
            foreach (string fileFilter in multipleFilters)
            {
                // add found file names to array list
                fileInfos.AddRange(Directory.GetFiles(sourceFolder, fileFilter, searchOption));
            }

            return fileInfos.ToArray();
        }
    }
}
