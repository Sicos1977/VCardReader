using System;
using System.Collections.Generic;

namespace VCardReader
{
    /// <summary>
    /// Will read an CVF file and returns it as an HTML document
    /// </summary>
    public class Reader
    {
        #region Implemented abstract class Reader properties and methods
        ///// <summary>
        ///// Returns a list with supported file extensions
        ///// </summary>
        //public override List<string> SupportedExtensions
        //{
        //    get { return new List<string> { ".VCF" }; }
        //}

        ///// <summary>
        ///// Returns an <see cref="Exception"/> when the <see cref="Read"/> method returns an 
        ///// <see cref="Status.Failed"/> or <see cref="Status.FailedWithReason"/>
        ///// </summary>
        //public override Exception Exception { get; protected set; }

        ///// <summary>
        ///// Returns a list of files with path when the <see cref="Read"/> method returns an <see cref="Status.Succes"/>
        ///// </summary>
        //public override List<string> Files { get; protected set; }

        ///// <summary>
        /////     Reads the given <paramref name="inputFile" /> and writes the output to the <paramref name="outputFolder" />
        /////     Use the property <see cref="Files" /> to return the written output
        ///// </summary>
        ///// <param name="inputFile">The file to read</param>
        ///// <param name="outputFolder">The folder where to write the output</param>
        ///// <returns>
        /////     <see cref="Status" />
        ///// </returns>
        //public override Status Read(string inputFile, string outputFolder)
        //{
        //    try
        //    {
        //        // TODO : Code schrijven
        //        return Status.Succes;
        //    }
        //    catch (Exception exception)
        //    {
        //        Exception = exception;
        //        return Status.Failed;
        //    }
        //}
        #endregion
    }
}
