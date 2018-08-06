using System;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid
{
    public class CVolume : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public CVolume()
          : base("Droid Volume (Cartesian)", "DVolC",
              "Defines the Volume of the printable area of machine",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Width", "X", "X-Width of Print Area", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth", "Y", "Y-Depth of Print Area", GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "Z", "Z-Height of Print Area", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Volume", "DV ->", "Droid Volume for use for Droid Components", GH_ParamAccess.item);
            pManager.AddCurveParameter("Preview", "P", "Preview of Printable area", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double x = new double();
            double y = new double();
            double z = new double();

            if (!DA.GetData(0, ref x)) return;
            if (!DA.GetData(1, ref y)) return;
            if (!DA.GetData(2, ref z)) return;

            DroidVolume vol = new DroidVolume(x, y, z);

            DA.SetData(0, vol);
            DA.SetDataList(1, vol.volumeOutline);
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
                return Properties.Resources.vol;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2b7cab2b-d992-4887-a753-2c41dae8d602"); }
        }
    }
}