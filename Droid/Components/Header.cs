using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid
{
    public class Header : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Header()
          : base("Droid Gcode Header", "DGHead",
              "Gcode Header creation",
              "Droid", "Gcode")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Has Heated Bed", "HB", "Option to use Heated Bed", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Heated Bed Temp", "HBT", "Set Heated Bed Temperature", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Extruder Temp", "ET", "Set Extruder Hotend Temperature", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Fan", "F", "Use Fan (Default = true)", GH_ParamAccess.item, true);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Header", "H ->", "Header Gcode", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool heatBedB = new bool();
            bool fan = new bool();
            int heatBedT = new int();
            int extrudeT = new int();

            if (!DA.GetData(0, ref heatBedB)) return;
            if (!DA.GetData(1, ref heatBedT)) return;
            if (!DA.GetData(2, ref extrudeT)) return;
            if (!DA.GetData(3, ref fan)) return;

            DroidHeader head = new DroidHeader(heatBedT, extrudeT, heatBedB, fan);

            DA.SetDataList(0, head.header);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return Properties.Resources.head;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("68d87950-1c9d-46b9-9997-d7ac7d5fb99e"); }
        }
    }
}