define([
	"dojo/_base/lang",
	"require",
	"esri/symbols/SimpleMarkerSymbol",
	"esri/symbols/SimpleFillSymbol",
	"esri/symbols/SimpleLineSymbol",
	"esri/symbols/PictureMarkerSymbol",
	"esri/symbols/TextSymbol",
	"esri/symbols/Font",
	"esri/Color"
],
function (
	lang,
	require,
	SimpleMarkerSymbol,
	SimpleFillSymbol,
	SimpleLineSymbol,
	PictureMarkerSymbol,
	TextSymbol,
	Font,
	Color
	) {

    return {
        'resource': {
            'btn_measurement_tool_01_normal': require.toUrl('./resources/btn_measurement_tool_01_normal.png'),
            'btn_measurement_tool_01_hover': require.toUrl('./resources/btn_measurement_tool_01_hover.png'),
            'btn_measurement_tool_01_selected': require.toUrl('./resources/btn_measurement_tool_01_selected.png'),
            'btn_measurement_tool_02_normal': require.toUrl('./resources/btn_measurement_tool_02_normal.png'),
            'btn_measurement_tool_02_hover': require.toUrl('./resources/btn_measurement_tool_02_hover.png'),
            'btn_measurement_tool_02_selected': require.toUrl('./resources/btn_measurement_tool_02_selected.png'),
            'btn_measurement_tool_03_normal': require.toUrl('./resources/btn_measurement_tool_03_normal.png'),
            'btn_measurement_tool_03_hover': require.toUrl('./resources/btn_measurement_tool_03_hover.png'),
            'btn_measurement_tool_03_selected': require.toUrl('./resources/btn_measurement_tool_03_selected.png'),
            'btn_measurement_tool_04_normal': require.toUrl('./resources/btn_measurement_tool_04_normal.png'),
            'btn_measurement_tool_04_hover': require.toUrl('./resources/btn_measurement_tool_04_hover.png'),
            'btn_measurement_tool_04_selected': require.toUrl('./resources/btn_measurement_tool_04_selected.png'),
            'btn_measurement_tool_05_normal': require.toUrl('./resources/btn_measurement_tool_05_normal.png'),
            'btn_measurement_tool_05_hover': require.toUrl('./resources/btn_measurement_tool_05_hover.png'),
            'btn_measurement_tool_05_selected': require.toUrl('./resources/btn_measurement_tool_05_selected.png')
        },

        "isGeodesic": true,

        "defaultSymbol": {
            "point": new PictureMarkerSymbol({
                "type": "esriPMS",
                "url": require.toUrl("./resources/mark2.png"),
                "height": 16 * 0.75,
                "width": 16 * 0.75,
                "angle": 0,
            }),

            "line": new SimpleLineSymbol({
                "type": "esriSLS",
                "style": "esriSLSSolid",
                "color": [39, 189, 138, 255],
                "width": 3
            }),

            "polygon": new SimpleFillSymbol(
					SimpleLineSymbol.STYLE_SOLID,
					new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color([39, 189, 138]), 3)
					, new Color([51, 202, 151, 0.5])),

            "text": new TextSymbol({
                "type": "textsymbol",
                "verticalAlignment": "middle",
                "haloSize": 2,
                "haloColor": [255, 255, 255, 255],
                "yoffset": "-18",
                "angle": 0,
                "color": [39, 189, 138, 255],
                "font": new Font({
                    "size": 6,
                    "weight": Font.WEIGHT_BOLD
                })
            })
        },

        "area": {
            'pattern': "#,###.##",
            "showSegmentLabel": true,
            "toolEnabled": true,
            "units": [
                {
                    VALUE: "square-kilometers",
                    LABEL: "ตารางกิโลเมตร",
                    SHORTLABEL: "ตร.กม.",
                    SEGMENT_UNIT: {
                        VALUE: "kilometers",
                        LABEL: "กิโลเมตร",
                        SHORTLABEL: "กม."
                    }
                },
                {
                    VALUE: "square-meters",
                    LABEL: "ตารางเมตร",
                    SHORTLABEL: "ตร.ม.",
                    SEGMENT_UNIT: {
                        VALUE: "meters",
                        LABEL: "เมตร",
                        SHORTLABEL: "ม."
                    }
                },
                //{
                //    VALUE: "area-rai",
                //    LABEL: "ไร่ งาน ตารางวา",
                //    SHORTLABEL: "ไร่ งาน ตารางวา",
                //    SEGMENT_UNIT: {
                //        VALUE: "wa",
                //        LABEL: "วา",
                //        SHORTLABEL: "วา"
                //    }
                //},
                //{
                //    VALUE: "acres",
                //    LABEL: "Acres",
                //    SHORTLABEL: "r.",
                //    SEGMENT_UNIT: {
                //        VALUE: "meters",
                //        LABEL: "เมตร",
                //        SHORTLABEL: "ม."
                //    }
                //},
                //{
                //    VALUE: "ares",
                //    LABEL: "Ares",
                //    SHORTLABEL: "r.",
                //    SEGMENT_UNIT: {
                //        VALUE: "meters",
                //        LABEL: "เมตร",
                //        SHORTLABEL: "ม."
                //    }
                //},
                //{
                //    VALUE: "hectares",
                //    LABEL: "Hectares",
                //    SHORTLABEL: "r.",
                //    SEGMENT_UNIT: {
                //        VALUE: "meters",
                //        LABEL: "เมตร",
                //        SHORTLABEL: "ม."
                //    }
                //},
                //{
                //    VALUE: "square-feet",
                //    LABEL: "Square-Feet",
                //    SHORTLABEL: "sq.feet",
                //    SEGMENT_UNIT: {
                //        VALUE: "feet",
                //        LABEL: "Feet",
                //        SHORTLABEL: "ft"
                //    }
                //},

                //{
                //    VALUE: "square-yards",
                //    LABEL: "Square-Yards",
                //    SHORTLABEL: "r.",
                //    SEGMENT_UNIT: {
                //        VALUE: "yards",
                //        LABEL: "Yards",
                //        SHORTLABEL: "yd"
                //    }
                //},
                //{
                //    VALUE: "square-miles",
                //    LABEL: "Square-Miles",
                //    SHORTLABEL: "r.",
                //    SEGMENT_UNIT: {
                //        VALUE: "miles",
                //        LABEL: "Miles",
                //        SHORTLABEL: "mi"
                //    }
                //}
            ],
        },

        "angle": {
            "toolEnabled": true,
            "baseLineLength": 100,//screen Pixel
            "units": [
                {
                    VALUE: "DEG",
                    LABEL: "Degree-องศา"
                },
                {
                    VALUE: "AZH",
                    LABEL: "Azimuth-องศา"

                }
            ],
            "symbols": {
                "firstPoint": new PictureMarkerSymbol({
                    "type": "esriPMS",
                    "url": require.toUrl("./resources/mark1.png"),
                    "height": 16 * 0.75,
                    "width": 16 * 0.75,
                    "angle": 0,
                }),
                "baseLine": new SimpleLineSymbol({
                    "type": "esriSLS",
                    "style": "esriSLSDash",
                    "color": [39, 189, 138, 255],
                    "width": 2
                }),
            }
        },

        "distance": {
            "showSegmentLabel": true,
            "toolEnabled": true,
            "units": [
                //{
                //    VALUE: "feet",
                //    LABEL: "Feet",
                //    SHORTLABEL: "ft"
                //},
                {
                    VALUE: "kilometers",
                    LABEL: "กิโลเมตร",
                    SHORTLABEL: "กม."
                },
                {
                    VALUE: "meters",
                    LABEL: "เมตร",
                    SHORTLABEL: "ม."
                },
                //{
                //    VALUE: "miles",
                //    LABEL: "Miles",
                //    SHORTLABEL: "mi"
                //},
                //{
                //    VALUE: "nautical-miles",
                //    LABEL: "Nautical Miles",
                //    SHORTLABEL: "nmi"
                //},
                //{
                //    VALUE: "yards",
                //    LABEL: "Yards",
                //    SHORTLABEL: "yd"
                //}
            ],
        },

        "freehandAngle": {
            "toolEnabled": true,
            "units": [
				{
				    VALUE: "DEG",
				    LABEL: "Degree-องศา"
				}
            ],
        },

        "location": {
            "toolEnabled": true,
            "units": [
				{
				    VALUE: "UTM",
				    LABEL: "UTM",
				    LABEL_X: "X",
				    LABEL_Y: "Y"
				},//  32647
				{
				    VALUE: "WebMercator",
				    LABEL: "WebMercator",
				    LABEL_X: "X",
				    LABEL_Y: "Y"
				},//	3857
				{
				    VALUE: "LATLONG",
				    LABEL: "LATLONG",
				    LABEL_X: "Longitude",
				    LABEL_Y: "Latitude"
				},// 4326
				{
				    VALUE: "MGRS",
				    LABEL: "MGRS",
				},
				{
				    VALUE: "DMS",
				    LABEL: "DMS"
				}
            ],
            "symbols": {
                "point": new PictureMarkerSymbol({
                    "type": "esriPMS",
                    "url": require.toUrl("./resources/pin2.png"),
                    "height": 49 * 0.75,
                    "width": 25 * 0.75,
                    "xoffset": 0,
                    "yoffset": 9
                }),
            }
        },
    };
});
