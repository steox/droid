using System;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid
{
    public class DVolume : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public DVolume()
          : base("Droid Volume (Delta)", "DVolD",
              "Defines the Volume of the printable area of machine",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Diameter", "D", "Diameter of Print Area", GH_ParamAccess.item);
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
            double diameter = new double();
            double z = new double();

            if (!DA.GetData(0, ref diameter)) return;
            if (!DA.GetData(1, ref z)) return;

            DroidVolume vol = new DroidVolume(diameter, z);

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
            get { return new Guid("5cb860e0-05de-43b0-b0b1-49c1adb25bf9"); }
        }
    }
}