using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using DroidLib;

namespace Droid
{
    public class Save : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Save()
          : base("Droid Save Gcode", "SaveG",
              "Save Gcode File",
              "Droid", "Gcode")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Save", "SAVE", "Save. Add Grasshopper Boolean Button", GH_ParamAccess.item, false);
            pManager.AddTextParameter("Gcode", "-> GC", "Insert GCode text for saving", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            bool button = new bool();
            List<string> gcode = new List<string>();

            if (!DA.GetData(0, ref button)) return;
            if (!DA.GetDataList(1, gcode)) return;

            int counter = 0;

            if (button)
            {
                if (counter == 0)
                {
                    SaveGCode mySave = new SaveGCode(gcode);
                    mySave.Save();
                    counter++;
                }
            }

            if (!button)
            {
                counter = 0;
            }
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
                return Properties.Resources.save;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("bfec2404-d95c-4ce9-bb31-5da6084f9a6d"); }
        }
    }
}