using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid.Components
{
    public class Header : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Header()
          : base(Title.header[0], Title.header[1], Title.header[2], Title.header[3], Title.header[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter(Info.heatedBed[0], Info.heatedBed[1], Info.heatedBed[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.heatedBedTemp[0], Info.heatedBedTemp[1], Info.heatedBedTemp[2], GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter(Info.extruderTemp[0], Info.extruderTemp[1], Info.extruderTemp[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.fan[0], Info.fan[1], Info.fan[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(Info.header[0], Info.header[1], Info.header[2], GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool heatBedB = new bool();
            int fan = new int();
            int heatBedT = new int();
            int extrudeT = new int();

            if (!DA.GetData(0, ref heatBedB)) return;
            if (!DA.GetData(1, ref heatBedT)) return;
            if (!DA.GetData(2, ref extrudeT)) return;
            if (!DA.GetData(3, ref fan) | (fan < 0) | (fan > 100)) return;

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