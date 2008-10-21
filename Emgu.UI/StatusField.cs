using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Emgu.UI
{
   public partial class StatusField : UserControl
   {
      private int _status;

      /// <summary> 
      /// A filed use to display the progress percentage
      /// </summary>
      public StatusField()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Get or Set the progress percentage
      /// </summary>
      public int Status
      {
         get { return _status; }
         set
         {
            _status = value;
            label1.Text = String.Format("{0}% completed", _status);
            Progressbar1.Value = _status;
         }
      }

      /// <summary>
      /// Clear the progress percentage.
      /// </summary>
      public void Clear()
      {
         _status = 0;
         label1.Text = "unknown";
         Progressbar1.Value = 0;
      }
   }
}
