using NetTopologySuite.Geometries;
using NetTopologySuite.IO;

namespace SELECT.Utilities
{
    public static class GeometryUtils
    {
        public static GeometryCollection Wkt2GeomColl(string[] features,int srid= 4326)
        {
            var geomArr = new Geometry[features.Length];
            var reader = new WKTReader();
            for (int i = 0; i < features.Length; i++)
            {
                geomArr[i] = (Geometry)reader.Read(features[i]);
                geomArr[i].SRID = srid;
            }

            ;
            var geomcoll = new GeometryCollection(geomArr);
            geomcoll.SRID = srid;
            return geomcoll;
        }
    }
}
