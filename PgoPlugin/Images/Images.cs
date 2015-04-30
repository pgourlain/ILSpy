using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Media.Imaging;

namespace PgoPlugin
{
    static class Images
    {
        static BitmapImage LoadBitmap(string imageName)
        {
            var name = Assembly.GetExecutingAssembly().GetName();
            var uri = new Uri("pack://application:,,,/" + name.Name + ";v" + name.Version + ";component/" + imageName + ".png");

            BitmapImage image = new BitmapImage(uri);
            image.Freeze();
            return image;
        }

        public static readonly BitmapImage Light = LoadBitmap("images/Light");
        public static readonly BitmapImage ComboButton = LoadBitmap("images/ComboButton");
    }
}
