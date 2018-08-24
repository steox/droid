using System;

using Grasshopper.Kernel;

using DroidLib;

namespace Droid.Components
{
    public class Parameters : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Parameters()
          : base(Title.parameters[0], Title.parameters[1], Title.parameters[2], Title.parameters[3], Title.parameters[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter(Info.layerHeight[0], Info.layerHeight[1], Info.layerHeight[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.firstLayerHeight[0], Info.firstLayerHeight[1], Info.firstLayerHeight[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.precision[0], Info.precision[1], Info.precision[2], GH_ParamAccess.item, 128);
            pManager.AddNumberParameter(Info.nozzle[0], Info.nozzle[1], Info.nozzle[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.infill[0], Info.infill[1], Info.infill[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.shell[0], Info.shell[1], Info.shell[2], GH_ParamAccess.item, 1);
            pManager.AddBooleanParameter(Info.skirt[0], Info.skirt[1], Info.skirt[2], GH_ParamAccess.item, false);
            pManager.AddIntegerParameter(Info.brimDist[0], Info.brimDist[1], Info.brimDist[2], GH_ParamAccess.item, 0);
            pManager.AddIntegerParameter(Info.cap[0], Info.cap[1], Info.cap[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.printSpeed[0], Info.printSpeed[1], Info.printSpeed[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.travelSpeed[0], Info.travelSpeed[1], Info.travelSpeed[2], GH_ParamAccess.item);
            pManager.AddBooleanParameter(Info.retraction[0], Info.retraction[1], Info.retraction[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.retractionDist[0], Info.retractionDist[1], Info.retractionDist[2], GH_ParamAccess.item);
            pManager.AddIntegerParameter(Info.retractionSpeed[0], Info.retractionSpeed[1], Info.retractionSpeed[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.filament[0], Info.filament[1], Info.filament[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.flowRate[0], Info.flowRate[1], Info.flowRate[2], GH_ParamAccess.item, 1.00);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.parameters[0], Info.parameters[1], Info.parameters[2], GH_ParamAccess.item);
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
            double flowRate = new double();

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
            if (!DA.GetData(15, ref flowRate) | (flowRate < 0)) return;

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
            parameters.flowRate = flowRate;

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