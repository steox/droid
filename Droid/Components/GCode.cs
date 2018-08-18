using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid
{
    public class GCode : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GCode()
          : base("Droid Gcode Creator", "GCode",
              "Creates Gcode infomation from Droid components",
              "Droid", "Gcode")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Paths", "-> DP", "Input Droid Paths", GH_ParamAccess.item);
            pManager.AddGenericParameter("Droid Parameters", "-> DPr", "Input Droid Parameters", GH_ParamAccess.item);
            pManager.AddTextParameter("Droid Header", "-> H", "Input Header Text (from Droid component, or user custom text)", GH_ParamAccess.list);
            pManager.AddTextParameter("Droid Footer", "-> F", "Input Footer Text (from Droid component, or user custom text)", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Gcode", "GC ->", "Gcode as text", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DroidPaths dPath = new DroidPaths();
            DroidParameters dPara = new DroidParameters();
            List<string> head = new List<string>();
            List<string> foot = new List<string>();

            if (!DA.GetData(0, ref dPath)) return;
            if (!DA.GetData(1, ref dPara)) return;
            if (!DA.GetDataList(2, head)) return;
            if (!DA.GetDataList(3, foot)) return;

            List<string> gcode = new List<string>();
            DroidGCode output = new DroidGCode(dPath.printList);

            gcode = output.Execute(dPara, head, foot);

            DA.SetDataList(0, gcode);
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
                return Properties.Resources.gcode;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("278478c4-8e11-445d-b0d1-59c44a04c334"); }
        }
    }
}