using System;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid.Components
{
    public class DVolume : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public DVolume()
          : base(Title.dVolume[0], Title.dVolume[1], Title.dVolume[2], Title.dVolume[3], Title.dVolume[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter(Info.diameter[0], Info.diameter[1], Info.diameter[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.height[0], Info.height[1], Info.height[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.droidVolume[0], Info.droidVolume[1], Info.droidVolume[2], GH_ParamAccess.item);
            pManager.AddCurveParameter(Info.preview[0], Info.preview[1], Info.preview[2], GH_ParamAccess.list);
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