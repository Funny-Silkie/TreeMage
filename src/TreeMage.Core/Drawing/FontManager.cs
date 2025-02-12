using PdfSharpCore.Drawing;
using PdfSharpCore.Fonts;
using PdfSharpCore.Utils;
using SixLabors.Fonts;
using System.Globalization;
using System.Runtime.InteropServices;

namespace TreeMage.Core.Drawing
{
    /// <summary>
    /// フォントを扱うクラスです。
    /// </summary>
    public static class FontManager
    {
        private static readonly Dictionary<int, Font> imageSharpFontCache = [];
        private static readonly Dictionary<XFontInfo, XFont> pdfFontCache = [];

        /// <summary>
        /// 標準のフォントファミリーを取得または設定します。
        /// </summary>
        public static string DefaultFontFamily
        {
            get => _defaultFontFamily;
            set
            {
                if (string.Equals(_defaultFontFamily, value, StringComparison.Ordinal)) return;
                _defaultFontFamily = value;

                imageSharpFontCache.Clear();
                pdfFontCache.Clear();
            }
        }

        private static string _defaultFontFamily;

        static FontManager()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) _defaultFontFamily = "Arial";
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) _defaultFontFamily = CultureInfo.CurrentCulture.Name switch
            {
                "ja-JP" => "Noto Sans CJK JP",
                _ => "Ubuntu",
            };
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) _defaultFontFamily = "Arial";
            else _defaultFontFamily = "Arial";

            GlobalFontSettings.FontResolver = new FontResolver();
        }

        /// <summary>
        /// 指定したサイズの<see cref="Font"/>を取得します。
        /// </summary>
        /// <param name="fontSize">フォントサイズ</param>
        /// <returns><paramref name="fontSize"/>に対応するフォント</returns>
        public static Font GetImageSharpFont(int fontSize)
        {
            if (!imageSharpFontCache.TryGetValue(fontSize, out Font? result))
            {
                result = SystemFonts.CreateFont(DefaultFontFamily, fontSize);
                imageSharpFontCache.Add(fontSize, result);
            }
            return result;
        }

        /// <summary>
        /// 指定したスタイルの<see cref="XFont"/>を取得します。
        /// </summary>
        /// <param name="fontSize">フォントサイズ</param>
        /// <returns>指定したスタイルに対応するフォント</returns>
        public static XFont GetPdfFont(int fontSize, XFontStyle style = XFontStyle.Regular)
        {
            if (!pdfFontCache.TryGetValue(new XFontInfo(fontSize, style), out XFont? result))
            {
                result = new XFont(DefaultFontFamily, fontSize, style, XPdfFontOptions.UnicodeDefault);
                pdfFontCache.Add(new XFontInfo(fontSize, style), result);
            }
            return result;
        }
    }

    /// <summary>
    /// <see cref="XFont"/>の生成情報を表します。
    /// </summary>
    /// <param name="FontSize">フォントサイズを取得します。</param>
    /// <param name="Style">フォントのスタイルを取得します。</param>
    internal record struct XFontInfo(int FontSize, XFontStyle Style);
}
