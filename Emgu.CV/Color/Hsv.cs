using System;
using System.Collections.Generic;
using System.Text;

namespace Emgu.CV
{
   ///<summary> 
   ///Defines a HSV (Hue Satuation Value) color
   ///</summary>
   [ColorInfo(ConversionCodeName = "HSV")]
   public class Hsv : ColorType
   {
      ///<summary> Create a HSV color using the specific values</summary>
      ///<param name="hue"> The hue value for this color ( 0 &lt; hue &lt; 180 )  </param>
      ///<param name="satuation"> The satuation value for this color </param>
      ///<param name="value"> The value for this color </param>
      public Hsv(double hue, double satuation, double value)
         : this()
      {
         _coordinate[0] = hue;
         _coordinate[1] = satuation;
         _coordinate[2] = value;
      }

      ///<summary> Create a HSV color using the default values (0.0, 0.0, 0.0)</summary>
      public Hsv()
         : base(3)
      { }

      ///<summary> Get or set the intensity of the hue color channel ( 0 &lt; hue &lt; 180 ) </summary>
      [DisplayColor(122, 122, 122)]
      public double H { get { return _coordinate[0]; } set { _coordinate[0] = value; } }

      ///<summary> Get or set the intensity of the satuation color channel </summary>
      [DisplayColor(122, 122, 122)]
      public double S { get { return _coordinate[1]; } set { _coordinate[1] = value; } }

      ///<summary> Get or set the intensity of the value color channel </summary>
      [DisplayColor(122, 122, 122)]
      public double V { get { return _coordinate[2]; } set { _coordinate[2] = value; } }

   }
}
