using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid.Components
{
    public class PrintPath : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public PrintPath()
          : base(Title.print[0], Title.print[1], Title.print[2], Title.print[3], Title.print[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.paths[0], Info.paths[1], Info.paths[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Print Paths", "PPath", "List of ordered polylines for print", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DroidPaths dPath = new DroidPaths();

            if (!DA.GetData(0, ref dPath)) return;

            List<Polyline> polylineList = new List<Polyline>();

            polylineList = dPath.printList;

            DA.SetDataList(0, polylineList);
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
                return Properties.Resources.slice;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fb7234e6-bc3b-4993-9284-aa2a33537b9a"); }
        }
    }
}