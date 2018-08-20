using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid.Components
{
    public class Footer : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Footer()
          : base(Title.footer[0], Title.footer[1], Title.footer[2], Title.footer[3], Title.footer[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter(Info.height[0], Info.height[1], Info.height[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(Info.footer[0], Info.footer[1], Info.footer[2], GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            double MZ = new double();

            if (!DA.GetData(0, ref MZ)) return;

            DroidFooter foot = new DroidFooter(MZ);

            DA.SetDataList(0, foot.footer);
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
                return Properties.Resources.foot;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("b2f0c976-2ca3-46de-8401-1d3bee414ae4"); }
        }
    }
}