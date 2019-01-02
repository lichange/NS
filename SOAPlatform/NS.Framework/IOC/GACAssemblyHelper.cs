using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace NS.Framework
{
    public class GACAssemblyHelper
    {
        public static IList<string> GetAssemblys()
        {
            return GacList.GetAssemblys();
        }
    }

    [ComImport]
    [Guid("21b8916c-f28e-11d2-a473-00c04f8ef448")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyEnum
    {

        IntPtr GetNextAssembly(
            /* [in] */ IntPtr pvReserved,
            /* [out] */ out IAssemblyName ppName,
            /* [in] */ uint dwFlags);

        void Reset();

        void Clone(
            /* [out] */ out IAssemblyEnum ppEnum);

    }

    [ComImport]
    [Guid("CD193BC0-B4BC-11d2-9833-00C04FC31D2E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAssemblyName
    {
        void SetProperty(
            /* [in] */ uint PropertyId,
            /* [in] */ IntPtr pvProperty,
            /* [in] */ uint cbProperty);

        void GetProperty(
            /* [in] */ uint PropertyId,
            /* [out] */ IntPtr pvProperty,
            /* [out][in] */ out uint pcbProperty);

        void Finalize();

        void GetDisplayName(
            /* [out] */ StringBuilder szDisplayName,
            /* [out][in] */ ref uint pccDisplayName,
            /* [in] */ uint dwDisplayFlags);

        void Reserved(
            /* [in] */ ref Guid refIID,
            /* [in] */ IntPtr pUnkReserved1,
            /* [in] */ IntPtr pUnkReserved2,
            /* [in] */ string szReserved,
            /* [in] */ Int64 llReserved,
            /* [in] */ IntPtr pvReserved,
            /* [in] */ uint cbReserved,
            /* [out] */ IntPtr ppReserved);

        void GetName(
            /* [out][in] */ ref uint lpcwBuffer,
            /* [out] */ StringBuilder pwzName);

        void GetVersion(
            /* [out] */ out uint pdwVersionHi,
            /* [out] */ out uint pdwVersionLow);

        void IsEqual(
            /* [in] */ IAssemblyName pName,
            /* [in] */ uint dwCmpFlags);

        void Clone(
            /* [out] */out IAssemblyName pName);

    }

    public enum ASM_DISPLAY_FLAGS
    {
        ASM_DISPLAYF_VERSION = 0x01,
        ASM_DISPLAYF_CULTURE = 0x02,
        ASM_DISPLAYF_PUBLIC_KEY_TOKEN = 0x04,
        ASM_DISPLAYF_PUBLIC_KEY = 0x08,
        ASM_DISPLAYF_CUSTOM = 0x10,
        ASM_DISPLAYF_PROCESSORARCHITECTURE = 0x20,
        ASM_DISPLAYF_LANGUAGEID = 0x40,
        ASM_DISPLAYF_RETARGET = 0x80,
        ASM_DISPLAYF_CONFIG_MASK = 0x100,
        ASM_DISPLAYF_MVID = 0x200,
        ASM_DISPLAYF_FULL =
                          ASM_DISPLAYF_VERSION |
                          ASM_DISPLAYF_CULTURE |
                          ASM_DISPLAYF_PUBLIC_KEY_TOKEN |
                          ASM_DISPLAYF_RETARGET
    } ;

    internal class GacList
    {
        [DllImport("fusion.dll")]
        static extern IntPtr CreateAssemblyEnum(out IAssemblyEnum pEnum, IntPtr pUnkReserved, IntPtr pName, uint dwFlags, IntPtr pvReserved);
        const uint ASM_CACHE_ZAP = 0x01;
        const uint ASM_CACHE_GAC = 0x02;
        const uint ASM_CACHE_DOWNLOAD = 0x04;
        const uint ASM_CACHE_ROOT = 0x08;

        public static IList<string> GetAssemblys()
        {
            IList<string> assemblyList = new List<string>();

            IAssemblyEnum gacEnum;
            //创建GAC程序集的枚举器
            CreateAssemblyEnum(out gacEnum, IntPtr.Zero, IntPtr.Zero, ASM_CACHE_GAC, IntPtr.Zero);
            {
                IAssemblyName asm;
                gacEnum.GetNextAssembly(IntPtr.Zero, out asm, 0);

                while (asm != null)
                {
                    StringBuilder sbuf = new StringBuilder(1024);

                    //uint ccbuf = 1024;
                    ////获取程序集显示名称
                    //asm.GetName(ref ccbuf, sbuf);

                    uint pccDisplayName = 4096;

                    uint flag = (uint)ASM_DISPLAY_FLAGS.ASM_DISPLAYF_FULL;

                    //获取程序集显示名称
                    asm.GetDisplayName(sbuf, ref pccDisplayName, flag);

                    //添加程序集名称到集合中
                    assemblyList.Add(sbuf.ToString());

                    //释放COM对象
                    Marshal.ReleaseComObject(asm);
                    //枚举下一个
                    gacEnum.GetNextAssembly(IntPtr.Zero, out asm, 0);
                }
            }
            Marshal.ReleaseComObject(gacEnum);

            return assemblyList;
        }
    }
}
