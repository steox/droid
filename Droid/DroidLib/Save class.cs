using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using System.IO;
using System.Windows.Forms;

using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using ClipperLib;

namespace DroidLib
{
    
    #region Save GCode

    public class SaveGCode
    {
        private List<string> _gcode = new List<string>();

        public SaveGCode(List<string> gcode)
        {
            _gcode = gcode;
        }

        public void Save()
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "GCode (*.gcode)|*.gcode";
            saveFileDialog1.Title = "Save GCode File";
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StreamWriter writer = new StreamWriter(myStream);
                    foreach (string x in _gcode)
                    {
                        writer.WriteLine(x);
                    }
                    writer.Close();
                    myStream.Close();
                }
            }
        }
    }
    #endregion
    
}
