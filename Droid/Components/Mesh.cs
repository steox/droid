using System;

using Grasshopper.Kernel;
using Rhino.Geometry;
using DroidLib;

namespace Droid.Components
{
    public class Mesh : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Mesh()
          : base(Title.mesh[0], Title.mesh[1], Title.mesh[2], Title.mesh[3], Title.mesh[4])
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter(Info.mesh[0], Info.mesh[1], Info.mesh[2], GH_ParamAccess.item);
            pManager.AddGenericParameter(Info.droidVolume[0], Info.droidVolume[1], Info.droidVolume[2], GH_ParamAccess.item);
            pManager.AddNumberParameter(Info.xPos[0], Info.xPos[1], Info.xPos[2], GH_ParamAccess.item, 0);
            pManager.AddNumberParameter(Info.yPos[0], Info.yPos[1], Info.yPos[2], GH_ParamAccess.item, 0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter(Info.droidMesh[0], Info.droidMesh[1], Info.droidMesh[2], GH_ParamAccess.item);
            pManager.AddMeshParameter(Info.preview[0], Info.preview[1], Info.preview[2], GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Mesh inputMesh = null;
            DroidVolume vol = null;
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

            if (vol.volumeOutline.Length == 2)
            {
                _inputMesh = inputMesh;
                BoundingBox bbx = _inputMesh.GetBoundingBox(worldXY);
                Point3d cnr = bbx.Corner(true, true, true);
                Point3d center = bbx.Center;
                center.Z = cnr.Z;                
                Vector3d toMiddle = new Vector3d((Point3d.Origin - center + trans));
                _inputMesh.Transform(Transform.Translation(toMiddle));
            }
            if (vol.volumeOutline.Length == 6)
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