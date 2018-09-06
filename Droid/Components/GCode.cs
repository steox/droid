using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid.Components
{
    public class GCode : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public GCode()
          : base(Title.gcode[0], Title.gcode[1], Title.gcode[2], Title.gcode[3], Title.gcode[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.paths[0], Info.paths[1], Info.paths[2], GH_ParamAccess.item);
            pManager.AddGenericParameter(Info.parameters[0], Info.parameters[1], Info.parameters[2], GH_ParamAccess.item);
            pManager.AddTextParameter(Info.header[0], Info.header[1], Info.header[2], GH_ParamAccess.list);
            pManager.AddTextParameter(Info.footer[0], Info.footer[1], Info.footer[2], GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(Info.gcode[0], Info.gcode[1], Info.gcode[2], GH_ParamAccess.list);
            pManager.AddTextParameter(Info.printInfo[0], Info.printInfo[1], Info.printInfo[2], GH_ParamAccess.list);
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
            List<string> info = new List<string>();
            DroidGCode output = new DroidGCode(dPath.printList);

            gcode = output.Execute(dPara, head, foot);
            info = output.Info(dPara);

            DA.SetDataList(0, gcode);
            DA.SetDataList(1, info);
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