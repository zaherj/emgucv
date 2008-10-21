using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Emgu.Util;

namespace Emgu.CV
{
   ///<summary>
   /// Wrapper to OpenCV Seq 
   ///</summary>
   public class Seq<T> : UnmanagedObject, IEnumerable<T> where T : struct
   {
      /// <summary>
      /// The pointer to the storage used by this sequence
      /// </summary>
      protected MemStorage _stor;

      #region Constructors
      /// <summary>
      /// Create a sequence using the specific <paramref name="seqFlag"/> and <paramref name="storage"/>
      /// </summary>
      /// <param name="seqFlag">Flags of the created sequence. If the sequence is not passed to any function working with a specific type of sequences, the sequence value may be set to 0, otherwise the appropriate type must be selected from the list of predefined sequence types</param>
      /// <param name="storage">the storage</param>
      public Seq(int seqFlag, MemStorage storage)
      {
         _stor = storage;
         _ptr = CvInvoke.cvCreateSeq(
             seqFlag, Marshal.SizeOf(typeof(MCvSeq)),
             Marshal.SizeOf(typeof(T)),
             storage.Ptr);
      }

      /// <summary>
      /// Create a contour of the specific kind, tyoe and flag
      /// </summary>
      /// <param name="kind">the kind of the sequence</param>
      /// <param name="eltype">the type of the sequence</param>
      /// <param name="flag">the flag of the sequence</param>
      /// <param name="stor">the storage</param>
      public Seq(CvEnum.SEQ_ELTYPE eltype, CvEnum.SEQ_KIND kind,  CvEnum.SEQ_FLAG flag, MemStorage stor)
         : this( ((int)kind | (int)eltype | (int)flag), stor)
      {
      }

      /// <summary>
      /// Create a sequence using the specific <paramref name="storage"/>
      /// </summary>
      /// <param name="storage">the storage</param>
      public Seq(MemStorage storage)
         : this(0, storage)
      {
      }

      /// <summary>
      /// Create a sequence 
      /// </summary>
      public Seq()
         : this(new MemStorage())
      {
      }

      /// <summary>
      /// Create a sequence from the unmanaged pointer and the storage used by the pointer
      /// </summary>
      /// <param name="seq">The unmanaged sequence</param>
      /// <param name="storage">The memory storage this sequence utilize</param>
      public Seq(IntPtr seq, MemStorage storage)
      {
         _ptr = seq;
         _stor = storage;
      }
      #endregion 

