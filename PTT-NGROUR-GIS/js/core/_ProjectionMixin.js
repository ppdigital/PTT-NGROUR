/**
 * @description 
 * -
 * @class _ProjectionMixin
 */
define([
    "esri/geometry/webMercatorUtils",
    "esri/geometry/jsonUtils",
    "esri/SpatialReference",

    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    webMercatorUtils,
    geomJsonUtil,
    SpatialReference,

    array,
    lang,
    declare,
    require
    ) {

    /********************************* Constants ********************************/
    var FOURTHPI = Math.PI / 4;
    var DEG_2_RAD = Math.PI / 180;
    var RAD_2_DEG = 180.0 / Math.PI;
    var BLOCK_SIZE = 100000; // size of square identifier (within grid zone designation),
    // (meters)

    // For diagram of zone sets, please see the "United States National Grid" white paper.
    var GRIDSQUARE_SET_COL_SIZE = 8;  // column width of grid square set  
    var GRIDSQUARE_SET_ROW_SIZE = 20; // row height of grid square set

    // UTM offsets
    var EASTING_OFFSET = 500000.0;   // (meters)
    var NORTHING_OFFSET = 10000000.0; // (meters)

    // scale factor of central meridian
    var k0 = 0.9996;

    var ECCENTRICTY_SQUARED;

    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {

        project: {
            /**
            * Convert projection from GCS to UTM
            * @method toUTM
            * @public
            * @param {esri.geometry.Geometry} geometry 
            * @param {Number} [toZone=47]
            */
            toUTM: function (geometry, toZone) {
                toZone = toZone || 47;
                var spatialReference = null;
                try {
                    geometry = typeof (geometry.toJson) == "function" ? geomJsonUtil.fromJson(geometry.toJson()) : geomJsonUtil.fromJson(geometry);
                } catch (err) {
                    throw new Error("geometry is not correct.");
                }
                if (geometry.spatialReference && geometry.spatialReference.wkid) {
                    spatialReference = geometry.spatialReference;
                    if (spatialReference.isWebMercator()) {
                        geometry = webMercatorUtils.webMercatorToGeographic(geometry);
                    }
                } else {
                    throw new Error("not found spatialReference in geometry.");
                }
                var outSR = this._getUTMSpatialreference(spatialReference.wkid, toZone);
                var datumConstans = this._getDatumConstants(outSR);
                var SEMI_MAJOR_AXIS = datumConstans.SEMI_MAJOR_AXIS;
                var INVERSE_FLATTENING = datumConstans.INVERSE_FLATTENING;
                var ns = datumConstans.ns;
                var jsonGeometry = {};
                if (geometry.x) {
                    var points = this._lltoUTM(geometry.y, geometry.x, datumConstans);
                    jsonGeometry = { x: points[0], y: points[1] };
                } else if (geometry.points) {
                    jsonGeometry = {
                        points: array.map(geometry.points, lang.hitch(this, function (point) {
                            var _point = this._lltoUTM(point[1], point[0], datumConstans);
                            return [_point[0], _point[1]];
                        }))
                    }
                } else if (geometry.paths) {
                    jsonGeometry = {
                        paths: array.map(geometry.paths, lang.hitch(this, function (path) {
                            return array.map(path, lang.hitch(this, function (point) {
                                var _point = this._lltoUTM(point[1], point[0], datumConstans);
                                return [_point[0], _point[1]];
                            }));
                        }))
                    };
                } else if (geometry.rings) {
                    jsonGeometry = {
                        rings: array.map(geometry.rings, lang.hitch(this, function (ring) {
                            return array.map(ring, lang.hitch(this, function (point) {
                                var _point = this._lltoUTM(point[1], point[0], datumConstans);
                                return [_point[0], _point[1]];
                            }));
                        }))
                    };
                } else if (geometry.xmax) {
                    var points1 = this._lltoUTM(geometry.ymax, geometry.xmax, datumConstans);
                    var points2 = this._lltoUTM(geometry.ymin, geometry.xmin, datumConstans);
                    jsonGeometry["xmax"] = points1[0];
                    jsonGeometry["ymax"] = points1[1];
                    jsonGeometry["xmin"] = points2[0];
                    jsonGeometry["ymin"] = points2[1];
                }
                jsonGeometry["spatialReference"] = { wkid: outSR };
                return geomJsonUtil.fromJson(jsonGeometry);
            },

            /**
            * Convert projection from UTM to GCS
            * @method fromUTM
            * @public
            * @param {esri.geometry.Geometry} geometry 
            */
            fromUTM: function (geometry) {
                var spatialReference = null;
                if (geometry.spatialReference && geometry.spatialReference.wkid) {
                    spatialReference = geometry.spatialReference;
                } else {
                    throw new Error("not found spatialReference in geometry");
                }
                var outSR = this._getGCSSpatialreference(spatialReference.wkid);
                var datumConstans = this._getDatumConstants(spatialReference.wkid);
                var SEMI_MAJOR_AXIS = datumConstans.SEMI_MAJOR_AXIS;
                var INVERSE_FLATTENING = datumConstans.INVERSE_FLATTENING;
                var jsonGeometry = {};
                if (geometry.x) {
                    var points = this._UTMtoLL(geometry.y, geometry.x, datumConstans);
                    jsonGeometry = { x: points[0], y: points[1] };
                } else if (geometry.points) {
                    jsonGeometry = {
                        points: array.map(geometry.points, lang.hitch(this, function (point) {
                            var _point = this._UTMtoLL(point[1], point[0], datumConstans);
                            return [_point[0], _point[1]];
                        }))
                    }
                } else if (geometry.paths) {
                    jsonGeometry = {
                        paths: array.map(geometry.paths, lang.hitch(this, function (path) {
                            return array.map(path, lang.hitch(this, function (point) {
                                var _point = this._UTMtoLL(point[1], point[0], datumConstans);
                                return [_point[0], _point[1]];
                            }));
                        }))
                    };
                } else if (geometry.rings) {
                    jsonGeometry = {
                        rings: array.map(geometry.rings, lang.hitch(this, function (ring) {
                            return array.map(ring, lang.hitch(this, function (point) {
                                var _point = this._UTMtoLL(point[1], point[0], datumConstans);
                                return [_point[0], _point[1]];
                            }));
                        }))
                    };
                } else if (geometry.xmax) {
                    var points1 = this._UTMtoLL(geometry.ymax, geometry.xmax, datumConstans);
                    var points2 = this._UTMtoLL(geometry.ymin, geometry.xmin, datumConstans);
                    jsonGeometry["xmax"] = points1[1];
                    jsonGeometry["ymax"] = points1[0];
                    jsonGeometry["xmin"] = points2[1];
                    jsonGeometry["ymin"] = points2[0];
                }
                jsonGeometry["spatialReference"] = { wkid: outSR };
                if (this.map && this.map.spatialReference && (this.map.spatialReference.wkid == 102100 || this.map.spatialReference.wkid == 3857)) {
                    return webMercatorUtils.geographicToWebMercator(geomJsonUtil.fromJson(jsonGeometry));
                }
                return geomJsonUtil.fromJson(jsonGeometry);
            },
            fromMGRS: function (usngStr_input) {
                // latlon is a 2-element array declared by calling routine
                var usngp = new Object();

                this._parseUSNG_str(usngStr_input, usngp);
                var coords = new Object();

                // convert USNG coords to UTM; this routine counts digits and sets precision
                this._USNGtoUTM(usngp.zone, usngp.let, usngp.sq1, usngp.sq2, usngp.east, usngp.north, coords)

                // southern hemisphere case
                if (usngp.let < 'N') {
                    coords.N -= NORTHING_OFFSET
                }
                var datumConstants = this._getDatumConstants(4326);
                datumConstants.zone = coords.zone;

                coords = this._UTMtoLL(coords.N, coords.E, datumConstants);

                return geomJsonUtil.fromJson({ x: coords[0], y: coords[1], spatialReference: { wkid: 4326 } });
            },
            toMGRS: function (geometry, precision) {
                precision = precision || 5;

                geometry = this.transform(geometry, 4326);

                var lat = geometry.y;
                var lon = geometry.x;

                lat = parseFloat(lat);
                lon = parseFloat(lon);

                var UTMZoneNumber = this._getZoneNumber(lat, lon);
                var datumConstants = this._getDatumConstants(4326);
                datumConstants.zone = UTMZoneNumber;

                var coords = this._lltoUTM(lat, lon, datumConstants);

                var UTMEasting = coords[0];
                var UTMNorthing = coords[1];

                //console.log(UTMEasting, UTMNorthing);

                // ...then convert UTM to USNG

                // southern hemispher case
                if (lat < 0) {
                    // Use offset for southern hemisphere
                    UTMNorthing += NORTHING_OFFSET;
                }

                var USNGLetters = this._findGridLetters(UTMZoneNumber, UTMNorthing, UTMEasting);
                var USNGNorthing = Math.round(UTMNorthing) % BLOCK_SIZE;
                var USNGEasting = Math.round(UTMEasting) % BLOCK_SIZE;

                // added... truncate digits to achieve specified precision
                USNGNorthing = Math.floor(USNGNorthing / Math.pow(10, (5 - precision)))
                USNGEasting = Math.floor(USNGEasting / Math.pow(10, (5 - precision)))

                var USNG = UTMZoneNumber + this._utmLetterDesignator(lat) + " " + USNGLetters + " ";

                // REVISIT: Modify to incorporate dynamic precision ?
                for (i = String(USNGEasting).length; i < precision; i++) {
                    USNG += "0";
                }

                USNG += USNGEasting + " ";

                for (i = String(USNGNorthing).length; i < precision; i++) {
                    USNG += "0";
                }

                USNG += USNGNorthing;

                USNG = USNG.replace(/ /g, "");

                return (USNG);
            },

            _USNGtoUTM: function (zone, letN, sq1, sq2, east, north, ret) {
                var USNGSqEast = "ABCDEFGHJKLMNPQRSTUVWXYZ"
                //Starts (southern edge) of N-S zones in millons of meters
                var zoneBase = [1.1, 2.0, 2.9, 3.8, 4.7, 5.6, 6.5, 7.3, 8.2, 9.1, 0, 0.8, 1.7, 2.6, 3.5, 4.4, 5.3, 6.2, 7.0, 7.9];

                var segBase = [0, 2, 2, 2, 4, 4, 6, 6, 8, 8, 0, 0, 0, 2, 2, 4, 4, 6, 6, 6];  //Starts of 2 million meter segments, indexed by zone 

                // convert easting to UTM
                var eSqrs = USNGSqEast.indexOf(sq1);
                var appxEast = 1 + eSqrs % 8;

                // convert northing to UTM
                var letNorth = "CDEFGHJKLMNPQRSTUVWX".indexOf(letN);
                if (zone % 2)  //odd number zone
                    var nSqrs = "ABCDEFGHJKLMNPQRSTUV".indexOf(sq2)
                else        // even number zone
                    var nSqrs = "FGHJKLMNPQRSTUVABCDE".indexOf(sq2);

                var zoneStart = zoneBase[letNorth];
                var appxNorth = Number(segBase[letNorth]) + nSqrs / 10;
                if (appxNorth < zoneStart)
                    appxNorth += 2;

                ret.N = appxNorth * 1000000 + Number(north) * Math.pow(10, 5 - north.length);
                ret.E = appxEast * 100000 + Number(east) * Math.pow(10, 5 - east.length)
                ret.zone = zone;
                ret.letter = letN;

                return;
            },
            _parseUSNG_str: function (usngStr_input, parts) {
                var j = 0;
                var k;
                var usngStr = [];
                var usngStr_temp = []

                usngStr_temp = usngStr_input.toUpperCase()

                // put usgn string in 'standard' form with no space delimiters
                var regexp = /%20/g
                usngStr = usngStr_temp.replace(regexp, "")
                regexp = / /g
                usngStr = usngStr_temp.replace(regexp, "")

                if (usngStr.length < 7) {
                    //alert("This application requires minimum USNG precision of 10,000 meters")
                    return 0;
                }

                // break usng string into its component pieces
                parts.zone = usngStr.charAt(j++) * 10 + usngStr.charAt(j++) * 1;
                parts.let = usngStr.charAt(j++)
                parts.sq1 = usngStr.charAt(j++)
                parts.sq2 = usngStr.charAt(j++)

                parts.precision = (usngStr.length - j) / 2;
                parts.east = '';
                parts.north = '';
                for (var k = 0; k < parts.precision; k++) {
                    parts.east += usngStr.charAt(j++)
                }

                if (usngStr[j] == " ") { j++ }
                for (var k = 0; k < parts.precision; k++) {
                    parts.north += usngStr.charAt(j++)
                }
            },

            _UTMtoLL: function (UTMNorthing, UTMEasting, cnts) {

                // remove 500,000 meter offset for longitude
                var xUTM = parseFloat(UTMEasting) - EASTING_OFFSET;
                var yUTM = parseFloat(UTMNorthing);
                var zoneNumber = parseInt(cnts.zone);

                // origin longitude for the zone (+3 puts origin in zone center) 
                var lonOrigin = (zoneNumber - 1) * 6 - 180 + 3;

                // M is the "true distance along the central meridian from the Equator to phi
                // (latitude)
                var M = yUTM / k0;
                var mu = M / (cnts.EQUATORIAL_RADIUS * (1 - cnts.ECC_SQUARED / 4 - 3 * cnts.ECC_SQUARED *
                                cnts.ECC_SQUARED / 64 - 5 * cnts.ECC_SQUARED * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 256));

                // phi1 is the "footprint latitude" or the latitude at the central meridian which
                // has the same y coordinate as that of the point (phi (lat), lambda (lon) ).
                var phi1Rad = mu + (3 * cnts.E1 / 2 - 27 * cnts.E1 * cnts.E1 * cnts.E1 / 32) * Math.sin(2 * mu)
                               + (21 * cnts.E1 * cnts.E1 / 16 - 55 * cnts.E1 * cnts.E1 * cnts.E1 * cnts.E1 / 32) * Math.sin(4 * mu)
                               + (151 * cnts.E1 * cnts.E1 * cnts.E1 / 96) * Math.sin(6 * mu);
                var phi1 = phi1Rad * RAD_2_DEG;

                // Terms used in the conversion equations
                var N1 = cnts.EQUATORIAL_RADIUS / Math.sqrt(1 - cnts.ECC_SQUARED * Math.sin(phi1Rad) *
                            Math.sin(phi1Rad));
                var T1 = Math.tan(phi1Rad) * Math.tan(phi1Rad);
                var C1 = cnts.ECC_PRIME_SQUARED * Math.cos(phi1Rad) * Math.cos(phi1Rad);
                var R1 = cnts.EQUATORIAL_RADIUS * (1 - cnts.ECC_SQUARED) / Math.pow(1 - cnts.ECC_SQUARED *
                              Math.sin(phi1Rad) * Math.sin(phi1Rad), 1.5);
                var D = xUTM / (N1 * k0);

                // Calculate latitude, in decimal degrees
                var lat = phi1Rad - (N1 * Math.tan(phi1Rad) / R1) * (D * D / 2 - (5 + 3 * T1 + 10
                      * C1 - 4 * C1 * C1 - 9 * cnts.ECC_PRIME_SQUARED) * D * D * D * D / 24 + (61 + 90 *
                        T1 + 298 * C1 + 45 * T1 * T1 - 252 * cnts.ECC_PRIME_SQUARED - 3 * C1 * C1) * D * D *
                        D * D * D * D / 720);
                lat = lat * RAD_2_DEG;

                // Calculate longitude, in decimal degrees
                var lon = (D - (1 + 2 * T1 + C1) * D * D * D / 6 + (5 - 2 * C1 + 28 * T1 - 3 *
                          C1 * C1 + 8 * cnts.ECC_PRIME_SQUARED + 24 * T1 * T1) * D * D * D * D * D / 120) /
                          Math.cos(phi1Rad);

                lon = lonOrigin + lon * RAD_2_DEG;
                //ret.lat = lat;
                //ret.lon = lon;
                return [lon, lat];
            },

            _findGridLetters: function (zoneNum, northing, easting) {

                zoneNum = parseInt(zoneNum);
                northing = parseFloat(northing);
                easting = parseFloat(easting);
                row = 1;

                // northing coordinate to single-meter precision
                north_1m = Math.round(northing);

                // Get the row position for the square identifier that contains the point
                while (north_1m >= BLOCK_SIZE) {
                    north_1m = north_1m - BLOCK_SIZE;
                    row++;
                }

                // cycle repeats (wraps) after 20 rows
                row = row % GRIDSQUARE_SET_ROW_SIZE;
                col = 0;

                // easting coordinate to single-meter precision
                east_1m = Math.round(easting)

                // Get the column position for the square identifier that contains the point
                while (east_1m >= BLOCK_SIZE) {
                    east_1m = east_1m - BLOCK_SIZE;
                    col++;
                }

                // cycle repeats (wraps) after 8 columns
                col = col % GRIDSQUARE_SET_COL_SIZE;

                return this._lettersHelper(this._findSet(zoneNum), row, col);
            },
            _findSet: function (zoneNum) {

                zoneNum = parseInt(zoneNum);
                zoneNum = zoneNum % 6;
                switch (zoneNum) {

                    case 0:
                        return 6;
                        break;

                    case 1:
                        return 1;
                        break;

                    case 2:
                        return 2;
                        break;

                    case 3:
                        return 3;
                        break;

                    case 4:
                        return 4;
                        break;

                    case 5:
                        return 5;
                        break;

                    default:
                        return -1;
                        break;
                }
            },
            _getZoneNumber: function (lat, lon) {

                lat = parseFloat(lat);
                lon = parseFloat(lon);

                // sanity check on input
                ////////////////////////////////   /*
                if (lon > 360 || lon < -180 || lat > 90 || lat < -90) {
                    //alert('Bad input. lat: ' + lat + ' lon: ' + lon);
                    //return null;
                    throw new Error("Bad input");
                }
                ////////////////////////////////  */

                // convert 0-360 to [-180 to 180] range
                var lonTemp = (lon + 180) - parseInt((lon + 180) / 360) * 360 - 180;
                var zoneNumber = parseInt((lonTemp + 180) / 6) + 1;

                // Handle special case of west coast of Norway
                if (lat >= 56.0 && lat < 64.0 && lonTemp >= 3.0 && lonTemp < 12.0) {
                    zoneNumber = 32;
                }

                // Special zones for Svalbard
                if (lat >= 72.0 && lat < 84.0) {
                    if (lonTemp >= 0.0 && lonTemp < 9.0) {
                        zoneNumber = 31;
                    }
                    else if (lonTemp >= 9.0 && lonTemp < 21.0) {
                        zoneNumber = 33;
                    }
                    else if (lonTemp >= 21.0 && lonTemp < 33.0) {
                        zoneNumber = 35;
                    }
                    else if (lonTemp >= 33.0 && lonTemp < 42.0) {
                        zoneNumber = 37;
                    }
                }
                return zoneNumber;
            },
            _utmLetterDesignator: function (lat) {
                var letterDesignator = "";
                lat = parseFloat(lat);

                if ((84 >= lat) && (lat >= 72))
                    letterDesignator = 'X';
                else if ((72 > lat) && (lat >= 64))
                    letterDesignator = 'W';
                else if ((64 > lat) && (lat >= 56))
                    letterDesignator = 'V';
                else if ((56 > lat) && (lat >= 48))
                    letterDesignator = 'U';
                else if ((48 > lat) && (lat >= 40))
                    letterDesignator = 'T';
                else if ((40 > lat) && (lat >= 32))
                    letterDesignator = 'S';
                else if ((32 > lat) && (lat >= 24))
                    letterDesignator = 'R';
                else if ((24 > lat) && (lat >= 16))
                    letterDesignator = 'Q';
                else if ((16 > lat) && (lat >= 8))
                    letterDesignator = 'P';
                else if ((8 > lat) && (lat >= 0))
                    letterDesignator = 'N';
                else if ((0 > lat) && (lat >= -8))
                    letterDesignator = 'M';
                else if ((-8 > lat) && (lat >= -16))
                    letterDesignator = 'L';
                else if ((-16 > lat) && (lat >= -24))
                    letterDesignator = 'K';
                else if ((-24 > lat) && (lat >= -32))
                    letterDesignator = 'J';
                else if ((-32 > lat) && (lat >= -40))
                    letterDesignator = 'H';
                else if ((-40 > lat) && (lat >= -48))
                    letterDesignator = 'G';
                else if ((-48 > lat) && (lat >= -56))
                    letterDesignator = 'F';
                else if ((-56 > lat) && (lat >= -64))
                    letterDesignator = 'E';
                else if ((-64 > lat) && (lat >= -72))
                    letterDesignator = 'D';
                else if ((-72 > lat) && (lat >= -80))
                    letterDesignator = 'C';
                else
                    letterDesignator = 'Z'; // This is here as an error flag to show 
                // that the latitude is outside the UTM limits
                return letterDesignator;
            },
            _lettersHelper: function (set, row, col) {

                // handle case of last row
                if (row == 0) {
                    row = GRIDSQUARE_SET_ROW_SIZE - 1;
                }
                else {
                    row--;
                }

                // handle case of last column
                if (col == 0) {
                    col = GRIDSQUARE_SET_COL_SIZE - 1;
                }
                else {
                    col--;
                }

                switch (set) {

                    case 1:
                        l1 = "ABCDEFGH";              // column ids
                        l2 = "ABCDEFGHJKLMNPQRSTUV";  // row ids
                        return l1.charAt(col) + l2.charAt(row);
                        break;

                    case 2:
                        l1 = "JKLMNPQR";
                        l2 = "FGHJKLMNPQRSTUVABCDE";
                        return l1.charAt(col) + l2.charAt(row);
                        break;

                    case 3:
                        l1 = "STUVWXYZ";
                        l2 = "ABCDEFGHJKLMNPQRSTUV";
                        return l1.charAt(col) + l2.charAt(row);
                        break;

                    case 4:
                        l1 = "ABCDEFGH";
                        l2 = "FGHJKLMNPQRSTUVABCDE";
                        return l1.charAt(col) + l2.charAt(row);
                        break;

                    case 5:
                        l1 = "JKLMNPQR";
                        l2 = "ABCDEFGHJKLMNPQRSTUV";
                        return l1.charAt(col) + l2.charAt(row);
                        break;

                    case 6:
                        l1 = "STUVWXYZ";
                        l2 = "FGHJKLMNPQRSTUVABCDE";
                        return l1.charAt(col) + l2.charAt(row);
                        break;
                }
            },
            _lltoUTM: function (lat, lon, cnts) {
                var utmcoords = [];
                // utmcoords is a 2-D array declared by the calling routine

                lat = parseFloat(lat);
                lon = parseFloat(lon);

                // Constrain reporting USNG coords to the latitude range [80S .. 84N]
                /////////////////
                if (lat > 84.0 || lat < -80.0) {
                    //return (UNDEFINED_STR);
                    throw new Error("Bad input");
                }
                //////////////////////

                // sanity check on input - turned off when testing with Generic Viewer
                /////////////////////  /*
                if (lon > 360 || lon < -180 || lat > 90 || lat < -90) {
                    //alert('Bad input. lat: ' + lat + ' lon: ' + lon);
                    //return null;
                    throw new Error("Bad input");
                }
                //////////////////////  */

                // Make sure the longitude is between -180.00 .. 179.99..
                // Convert values on 0-360 range to this range.
                var lonTemp = (lon + 180) - parseInt((lon + 180) / 360) * 360 - 180;
                var latRad = lat * DEG_2_RAD;
                var lonRad = lonTemp * DEG_2_RAD;

                // user-supplied zone number will force coordinates to be computed in a particular zone
                var zoneNumber;
                if (!cnts.zone) {
                    zoneNumber = this._getZoneNumber(lat, lon);
                }
                else {
                    zoneNumber = cnts.zone
                }

                var lonOrigin = (zoneNumber - 1) * 6 - 180 + 3;  // +3 puts origin in middle of zone
                var lonOriginRad = lonOrigin * DEG_2_RAD;

                // compute the UTM Zone from the latitude and longitude
                var UTMZone = zoneNumber + "" + this._utmLetterDesignator(lat) + " ";

                var N = cnts.EQUATORIAL_RADIUS / Math.sqrt(1 - cnts.ECC_SQUARED *
                                          Math.sin(latRad) * Math.sin(latRad));
                var T = Math.tan(latRad) * Math.tan(latRad);
                var C = cnts.ECC_PRIME_SQUARED * Math.cos(latRad) * Math.cos(latRad);
                var A = Math.cos(latRad) * (lonRad - lonOriginRad);

                // Note that the term Mo drops out of the "M" equation, because phi 
                // (latitude crossing the central meridian, lambda0, at the origin of the
                //  x,y coordinates), is equal to zero for UTM.
                var M = cnts.EQUATORIAL_RADIUS * ((1 - cnts.ECC_SQUARED / 4
                      - 3 * (cnts.ECC_SQUARED * cnts.ECC_SQUARED) / 64
                      - 5 * (cnts.ECC_SQUARED * cnts.ECC_SQUARED * cnts.ECC_SQUARED) / 256) * latRad
                      - (3 * cnts.ECC_SQUARED / 8 + 3 * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 32
                      + 45 * cnts.ECC_SQUARED * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 1024)
                          * Math.sin(2 * latRad) + (15 * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 256
                      + 45 * cnts.ECC_SQUARED * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 1024) * Math.sin(4 * latRad)
                      - (35 * cnts.ECC_SQUARED * cnts.ECC_SQUARED * cnts.ECC_SQUARED / 3072) * Math.sin(6 * latRad));

                var UTMEasting = (k0 * N * (A + (1 - T + C) * (A * A * A) / 6
                              + (5 - 18 * T + T * T + 72 * C - 58 * cnts.ECC_PRIME_SQUARED)
                              * (A * A * A * A * A) / 120)
                              + EASTING_OFFSET);

                var UTMNorthing = (k0 * (M + N * Math.tan(latRad) * ((A * A) / 2 + (5 - T + 9
                                * C + 4 * C * C) * (A * A * A * A) / 24
                                + (61 - 58 * T + T * T + 600 * C - 330 * cnts.ECC_PRIME_SQUARED)
                                * (A * A * A * A * A * A) / 720)));

                // added by LRM 2/08...not entirely sure this doesn't just move a bug somewhere else
                // utm values in southern hemisphere
                //  if (UTMNorthing < 0) {
                //	UTMNorthing += NORTHING_OFFSET;
                //  }

                utmcoords[0] = UTMEasting
                utmcoords[1] = UTMNorthing
                utmcoords[2] = zoneNumber

                return utmcoords;
            },

            _toCartecian: function (lon, lat, inSR, outSR) {
                var inDatumConstants = this._getDatumConstants(inSR);
                var outDatumConstants = this._getDatumConstants(outSR);
                var transformConstants = this._getTransfromConstants(inSR, outSR);
                //console.log(inDatumConstants, outDatumConstants, transformConstants);
                var f = 1.0 / inDatumConstants.INVERSE_FLATTENING;
                var e2 = (2.0 * f) - (f * f);

                var rLat = lat * Math.PI / 180.0;
                var rLon = lon * Math.PI / 180.0;
                var sinLat = Math.sin(rLat);
                var cosLat = Math.cos(rLat);
                var v = inDatumConstants.EQUATORIAL_RADIUS / Math.sqrt(1.0 - (e2 * sinLat * sinLat));
                var h = 0;
                var x = (v + h) * (cosLat * Math.cos(rLon));
                var y = (v + h) * cosLat * Math.sin(rLon);
                var z = ((1 - e2) * v + h) * sinLat;

                var x2 = x + transformConstants.x;
                var y2 = y + transformConstants.y;
                var z2 = z + transformConstants.z;


                var f = 1.0 / outDatumConstants.INVERSE_FLATTENING;
                var e2 = (2.0 * f) - (f * f);

                var p = Math.sqrt((x2 * x2) + (y2 * y2));
                var r = Math.sqrt((p * p) + (z2 * z2));
                var u = Math.atan((z2 / p) * ((1.0 - f) + (e2 * outDatumConstants.EQUATORIAL_RADIUS) / r));
                var sinU = Math.sin(u);
                var cosU = Math.cos(u);
                var lattopLine = z2 * (1.0 - f) + e2 * outDatumConstants.EQUATORIAL_RADIUS * sinU * sinU * sinU;
                var latBottomLine = (1.0 - f) * (p - e2 * outDatumConstants.EQUATORIAL_RADIUS * cosU * cosU * cosU);
                var rLon = Math.atan(y2 / x2);
                var rLat = Math.atan(lattopLine / latBottomLine);
                rLon = rLon < 0 ? Math.PI + rLon : rLon;
                var lonNew = rLon * 180.0 / Math.PI;
                var latNew = rLat * 180.0 / Math.PI;

                return [lonNew, latNew];
            },
            _isGeographic: function (srid) {
                if (srid >= 32600) {
                    return false;
                } else if (srid >= 24000) {
                    return false;
                }
                return true;
            },
            _isWebMercator: function (srid) {
                if (srid != 102100 && srid != 3857) {
                    return false;
                }
                return true;
            },
            _getDatumConstants: function (sr) {
                //var SEMI_MAJOR_AXIS = 0;
                var EQUATORIAL_RADIUS = 0;
                var INVERSE_FLATTENING = 0;
                var ECC_SQUARED = 0;
                var ns = "N";
                var zone = 47;
                if (sr >= 32600 || sr == 4326 || sr == 102100 || sr == 3857) {
                    EQUATORIAL_RADIUS = 6378137.0;
                    INVERSE_FLATTENING = 298.25722356300003;
                    var f = 1.0 / INVERSE_FLATTENING;
                    ECC_SQUARED = (2.0 * f) - (f * f);
                    if (sr >= 32700 && sr < 32800) {
                        zone = sr - 32700;
                        ns = "S"
                    } else if (sr >= 32600 && sr < 32700) {
                        zone = sr - 32600;
                        ns = "N"
                    } else {
                        zone = null;
                        ns = null;
                        //throw new Error("invalid 'srid': " + sr);
                    }
                } else if (sr >= 24000 || sr == 4240) {
                    EQUATORIAL_RADIUS = 6377276.3449999997;
                    INVERSE_FLATTENING = 300.80169999999998;
                    var f = 1.0 / INVERSE_FLATTENING;
                    ECC_SQUARED = (2.0 * f) - (f * f);
                    if (sr >= 24000 && sr < 24100) {
                        zone = sr - 24000;
                        ns = "N"
                    } else {
                        zone = null;
                        ns = null;
                        //throw new Error("invalid 'srid': " + sr);
                    }
                } else {
                    throw new Error("this 'srid' is not support");
                }

                var ECC_PRIME_SQUARED = ECC_SQUARED / (1 - ECC_SQUARED);
                var E1 = (1 - Math.sqrt(1 - ECC_SQUARED)) / (1 + Math.sqrt(1 - ECC_SQUARED));

                return {
                    EQUATORIAL_RADIUS: EQUATORIAL_RADIUS,
                    INVERSE_FLATTENING: INVERSE_FLATTENING,
                    ECC_SQUARED: ECC_SQUARED,
                    ECC_PRIME_SQUARED: ECC_PRIME_SQUARED,
                    E1: E1,
                    INVERSE_FLATTENING: INVERSE_FLATTENING,
                    zone: zone,
                    ns: ns
                };
            },
            _getTransfromConstants: function (inSR, outSR) {
                var x, y, z;
                if (inSR >= 32600 || inSR == 4326 || inSR == 102100 || inSR == 3857) {
                    if (outSR >= 24000 || outSR == 4240) {
                        x = -209.0;
                        y = -818.0;
                        z = -290.0;
                    }
                } else if (inSR >= 24000 || inSR == 4240) {
                    if (outSR >= 32600 || outSR == 4326 || outSR == 102100 || outSR == 3857) {
                        x = 209.0;
                        y = 818.0;
                        z = 290.0;
                    }
                } else {
                    throw new Error("this 'srid' is not support");
                }
                return { x: x, y: y, z: z };
            },
            _getGCSSpatialreference: function (sr) {
                if (sr >= 32600 || sr == 4326 || sr == 102100 || sr == 3857) {
                    return 4326;
                } else if (sr >= 24000 || sr == 4240) {
                    return 4240;
                } else {
                    throw new Error("this 'srid' is not support");
                }
            },
            _getUTMSpatialreference: function (sr, zone) {
                if (sr >= 32600 || sr == 4326 || sr == 102100 || sr == 3857) {
                    return 32600 + Number(zone);
                } else if (sr >= 24000 || sr == 4240) {
                    return 24000 + Number(zone);
                } else {
                    throw new Error("this 'srid' is not support");
                }
            },

            transform: function (geometry, outSR) {
                var datumConstant = this._getDatumConstants(outSR);
                var spatialReference = null;
                var inSR = 0;
                try {
                    geometry = typeof (geometry.toJson) == "function" ? geomJsonUtil.fromJson(geometry.toJson()) : geomJsonUtil.fromJson(geometry);
                } catch (err) { }
                if (geometry.spatialReference && geometry.spatialReference.wkid) {
                    spatialReference = geometry.spatialReference;
                    inSR = spatialReference.wkid;
                } else {
                    throw new Error("not found spatialReference in geometry");
                }
                if (inSR != outSR) {
                    if (spatialReference.isWebMercator()) {
                        geometry = webMercatorUtils.webMercatorToGeographic(geometry);
                    } else if (!this._isGeographic(spatialReference.wkid)) {
                        geometry = this.fromUTM(geometry);
                    }
                    var jsonGeometry = {};
                    if (this._getGCSSpatialreference(outSR) != this._getGCSSpatialreference(inSR)) {
                        if (geometry.x) {
                            var points = this._toCartecian(geometry.x, geometry.y, inSR, outSR);
                            jsonGeometry = { x: points[0], y: points[1] };
                        } else if (geometry.points) {
                            jsonGeometry = {
                                points: array.map(geometry.points, lang.hitch(this, function (point) {
                                    return this._toCartecian(point[1], point[0], inSR, outSR);
                                }))
                            }
                        } else if (geometry.paths) {
                            jsonGeometry = {
                                paths: array.map(geometry.paths, lang.hitch(this, function (path) {
                                    return array.map(path, lang.hitch(this, function (point) {
                                        return this._toCartecian(point[1], point[0], inSR, outSR);
                                    }));
                                }))
                            };
                        } else if (geometry.rings) {
                            jsonGeometry = {
                                rings: array.map(geometry.rings, lang.hitch(this, function (ring) {
                                    return array.map(ring, lang.hitch(this, function (point) {
                                        return this._toCartecian(point[1], point[0], inSR, outSR);
                                    }));
                                }))
                            };
                        } else if (geometry.xmax) {
                            var points1 = this._toCartecian(geometry.ymax, geometry.xmax, inSR, outSR);
                            var points2 = this._toCartecian(geometry.ymin, geometry.xmin, inSR, outSR);
                            jsonGeometry["xmax"] = points1[1];
                            jsonGeometry["ymax"] = points1[0];
                            jsonGeometry["xmin"] = points2[1];
                            jsonGeometry["ymin"] = points2[0];
                        }
                        jsonGeometry["spatialReference"] = { wkid: this._getGCSSpatialreference(outSR) };
                        jsonGeometry = geomJsonUtil.fromJson(jsonGeometry);
                    } else {
                        jsonGeometry = geometry;
                    }

                    if (this._isWebMercator(outSR)) {
                        jsonGeometry = webMercatorUtils.geographicToWebMercator(jsonGeometry);
                    } else if (!this._isGeographic(outSR)) {
                        jsonGeometry = this.toUTM(jsonGeometry, datumConstant.zone);
                    }
                    return jsonGeometry;
                } else {
                    return geometry;
                }
            }

        }
    });

    return wgDeclare;
});