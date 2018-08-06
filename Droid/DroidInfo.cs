using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Droid
{
    public class DroidInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "Droid";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return Properties.Resources.logosmall;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "3D printing related Library including model Slicing, custom paths and Gcode generation. Designed to be used for desktop 3d Printers, up to large scale Robotic Fabricators using FFF technologies and running from Gcode. Available to be used to prepare and print models in a 'Plug and Play' style with Droid components, or in a more controlled and experiemental manner with Custom input and output print paths";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("58c7e243-8ae2-4118-9aee-410341e308e0");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "YTSProject - YT Sebastian Teo";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "www.ytsproject.com";
            }
        }
    }
}
