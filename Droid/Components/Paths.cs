using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid.Components
{
    public class Paths : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Paths()
          : base(Title.paths[0], Title.paths[1], Title.paths[2], Title.paths[3], Title.paths[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter(Info.polylines[0], Info.polylines[1], Info.polylines[2], GH_ParamAccess.list);
            pManager.AddBooleanParameter(Info.sortZ[0], Info.sortZ[1], Info.sortZ[2], GH_ParamAccess.item, false);
            pManager.AddGenericParameter(Info.droidVolume[0], Info.droidVolume[1], Info.droidVolume[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.paths[0], Info.paths[1], Info.paths[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Curve> contour = new List<Curve>();
            bool sort = new bool();
            DroidVolume dv = new DroidVolume();

            if (!DA.GetDataList(0, contour)) return;
            if (!DA.GetData(1, ref sort)) return;
            if (!DA.GetData(2, ref dv)) return;

            DroidPaths myDroid = new DroidPaths(contour, sort, dv);

            DA.SetData(0, myDroid);
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
                return Properties.Resources.path;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("44ef37c7-bf02-460c-a4c2-20fd8b071965"); }
        }
    }
}