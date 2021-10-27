using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace SELECT.Utilities
{
    public static class GeometryUtils
    {
        public static GeometryCollection Wkt2GeomColl(string[] features,int srid= 4326)
        {
            Geometry[] geomArr = new Geometry[features.Length];
            WKTReader reader = new();
            for (int i = 0; i < features.Length; i++)
            {
                geomArr[i] = (Geometry)reader.Read(features[i]);
                geomArr[i].SRID = srid;
            }

            ;
            GeometryCollection geomcoll = new(geomArr)
            {
                SRID = srid
            };
            return geomcoll;
        }
    }
}
