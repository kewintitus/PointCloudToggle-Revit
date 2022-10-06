using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.PointClouds;

namespace PointCloudToggle
{
    [TransactionAttribute(TransactionMode.Manual)]
    public class Class1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            PointCloudOverrides pointCloudOverrides = doc.ActiveView.GetPointCloudOverrides();
            PointCloudOverrideSettings settings = new PointCloudOverrideSettings();
            
            FilteredElementCollector pointClouds = new FilteredElementCollector(doc);
            List<ElementId> pointCloudId = new List<ElementId>();
            ElementCategoryFilter pointCloudfilter = new ElementCategoryFilter(BuiltInCategory.OST_PointClouds);
            IList<Element> listPointClouds = pointClouds.WherePasses(pointCloudfilter).WhereElementIsNotElementType().ToElements();
            View currentView = uidoc.ActiveView;
            if (listPointClouds.Count==0)
            {
                IEnumerable<ElementId> hiddenPointClouds = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_PointClouds)
                    .WhereElementIsNotElementType().Where(x => x.IsHidden(currentView))
                .Select(x => x.Id);

                foreach(ElementId hiddenPointCloud in hiddenPointClouds)
                {
                    Element pC = doc.GetElement(hiddenPointCloud);
                    listPointClouds.Add(pC);
                }
                
            }
            foreach(Element pc in listPointClouds)
            {
                settings = pointCloudOverrides.GetPointCloudScanOverrideSettings(pc.Id);
                pointCloudId.Add(pc.Id);
            }
           
            try
            {

                using (Transaction transaction = new Transaction(doc, "PointCloudToggle"))
                {
                    transaction.Start();
                    //if (currentView.IsInTemporaryViewMode(TemporaryViewMode.RevealHiddenElements))
                    if (settings.Visible == false)
                    {
                        settings.Visible = true;
                        
                    }
                    else 
                    {
                        settings.Visible = false; 
                    }
                    foreach (ElementId pId in pointCloudId)
                    {
                        pointCloudOverrides.SetPointCloudScanOverrideSettings(pId, settings,"ToggleONorOFF",doc);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception e )
            {
                message = e.Message;
            }
            return Result.Succeeded;
        }
    }
}
