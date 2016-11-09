using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.ItemSources
{
    public static class FontFamilyItemSource
    {
        private static List<System.Windows.Media.FontFamily> _Fonts;

        /// <summary>
        /// システムにインストールされたフォントファミリを取得
        /// </summary>
        /// <returns></returns>
        public static List<System.Windows.Media.FontFamily> Get()
        {
            if (FontFamilyItemSource._Fonts != null)
            {
                return FontFamilyItemSource._Fonts;
            }

            var list = new List<System.Windows.Media.FontFamily>();
            try
            {
                //InstalledFontCollectionオブジェクトの取得
                System.Drawing.Text.InstalledFontCollection ifc =
                    new System.Drawing.Text.InstalledFontCollection();

                //インストールされているすべてのフォントファミリを取得
                System.Drawing.FontFamily[] ffs = ifc.Families;

                // 日本語のCulture ID取得
                int cultureId = System.Globalization.CultureInfo.GetCultureInfo("ja-JP").LCID;

                foreach (var ff in ffs)
                {
                    list.Add(new System.Windows.Media.FontFamily(ff.GetName(cultureId)));
                }
            }
            catch (Exception)
            {
            }

            FontFamilyItemSource._Fonts = list;

            return list;
        }
    }
}
