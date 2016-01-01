using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace QuartetEditor.Views.Converters
{
    /// <summary>
    /// フォントファミリ名に変換
    /// </summary>
    public class FontFamilyToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                var v = value as FontFamily;
                var currentLang = XmlLanguage.GetLanguage(culture.IetfLanguageTag);
                return v.FamilyNames.FirstOrDefault(o => o.Key == currentLang).Value ?? v.Source;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
