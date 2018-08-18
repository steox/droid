using System;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid
{
    public class Mesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Mesh()
          : base("Droid Mesh", "DMesh",
              "Converts and Centers mesh for Droid Slicer",
              "Droid", "Droid")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Input (closed) Mesh", GH_ParamAccess.item);
            pManager.AddGenericParameter("Droid Volume", "-> DV", "Input Droid Volume", GH_ParamAccess.item);
            pManager.AddNumberParameter("X Position", "X", "Re-Position of Mesh on X-Axis", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Y Position", "Y", "Re-Position of Mesh on Y-Axis", GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Droid Mesh", "DM ->", "Droid Mesh for use with Droid Components", GH_ParamAccess.item);
            pManager.AddMeshParameter("Preview", "P", "Preview of Mesh Position", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Mesh inputMesh = new Rhino.Geometry.Mesh();
            DroidVolume vol = new DroidVolume();
            double x = new double();
            double y = new double();

            if (!DA.GetData(0, ref inputMesh)) return;
            if (!DA.GetData(1, ref vol)) return;
            if (!DA.GetData(2, ref x)) return;
            if (!DA.GetData(3, ref y)) return;
            
            Vector3d normal = new Vector3d(0, 0, 1);
            Plane worldXY = new Plane(Point3d.Origin, normal);

            Vector3d trans = new Vector3d(x, y, 0);
            Rhino.Geometry.Mesh _inputMesh = new Rhino.Geometry.Mesh();

            if (vol.size.Count == 2)
            {
                _inputMesh = inputMesh;
                BoundingBox bbx = _inputMesh.GetBoundingBox(worldXY);
                Point3d cnr = bbx.Corner(true, true, true);
                Point3d center = bbx.Center;
                center.Z = cnr.Z;                
                Vector3d toMiddle = new Vector3d((Point3d.Origin - center + trans));
                _inputMesh.Transform(Transform.Translation(toMiddle));
            }
            if (vol.size.Count == 3)
            {
                _inputMesh = inputMesh;
                BoundingBox bbx = _inputMesh.GetBoundingBox(worldXY);
                Point3d cnr = bbx.Corner(true, true, true);
                Point3d center = bbx.Center;
                center.Z = cnr.Z;
                Point3d middle = new Point3d((vol.size[0] / 2), (vol.size[1] / 2), 0);
                Vector3d toMiddle = new Vector3d((middle - center + trans));
                _inputMesh.Transform(Transform.Translation(toMiddle));
            }

            DroidMesh dMesh = new DroidMesh(_inputMesh);

            DA.SetData(0, dMesh);
            DA.SetData(1, _inputMesh);
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
                return Properties.Resources.logosmall;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2302f80a-520d-4a7f-b21b-2036ae1b3dc0"); }
        }
    }
}