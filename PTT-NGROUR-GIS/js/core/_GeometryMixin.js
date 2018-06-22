/**
 * @description 
 * -
 * @class _GeometryMixin
 */
define([
    "esri/geometry/jsonUtils",

    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    geomJsonUtil,

    array,
    lang,
    declare,
    require
    ) {


    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {

        geometry: {
            _dbProjection: 102100,

            /**
            * Convert from st_geometry string to geometry json.
            * @method geometry.fromST
            * @public
            * @param {String} stringST 
            * @return {esri.geometry.Geometry}
            */
            fromST: function (stringST) {
                var geomJson = {};
                var stringSplite = null;
                var stringSplite2 = null;
                var stringSplite3 = null;
                stringST = lang.trim(stringST);
                if (stringST.indexOf("POINT") == 0) {
                    stringST = stringST.replace("POINT", "");
                    stringST = lang.trim(stringST);
                    stringST = stringST.replace("(", "").replace(")", "");
                    stringST = lang.trim(stringST);
                    stringSplite = stringST.split(" ");
                    geomJson["x"] = Number(stringSplite[0]);
                    geomJson["y"] = Number(stringSplite[1]);
                } else if (stringST.indexOf("MULTIPOINT") == 0) {
                    geomJson["points"] = new Array();
                    stringST = stringST.replace("MULTIPOINT", "");
                    stringST = lang.trim(stringST);
                    stringST = stringST.replace(/\(/igm, "").replace(/\)/igm, "");
                    stringSplite2 = stringST.split(",");
                    for (var j = 0; j < stringSplite2.length; j++) {
                        stringSplite3 = lang.trim(stringSplite2[j]).split(" ");
                        geomJson["points"][j] = [Number(lang.trim(stringSplite3[0])), Number(lang.trim(stringSplite3[1]))];
                    }
                } else if (stringST.indexOf("LINESTRING") == 0) {
                    geomJson["paths"] = new Array();
                    stringST = stringST.replace("LINESTRING", "");
                    stringST = lang.trim(stringST);
                    stringSplite = stringST.split(/\(*?\)\,/);
                    for (var i = 0; i < stringSplite.length; i++) {
                        geomJson["paths"][i] = new Array();
                        stringST = stringSplite[i].replace(/\(/igm, "").replace(/\)/igm, "");
                        stringSplite2 = stringST.split(",");
                        for (var j = 0; j < stringSplite2.length; j++) {
                            stringSplite3 = lang.trim(stringSplite2[j]).split(" ");
                            geomJson["paths"][i].push([Number(lang.trim(stringSplite3[0])), Number(lang.trim(stringSplite3[1]))]);
                        }
                    }
                } else if (stringST.indexOf("MULTILINESTRING") == 0) {
                    geomJson["paths"] = new Array();
                    stringST = stringST.replace("MULTILINESTRING", "");
                    stringST = lang.trim(stringST);
                    stringSplite = stringST.split(/\(*?\)\,/);
                    for (var i = 0; i < stringSplite.length; i++) {
                        geomJson["paths"][i] = new Array();
                        stringST = stringSplite[i].replace(/\(/igm, "").replace(/\)/igm, "");
                        stringSplite2 = stringST.split(",");
                        for (var j = 0; j < stringSplite2.length; j++) {
                            stringSplite3 = lang.trim(stringSplite2[j]).split(" ");
                            geomJson["paths"][i].push([Number(lang.trim(stringSplite3[0])), Number(lang.trim(stringSplite3[1]))]);
                        }
                    }
                } else if (stringST.indexOf("POLYGON") == 0) {
                    geomJson["rings"] = new Array();
                    stringST = stringST.replace("POLYGON", "");
                    stringST = lang.trim(stringST);
                    stringSplite = stringST.split(/\(*?\)\,/);
                    for (var i = 0; i < stringSplite.length; i++) {
                        geomJson["rings"][i] = new Array();
                        stringST = stringSplite[i].replace(/\(/igm, "").replace(/\)/igm, "");
                        stringSplite2 = stringST.split(",");
                        for (var j = 0; j < stringSplite2.length; j++) {
                            stringSplite3 = lang.trim(stringSplite2[j]).split(" ");
                            geomJson["rings"][i].push([Number(lang.trim(stringSplite3[0])), Number(lang.trim(stringSplite3[1]))]);
                        }
                    }
                } else if (stringST.indexOf("MULTIPOLYGON") == 0) {
                    geomJson["rings"] = new Array();
                    stringST = stringST.replace("MULTIPOLYGON", "");
                    stringST = lang.trim(stringST);
                    stringSplite = stringST.split(/\(*?\)\,/);
                    for (var i = 0; i < stringSplite.length; i++) {
                        geomJson["rings"][i] = new Array();
                        stringST = stringSplite[i].replace(/\(/igm, "").replace(/\)/igm, "");
                        stringSplite2 = stringST.split(",");
                        for (var j = 0; j < stringSplite2.length; j++) {
                            stringSplite3 = lang.trim(stringSplite2[j]).split(" ");
                            geomJson["rings"][i].push([Number(lang.trim(stringSplite3[0])), Number(lang.trim(stringSplite3[1]))]);
                        }
                    }
                }
                geomJson["spatialReference"] = { "wkid": this._dbProjection };
                return geomJsonUtil.fromJson(geomJson);
            },

            /**
            * Convert from geometry json to st_geometry string.
            * @method geometry.toST
            * @public
            * @param {esri.geometry.Geometry} geometry 
            * @return {String}
            */
            toST: function (geometry) {
                var stringST = "";
                if (geometry.x && geometry.y) {
                    stringST = lang.replace("POINT({x} {y})", geometry);
                } else if (geometry.points) {
                    stringST = "MULTIPOINT({0})";
                    stringST = lang.replace(stringST, [geometry.points.map(lang.hitch(this, function (r) {
                        return r.join(" ");
                    })).join(","), stringST]);
                } else if (geometry.paths) {
                    if (geometry.paths.length == 1) {
                        stringST = "LINESTRING{0}";
                    } else if (geometry.paths.length > 1) {
                        stringST = "MULTILINESTRING({0})";
                    }
                    stringST = lang.replace(stringST, [geometry.paths.map(lang.hitch(this, function (r) {
                        return lang.replace("({0})", [r.map(lang.hitch(this, function (p) {
                            return p.join(" ");
                        })).join(",")]);
                    })).join(","), stringST]);
                } else if (geometry.rings) {
                    if (geometry.rings.length == 1) {
                        stringST = "POLYGON{0}";
                    } else if (geometry.rings.length > 1) {
                        stringST = "MULTIPOLYGON({0})";
                    }
                    stringST = lang.replace(stringST, [geometry.rings.map(lang.hitch(this, function (r) {
                        return lang.replace("(({0}))", [r.map(lang.hitch(this, function (p) {
                            return p.join(" ");
                        })).join(",")]);
                    })).join(",")]);
                } else if (geometry.xmin && geometry.ymin) {
                    stringST = lang.replace("POLYGON(({xmin} {ymax},{xmax} {ymax},{xmax} {ymin},{xmin} {ymin},{xmin} {ymax}))", geometry);
                }
                return stringST;
            }
        }
    });

    return wgDeclare;
});