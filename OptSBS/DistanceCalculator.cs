using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecalculateLDCG
{
    public class HaversineDistanceCalculator
    {
        /// <summary>
        /// This function will be used to calculate the distance between two GeoPoint using Haversine method where the result is in miles.
        /// </summary>
        /// <param name="latitude1">source latitude </param>
        /// <param name="longitude1">source longitude</param>
        /// <param name="latitude2">destination latitude</param>
        /// <param name="longitude2">destination longitude</param>
        /// <returns>Distance in miles</returns>
        public static double GetHaversineDistanceInMiles(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double EarthRadiusInMiles = 3960;

            double normalizedDistance = GetNormalizedHaversineDistance(latitude1, longitude1, latitude2, longitude2);
            return normalizedDistance * EarthRadiusInMiles;
        }

        /// <summary>
        /// This function will be used to calculate the distance between two GeoPoint using Haversine method where the result is in meters.
        /// </summary>
        /// <param name="latitude1">source latitude </param>
        /// <param name="longitude1">source longitude</param>
        /// <param name="latitude2">destination latitude</param>
        /// <param name="longitude2">destination longitude</param>
        /// <returns>Distance in miles</returns>
        public static double GetHaversineDistanceInMeters(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            const double EarthRadiusInMeters = 6371 * 1000;

            double normalizedDistance = GetNormalizedHaversineDistance(latitude1, longitude1, latitude2, longitude2);
            return normalizedDistance * EarthRadiusInMeters;
        }

        /// <summary>
        /// Normlized haversine distance is haversine distance for a sphere with radius of 1.
        /// </summary>
        /// <param name="latitude1"></param>
        /// <param name="longitude1"></param>
        /// <param name="latitude2"></param>
        /// <param name="longitude2"></param>
        /// <returns></returns>
        public static double GetNormalizedHaversineDistance(double latitude1, double longitude1, double latitude2, double longitude2)
        {

            // for more info about the formula used here http://en.wikipedia.org/wiki/Haversine_formula
            double dLat = ConvertDegreesToRadians(latitude2 - latitude1);
            double dLon = ConvertDegreesToRadians(longitude2 - longitude1);
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ConvertDegreesToRadians(latitude1)) * Math.Cos(ConvertDegreesToRadians(latitude2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double normalizedDistance = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return normalizedDistance;
        }


        /// <summary>
        /// Convert degrees to Radians.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private static double ConvertDegreesToRadians(double val)
        {
            return (Math.PI / 180) * val;
        }
    }

}
