
/*
   Copyright 2014-2016 Kees van Spelde

   Licensed under The Code Project Open License (CPOL) 1.02;
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.codeproject.com/info/cpol10.aspx

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

namespace VCardReader
{
    /// <summary>
    ///     The access classification of a vCard.
    /// </summary>
    /// <remarks>
    ///     The access classification defines the intent of the vCard owner.
    /// </remarks>
    public enum AccessClassification
    {
        /// <summary>
        ///     The vCard classification is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     The vCard is classified as public.
        /// </summary>
        Public = 1,

        /// <summary>
        ///     The vCard is classified as private.
        /// </summary>
        Private = 2,

        /// <summary>
        ///     The vCard is classified as confidential.
        /// </summary>
        Confidential = 3
    }
}