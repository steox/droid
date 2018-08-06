using System;

using Grasshopper.Kernel;

using DroidLib;

namespace Droid
{
    public class Parameters : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Parameters()
          : base("Droid Parameters", "DPmters",
              "Custom Parameter creation for Droid",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("Layer Height", "LH", "Layer Height Resolution", GH_ParamAccess.item);
            pManager.AddNumberParameter("First Layer Height", "FLH", "First Layer Height Resolution", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Precision", "P", "Precision of sliced geometry (Whole Number [int]). Default value is 128", GH_ParamAccess.item, 128);
            pManager.AddNumberParameter("Nozzle", "N", "Nozzle diameter, in millimeters", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Infill", "I", "Infill percentage (0% - 99%) (Whole Number [int])", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Shell Number", "S", "Outer Shell thickness, in Nozzle thickness (Shell Thickness = Nozzle diameter * Shell Number). Default = 1", GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter("Brim / Skirt", "BS", "For Brim = True, for Skirt = False. Default is False", GH_ParamAccess.item, false);
            pManager.AddIntegerParameter("Brim / Skirt distance", "BSD", "For Brim : Number of Brim Offsets, For Skirt : Distance (in multiples of Nozzle Diameter) of Skirt Offset, For None : Parameter = 0 (default)", GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter("Top / Bottom", "TB", "Number of layers as Top and Bottom Caps", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Print Speed", "PS", "Speed of Printing in units per/s", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Travel Speed", "TS", "Speed of travelling when not printing in units per/s", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Retraction", "R", "Enable Retraction = true", GH_ParamAccess.item);
            pManager.AddNumberParameter("Rectraction Distance", "RD", "Retraction Distance in millimeters", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Retraction Speed", "RS", "Feedrate of Retraction in units per/s", GH_ParamAccess.item);
            pManager.AddNumberParameter("Filament Diameter", "F", "Filament Diameter, in millimeters. Also Extrusion volume (mm^2) per mm of extrusion for large scale extruders", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Paramerters", "DPr ->", "Parameter Data for Droid", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // Inputs
            double layerHeight = new double();
            double firstLayerHeight = new double();
            int scale = new int();
            double nozzle = new double();
            int infillPercent = new int();
            int shellNumber = new int();
            bool brimSkirt = new bool();
            int brimSkirtInt = new int();
            int capThickness = new int();
            int printSpeed = new int();
            int travelSpeed = new int();
            bool retraction = new bool();
            double retractionDistance = new double();
            int rectractionSpeed = new int();
            double filamentDiameter = new double();

            if (!DA.GetData(0, ref layerHeight) | (layerHeight <= 0)) return;
            if (!DA.GetData(1, ref firstLayerHeight) | (firstLayerHeight <= 0)) return;
            if (!DA.GetData(2, ref scale) | (scale <= 0)) return;
            if (!DA.GetData(3, ref nozzle) | (nozzle <= 0.0)) return;
            if (!DA.GetData(4, ref infillPercent) | (infillPercent < 0) | (infillPercent > 99)) return;
            if (!DA.GetData(5, ref shellNumber) | (shellNumber <= 0)) return;
            if (!DA.GetData(6, ref brimSkirt)) return;
            if (!DA.GetData(7, ref brimSkirtInt) | (brimSkirtInt < 0)) return;
            if (!DA.GetData(8, ref capThickness) | (capThickness < 0)) return;
            if (!DA.GetData(9, ref printSpeed) | (printSpeed < 0)) return;
            if (!DA.GetData(10, ref travelSpeed) | (travelSpeed < 0)) return;
            if (!DA.GetData(11, ref retraction)) return;
            if (!DA.GetData(12, ref retractionDistance) | (retractionDistance < 0)) return;
            if (!DA.GetData(13, ref rectractionSpeed) | (rectractionSpeed < 0)) return;
            if (!DA.GetData(14, ref filamentDiameter) | (filamentDiameter < 0)) return;

            DroidParameters parameters = new DroidParameters();

            parameters.layerHeight = layerHeight;
            parameters.firstLayerHeight = firstLayerHeight;
            parameters.scale = scale;
            parameters.nozzle = nozzle;
            parameters.infillPercent = infillPercent;
            parameters.shellNumber = shellNumber;
            parameters.brimSkirt = brimSkirt;
            parameters.brimSkirtInt = brimSkirtInt;
            parameters.capThickness = capThickness;
            parameters.printSpeed = printSpeed;
            parameters.travelSpeed = travelSpeed;
            parameters.retraction = retraction;
            parameters.rectractionDistance = retractionDistance;
            parameters.retractionSpeed = rectractionSpeed;
            parameters.filamentDiameter = filamentDiameter;

            DA.SetData(0, parameters);
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
                return Properties.Resources.para;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("345535c4-2fd8-484e-8e6e-8006d9b17e32"); }
        }
    }
}