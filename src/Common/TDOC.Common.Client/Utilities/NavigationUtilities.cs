using TDOC.Common.Client.Extensions;

namespace TDOC.Common.Client.Utilities
{
    public static class NavigationUtilities
    {
        /// <summary>
        /// Checks if the URI provided by <see cref="NavigationManager"/> should be displayed in embedded mode.
        /// </summary>
        /// <param name="navigation"><see cref="NavigationManager"/></param>
        /// <returns><c>true</c> if the URI provided by <see cref="NavigationManager"/> should be displayed in embedded mode; <c>false</c> otherwise.</returns>
        public static bool IsEmbeddedClient(NavigationManager navigation) => 
            navigation.TryGetQueryString("EmbeddedClient", out int value) && (value == 1);

        /// <summary>
        /// Extracts value of the integer paramater if it is provided by Uri. Else 0.
        /// </summary>
        /// <param name="navigation"><see cref="NavigationManager"/></param>
        /// <param name="param">Parameter name.</param>
        /// <returns>Position key Id if provided by Uri.</returns>
        public static int GetIntegerParamValue(NavigationManager navigation, string param) =>
            navigation.TryGetQueryString(param, out int value) ? value : 0;

        /// <summary>
        /// Get uri segment index by search value.
        /// </summary>
        /// <param name="navigation"><see cref="NavigationManager"/></param>
        /// <param name="searchValue">Search value in segments.</param>
        /// <returns>Index of search value in uri.</returns>
        public static int GetUriSegmentIndex(NavigationManager navigation, string searchValue, out List<string> uriSegments)
        {
            uriSegments = new List<string>(new Uri(navigation.Uri).Segments);
            return uriSegments.FindIndex(segment => segment.Contains(searchValue));
        }

        /// <summary>
        /// Get uri segment by search value.
        /// </summary>
        /// <param name="navigation"><see cref="NavigationManager"/></param>
        /// <param name="searchValue">Search value in segments.</param>
        /// <param name="incrementalIndex">Incremental index.</param>
        /// <returns>True if segment was found successfully; otherwise, false.</returns>
        public static bool GetUriSegment(NavigationManager navigation, string searchValue, int incrementalIndex, out string segment)
        {
            segment = "";

            int segmentIndex = GetUriSegmentIndex(navigation, searchValue, out List<string> uriSegments);
            if (segmentIndex == -1)
                return false;

            segmentIndex += incrementalIndex;
            if (segmentIndex < 0 || segmentIndex >= uriSegments.Count)
                return false;

            segment = uriSegments[segmentIndex].TrimEnd('/');
            return true;
        }
    }
}