      /// <summary>
      /// Push the data to the sequence
      /// </summary>
      /// <param name="data">The data to be push into the sequence</param>
      public void Push(T data)
      {
         IntPtr dataCopy = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(T)));
         Marshal.StructureToPtr(data, dataCopy, false);
         CvInvoke.cvSeqPush(Ptr, dataCopy);
         Marshal.FreeHGlobal(dataCopy);
      }

      /// <summary>
      /// A Pointer to the storage used by this Seq
      /// </summary>
      public MemStorage Storage { get { return _stor; } }

      /// <summary>
      /// Get the MCvSeq structure
      /// </summary>
      public MCvSeq MCvSeq
      {
         get { return (MCvSeq)Marshal.PtrToStructure(Ptr, typeof(MCvSeq)); }
      }

      /// <summary>
      /// Get the minimum area rectangle for this point sequence
      /// </summary>
      /// <returns>The minimum area rectangle</returns>
      public Box2D<int> GetMinAreaRect()
      {
         return GetMinAreaRect(null);
      }

      /// <summary>
      /// Get the minimum area rectangle for this point sequence
      /// </summary>
      /// <param name="stor">The temporary storage to use</param>
      /// <returns>The minimum area rectangle</returns>
      public Box2D<int> GetMinAreaRect(MemStorage stor)
      {
         Box2D<int> box = new Box2D<int>();
         MCvBox2D cvbox = CvInvoke.cvMinAreaRect2(Ptr, stor == null ? IntPtr.Zero : stor.Ptr);
         box.MCvBox2D = cvbox;
         return box;
      }

      /// <summary>
      /// Get the convex hull of this point sequence
      /// </summary>
      /// <param name="orientation">The orientation of the convex hull</param>
      /// <param name="stor">The storage for the resulting sequence</param>
      /// <returns>The result convex hull</returns>
      public Seq<T> GetConvexHull(CvEnum.ORIENTATION orientation, MemStorage stor)
      {
         IntPtr hull = CvInvoke.cvConvexHull2(Ptr, stor.Ptr, orientation, 1);
         return new Seq<T>(hull, stor);
      }

      /// <summary>
      /// Get the convex hull of this point sequence
      /// </summary>
      /// <param name="orientation">The orientation of the convex hull</param>
      /// <returns>The result convex hull</returns>
      public Seq<T> GetConvexHull(CvEnum.ORIENTATION orientation)
      {
         MemStorage stor = new MemStorage();
         return GetConvexHull(orientation, stor);
      }

      /// <summary>
      /// Obtain the <paramref name="index"/> element in this sequence
      /// </summary>
      /// <param name="index">the index of the element</param>
      /// <returns>the <paramref name="index"/> element in this sequence</returns>
      public T this[int index]
      {
         get
         {
            return (T)Marshal.PtrToStructure(CvInvoke.cvGetSeqElem(_ptr, index), typeof(T));
         }
      }

      /// <summary>
      /// Convert this sequence to array
      /// </summary>
      /// <returns>the array representation of this sequence</returns>
      public T[] ToArray()
      {
         T[] res = new T[Total];
         for (int i = 0; i < res.Length; i++)
            res[i] = this[i];
         return res;
      }

      /// <summary>
      /// return an enumerator of the elements in the sequence
      /// </summary>
      /// <returns>an enumerator of the elements in the sequence</returns>
      public IEnumerator<T> GetEnumerator()
      {
         for (int i = 0; i < Total; i++)
            yield return this[i];
      }

      /// <summary>
      /// return an enumerator of the elements in the sequence
      /// </summary>
      /// <returns>an enumerator of the elements in the sequence</returns>
      IEnumerator IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      /// <summary>
      /// Same as h_next pointer in CvSeq
      /// </summary>
      public Seq<T> HNext
      {
         get
         {
            MCvSeq seq = MCvSeq;
            return seq.h_next == IntPtr.Zero ? null : new Seq<T>(seq.h_next, Storage);
         }
         set
         {
            MCvSeq seq = MCvSeq;
            seq.h_next = value == null ? IntPtr.Zero : value.Ptr;
            Marshal.StructureToPtr(seq, _ptr, false);
         }
      }

      /// <summary>
      /// Same as h_prev pointer in CvSeq
      /// </summary>
      public Seq<T> HPrev
      {
         get
         {
            MCvSeq seq = MCvSeq;
            return seq.h_prev == IntPtr.Zero ? null : new Seq<T>(seq.h_prev, Storage);
         }
         set
         {
            MCvSeq seq = MCvSeq;
            seq.h_prev = value == null ? IntPtr.Zero : value.Ptr;
            Marshal.StructureToPtr(seq, _ptr, false);
         }

      }

      /// <summary>
      /// Same as v_next pointer in CvSeq
      /// </summary>
      public Seq<T> VNext
      {
         get
         {
            MCvSeq seq = MCvSeq;
            return seq.v_next == IntPtr.Zero ? null : new Seq<T>(seq.v_next, Storage);
         }
         set
         {
            MCvSeq seq = MCvSeq;
            seq.v_next = value == null ? IntPtr.Zero : value.Ptr;
            Marshal.StructureToPtr(seq, _ptr, false);
         }
      }

      /// <summary>
      /// Same as v_prev pointer in CvSeq
      /// </summary>
      public Seq<T> VPrev
      {
         get
         {
            MCvSeq seq = MCvSeq;
            return seq.v_prev == IntPtr.Zero ? null : new Seq<T>(seq.v_prev, Storage);
         }
         set
         {
            MCvSeq seq = MCvSeq;
            seq.v_prev = value == null ? IntPtr.Zero : value.Ptr;
            Marshal.StructureToPtr(seq, _ptr, false);
         }
      }

      /// <summary>
      /// Creates a sequence that represents the specified slice of the input sequence. The new sequence either shares the elements with the original sequence or has own copy of the elements. So if one needs to process a part of sequence but the processing function does not have a slice parameter, the required sub-sequence may be extracted using this function
      /// </summary>
      /// <param name="slice">The part of the sequence to extract</param>
      /// <param name="storage">The destination storage to keep the new sequence header and the copied data if any. If it is NULL, the function uses the storage containing the input sequence.</param>
      /// <param name="copy_data">The flag that indicates whether to copy the elements of the extracted slice </param>
      /// <returns>A sequence that represents the specified slice of the input sequence</returns>
      public Seq<T> Slice(MCvSlice slice, MemStorage storage, bool copy_data)
      {
         return new Seq<T>(CvInvoke.cvSeqSlice(Ptr, slice, storage.Ptr, copy_data), storage);
      }

      ///<summary> Get the number of eelments in the sequence</summary>
      public int Total
      {
         get { return MCvSeq.total; }
      }

      /// <summary>
      /// Release the sequence and all the memory associate with it
      /// </summary>
      protected override void DisposeObject()
      {
      }

      ///<summary> 
      /// Get the area of the contour 
      ///</summary>
      public double Area
      {
         get
         {
            return Math.Abs(CvInvoke.cvContourArea(Ptr, new MCvSlice(0, 0x3fffffff)));
         }
      }

      ///<summary> 
      /// Indicate if the coutour is a convex one 
      ///</summary>
      public bool Convex
      {
         get
         {
            return CvInvoke.cvCheckContourConvexity(Ptr) != 0;
         }
      }

      ///<summary> 
      /// The perimeter of the sequence 
      ///</summary>
      public double Perimeter
      {
         get
         {
            return Math.Abs(CvInvoke.cvContourPerimeter(Ptr));
         }
      }

      /// <summary>
      /// Approximates one curves and returns the approximation result
      /// </summary>
      /// <param name="accuracy">The desired approximation accuracy</param>
      /// <param name="storage"> The storage the resulting sequence use</param>
      /// <returns>The approximated contour</returns>
      public Seq<T> ApproxPoly(double accuracy, MemStorage storage)
      {
         return ApproxPoly(accuracy, 0, storage);
      }

      /// <summary>
      /// Approximates one or more curves and returns the approximation result[s]. In case of multiple curves approximation the resultant tree will have the same structure as the input one (1:1 correspondence)
      /// </summary>
      /// <param name="accuracy">The desired approximation accuracy</param>
      /// <param name="storage"> The storage the resulting sequence use</param>
      /// <param name="maxLevel">
      /// Maximal level for sequence approximation. 
      /// If 0, only sequence is arrpoximated. 
      /// If 1, the sequence and all sequence after it on the same level are approximated. 
      /// If 2, all sequence after and all sequence one level below the contours are approximated, etc. If the value is negative, the function does not approximate the sequence following after contour but draws child sequences of sequence up to abs(maxLevel)-1 level
      /// </param>
      /// <returns>The approximated contour</returns>
      public Seq<T> ApproxPoly(double accuracy, int maxLevel, MemStorage storage)
      {
         return new Seq<T>(
             CvInvoke.cvApproxPoly(
             Ptr,
             System.Runtime.InteropServices.Marshal.SizeOf(typeof(MCvContour)),
             storage.Ptr,
             CvEnum.APPROX_POLY_TYPE.CV_POLY_APPROX_DP,
             accuracy,
             maxLevel),
             storage);
      }

      /// <summary>
      /// Approximates one curve and returns the approximation result. 
      /// </summary>
      /// <param name="accuracy">The desired approximation accuracy</param>
      /// <returns>The approximated contour</returns>
      public Seq<T> ApproxPoly(double accuracy)
      {
         MemStorage storage = new MemStorage();
         return ApproxPoly(accuracy, storage);
      }

      ///<summary> The smallest Bouding Rectangle </summary>
      public Rectangle<double> BoundingRectangle
      {
         get
         {
            Rectangle<double> res = new Rectangle<double>();
            res.MCvRect = CvInvoke.cvBoundingRect(Ptr, false);
            return res;
         }
      }

      /// <summary>
      /// Removes all elements from the sequence. The function does not return the memory to the storage, but this memory is reused later when new elements are added to the sequence. This function time complexity is O(1). 
      /// </summary>
      public void Clear()
      {
         CvInvoke.cvClearSeq(Ptr);
      }

      /// <summary>
      /// Determines whether the point is inside contour, outside, or lies on an edge (or coinsides with a vertex)
      /// </summary>
      /// <param name="point">The point to be tested</param>
      /// <returns>positive if inside; negative if out side; 0 if on the contour</returns>
      public double InContour(Point2D<float> point)
      {
         return CvInvoke.cvPointPolygonTest(Ptr, point.MCvPoint2D32f, 0);
      }

      /// <summary>
      /// Determines the distance from the point to the contour
      /// </summary>
      /// <param name="point">The point to measured distance</param>
      /// <returns>positive distance if inside; negative distance if outside; 0 if on the contour</returns>
      public double Distance(Point2D<float> point)
      {
         return CvInvoke.cvPointPolygonTest(Ptr, point.MCvPoint2D32f, 1);
      }

      /// <summary>
      /// Get the moments for this point sequence
      /// </summary>
      /// <returns>the moments for this point sequence</returns>
      public MCvMoments GetMoments()
      {
         MCvMoments moment = new MCvMoments();
         CvInvoke.cvMoments(Ptr, ref moment, 0);
         return moment;
      }
   }
}
