using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;

namespace PointCloudToggle
{
    class ExternalApp:IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            application.CreateRibbonTab("PointCloud_Tools");

            string path = Assembly.GetExecutingAssembly().Location;
            PushButtonData button = new PushButtonData("Toggle_Button", "PointCloud ON/OFF", path, "PointCloudToggle.Class1");

            RibbonPanel panel = application.CreateRibbonPanel("PointCloud_Tools", "PointCloudON/OFF");

            // string iconName = "Plugin001.Images.3DBoxIcon.png";
            //string iconPath = Path.GetFullPath(iconName); 

            Uri imagePath = new Uri(@"pack:\\application:,,,/PointCloudToggle;component/Images/PC_ICON_1.png");
            BitmapImage image = new BitmapImage(imagePath);

            PushButton pushButton = panel.AddItem(button) as PushButton;
            pushButton.LargeImage = image;

            return Result.Succeeded;
        }
    }
}
